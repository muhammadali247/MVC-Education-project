using EduHome.Models.Common;

namespace EduHome.Models;

public class SocialMedia : BaseSectionEntity
{
    public int TeacherId { get; set; }
    public Teacher Teacher { get; set;}
    public string Icon { get; set; }
    public string Link { get; set; }
}
