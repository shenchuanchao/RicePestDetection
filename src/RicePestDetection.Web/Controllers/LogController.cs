using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RicePestDetection.Web.Services;

namespace RicePestDetection.Web.Controllers;

/// <summary>
/// 系统日志控制器（管理员）
/// </summary>
[Authorize(Roles = "Admin")]
public class LogController : Controller
{
    private readonly ILogService _logService;

    public LogController(ILogService logService)
    {
        _logService = logService;
    }

    /// <summary>
    /// 系统日志列表
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index(int page = 1)
    {
        var logs = await _logService.GetAllAsync(page, 30);
        ViewData["Page"] = page;
        return View(logs);
    }
}
