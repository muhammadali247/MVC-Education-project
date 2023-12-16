using System.ComponentModel.DataAnnotations;

namespace EduHome.ViewModels.AuthViewModel;

public class ResetPasswordViewModel
{
    [Required, DataType(DataType.Password)]
    public string NewPassword { get; set; }
    [Required, DataType(DataType.Password), Compare(nameof(NewPassword))]
    public string ConfirmNewPassword { get; set; }
}
