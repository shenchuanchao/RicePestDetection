using RicePestDetection.Web.Models;

namespace RicePestDetection.Web.Services;

/// <summary>
/// 病虫害知识库服务接口
/// </summary>
public interface IPestService
{
    /// <summary>
    /// 获取所有病虫害（可按类别和关键词筛选）
    /// </summary>
    Task<List<Pest>> GetAllAsync(string? category = null, string? keyword = null);

    /// <summary>
    /// 根据ID获取病虫害详情
    /// </summary>
    Task<Pest?> GetByIdAsync(int id);

    /// <summary>
    /// 根据名称获取病虫害
    /// </summary>
    Task<Pest?> GetByNameAsync(string name);

    /// <summary>
    /// 新增病虫害
    /// </summary>
    Task<Pest> CreateAsync(Pest pest);

    /// <summary>
    /// 更新病虫害
    /// </summary>
    Task<bool> UpdateAsync(Pest pest);

    /// <summary>
    /// 删除病虫害
    /// </summary>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// 获取病虫害总数
    /// </summary>
    Task<int> GetTotalCountAsync();
}
