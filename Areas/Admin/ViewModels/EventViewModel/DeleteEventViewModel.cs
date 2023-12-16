using System.ComponentModel.DataAnnotations;

namespace EduHome.Areas.Admin.ViewModels.EventViewModel;

public class DeleteEventViewModel
{
    public int id { get; set; }
    public string Image { get; set; }
    public string Name { get; set; }
    public string Time { get; set; }
    public string Venue { get; set; }
}
