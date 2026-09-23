using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RicePestDetection.Web.Services;

namespace RicePestDetection.Web.Controllers;

/// <summary>
/// 系统管理控制器（管理员）
/// </summary>
[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IUserService _userService;
    private readonly ILogService _logService;

    public AdminController(IUserService userService, ILogService logService)
    {
        _userService = userService;
        _logService = logService;
    }

    /// <summary>
    /// 用户管理列表
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Users()
    {
        var users = await _userService.GetAllUsersAsync();
        return View(users);
    }

    /// <summary>
    /// 切换用户启用状态
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        var result = await _userService.ToggleActiveAsync(id);

        if (result && user != null)
        {
            var action = user.IsActive ? "禁用" : "启用";
            await _logService.LogAsync(GetUserId(), "用户管理", $"{action}用户：{user.UserName}");
            TempData["Success"] = $"已{action}用户 {user.UserName}";
        }
        else
        {
            TempData["Error"] = "操作失败";
        }

        return RedirectToAction(nameof(Users));
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
