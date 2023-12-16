using AutoMapper;
using EduHome.Areas.Admin.ViewModels.SliderViewModel;
using EduHome.Models;

namespace EduHome.Mappers;

public class SliderMapperProfile : Profile
{
	public SliderMapperProfile()
	{
		CreateMap<Slider, AdminSliderViewModel>().ReverseMap();
		CreateMap<Slider, CreateSliderViewModel>().ReverseMap();
		CreateMap<Slider, UpdateSliderViewModel>()
            .ForMember(usm => usm.Image, x => x.Ignore())
            .ReverseMap();
		CreateMap<Slider, DeleteSliderViewModel>().ReverseMap();
		CreateMap<Slider, SliderDetailViewModel>().ReverseMap();
	}
}
