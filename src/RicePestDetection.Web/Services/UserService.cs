using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RicePestDetection.Web.Data;
using RicePestDetection.Web.Models;

namespace RicePestDetection.Web.Services;

/// <summary>
/// 用户服务实现
/// </summary>
public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AppDbContext _context;

    public UserService(UserManager<ApplicationUser> userManager, AppDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    /// <summary>
    /// 获取所有用户列表
    /// </summary>
    public async Task<List<ApplicationUser>> GetAllUsersAsync()
    {
        return await _userManager.Users.OrderByDescending(u => u.CreatedAt).ToListAsync();
    }

    /// <summary>
    /// 根据ID获取用户
    /// </summary>
    public async Task<ApplicationUser?> GetUserByIdAsync(int userId)
    {
        return await _userManager.FindByIdAsync(userId.ToString());
    }

    /// <summary>
    /// 切换用户启用状态
    /// </summary>
    public async Task<bool> ToggleActiveAsync(int userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;

        user.IsActive = !user.IsActive;
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    /// <summary>
    /// 修改用户密码
    /// </summary>
    public async Task<IdentityResult> ChangePasswordAsync(int userId, string oldPassword, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "用户不存在" });
        }

        return await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
    }

    /// <summary>
    /// 获取用户总数
    /// </summary>
    public async Task<int> GetTotalCountAsync()
    {
        return await _userManager.Users.CountAsync();
    }
}
