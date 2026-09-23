using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RicePestDetection.Web.Models;
using RicePestDetection.Web.Services;
using RicePestDetection.Web.ViewModels;

namespace RicePestDetection.Web.Controllers;

/// <summary>
/// 病虫害知识库控制器
/// </summary>
public class PestController : Controller
{
    private readonly IPestService _pestService;
    private readonly ILogService _logService;
    private readonly IWebHostEnvironment _environment;

    public PestController(IPestService pestService, ILogService logService, IWebHostEnvironment environment)
    {
        _pestService = pestService;
        _logService = logService;
        _environment = environment;
    }

    /// <summary>
    /// 病虫害列表（支持按类别和关键词筛选）
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index(string? category, string? keyword)
    {
        var pests = await _pestService.GetAllAsync(category, keyword);
        ViewData["Category"] = category;
        ViewData["Keyword"] = keyword;
        return View(pests);
    }

    /// <summary>
    /// 病虫害详情
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var pest = await _pestService.GetByIdAsync(id);
        if (pest == null) return NotFound();
        return View(pest);
    }

    /// <summary>
    /// 新增病虫害页面（管理员）
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View();
    }

    /// <summary>
    /// 新增病虫害提交（管理员）
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PestViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var pest = new Pest
        {
            Name = model.Name,
            Category = model.Category,
            Symptoms = model.Symptoms,
            HarmDescription = model.HarmDescription,
            PreventionMethod = model.PreventionMethod,
            ImagePath = await SaveImageAsync(model.ImageFile)
        };

        await _pestService.CreateAsync(pest);
        var user = HttpContext.User;
        await _logService.LogAsync(GetUserId(user), "新增病虫害", $"新增病虫害：{pest.Name}");

        TempData["Success"] = "病虫害信息添加成功";
        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// 编辑病虫害页面（管理员）
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var pest = await _pestService.GetByIdAsync(id);
        if (pest == null) return NotFound();

        var model = new PestViewModel
        {
            Id = pest.Id,
            Name = pest.Name,
            Category = pest.Category,
            Symptoms = pest.Symptoms,
            HarmDescription = pest.HarmDescription,
            PreventionMethod = pest.PreventionMethod,
            ImagePath = pest.ImagePath
        };

        return View(model);
    }

    /// <summary>
    /// 编辑病虫害提交（管理员）
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(PestViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var pest = new Pest
        {
            Id = model.Id,
            Name = model.Name,
            Category = model.Category,
            Symptoms = model.Symptoms,
            HarmDescription = model.HarmDescription,
            PreventionMethod = model.PreventionMethod,
            ImagePath = model.ImagePath
        };

        // 如果上传了新图片，保存并更新路径
        if (model.ImageFile != null && model.ImageFile.Length > 0)
        {
            pest.ImagePath = await SaveImageAsync(model.ImageFile);
        }

        var result = await _pestService.UpdateAsync(pest);
        if (result)
        {
            await _logService.LogAsync(GetUserId(HttpContext.User), "编辑病虫害", $"编辑病虫害：{pest.Name}");
            TempData["Success"] = "病虫害信息更新成功";
        }
        else
        {
            TempData["Error"] = "更新失败，病虫害不存在";
        }

        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// 删除病虫害（管理员）
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var pest = await _pestService.GetByIdAsync(id);
        var result = await _pestService.DeleteAsync(id);
        if (result && pest != null)
        {
            await _logService.LogAsync(GetUserId(HttpContext.User), "删除病虫害", $"删除病虫害：{pest.Name}");
            TempData["Success"] = "病虫害信息删除成功";
        }
        else
        {
            TempData["Error"] = "删除失败";
        }

        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// 保存上传的图片
    /// </summary>
    private async Task<string?> SaveImageAsync(IFormFile? file)
    {
        if (file == null || file.Length == 0) return null;

        var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "pests");
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return $"/uploads/pests/{fileName}";
    }

    /// <summary>
    /// 获取当前用户ID
    /// </summary>
    private int? GetUserId(System.Security.Claims.ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        return userIdClaim != null && int.TryParse(userIdClaim.Value, out var id) ? id : null;
    }
}
