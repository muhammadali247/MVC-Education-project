using EduHome.Models.Common;

namespace EduHome.Models;

public class Event : BaseSectionEntity
{
    public string Image { get; set; }
    public string Name { get; set; }
    public string Time { get; set; }
    public string Venue { get; set; }
    public string Description { get; set; }
    public List<EventSpeaker> EventSpeakers { get; set; }
}
