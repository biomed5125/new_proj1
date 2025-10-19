// Features/Doctor/Repositories/IDoctorWriteRepo.cs
using HMS.Module.Doctor.Features.Doctor.Models.Entities;

namespace HMS.Module.Doctor.Features.Doctor.Repositories;

public interface IDoctorWriteRepo
{
    Task AddAsync(myDoctor e, CancellationToken ct);
    Task UpdateAsync(myDoctor e, CancellationToken ct);
    Task<bool> SoftDeleteAsync(long id, CancellationToken ct);
    Task SaveAsync(CancellationToken ct);
}
