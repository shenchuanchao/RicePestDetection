using Microsoft.AspNetCore.Identity;
using RicePestDetection.Web.Models;

namespace RicePestDetection.Web.Data;

/// <summary>
/// 数据库种子数据，初始化管理员账号和常见病虫害知识
/// </summary>
public static class SeedData
{
    /// <summary>
    /// 初始化种子数据
    /// </summary>
    public static async Task InitializeAsync(
        IServiceProvider serviceProvider,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<int>> roleManager)
    {
        // 1. 创建角色
        string[] roleNames = { "Admin", "User" };
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole<int>(roleName));
            }
        }

        // 2. 创建管理员账号
        var adminEmail = "admin@rice.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = "admin",
                Email = adminEmail,
                NickName = "系统管理员",
                IsActive = true,
                CreatedAt = DateTime.Now
            };
            var result = await userManager.CreateAsync(adminUser, "Admin@123456");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        // 3. 创建普通用户测试账号
        var userEmail = "user@rice.com";
        var normalUser = await userManager.FindByEmailAsync(userEmail);
        if (normalUser == null)
        {
            normalUser = new ApplicationUser
            {
                UserName = "user",
                Email = userEmail,
                NickName = "测试用户",
                IsActive = true,
                CreatedAt = DateTime.Now
            };
            var result = await userManager.CreateAsync(normalUser, "User@123456");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(normalUser, "User");
            }
        }

        // 4. 初始化病虫害数据（仅在表为空时）
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        if (!context.Pests.Any())
        {
            var pests = GetInitialPests();
            await context.Pests.AddRangeAsync(pests);
            await context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// 获取初始病虫害数据
    /// </summary>
    private static List<Pest> GetInitialPests()
    {
        return new List<Pest>
        {
            new()
            {
                Name = "稻瘟病",
                Category = "Disease",
                Symptoms = "叶片出现梭形病斑，中央灰白色，边缘褐色，潮湿时病斑背面产生灰色霉层。",
                HarmDescription = "可造成叶片枯死、穗颈折断，严重时导致白穗，减产可达40%-50%。",
                PreventionMethod = "1.选用抗病品种；2.合理施肥，避免偏施氮肥；3.发病初期喷施三环唑或稻瘟灵。",
                CreatedAt = DateTime.Now
            },
            new()
            {
                Name = "纹枯病",
                Category = "Disease",
                Symptoms = "叶鞘产生暗绿色水渍状边缘模糊的小斑，后扩大成云纹状大斑，中央灰白色。",
                HarmDescription = "导致叶片枯死、结实率下降、千粒重降低，一般减产10%-30%。",
                PreventionMethod = "1.合理密植，通风透光；2.浅水勤灌，适时晒田；3.喷施井岗霉素或苯甲·丙环唑。",
                CreatedAt = DateTime.Now
            },
            new()
            {
                Name = "白叶枯病",
                Category = "Disease",
                Symptoms = "叶尖或叶缘产生黄绿色或暗绿色斑点，沿叶脉扩展成条斑，病健交界明显。",
                HarmDescription = "叶片干枯，影响光合作用，导致减产，严重时颗粒无收。",
                PreventionMethod = "1.选用抗病品种；2.避免深水淹灌；3.喷施噻菌铜或叶枯唑。",
                CreatedAt = DateTime.Now
            },
            new()
            {
                Name = "褐斑病",
                Category = "Disease",
                Symptoms = "叶片出现褐色椭圆形或卵圆形病斑，中央灰褐色，边缘深褐色，有黄色晕圈。",
                HarmDescription = "叶片枯黄，影响光合作用，降低结实率和千粒重，一般减产10%-20%。",
                PreventionMethod = "1.选用抗病品种；2.合理施肥，增施磷钾肥；3.发病初期喷施咪鲜胺或苯醚甲环唑。",
                CreatedAt = DateTime.Now
            },
            new()
            {
                Name = "叶枯病",
                Category = "Disease",
                Symptoms = "叶片从叶尖开始变白枯死，沿叶缘向下扩展，病部可见浅褐色条纹。",
                HarmDescription = "叶片大面积枯死，影响光合作用和灌浆，导致减产。",
                PreventionMethod = "1.选用抗病品种；2.避免过量施氮肥；3.喷施噻菌铜或中生菌素。",
                CreatedAt = DateTime.Now
            },
            new()
            {
                Name = "稻曲病",
                Category = "Disease",
                Symptoms = "稻粒上形成黄色或墨绿色的粉状霉块，俗称'丰产果'。",
                HarmDescription = "降低稻米品质，稻曲病菌含毒素，影响人畜健康。",
                PreventionMethod = "1.选用抗病品种；2.破口前7-10天喷施井岗霉素或戊唑醇。",
                CreatedAt = DateTime.Now
            },
            new()
            {
                Name = "稻飞虱",
                Category = "Pest",
                Symptoms = "稻株下部叶片枯黄，严重时出现'冒穿'、倒伏，稻田可见大量飞虱群集。",
                HarmDescription = "吸食稻株汁液，传播病毒病，造成减产甚至绝收。",
                PreventionMethod = "1.保护天敌（蜘蛛、黑肩绿盲蝽）；2.喷施吡蚜酮或噻虫嗪。",
                CreatedAt = DateTime.Now
            },
            new()
            {
                Name = "稻纵卷叶螟",
                Category = "Pest",
                Symptoms = "幼虫将稻叶纵卷成苞，在苞内取食叶肉，留下白色条斑。",
                HarmDescription = "叶片受损，影响光合作用，导致减产。",
                PreventionMethod = "1.设置诱虫灯诱杀成虫；2.在低龄幼虫期喷施氯虫苯甲酰胺或甲维盐。",
                CreatedAt = DateTime.Now
            },
            new()
            {
                Name = "二化螟",
                Category = "Pest",
                Symptoms = "稻株出现枯心苗、枯鞘，后期出现白穗、虫伤株。",
                HarmDescription = "造成枯心、白穗，严重影响产量。",
                PreventionMethod = "1.冬季翻耕灭蛹；2.在蚁螟盛期喷施氯虫苯甲酰胺或杀虫双。",
                CreatedAt = DateTime.Now
            }
        };
    }
}
