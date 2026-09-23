using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RicePestDetection.Web.Data;
using RicePestDetection.Web.Models;
using RicePestDetection.Web.Services;

namespace RicePestDetection.Tests.Services;

/// <summary>
/// PestService 单元测试
/// </summary>
public class PestServiceTests
{
    private readonly AppDbContext _context;
    private readonly PestService _service;

    public PestServiceTests()
    {
        // 使用内存数据库进行测试
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);
        _service = new PestService(_context);

        // 初始化测试数据
        SeedTestData();
    }

    private void SeedTestData()
    {
        _context.Pests.AddRange(new List<Pest>
        {
            new() { Name = "稻瘟病", Category = "Disease", Symptoms = "叶片病斑", PreventionMethod = "喷施三环唑" },
            new() { Name = "稻飞虱", Category = "Pest", Symptoms = "叶片枯黄", PreventionMethod = "喷施吡蚜酮" },
            new() { Name = "纹枯病", Category = "Disease", Symptoms = "云纹状病斑", PreventionMethod = "喷施井岗霉素" }
        });
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllPests()
    {
        var result = await _service.GetAllAsync();
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetAllAsync_WithCategoryFilter_ShouldReturnFilteredPests()
    {
        var result = await _service.GetAllAsync(category: "Disease");
        result.Should().HaveCount(2);
        result.Should().OnlyContain(p => p.Category == "Disease");
    }

    [Fact]
    public async Task GetAllAsync_WithKeyword_ShouldReturnMatchingPests()
    {
        var result = await _service.GetAllAsync(keyword: "稻瘟");
        result.Should().HaveCount(1);
        result.First().Name.Should().Be("稻瘟病");
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnPest()
    {
        var pest = await _service.GetByIdAsync(1);
        pest.Should().NotBeNull();
        pest!.Name.Should().Be("稻瘟病");
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        var pest = await _service.GetByIdAsync(999);
        pest.Should().BeNull();
    }

    [Fact]
    public async Task GetByNameAsync_WithValidName_ShouldReturnPest()
    {
        var pest = await _service.GetByNameAsync("稻飞虱");
        pest.Should().NotBeNull();
        pest!.Category.Should().Be("Pest");
    }

    [Fact]
    public async Task CreateAsync_ShouldAddPest()
    {
        var newPest = new Pest { Name = "白叶枯病", Category = "Disease", Symptoms = "叶片枯黄" };
        var result = await _service.CreateAsync(newPest);

        result.Id.Should().BeGreaterThan(0);
        var allPests = await _service.GetAllAsync();
        allPests.Should().HaveCount(4);
    }

    [Fact]
    public async Task UpdateAsync_WithValidPest_ShouldUpdate()
    {
        var pest = await _service.GetByIdAsync(1);
        pest!.Symptoms = "更新后的症状";
        var result = await _service.UpdateAsync(pest);

        result.Should().BeTrue();
        var updated = await _service.GetByIdAsync(1);
        updated!.Symptoms.Should().Be("更新后的症状");
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ShouldReturnFalse()
    {
        var pest = new Pest { Id = 999, Name = "不存在" };
        var result = await _service.UpdateAsync(pest);
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_ShouldDelete()
    {
        var result = await _service.DeleteAsync(1);
        result.Should().BeTrue();

        var deleted = await _service.GetByIdAsync(1);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ShouldReturnFalse()
    {
        var result = await _service.DeleteAsync(999);
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetTotalCountAsync_ShouldReturnCorrectCount()
    {
        var count = await _service.GetTotalCountAsync();
        count.Should().Be(3);
    }
}
