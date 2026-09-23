using RicePestDetection.Web.Services;

namespace RicePestDetection.Web.ViewModels;

/// <summary>
/// 统计视图模型
/// </summary>
public class StatisticsViewModel
{
    public Dictionary<DateTime, int> DetectionTrend { get; set; } = new();
    public Dictionary<string, int> PestDistribution { get; set; } = new();
    public UserStatistics UserStatistics { get; set; } = new();
    public int TotalDetections { get; set; }
    public int TotalPests { get; set; }
}
