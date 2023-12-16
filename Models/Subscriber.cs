using EduHome.Models.Common;

namespace EduHome.Models;

public class Subscriber : BaseEntity
{
    public string Email { get; set; }
    public bool IsActive { get; set; }
}
