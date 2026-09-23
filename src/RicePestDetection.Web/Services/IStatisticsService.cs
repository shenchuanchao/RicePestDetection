namespace RicePestDetection.Web.Services;

/// <summary>
/// 统计服务接口
/// </summary>
public interface IStatisticsService
{
    /// <summary>
    /// 获取按日期分组的检测次数统计
    /// </summary>
    /// <param name="days">最近天数</param>
    Task<Dictionary<DateTime, int>> GetDetectionTrendAsync(int days = 30);

    /// <summary>
    /// 获取各病虫害类型的检测数量分布
    /// </summary>
    Task<Dictionary<string, int>> GetPestDistributionAsync();

    /// <summary>
    /// 获取用户统计数据
    /// </summary>
    Task<UserStatistics> GetUserStatisticsAsync();
}

/// <summary>
/// 用户统计数据
/// </summary>
public class UserStatistics
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int AdminCount { get; set; }
    public int NormalUserCount { get; set; }
}
