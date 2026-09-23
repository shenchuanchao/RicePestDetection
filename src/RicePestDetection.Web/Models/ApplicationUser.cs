using Microsoft.AspNetCore.Identity;

namespace RicePestDetection.Web.Models;

/// <summary>
/// 系统用户实体，继承自 IdentityUser，扩展手机号、角色等字段
/// </summary>
public class ApplicationUser : IdentityUser<int>
{
    /// <summary>
    /// 用户昵称
    /// </summary>
    public string? NickName { get; set; }

    /// <summary>
    /// 账号是否启用
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// 导航属性：用户的检测记录
    /// </summary>
    public ICollection<DetectionRecord> DetectionRecords { get; set; } = new List<DetectionRecord>();

    /// <summary>
    /// 导航属性：用户的操作日志
    /// </summary>
    public ICollection<Log> Logs { get; set; } = new List<Log>();
}
