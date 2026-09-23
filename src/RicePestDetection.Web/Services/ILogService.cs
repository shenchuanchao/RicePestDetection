using RicePestDetection.Web.Models;

namespace RicePestDetection.Web.Services;

/// <summary>
/// 系统日志服务接口
/// </summary>
public interface ILogService
{
    /// <summary>
    /// 记录操作日志
    /// </summary>
    Task LogAsync(int? userId, string actionType, string? content = null);

    /// <summary>
    /// 获取所有日志（按时间倒序）
    /// </summary>
    Task<List<Log>> GetAllAsync(int page = 1, int pageSize = 20);

    /// <summary>
    /// 获取日志总数
    /// </summary>
    Task<int> GetTotalCountAsync();
}
