using AutoMapper;
using LocalEventFinder.Models;
using LocalEventFinder.Models.DTO;

namespace LocalEventFinder.Mapping
{
    /// <summary>
    /// Профиль маппинга для AutoMapper
    /// </summary>
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Venue, VenueDto>()
                .ForMember(dest => dest.EventsCount, opt => opt.MapFrom(src => src.Events.Count));
            CreateMap<CreateVenueDto, Venue>();
            CreateMap<UpdateVenueDto, Venue>();

            CreateMap<Organizer, OrganizerDto>()
                .ForMember(dest => dest.EventsCount, opt => opt.MapFrom(src => src.Events.Count));
            CreateMap<CreateOrganizerDto, Organizer>();

            CreateMap<Event, EventDto>()
                .ForMember(dest => dest.CurrentAttendees, opt => opt.MapFrom(src => src.Attendees.Count))
                .ForMember(dest => dest.VenueId, opt => opt.MapFrom(src => src.VenueId))
                .ForMember(dest => dest.OrganizerId, opt => opt.MapFrom(src => src.OrganizerId));

            CreateMap<Event, EventDetailsDto>()
                .ForMember(dest => dest.CurrentAttendees, opt => opt.MapFrom(src => src.Attendees.Count))
                .ForMember(dest => dest.Venue, opt => opt.MapFrom(src => src.Venue))
                .ForMember(dest => dest.Organizer, opt => opt.MapFrom(src => src.Organizer))
                .ForMember(dest => dest.Attendees, opt => opt.MapFrom(src => src.Attendees));

            CreateMap<CreateEventDto, Event>();

            CreateMap<EventAttendee, EventAttendeeDto>()
                .ForMember(dest => dest.EventTitle, opt => opt.MapFrom(src => src.Event.Title));
            CreateMap<RegisterForEventDto, EventAttendee>()
                .ForMember(dest => dest.RegistrationDate, opt => opt.MapFrom(_ => DateTime.UtcNow));
        }
    }
}
