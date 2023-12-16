using AutoMapper;
using EduHome.Areas.Admin.ViewModels.EventViewModel;
using EduHome.Models;
using EduHome.ViewModels.EventViewModel;

namespace EduHome.Mappers;

public class EventMapperProfile : Profile
{
    public EventMapperProfile()
    {
        CreateMap<Event, EventAdsViewModel>().ReverseMap();
        CreateMap<Event, EventViewModel>().ReverseMap();
        CreateMap<Event,CreateEventViewModel>()
            .ForMember(cvm => cvm.SpeakerId,x => x.MapFrom(e => e.EventSpeakers.Select(es => es.Speaker.Id)))
            .ReverseMap();
        CreateMap<Event,UpdateEventViewModel>()
            .ForMember(uvm => uvm.Image, x => x.Ignore())
            .ReverseMap();
        CreateMap<Event, DeleteEventViewModel>().ReverseMap();
        CreateMap<Event, EventDetailViewModel>()
            //.ForMember(evm => evm.speakerNames,x => x.MapFrom(e => e.EventSpeakers.Select(es => es.Speaker.Name)))
            //.ForMember(evm => evm.speakerDuty, x => x.MapFrom(e => e.EventSpeakers.Select(es => es.Speaker.Duty)))
            //.ForMember(evm => evm.speakerNames,x => x.MapFrom(e => e.EventSpeakers.Select(es => es.Speaker.Image)))
            .ForMember(evm => evm.Speakers,x => x.MapFrom(e => e.EventSpeakers.Select(es => es.Speaker)))
            .ReverseMap();
    }
}
