namespace RicePestDetection.Web.Services;

/// <summary>
/// ML推理服务接口，负责加载ONNX模型并对图片进行分类推理
/// </summary>
public interface IMLPredictionService
{
    /// <summary>
    /// 对指定路径的图片进行推理
    /// </summary>
    /// <param name="imagePath">图片文件路径</param>
    /// <returns>预测结果</returns>
    Task<DetectionPrediction> PredictAsync(string imagePath);
}

/// <summary>
/// 检测预测结果
/// </summary>
public class DetectionPrediction
{
    /// <summary>
    /// 预测的病虫害名称
    /// </summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// 置信度（0-1）
    /// </summary>
    public float Confidence { get; set; }

    /// <summary>
    /// 各类别得分
    /// </summary>
    public Dictionary<string, float> Scores { get; set; } = new();
}
