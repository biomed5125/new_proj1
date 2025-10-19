namespace HMS.Api.Features.Lab.Models.Dtos;

public sealed record ResultRow(
    long LabResultId,
    long LabRequestId,
    string? TestCode,
    string? Value,
    string? Units,
    string Status,
    DateTime CreatedAtUtc
);