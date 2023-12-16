using AutoMapper;
using EduHome.Areas.Admin.ViewModels.SpeakerViewModel;
using EduHome.Models;

namespace EduHome.Mappers;

public class SpeakerMapperProfile : Profile
{
	public SpeakerMapperProfile()
	{
		CreateMap<Speaker, AdminSpeakerViewModel>().ReverseMap();
        CreateMap<Speaker, CreateSpeakerViewModel>().ReverseMap();
        CreateMap<Speaker, UpdateSpeakerViewModel>()
			.ForMember(usm => usm.Image,x => x.Ignore())
            .ReverseMap();
        CreateMap<Speaker, DeleteSpeakerViewModel>().ReverseMap();
        CreateMap<Speaker, SpeakerDetailViewModel>().ReverseMap();
    }
}
