using Microsoft.EntityFrameworkCore;
using RicePestDetection.Web.Data;
using RicePestDetection.Web.Models;

namespace RicePestDetection.Web.Services;

/// <summary>
/// 检测服务实现，负责图片保存、调用ML推理、保存检测记录
/// </summary>
public class DetectionService : IDetectionService
{
    private readonly AppDbContext _context;
    private readonly IMLPredictionService _mlPredictionService;
    private readonly IPestService _pestService;
    private readonly IWebHostEnvironment _environment;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DetectionService> _logger;

    public DetectionService(
        AppDbContext context,
        IMLPredictionService mlPredictionService,
        IPestService pestService,
        IWebHostEnvironment environment,
        IConfiguration configuration,
        ILogger<DetectionService> logger)
    {
        _context = context;
        _mlPredictionService = mlPredictionService;
        _pestService = pestService;
        _environment = environment;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// 保存图片并执行检测
    /// </summary>
    public async Task<DetectionRecord> DetectAsync(Stream imageStream, string fileName, int userId)
    {
        // 1. 校验文件扩展名
        var allowedExtensions = (_configuration["FileStorage:AllowedExtensions"] ?? ".jpg,.jpeg,.png,.bmp")
            .Split(',', StringSplitOptions.RemoveEmptyEntries);
        var extension = Path.GetExtension(fileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
        {
            throw new ArgumentException($"不支持的文件格式，仅支持：{string.Join("、", allowedExtensions)}");
        }

        // 2. 保存图片到服务器
        var uploadFolder = Path.Combine(_environment.WebRootPath, "uploads");
        if (!Directory.Exists(uploadFolder))
        {
            Directory.CreateDirectory(uploadFolder);
        }

        var uniqueFileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadFolder, uniqueFileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await imageStream.CopyToAsync(fileStream);
        }

        _logger.LogInformation("图片已保存：{FilePath}", filePath);

        // 3. 调用ML推理服务
        var relativePath = $"/uploads/{uniqueFileName}";
        var prediction = await _mlPredictionService.PredictAsync(filePath);

        // 4. 根据预测结果匹配病虫害知识库
        var pest = await _pestService.GetByNameAsync(prediction.Label);
        var pestId = pest?.Id;

        // 5. 保存检测记录
        var record = new DetectionRecord
        {
            UserId = userId,
            PestId = pestId,
            ImagePath = relativePath,
            ResultName = prediction.Label,
            Confidence = prediction.Confidence,
            CreatedAt = DateTime.Now
        };

        _context.DetectionRecords.Add(record);
        await _context.SaveChangesAsync();

        _logger.LogInformation("检测完成：{Result}，置信度：{Confidence}", prediction.Label, prediction.Confidence);
        return record;
    }

    /// <summary>
    /// 根据ID获取检测记录
    /// </summary>
    public async Task<DetectionRecord?> GetRecordByIdAsync(int id)
    {
        return await _context.DetectionRecords
            .Include(r => r.Pest)
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    /// <summary>
    /// 获取用户的所有检测记录
    /// </summary>
    public async Task<List<DetectionRecord>> GetRecordsByUserIdAsync(int userId)
    {
        return await _context.DetectionRecords
            .Include(r => r.Pest)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// 获取所有检测记录（管理员）
    /// </summary>
    public async Task<List<DetectionRecord>> GetAllRecordsAsync()
    {
        return await _context.DetectionRecords
            .Include(r => r.Pest)
            .Include(r => r.User)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// 删除检测记录
    /// </summary>
    public async Task<bool> DeleteRecordAsync(int id, int userId)
    {
        var record = await _context.DetectionRecords.FindAsync(id);
        if (record == null || record.UserId != userId) return false;

        // 删除图片文件
        var filePath = Path.Combine(_environment.WebRootPath, record.ImagePath.TrimStart('/'));
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        _context.DetectionRecords.Remove(record);
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// 获取检测总次数
    /// </summary>
    public async Task<int> GetTotalCountAsync()
    {
        return await _context.DetectionRecords.CountAsync();
    }
}
