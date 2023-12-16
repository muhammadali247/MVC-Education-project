using System.ComponentModel.DataAnnotations;

namespace EduHome.Areas.Admin.ViewModels.SliderViewModel;

public class AdminSliderViewModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    [Required, MaxLength(300)]
    public string Description { get; set; }
    public string Image { get; set; }
}
