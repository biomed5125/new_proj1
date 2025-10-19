namespace HMS.Api.Features.Dashboard.Lab;

public record OrderRow(
    long RequestId,
    string OrderNo,
    long PatientId,
    string Tests,         // comma-separated codes
    string Status,
    DateTime CreatedAtUtc);

public record SampleRow(
    long SampleId,
    string Accession,
    long RequestId,
    string Status,
    DateTime CreatedAtUtc);

// Keep ResultRow minimal for now (we’ll enrich after we confirm entity fields)
public record ResultRow(
    long ResultId,
    long RequestId,
    string TestCode,
    string? Value,
    string? Units,
    string Status,
    DateTime CreatedAtUtc);
