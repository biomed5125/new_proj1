using System;
using System.Collections.Generic;
using HMS.Module.Lab.Features.Lab.Models.Enums;

namespace HMS.Module.Lab.Features.Lab.Dashboard;

// --- Orders (list) ---
public sealed record DashOrderRowDto(
    long RequestId,
    string OrderNo,
    string? PatientDisplay,
    string? PatientCode,
    string Tests,
    string Status,
    string AtLocal,
    string? Source);


// --- Order details (single) ---
public sealed record DashOrderItem(
    long LabRequestItemId,
    string Code,
    string Name,
    string Unit,
    decimal Price
);

public sealed record DashSampleDto(
    long LabSampleId,
    string AccessionNumber,
    LabSampleStatus Status,
    DateTime CreatedAtUtc
);

public sealed record DashOrderDetailsDto(
    long LabRequestId,
    string OrderNo,
    long? PatientId,
    IReadOnlyList<DashOrderItem> Items,
    DashSampleDto? Sample
);

// --- Samples (list) ---
public sealed record DashSampleRowDto(
    long LabSampleId,
    long LabRequestId,
    string AccessionNumber,
    LabSampleStatus Status,
    DateTime CreatedAtUtc
);

// --- Results (list) ---
public sealed record DashResultRowDto(
    long ResultId,
    long RequestId,
    long RequestItemId,
    string Code,
    string Name,
    string Value,
    string Unit,
    ResultFlag? Flag,
    LabResultStatus Status,
    string AccessionNo,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc
);

// --- Result history (per request item) ---
public sealed record ResultHistoryDto(
    DateTime AtUtc,
    string Value,
    string Unit,
    ResultFlag? Flag,
    LabResultStatus Status,
    string AccessionNo
);

// --- Summary (header widgets) ---
public sealed record SummaryDto(
    int TotalOrders,
    int PendingSamples,
    int TotalResults,
    int FinalResults
);
