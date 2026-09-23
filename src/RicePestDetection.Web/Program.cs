using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.WebEncoders;
using RicePestDetection.Web.Data;
using RicePestDetection.Web.Models;
using RicePestDetection.Web.Services;
using System.Text.Encodings.Web;
using System.Text.Unicode;

var builder = WebApplication.CreateBuilder(args);

// 配置 HTML 编码器：允许所有 Unicode 字符直接输出（不转义为 HTML 实体）
builder.Services.Configure<WebEncoderOptions>(options =>
{
    options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All);
});

// ========== 数据库配置 ==========
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// ========== Identity 配置 ==========
builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>(options =>
    {
        // 密码策略
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;

        // 用户设置
        options.User.RequireUniqueEmail = true;

        // 登录设置
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// ========== MVC 配置 ==========
builder.Services.AddControllersWithViews();

// ========== Session 配置（用于 TempData 等） ==========
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ========== 业务服务注册 ==========
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPestService, PestService>();
builder.Services.AddScoped<IDetectionService, DetectionService>();
builder.Services.AddScoped<IStatisticsService, StatisticsService>();
builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddSingleton<IMLPredictionService, MLPredictionService>();

var app = builder.Build();

// ========== HTTP 请求管道配置 ==========
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 认证与授权（顺序很重要：UseAuthentication 必须在 UseAuthorization 之前）
app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

// ========== 路由配置 ==========
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// ========== 数据库迁移与种子数据初始化 ==========
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<AppDbContext>();

    // 自动应用迁移（确保数据库表存在）
    await dbContext.Database.MigrateAsync();

    // 初始化种子数据
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();
    await SeedData.InitializeAsync(services, userManager, roleManager);
}

app.Run();
