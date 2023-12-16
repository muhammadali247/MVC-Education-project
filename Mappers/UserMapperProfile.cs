using AutoMapper;
using EduHome.Areas.Admin.ViewModels.UserViewModel;
using EduHome.Models.Identity;
using EduHome.ViewModels.AccountViewModel;

namespace EduHome.Mappers;

public class UserMapperProfile : Profile
{
    public UserMapperProfile()
    {
        CreateMap<RegisterViewModel, AppUser>().ReverseMap();
        CreateMap<AppUser,ChangeUserRoleViewModel>()
            .ReverseMap();
        CreateMap<AppUser, AdminUserViewModel>().ReverseMap();
        CreateMap<AppUser, ChangeUserRoleViewModel>().ReverseMap();
    }
}
