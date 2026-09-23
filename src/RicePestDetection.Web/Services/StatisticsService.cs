using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RicePestDetection.Web.Data;
using RicePestDetection.Web.Models;

namespace RicePestDetection.Web.Services;

/// <summary>
/// 统计服务实现
/// </summary>
public class StatisticsService : IStatisticsService
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public StatisticsService(AppDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    /// <summary>
    /// 获取按日期分组的检测次数统计
    /// </summary>
    public async Task<Dictionary<DateTime, int>> GetDetectionTrendAsync(int days = 30)
    {
        var startDate = DateTime.Now.AddDays(-days + 1).Date;

        var records = await _context.DetectionRecords
            .Where(r => r.CreatedAt >= startDate)
            .GroupBy(r => r.CreatedAt.Date)
            .Select(g => new { Date = g.Key, Count = g.Count() })
            .ToListAsync();

        // 转换为字典
        var result = records.ToDictionary(r => r.Date, r => r.Count);

        // 填充没有检测记录的日期
        for (var date = startDate; date <= DateTime.Now.Date; date = date.AddDays(1))
        {
            if (!result.ContainsKey(date))
            {
                result[date] = 0;
            }
        }

        return result.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
    }

    /// <summary>
    /// 获取各病虫害类型的检测数量分布
    /// </summary>
    public async Task<Dictionary<string, int>> GetPestDistributionAsync()
    {
        var records = await _context.DetectionRecords
            .GroupBy(r => r.ResultName)
            .Select(g => new { Name = g.Key, Count = g.Count() })
            .OrderByDescending(g => g.Count)
            .ToListAsync();

        return records.ToDictionary(r => r.Name, r => r.Count);
    }

    /// <summary>
    /// 获取用户统计数据
    /// </summary>
    public async Task<UserStatistics> GetUserStatisticsAsync()
    {
        var allUsers = await _userManager.Users.ToListAsync();

        var adminCount = 0;
        var normalUserCount = 0;
        var activeUsers = 0;

        foreach (var user in allUsers)
        {
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Admin"))
            {
                adminCount++;
            }
            else
            {
                normalUserCount++;
            }

            if (user.IsActive)
            {
                activeUsers++;
            }
        }

        return new UserStatistics
        {
            TotalUsers = allUsers.Count,
            ActiveUsers = activeUsers,
            AdminCount = adminCount,
            NormalUserCount = normalUserCount
        };
    }
}
