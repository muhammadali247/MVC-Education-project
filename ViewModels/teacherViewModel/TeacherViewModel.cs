using EduHome.Models;

namespace EduHome.ViewModels.teacherViewModel;

public class TeacherViewModel
{
    public int Id { get; set; }
    public string Image { get; set; }
    public string Name { get; set; }
    public string Duty { get; set; }
    public string Icon { get; set; }
    public string Link { get; set; }
    public List<SocialMedia>? SocialMedias { get; set; }
}
