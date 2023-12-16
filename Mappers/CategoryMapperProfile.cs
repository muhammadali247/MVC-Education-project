using AutoMapper;
using EduHome.Areas.Admin.ViewModels.CategoryViewModel;
using EduHome.Models;

namespace EduHome.Mappers;

public class CategoryMapperProfile : Profile
{
	public CategoryMapperProfile()
	{
		CreateMap<Category, CategoryAdminViewModel>().ReverseMap();
		CreateMap<Category, CreateCategoryViewModel>().ReverseMap();
		CreateMap<Category, UpdateCategoryViewModel>().ReverseMap();
        CreateMap<Category, DeleteCategoryViewModel>().ReverseMap();
        CreateMap<Category, CategoryDetailViewModel>().ReverseMap();
    }
}
