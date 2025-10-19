using AutoMapper;
using HMS.Module.Admission.Features.Admission.Models.Dtos;
using HMS.Module.Admission.Features.Admission.Models.Entities;

namespace HMS.Module.Admission.Features.Admission.Mappings;

public sealed class AdmissionProfile : Profile
{
    public AdmissionProfile()
    {
        CreateMap<myAdmission, AdmissionDto>()
            .ForCtorParam(nameof(AdmissionDto.Status), o => o.MapFrom(s => (int)s.Status));
    }
}
