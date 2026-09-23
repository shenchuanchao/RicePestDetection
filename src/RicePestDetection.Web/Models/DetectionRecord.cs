namespace RicePestDetection.Web.Models;

/// <summary>
/// 检测记录实体，记录每次图像检测的结果
/// </summary>
public class DetectionRecord
{
    public int Id { get; set; }

    /// <summary>
    /// 外键：用户ID
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 导航属性：所属用户
    /// </summary>
    public ApplicationUser? User { get; set; }

    /// <summary>
    /// 外键：识别出的病虫害ID（可为空，当置信度过低时无匹配）
    /// </summary>
    public int? PestId { get; set; }

    /// <summary>
    /// 导航属性：识别出的病虫害
    /// </summary>
    public Pest? Pest { get; set; }

    /// <summary>
    /// 上传图片的存储路径
    /// </summary>
    public string ImagePath { get; set; } = string.Empty;

    /// <summary>
    /// 识别结果名称（病虫害名称）
    /// </summary>
    public string ResultName { get; set; } = string.Empty;

    /// <summary>
    /// 置信度（0-1之间）
    /// </summary>
    public float Confidence { get; set; }

    /// <summary>
    /// 检测时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
