namespace EduHome.Areas.Admin.ViewModels.SliderViewModel;

public class UpdateSliderViewModel
{
    public int Id { get; set; }
    public IFormFile? Image { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
}
