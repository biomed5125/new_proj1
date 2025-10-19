//using HMS.Api.Features.Patient.Models.Dtos;
//using HMS.Api.Features.Patient.Models.Entities;
//using HMS.Api.Features.Patient.Queries;

//namespace HMS.Api.Features.Patient.Repositories
//{

//    public interface IPatientRepository
//    {
//        //Task<PatientDto?> GetAsync(long id, CancellationToken ct);
//        //Task<IReadOnlyList<PatientDto>> ListAsync(PatientQuery q, CancellationToken ct);

//        Task<myPatient?> GetByIdAsync(long id, CancellationToken ct);
//        Task<bool> ExistsDuplicateAsync
//            (string firstName, string lastName, string? phone, long? excludeId, CancellationToken ct);
//        Task<List<myPatient>> ListAsync(CancellationToken ct);
//        Task AddAsync(myPatient entity, CancellationToken ct);
//        Task UpdateAsync(myPatient entity, CancellationToken ct);
//        Task SoftDeleteAsync(myPatient entity, CancellationToken ct);
//    }
//}