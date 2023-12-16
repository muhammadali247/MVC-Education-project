using AutoMapper;
using EduHome.Areas.Admin.ViewModels.CourseViewModel;
using EduHome.Models;
using EduHome.ViewModels.CourseViewModel;

namespace EduHome.Mappers;

public class CourseMapperProfile : Profile
{
	public CourseMapperProfile()
	{
		CreateMap<Course,CourseViewModel>()
			//.ForMember(cvm => cvm.CategoryNames, x => x.MapFrom(c => c.CourseCategories.Select(cs => cs.Category.Name.Select(n => n))))
   //         .ForMember(cvm => cvm.Categories, x => x.MapFrom(c => c.CourseCategories.Select(cs => cs.Category)))
            .ReverseMap();
		CreateMap<Course,CourseDetailViewModel>()
            .ForMember(cvm => cvm.Categories, x => x.MapFrom(c => c.CourseCategories.Select(cs => cs.Category)))
            .ReverseMap();
		CreateMap<Course, CreateCourseViewModel>()
            //.ForMember(cvm => cvm.Categories,x => x.MapFrom(c => c.CourseCategories.Select(cs => cs.Course))) /*a*/ 
            .ForMember(cvm => cvm.CategoryId, x => x.MapFrom(c => c.CourseCategories.Select(cs => cs.Category.Id)))
            .ReverseMap();
		CreateMap<Course, UpdateCourseViewModel>()
			.ForMember(uvm => uvm.Image,x => x.Ignore())
			.ReverseMap();
		CreateMap<Course, CourseAdminViewModel>().ReverseMap();
		CreateMap<Course, DeleteCourseViewModel>().ReverseMap();
		CreateMap<Course, AdminCourseDetailViewModel>().ReverseMap();
	}
}
