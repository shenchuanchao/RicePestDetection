using Microsoft.AspNetCore.Mvc;
using RicePestDetection.Web.Services;
using RicePestDetection.Web.ViewModels;

namespace RicePestDetection.Web.Controllers;

/// <summary>
/// 数据统计控制器
/// </summary>
public class StatisticsController : Controller
{
    private readonly IStatisticsService _statisticsService;
    private readonly IDetectionService _detectionService;
    private readonly IPestService _pestService;

    public StatisticsController(
        IStatisticsService statisticsService,
        IDetectionService detectionService,
        IPestService pestService)
    {
        _statisticsService = statisticsService;
        _detectionService = detectionService;
        _pestService = pestService;
    }

    /// <summary>
    /// 统计页面
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var viewModel = new StatisticsViewModel
        {
            DetectionTrend = await _statisticsService.GetDetectionTrendAsync(30),
            PestDistribution = await _statisticsService.GetPestDistributionAsync(),
            UserStatistics = await _statisticsService.GetUserStatisticsAsync(),
            TotalDetections = await _detectionService.GetTotalCountAsync(),
            TotalPests = await _pestService.GetTotalCountAsync()
        };

        return View(viewModel);
    }
}
