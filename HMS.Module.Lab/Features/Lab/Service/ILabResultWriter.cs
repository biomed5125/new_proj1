using System.Threading;
using System.Threading.Tasks;
using HMS.Module.Lab.Features.Lab.Models.Enums;

namespace HMS.Module.Lab.Features.Lab.Service
{
    public interface ILabResultWriter
    {
        /// <summary>
        /// Upsert (insert or update) the active result row for a LabRequestItem identified by accession + test code.
        /// - Resolves LabRequestId via LabSamples by AccessionNumber
        /// - Resolves RequestItem via InstrumentMaps(DeviceId, InstrumentTestCode) -> LabTestCode
        /// - Creates the RequestItem if it is missing (unit from analyzer if provided)
        /// - Writes/updates a single "active" result per item (Final)
        /// </summary>
        Task UpsertResultAsync(
            string accession,
            long deviceId,
            string instrumentTestCode,
            string? value,
            string? units,
            ResultFlag? flag,           // mapped flag if you have it; otherwise null
            string? rawFlagOrNotes,     // raw H/L or any notes from the analyzer
            CancellationToken ct);
    }
}
