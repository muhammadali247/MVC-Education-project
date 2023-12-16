using EduHome.Models.Common;

namespace EduHome.Models;

public class Blog : BaseSectionEntity
{
    public string Image { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int CommentCount { get; set; }
}
