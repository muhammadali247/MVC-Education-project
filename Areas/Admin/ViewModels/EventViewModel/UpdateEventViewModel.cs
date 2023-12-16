namespace EduHome.Areas.Admin.ViewModels.EventViewModel;

public class UpdateEventViewModel
{
    public int Id { get; set; }
    public IFormFile? Image { get; set; }
    public string Name { get; set; }
    public string Time { get; set; }
    public string Venue { get; set; }
    public string Description { get; set; }
}
