using EduHome.Models;
using System.ComponentModel.DataAnnotations;

namespace EduHome.ViewModels.CourseViewModel;

public class CourseViewModel
{
    public int Id { get; set; }
    public string Image { get; set; }
    [Required, MaxLength(75)]
    public string Name { get; set; }
    [Required, MaxLength(175)]
    public string Description { get; set; }
    public List<Category> Categories { get; set; }
    //public List<CourseCategory> CourseCategories { get; set; }
    //public List<string> CategoryNames { get; set; }
}
