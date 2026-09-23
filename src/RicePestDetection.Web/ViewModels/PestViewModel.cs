using System.ComponentModel.DataAnnotations;

namespace RicePestDetection.Web.ViewModels;

/// <summary>
/// 病虫害视图模型
/// </summary>
public class PestViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "请输入名称")]
    [Display(Name = "名称")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "请选择类别")]
    [Display(Name = "类别")]
    public string Category { get; set; } = string.Empty;

    [Display(Name = "症状描述")]
    public string? Symptoms { get; set; }

    [Display(Name = "危害描述")]
    public string? HarmDescription { get; set; }

    [Display(Name = "防治方法")]
    public string? PreventionMethod { get; set; }

    [Display(Name = "示例图片")]
    public IFormFile? ImageFile { get; set; }

    public string? ImagePath { get; set; }
}
