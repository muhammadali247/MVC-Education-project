using EduHome.Models.Common;

namespace EduHome.Models;

public class Skill : BaseSectionEntity
{
    public string Name { get; set; }
    public List<TeacherSkill> teacherSkills { get; set; }
}
