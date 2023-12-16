using System.ComponentModel.DataAnnotations;

namespace EduHome.ViewModels.AuthViewModel;

public class ForgotPasswordViewModel
{
    [Required, DataType(DataType.EmailAddress)]
    public string Email { get; set; }
}
