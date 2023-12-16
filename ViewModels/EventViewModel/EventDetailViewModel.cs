    using EduHome.Models;

namespace EduHome.ViewModels.EventViewModel;

public class EventDetailViewModel
{
    public string Image { get; set; }
    public string Name { get; set; }
    public string Time { get; set; }
    public string Venue { get; set; }
    public string Description { get; set; }
    //public List<string> speakerNames { get; set; }
    //public List<string> speakerDuty { get; set; }
    //public List<string> speakerImage { get; set; }
    public List<Speaker> Speakers { get; set; }
}
