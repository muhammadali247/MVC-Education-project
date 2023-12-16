using EduHome.Models.Common;

namespace EduHome.Models;

public class Speaker : BaseSectionEntity
{
    public string Image { get; set; }
    public string Name { get; set; }
    public string Duty { get; set; }
    public List<EventSpeaker> EventSpeakers { get; set; }
}
