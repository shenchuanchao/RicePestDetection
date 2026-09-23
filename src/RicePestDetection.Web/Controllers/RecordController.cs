using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RicePestDetection.Web.Services;

namespace RicePestDetection.Web.Controllers;

/// <summary>
/// 检测记录控制器
/// </summary>
public class RecordController : Controller
{
    private readonly IDetectionService _detectionService;
    private readonly ILogService _logService;

    public RecordController(IDetectionService detectionService, ILogService logService)
    {
        _detectionService = detectionService;
        _logService = logService;
    }

    /// <summary>
    /// 检测记录列表
    /// 普通用户查看自己的记录，管理员查看所有记录
    /// </summary>
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Index()
    {
        var userId = GetUserId();
        var isAdmin = User.IsInRole("Admin");

        var records = isAdmin
            ? await _detectionService.GetAllRecordsAsync()
            : await _detectionService.GetRecordsByUserIdAsync(userId);

        return View(records);
    }

    /// <summary>
    /// 检测记录详情
    /// </summary>
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Details(int id)
    {
        var record = await _detectionService.GetRecordByIdAsync(id);
        if (record == null) return NotFound();

        var userId = GetUserId();
        var isAdmin = User.IsInRole("Admin");
        if (record.UserId != userId && !isAdmin)
        {
            return Forbid();
        }

        return View(record);
    }

    /// <summary>
    /// 删除检测记录
    /// </summary>
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetUserId();
        var result = await _detectionService.DeleteRecordAsync(id, userId);

        if (result)
        {
            await _logService.LogAsync(userId, "删除记录", $"删除检测记录ID：{id}");
            TempData["Success"] = "记录删除成功";
        }
        else
        {
            TempData["Error"] = "删除失败，记录不存在或无权操作";
        }

        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// 获取当前用户ID
    /// </summary>
    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        return userIdClaim != null && int.TryParse(userIdClaim.Value, out var id) ? id : 0;
    }
}
