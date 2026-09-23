using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RicePestDetection.Web.Models;
using RicePestDetection.Web.Services;

namespace RicePestDetection.Web.Controllers;

/// <summary>
/// 首页控制器
/// </summary>
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IPestService _pestService;
    private readonly IDetectionService _detectionService;

    public HomeController(
        ILogger<HomeController> logger,
        IPestService pestService,
        IDetectionService detectionService)
    {
        _logger = logger;
        _pestService = pestService;
        _detectionService = detectionService;
    }

    /// <summary>
    /// 首页
    /// </summary>
    public async Task<IActionResult> Index()
    {
        ViewBag.TotalPests = await _pestService.GetTotalCountAsync();
        ViewBag.TotalDetections = await _detectionService.GetTotalCountAsync();
        ViewBag.RecentPests = await _pestService.GetAllAsync();
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
