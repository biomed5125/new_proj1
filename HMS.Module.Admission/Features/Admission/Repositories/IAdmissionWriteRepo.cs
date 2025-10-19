using HMS.Module.Admission.Features.Admission.Models.Entities;

namespace HMS.Module.Admission.Features.Admission.Repositories;

public interface IAdmissionWriteRepo
{
    Task AddAsync(myAdmission e, CancellationToken ct);
    Task UpdateAsync(myAdmission e, CancellationToken ct);
    Task SoftDeleteAsync(long id, CancellationToken ct);
    Task SaveAsync(CancellationToken ct);
}
