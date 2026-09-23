using RicePestDetection.Web.Models;

namespace RicePestDetection.Web.ViewModels;

/// <summary>
/// 检测结果视图模型
/// </summary>
public class DetectionResultViewModel
{
    public int RecordId { get; set; }
    public string ImagePath { get; set; } = string.Empty;
    public string ResultName { get; set; } = string.Empty;
    public float Confidence { get; set; }
    public string ConfidencePercent => $"{Confidence * 100:F2}%";
    public DateTime CreatedAt { get; set; }
    public Pest? Pest { get; set; }
    public Dictionary<string, float> Scores { get; set; } = new();
}
