using AutoMapper;
using HMS.Module.Patient.Features.Patient.Models.Dtos;
using HMS.Module.Patient.Features.Patient.Models.Entities;

namespace HMS.Api.Features.Patient.Mappings
{
    public class PatientProfile : Profile
    {
        public PatientProfile()
        {
            CreateMap<myPatient, PatientDto>()
                .ForCtorParam("PatientId", o => o.MapFrom(s => s.PatientId))
                .ForCtorParam("Mrn", o => o.MapFrom(s => s.Mrn ?? ""))
                .ForCtorParam("FirstName", o => o.MapFrom(s => s.FirstName ?? ""))
                .ForCtorParam("LastName", o => o.MapFrom(s => s.LastName ?? ""))
                .ForCtorParam("DateOfBirth", o => o.MapFrom(s => s.DateOfBirth))
                .ForCtorParam("Gender", o => o.MapFrom(s => s.Gender))
                .ForCtorParam("Phone", o => o.MapFrom(s => s.Phone))
                .ForCtorParam("Email", o => o.MapFrom(s => s.Email));
        }
    }
}
