using Microsoft.AspNetCore.Identity;
using RicePestDetection.Web.Models;

namespace RicePestDetection.Web.Services;

/// <summary>
/// 用户服务接口
/// </summary>
public interface IUserService
{
    /// <summary>
    /// 获取所有用户列表
    /// </summary>
    Task<List<ApplicationUser>> GetAllUsersAsync();

    /// <summary>
    /// 根据ID获取用户
    /// </summary>
    Task<ApplicationUser?> GetUserByIdAsync(int userId);

    /// <summary>
    /// 切换用户启用状态
    /// </summary>
    Task<bool> ToggleActiveAsync(int userId);

    /// <summary>
    /// 修改用户密码
    /// </summary>
    Task<IdentityResult> ChangePasswordAsync(int userId, string oldPassword, string newPassword);

    /// <summary>
    /// 获取用户总数
    /// </summary>
    Task<int> GetTotalCountAsync();
}
