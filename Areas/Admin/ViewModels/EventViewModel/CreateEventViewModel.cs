using EduHome.Models;

namespace EduHome.Areas.Admin.ViewModels.EventViewModel;

public class CreateEventViewModel
{
    public IFormFile Image { get; set; }
    public string Name { get; set; }
    public string Time { get; set; }
    public string Venue { get; set; }
    public string Description { get; set; }
    public List<int> SpeakerId { get; set; }
}
