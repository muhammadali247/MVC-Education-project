using AutoMapper;
using EduHome.Areas.Admin.ViewModels.TeacherViewModel;
using EduHome.Models;
using EduHome.ViewModels.teacherViewModel;

namespace EduHome.Mappers;

public class TeacherMapperProfile : Profile
{
	public TeacherMapperProfile()
	{
		CreateMap<Teacher,AdminTeacherViewModel>().ReverseMap();
		CreateMap<Teacher, CreateTeacherViewModel>()
			.ForMember(cvm => cvm.SocialMediaId, x => x.MapFrom(t => t.SocialMedias.Select(sm => sm.Id)))
			.ReverseMap();
		CreateMap<Teacher, TeacherViewModel>()
            //.ForMember(tvm => tvm.SocialMedias,x => x.MapFrom(t => t.SocialMedias.Select(sm => sm)))
            //         .ForMember(tvm => tvm.Icon, x => x.MapFrom(t => t.SocialMedias.Select(sm => sm.Icon)))
            //.ForMember(tvm => tvm.SocialMedias, x => x.MapFrom(t => t.SocialMedias.Select(sm => sm)))

            .ForMember(tvm => tvm.SocialMedias, x => x.MapFrom(t => t.SocialMedias))
			.ReverseMap();

            //.ForMember(tvm => tvm.Icon, x => x.MapFrom(t => t.SocialMedias.FirstOrDefault().Icon))
            //.ForMember(tvm => tvm.Link, x => x.MapFrom(t => t.SocialMedias.FirstOrDefault().Link))
		CreateMap<Teacher, TeacherDetailViewModel>()
            .ForMember(tvm => tvm.SkillNames,x => x.MapFrom(t => t.teacherSkills.Select(ts => ts.Skill.Name)))
			.ForMember(tvm => tvm.Percentage,x => x.MapFrom(t => t.teacherSkills.Select(ts => ts.Percentage)))
			.ForMember(tvm => tvm.SocialMedias, x => x.MapFrom(t => t.SocialMedias))
			.ReverseMap();
	}
}
