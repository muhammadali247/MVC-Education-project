using EduHome.Models.Common;

namespace EduHome.Models;

public class Teacher : BaseSectionEntity
{
    public string Image { get; set; }
    public string Name { get; set; }
    public string Duty { get; set; }
    public string About { get; set; }
    public string Degree { get; set; }
    public string Experience { get; set; }
    public string Hobbies { get; set; }
    public string Faculty { get; set; }
    public string Mail { get; set; }
    public string PhoneNumber { get; set; }
    public string Skype { get; set; }
    public List<TeacherSkill> teacherSkills { get; set; }
    public List<SocialMedia>? SocialMedias { get; set; }
}
