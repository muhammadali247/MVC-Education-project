namespace EduHome.Models.Common;

public class BaseSectionEntity : BaseEntity
{
    public bool IsDeleted { get; set; }
    public DateTime CreatedTime { get; set; }
    public string CreatedBy { get; set; }
    public DateTime UpdatedTime { get; set; }
    public string UpdatedBy { get; set; }
}
