namespace RicePestDetection.Web.Models;

/// <summary>
/// 系统日志实体，记录关键操作
/// </summary>
public class Log
{
    public int Id { get; set; }

    /// <summary>
    /// 外键：操作人ID（可为空，如未登录的操作）
    /// </summary>
    public int? UserId { get; set; }

    /// <summary>
    /// 导航属性：操作人
    /// </summary>
    public ApplicationUser? User { get; set; }

    /// <summary>
    /// 操作类型，如"登录"、"新增病虫害"、"删除记录"等
    /// </summary>
    public string ActionType { get; set; } = string.Empty;

    /// <summary>
    /// 操作内容描述
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// 操作时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
