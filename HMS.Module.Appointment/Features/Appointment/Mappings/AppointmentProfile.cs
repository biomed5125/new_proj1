using AutoMapper;
using HMS.Module.Appointment.Features.Appointment.Models.Dtos;
using HMS.Module.Appointment.Features.Appointment.Models.Entities;

namespace HMS.Module.Appointment.Features.Appointment.Mappings
{
    public class AppointmentProfile : Profile
    {
        public AppointmentProfile()
        {
            // Entity -> DTO
            CreateMap<myAppointment, AppointmentDto>()
                .ForMember(d => d.AppointmentId, o => o.MapFrom(s => s.AppointmentId))
                .ForMember(d => d.AppointmentNo, o => o.MapFrom(s => s.AppointmentNo));

            // Create DTO -> Entity
            // NOTE: If your entity property is 'ScheduledAtUtc' instead of 'ScheduledAt',
            // change the MapFrom target accordingly.
            CreateMap<CreateAppointmentDto, myAppointment>()
                .ForMember(d => d.AppointmentId, o => o.Ignore())
                .ForMember(d => d.AppointmentNo, o => o.Ignore())  // generated server-side
                .ForMember(d => d.Status, o => o.Ignore())  // set in service
                .ForMember(d => d.ScheduledAtUtc, o => o.MapFrom(s => s.ScheduledAtUtc))
                .ForMember(d => d.DurationMinutes, o => o.MapFrom(s => s.DurationMinutes))
                .ForMember(d => d.CreatedAt, o => o.Ignore())
                .ForMember(d => d.CreatedBy, o => o.Ignore())
                .ForMember(d => d.UpdatedAt, o => o.Ignore())
                .ForMember(d => d.UpdatedBy, o => o.Ignore());

            // Update DTO -> Entity (do NOT allow changing AppointmentNo)
            CreateMap<RescheduleAppointmentDto, myAppointment>()
                .ForMember(d => d.AppointmentNo, o => o.Ignore())
                .ForMember(d => d.ScheduledAtUtc, o => o.MapFrom(s => s.ScheduledAtUtc))
                .ForMember(d => d.DurationMinutes, o => o.MapFrom(s => s.DurationMinutes))
                .ForMember(d => d.CreatedAt, o => o.Ignore())
                .ForMember(d => d.CreatedBy, o => o.Ignore())
                .ForMember(d => d.UpdatedAt, o => o.Ignore())
                .ForMember(d => d.UpdatedBy, o => o.Ignore());
        }
    }
}
