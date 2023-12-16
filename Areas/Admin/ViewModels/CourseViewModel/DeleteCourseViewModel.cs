using System.ComponentModel.DataAnnotations;

namespace EduHome.Areas.Admin.ViewModels.CourseViewModel;

public class DeleteCourseViewModel
{
    public int Id { get; set; }
    public string Image { get; set; }
    [Required, MaxLength(75)]
    public string Name { get; set; }
    [Required, MaxLength(175)]
    public string Description { get; set; }
    [Required, MaxLength(175)]
    public string Language { get; set; }
    public ushort Students { get; set; }
    public ushort CourseFee { get; set; }
}
