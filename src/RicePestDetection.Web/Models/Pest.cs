namespace RicePestDetection.Web.Models;

/// <summary>
/// 病虫害实体，存储水稻常见病虫害的知识库信息
/// </summary>
public class Pest
{
    public int Id { get; set; }

    /// <summary>
    /// 病虫害名称，如"稻瘟病"、"稻飞虱"
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 类别：Disease（病害）或 Pest（虫害）
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// 症状描述
    /// </summary>
    public string? Symptoms { get; set; }

    /// <summary>
    /// 危害描述
    /// </summary>
    public string? HarmDescription { get; set; }

    /// <summary>
    /// 防治方法
    /// </summary>
    public string? PreventionMethod { get; set; }

    /// <summary>
    /// 示例图片路径
    /// </summary>
    public string? ImagePath { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// 导航属性：该病虫害的检测记录
    /// </summary>
    public ICollection<DetectionRecord> DetectionRecords { get; set; } = new List<DetectionRecord>();
}
