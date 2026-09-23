using Microsoft.EntityFrameworkCore;
using RicePestDetection.Web.Data;
using RicePestDetection.Web.Models;

namespace RicePestDetection.Web.Services;

/// <summary>
/// 病虫害知识库服务实现
/// </summary>
public class PestService : IPestService
{
    private readonly AppDbContext _context;

    public PestService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 获取所有病虫害（可按类别和关键词筛选）
    /// </summary>
    public async Task<List<Pest>> GetAllAsync(string? category = null, string? keyword = null)
    {
        var query = _context.Pests.AsQueryable();

        if (!string.IsNullOrEmpty(category))
        {
            query = query.Where(p => p.Category == category);
        }

        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(p =>
                p.Name.Contains(keyword) ||
                (p.Symptoms != null && p.Symptoms.Contains(keyword)) ||
                (p.PreventionMethod != null && p.PreventionMethod.Contains(keyword)));
        }

        return await query.OrderBy(p => p.Name).ToListAsync();
    }

    /// <summary>
    /// 根据ID获取病虫害详情
    /// </summary>
    public async Task<Pest?> GetByIdAsync(int id)
    {
        return await _context.Pests.FindAsync(id);
    }

    /// <summary>
    /// 根据名称获取病虫害
    /// </summary>
    public async Task<Pest?> GetByNameAsync(string name)
    {
        return await _context.Pests.FirstOrDefaultAsync(p => p.Name == name);
    }

    /// <summary>
    /// 新增病虫害
    /// </summary>
    public async Task<Pest> CreateAsync(Pest pest)
    {
        pest.CreatedAt = DateTime.Now;
        _context.Pests.Add(pest);
        await _context.SaveChangesAsync();
        return pest;
    }

    /// <summary>
    /// 更新病虫害
    /// </summary>
    public async Task<bool> UpdateAsync(Pest pest)
    {
        var existing = await _context.Pests.FindAsync(pest.Id);
        if (existing == null) return false;

        existing.Name = pest.Name;
        existing.Category = pest.Category;
        existing.Symptoms = pest.Symptoms;
        existing.HarmDescription = pest.HarmDescription;
        existing.PreventionMethod = pest.PreventionMethod;
        existing.ImagePath = pest.ImagePath;

        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// 删除病虫害
    /// </summary>
    public async Task<bool> DeleteAsync(int id)
    {
        var pest = await _context.Pests.FindAsync(id);
        if (pest == null) return false;

        _context.Pests.Remove(pest);
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// 获取病虫害总数
    /// </summary>
    public async Task<int> GetTotalCountAsync()
    {
        return await _context.Pests.CountAsync();
    }
}
