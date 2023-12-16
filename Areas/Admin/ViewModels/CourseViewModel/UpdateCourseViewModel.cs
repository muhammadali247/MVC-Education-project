using System.ComponentModel.DataAnnotations;

namespace EduHome.Areas.Admin.ViewModels.CourseViewModel;

public class UpdateCourseViewModel
{
    public int Id { get; set; }
    public IFormFile? Image { get; set; }
    [Required, MaxLength(75)]
    public string Name { get; set; }
    [Required, MaxLength(175)]
    public string Description { get; set; }
    public string Starts { get; set; }
    public string Duration { get; set; }
    public string ClassDuration { get; set; }
    public string SkillLevel { get; set; }
    public string Language { get; set; }
    public ushort Students { get; set; }
    public string Assesments { get; set; }
    public ushort CourseFee { get; set; }
}
