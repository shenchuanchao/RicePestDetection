using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RicePestDetection.Web.Services;
using RicePestDetection.Web.ViewModels;

namespace RicePestDetection.Web.Controllers;

/// <summary>
/// 图像检测控制器
/// </summary>
public class DetectionController : Controller
{
    private readonly IDetectionService _detectionService;
    private readonly ILogService _logService;

    public DetectionController(IDetectionService detectionService, ILogService logService)
    {
        _detectionService = detectionService;
        _logService = logService;
    }

    /// <summary>
    /// 检测页面
    /// </summary>
    [HttpGet]
    [Authorize]
    public IActionResult Index()
    {
        return View();
    }

    /// <summary>
    /// 上传图片并执行检测
    /// </summary>
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(IFormFile image)
    {
        if (image == null || image.Length == 0)
        {
            ModelState.AddModelError(string.Empty, "请选择要上传的图片");
            return View("Index");
        }

        try
        {
            var userId = GetUserId();
            using var stream = image.OpenReadStream();
            var record = await _detectionService.DetectAsync(stream, image.FileName, userId);

            await _logService.LogAsync(userId, "图像检测",
                $"检测结果：{record.ResultName}，置信度：{record.Confidence * 100:F2}%");

            return RedirectToAction(nameof(Result), new { id = record.Id });
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View("Index");
        }
        catch (Exception)
        {
            ModelState.AddModelError(string.Empty, "检测过程中出现错误，请重试");
            return View("Index");
        }
    }

    /// <summary>
    /// 检测结果页面
    /// </summary>
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Result(int id)
    {
        var record = await _detectionService.GetRecordByIdAsync(id);
        if (record == null) return NotFound();

        // 验证记录归属（普通用户只能查看自己的记录）
        var userId = GetUserId();
        var isAdmin = User.IsInRole("Admin");
        if (record.UserId != userId && !isAdmin)
        {
            return Forbid();
        }

        var viewModel = new DetectionResultViewModel
        {
            RecordId = record.Id,
            ImagePath = record.ImagePath,
            ResultName = record.ResultName,
            Confidence = record.Confidence,
            CreatedAt = record.CreatedAt,
            Pest = record.Pest
        };

        return View(viewModel);
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
