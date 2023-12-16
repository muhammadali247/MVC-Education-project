using EduHome.Models.Common;

namespace EduHome.Models;

public class Category : BaseSectionEntity
{
    public string Name { get; set; }
    public List<CourseCategory> CourseCategories { get; set; }
}
