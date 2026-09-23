using System.ComponentModel.DataAnnotations;

namespace RicePestDetection.Web.ViewModels;

/// <summary>
/// 注册视图模型
/// </summary>
public class RegisterViewModel
{
    [Required(ErrorMessage = "请输入用户名")]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "用户名长度为3-20个字符")]
    [Display(Name = "用户名")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "请输入邮箱")]
    [EmailAddress(ErrorMessage = "邮箱格式不正确")]
    [Display(Name = "邮箱")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "请输入密码")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "密码长度至少6位")]
    [DataType(DataType.Password)]
    [Display(Name = "密码")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "请确认密码")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "两次输入的密码不一致")]
    [Display(Name = "确认密码")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Display(Name = "昵称")]
    public string? NickName { get; set; }

    [Display(Name = "手机号")]
    [Phone(ErrorMessage = "手机号格式不正确")]
    public string? Phone { get; set; }
}
