namespace EduHome.Areas.Admin.ViewModels.SpeakerViewModel;

public class UpdateSpeakerViewModel
{
    public int Id { get; set; }
    public IFormFile? Image { get; set; }
    public string Name { get; set; }
    public string Duty { get; set; }
}
