using RicePestDetection.Web.Models;

namespace RicePestDetection.Web.Services;

/// <summary>
/// 检测服务接口
/// </summary>
public interface IDetectionService
{
    /// <summary>
    /// 保存图片并执行检测，返回检测记录
    /// </summary>
    /// <param name="imageStream">图片流</param>
    /// <param name="fileName">文件名</param>
    /// <param name="userId">用户ID</param>
    /// <returns>检测记录</returns>
    Task<DetectionRecord> DetectAsync(Stream imageStream, string fileName, int userId);

    /// <summary>
    /// 根据ID获取检测记录
    /// </summary>
    Task<DetectionRecord?> GetRecordByIdAsync(int id);

    /// <summary>
    /// 获取用户的所有检测记录
    /// </summary>
    Task<List<DetectionRecord>> GetRecordsByUserIdAsync(int userId);

    /// <summary>
    /// 获取所有检测记录（管理员）
    /// </summary>
    Task<List<DetectionRecord>> GetAllRecordsAsync();

    /// <summary>
    /// 删除检测记录
    /// </summary>
    Task<bool> DeleteRecordAsync(int id, int userId);

    /// <summary>
    /// 获取检测总次数
    /// </summary>
    Task<int> GetTotalCountAsync();
}
