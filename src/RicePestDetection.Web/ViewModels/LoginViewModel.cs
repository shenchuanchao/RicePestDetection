using System.ComponentModel.DataAnnotations;

namespace RicePestDetection.Web.ViewModels;

/// <summary>
/// 登录视图模型
/// </summary>
public class LoginViewModel
{
    [Required(ErrorMessage = "请输入用户名")]
    [Display(Name = "用户名")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "请输入密码")]
    [DataType(DataType.Password)]
    [Display(Name = "密码")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "记住我")]
    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
}
