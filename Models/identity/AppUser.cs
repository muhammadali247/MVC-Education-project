using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace EduHome.Models.Identity;

public class AppUser : IdentityUser
{
    [Required, MaxLength(256)]
    public string Fullname { get; set; }
    public bool isActive { get; set; }
    //public string TwoFactorCode { get; set; }
    //public bool IsTwoFactorEnabled { get; set; }
}