using Microsoft.EntityFrameworkCore;
using RicePestDetection.Web.Data;
using RicePestDetection.Web.Models;

namespace RicePestDetection.Web.Services;

/// <summary>
/// 系统日志服务实现
/// </summary>
public class LogService : ILogService
{
    private readonly AppDbContext _context;

    public LogService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 记录操作日志
    /// </summary>
    public async Task LogAsync(int? userId, string actionType, string? content = null)
    {
        var log = new Log
        {
            UserId = userId,
            ActionType = actionType,
            Content = content,
            CreatedAt = DateTime.Now
        };
        _context.Logs.Add(log);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// 获取所有日志（分页，按时间倒序）
    /// </summary>
    public async Task<List<Log>> GetAllAsync(int page = 1, int pageSize = 20)
    {
        return await _context.Logs
            .Include(l => l.User)
            .OrderByDescending(l => l.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    /// <summary>
    /// 获取日志总数
    /// </summary>
    public async Task<int> GetTotalCountAsync()
    {
        return await _context.Logs.CountAsync();
    }
}
