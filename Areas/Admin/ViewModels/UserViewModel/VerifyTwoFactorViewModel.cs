using System.ComponentModel.DataAnnotations;

namespace EduHome.Areas.Admin.ViewModels.UserViewModel;

public class VerifyTwoFactorViewModel
{
    [Required]
    public string UserId { get; set; }

    [Required]
    public string Code { get; set; }
}