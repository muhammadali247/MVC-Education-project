namespace EduHome.Areas.Admin.ViewModels.BlogViewModel;

public class CreateBlogViewModel
{
    public IFormFile Image { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int CommentCount { get; set; }
}
