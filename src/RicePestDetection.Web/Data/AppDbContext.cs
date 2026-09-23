using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RicePestDetection.Web.Models;

namespace RicePestDetection.Web.Data;

/// <summary>
/// 应用数据库上下文，集成 Identity 与业务实体
/// </summary>
public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// 病虫害表
    /// </summary>
    public DbSet<Pest> Pests => Set<Pest>();

    /// <summary>
    /// 检测记录表
    /// </summary>
    public DbSet<DetectionRecord> DetectionRecords => Set<DetectionRecord>();

    /// <summary>
    /// 系统日志表
    /// </summary>
    public DbSet<Log> Logs => Set<Log>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // 配置 Pest 实体
        builder.Entity<Pest>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Category).HasMaxLength(20).IsRequired();
            entity.Property(e => e.Symptoms).HasMaxLength(1000);
            entity.Property(e => e.HarmDescription).HasMaxLength(1000);
            entity.Property(e => e.PreventionMethod).HasMaxLength(1000);
            entity.Property(e => e.ImagePath).HasMaxLength(200);
        });

        // 配置 DetectionRecord 实体
        builder.Entity<DetectionRecord>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ImagePath).HasMaxLength(200).IsRequired();
            entity.Property(e => e.ResultName).HasMaxLength(50).IsRequired();
            entity.HasOne(e => e.User)
                  .WithMany(u => u.DetectionRecords)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Pest)
                  .WithMany(p => p.DetectionRecords)
                  .HasForeignKey(e => e.PestId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // 配置 Log 实体
        builder.Entity<Log>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ActionType).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Content).HasMaxLength(500);
            entity.HasOne(e => e.User)
                  .WithMany(u => u.Logs)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // 配置 ApplicationUser 实体
        builder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(e => e.NickName).HasMaxLength(50);
        });
    }
}
