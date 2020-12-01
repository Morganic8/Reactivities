using AutoMapper;
using Domain;

namespace Application.Activities {
    public class MappingProfile : Profile {
        public MappingProfile() {

            //From object => to object we want to Map too
            CreateMap<Activity, ActivityDTO>();
            CreateMap<UserActivity, AttendeeDTO>()
                //d = destination property
                //o = option
                //s = source of the property
                .ForMember(d => d.Username, o => o.MapFrom(s => s.AppUser.UserName))
                .ForMember(d => d.DisplayName, o => o.MapFrom(s => s.AppUser.DisplayName));
        }
    }
}