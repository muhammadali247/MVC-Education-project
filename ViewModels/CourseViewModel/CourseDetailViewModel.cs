using EduHome.Models;
using System.ComponentModel.DataAnnotations;

namespace EduHome.ViewModels.CourseViewModel;

public class CourseDetailViewModel
{
    public string Image { get; set; }
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
    public string CourseFee { get; set; }
    public List<Category> Categories { get; set; }
    public List<CourseCategory> CourseCategories { get; set; }
}
