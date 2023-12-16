using AutoMapper;
using EduHome.Areas.Admin.ViewModels.BlogViewModel;
using EduHome.Models;
using EduHome.ViewModels.BlogViewModel;

namespace EduHome.Mappers;

public class BlogMapperProfile : Profile
{
	public BlogMapperProfile()
	{
        CreateMap<Blog, BlogViewModel>().ReverseMap();
        CreateMap<CreateBlogViewModel, Blog>().ReverseMap();
        CreateMap<Blog,BlogAdsViewModel>().ReverseMap();
        CreateMap<Blog,UpdateBlogViewModel>()
            .ForMember(pvm => pvm.Image,x => x.Ignore())
            .ReverseMap();
        CreateMap<Blog, DeleteBlogViewModel>().ReverseMap();
        CreateMap<Blog, BlogDetailViewModel>().ReverseMap();
    }
}
