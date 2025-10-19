using HMS.Module.Appointment.Features.Appointment.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Module.Appointment.Features.Appointment.Repositories
{
    public interface IAppointmentReadRepo
    {
        Task<myAppointment?> GetAsync(long id, CancellationToken ct);
        Task<List<myAppointment>> ListAsync(DateTime? fromUtc, DateTime? toUtc, long? patientId, long? doctorId, CancellationToken ct);
    }
}
