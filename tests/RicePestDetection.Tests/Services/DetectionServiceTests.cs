using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using RicePestDetection.Web.Data;
using RicePestDetection.Web.Models;
using RicePestDetection.Web.Services;

namespace RicePestDetection.Tests.Services;

/// <summary>
/// DetectionService 单元测试
/// </summary>
public class DetectionServiceTests
{
    private readonly AppDbContext _context;
    private readonly Mock<IMLPredictionService> _mlMock;
    private readonly Mock<IPestService> _pestServiceMock;
    private readonly Mock<IWebHostEnvironment> _envMock;
    private readonly Mock<IConfiguration> _configMock;
    private readonly Mock<ILogger<DetectionService>> _loggerMock;
    private readonly DetectionService _service;

    public DetectionServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);

        _mlMock = new Mock<IMLPredictionService>();
        _pestServiceMock = new Mock<IPestService>();
        _envMock = new Mock<IWebHostEnvironment>();
        _configMock = new Mock<IConfiguration>();
        _loggerMock = new Mock<ILogger<DetectionService>>();

        // 配置 WebHostEnvironment
        _envMock.Setup(e => e.WebRootPath).Returns(Path.GetTempPath());

        // 配置文件存储设置
        _configMock.Setup(c => c["FileStorage:AllowedExtensions"]).Returns(".jpg,.jpeg,.png,.bmp");

        _service = new DetectionService(
            _context,
            _mlMock.Object,
            _pestServiceMock.Object,
            _envMock.Object,
            _configMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task DetectAsync_WithValidImage_ShouldReturnRecord()
    {
        // 准备
        var imageBytes = new byte[] { 1, 2, 3, 4, 5 };
        using var stream = new MemoryStream(imageBytes);

        _mlMock.Setup(m => m.PredictAsync(It.IsAny<string>()))
            .ReturnsAsync(new DetectionPrediction
            {
                Label = "稻瘟病",
                Confidence = 0.95f
            });

        _pestServiceMock.Setup(p => p.GetByNameAsync("稻瘟病"))
            .ReturnsAsync(new Pest { Id = 1, Name = "稻瘟病" });

        // 执行
        var result = await _service.DetectAsync(stream, "test.jpg", 1);

        // 验证
        result.Should().NotBeNull();
        result.ResultName.Should().Be("稻瘟病");
        result.Confidence.Should().Be(0.95f);
        result.UserId.Should().Be(1);
    }

    [Fact]
    public async Task DetectAsync_WithInvalidExtension_ShouldThrow()
    {
        using var stream = new MemoryStream(new byte[] { 1, 2, 3 });

        Func<Task> act = () => _service.DetectAsync(stream, "test.txt", 1);

        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetRecordByIdAsync_WithValidId_ShouldReturnRecord()
    {
        // 添加用户以满足外键关系
        var user = new ApplicationUser { UserName = "testuser", Email = "test@test.com" };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // 准备测试数据
        var record = new DetectionRecord
        {
            UserId = user.Id,
            ImagePath = "/uploads/test.jpg",
            ResultName = "稻瘟病",
            Confidence = 0.9f
        };
        _context.DetectionRecords.Add(record);
        await _context.SaveChangesAsync();

        // 执行
        var result = await _service.GetRecordByIdAsync(record.Id);

        // 验证
        result.Should().NotBeNull();
        result!.ResultName.Should().Be("稻瘟病");
    }

    [Fact]
    public async Task GetRecordsByUserIdAsync_ShouldReturnUserRecords()
    {
        _context.DetectionRecords.AddRange(new List<DetectionRecord>
        {
            new() { UserId = 1, ImagePath = "/uploads/1.jpg", ResultName = "稻瘟病", Confidence = 0.9f },
            new() { UserId = 1, ImagePath = "/uploads/2.jpg", ResultName = "健康", Confidence = 0.8f },
            new() { UserId = 2, ImagePath = "/uploads/3.jpg", ResultName = "稻飞虱", Confidence = 0.7f }
        });
        await _context.SaveChangesAsync();

        var records = await _service.GetRecordsByUserIdAsync(1);
        records.Should().HaveCount(2);
        records.Should().OnlyContain(r => r.UserId == 1);
    }

    [Fact]
    public async Task GetAllRecordsAsync_ShouldReturnAllRecords()
    {
        // 添加用户以满足外键关系
        var user = new ApplicationUser { UserName = "testuser", Email = "test@test.com" };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        _context.DetectionRecords.AddRange(new List<DetectionRecord>
        {
            new() { UserId = user.Id, ImagePath = "/uploads/1.jpg", ResultName = "稻瘟病", Confidence = 0.9f },
            new() { UserId = user.Id, ImagePath = "/uploads/2.jpg", ResultName = "健康", Confidence = 0.8f }
        });
        await _context.SaveChangesAsync();

        var records = await _service.GetAllRecordsAsync();
        records.Should().HaveCount(2);
    }

    [Fact]
    public async Task DeleteRecordAsync_WithValidOwner_ShouldDelete()
    {
        _context.DetectionRecords.Add(new DetectionRecord
        {
            UserId = 1,
            ImagePath = "/uploads/test.jpg",
            ResultName = "稻瘟病",
            Confidence = 0.9f
        });
        await _context.SaveChangesAsync();

        // 创建一个空文件以便删除
        var filePath = Path.Combine(Path.GetTempPath(), "uploads", "test.jpg");
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
        File.WriteAllBytes(filePath, new byte[] { 1 });

        var result = await _service.DeleteRecordAsync(1, 1);
        result.Should().BeTrue();

        var deleted = await _service.GetRecordByIdAsync(1);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task DeleteRecordAsync_WithInvalidOwner_ShouldReturnFalse()
    {
        _context.DetectionRecords.Add(new DetectionRecord
        {
            UserId = 1,
            ImagePath = "/uploads/test.jpg",
            ResultName = "稻瘟病",
            Confidence = 0.9f
        });
        await _context.SaveChangesAsync();

        var result = await _service.DeleteRecordAsync(1, 2);
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetTotalCountAsync_ShouldReturnCorrectCount()
    {
        _context.DetectionRecords.AddRange(new List<DetectionRecord>
        {
            new() { UserId = 1, ImagePath = "/uploads/1.jpg", ResultName = "稻瘟病", Confidence = 0.9f },
            new() { UserId = 2, ImagePath = "/uploads/2.jpg", ResultName = "健康", Confidence = 0.8f }
        });
        await _context.SaveChangesAsync();

        var count = await _service.GetTotalCountAsync();
        count.Should().Be(2);
    }
}
