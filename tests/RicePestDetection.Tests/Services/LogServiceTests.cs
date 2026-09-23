using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RicePestDetection.Web.Data;
using RicePestDetection.Web.Models;
using RicePestDetection.Web.Services;

namespace RicePestDetection.Tests.Services;

/// <summary>
/// LogService 单元测试
/// </summary>
public class LogServiceTests
{
    private readonly AppDbContext _context;
    private readonly LogService _service;

    public LogServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);
        _service = new LogService(_context);
    }

    [Fact]
    public async Task LogAsync_ShouldAddLog()
    {
        await _service.LogAsync(1, "登录", "用户登录");

        var logs = await _service.GetAllAsync();
        logs.Should().HaveCount(1);
        logs[0].ActionType.Should().Be("登录");
        logs[0].Content.Should().Be("用户登录");
        logs[0].UserId.Should().Be(1);
    }

    [Fact]
    public async Task LogAsync_WithNullUserId_ShouldAddLog()
    {
        await _service.LogAsync(null, "访问", "匿名访问");

        var logs = await _service.GetAllAsync();
        logs.Should().HaveCount(1);
        logs[0].UserId.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnOrderedByDateDescending()
    {
        await _service.LogAsync(1, "操作1", null);
        await Task.Delay(10);
        await _service.LogAsync(1, "操作2", null);

        var logs = await _service.GetAllAsync();
        logs.Should().HaveCount(2);
        logs[0].ActionType.Should().Be("操作2");
        logs[1].ActionType.Should().Be("操作1");
    }

    [Fact]
    public async Task GetAllAsync_WithPaging_ShouldReturnCorrectPage()
    {
        for (int i = 0; i < 35; i++)
        {
            await _service.LogAsync(1, $"操作{i}", null);
        }

        var page1 = await _service.GetAllAsync(page: 1, pageSize: 20);
        page1.Should().HaveCount(20);

        var page2 = await _service.GetAllAsync(page: 2, pageSize: 20);
        page2.Should().HaveCount(15);
    }

    [Fact]
    public async Task GetTotalCountAsync_ShouldReturnCorrectCount()
    {
        await _service.LogAsync(1, "操作1", null);
        await _service.LogAsync(2, "操作2", null);

        var count = await _service.GetTotalCountAsync();
        count.Should().Be(2);
    }
}
