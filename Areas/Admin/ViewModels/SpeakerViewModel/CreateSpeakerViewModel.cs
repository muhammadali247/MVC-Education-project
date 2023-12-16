namespace EduHome.Areas.Admin.ViewModels.SpeakerViewModel;

public class CreateSpeakerViewModel
{
    public int Id { get; set; }
    public IFormFile Image { get; set; }
    public string Name { get; set; }
    public string Duty { get; set; }
}

