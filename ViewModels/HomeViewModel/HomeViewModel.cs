using EduHome.Models;

namespace EduHome.ViewModels.HomeViewModel;

public class HomeViewModel
{
    public IEnumerable<Slider> Sliders { get; set; }
    public IEnumerable<Course> Courses { get; set; }
    public IEnumerable<Event> Events { get; set; }
    public IEnumerable<Blog> Blogs { get; set; }
}
