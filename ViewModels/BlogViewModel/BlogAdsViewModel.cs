namespace EduHome.ViewModels.BlogViewModel;

public class BlogAdsViewModel
{
    public int Id { get; set; }
    public string? Image { get; set; }
    public string Title { get; set; }
    public DateTime CreatedTime { get; set; }
    public string CreatedBy { get; set; }
    public int CommentCount { get; set; }
}