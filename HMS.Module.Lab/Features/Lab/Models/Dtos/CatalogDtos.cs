using HMS.Module.Lab.Features.Lab.Models.Enums;

namespace HMS.Module.Lab.Features.Lab.Models.Dtos;

public sealed record LabTestDto(long LabTestId, string Code, string Name, string Specimen, string? Unit, decimal Price, int TatMinutes, bool IsActive);
//public sealed record UpsertTestDto(string Code, string Name, long SpecimenTypeId, string? Unit, decimal Price, int TatMinutes, bool IsActive, decimal? RefLow, decimal? RefHigh);
public sealed record SpecimenTypeDto(long SpecimenTypeId, string Name, string? Code);
//public sealed record ReferenceRangeDto(
//    long? ReferenceRangeId,
//    string Sex,              // "M","F","U"  (U = unisex/unknown)
//    int? AgeMinDays,         // null = open
//    int? AgeMaxDays,         // null = open
//    decimal? NormalLow,
//    decimal? NormalHigh,
//    decimal? CriticalLow,
//    decimal? CriticalHigh,
//    string? UnitOverride,
//    string? Method,
//    string? Notes
//)
//{
//    public IReadOnlyList<ReferenceRangeDto>? Ranges { get; init; }
//}

public sealed record PanelDto(long LabPanelId, string Code, string Name, bool IsActive, IReadOnlyList<long> TestIds);
public sealed record UpsertPanelDto(string Code, string Name, bool IsActive, IReadOnlyList<long> TestIds);

public sealed record InstrumentTestMapListItemDto(
    long InstrumentTestMapId,
    long DeviceId,
    long LabTestId,
    string LabTestCode,
    string LabTestName,
    string InstrumentTestCode
);

public sealed record UpsertInstrumentTestMapDto(
    long DeviceId,
    long LabTestId,
    string InstrumentTestCode
);

// used by result upsert-by-accession
public sealed record UpsertByAccessionDto(
    string Accession,
    long DeviceId,
    string InstrumentTestCode,
    string? Value,
    string? Unit,
    ResultFlag? Flag,
    string? RawFlagOrNotes
);
