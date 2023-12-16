using EduHome.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace EduHome.Models;

public class Slider : BaseSectionEntity
{
    [Required, MaxLength(120)]
    public string Title { get; set; }
    [Required, MaxLength(300)]
    public string Description { get; set; }
    public string Image { get; set; }
}
