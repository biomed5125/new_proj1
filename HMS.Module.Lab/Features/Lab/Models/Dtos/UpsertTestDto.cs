
namespace HMS.Module.Lab.Features.Lab.Models.Dtos
{
    public sealed record UpsertTestDto(string Code, string Name, long SpecimenTypeId, string? Unit, decimal Price, int TatMinutes, bool IsActive, decimal? RefLow, decimal? RefHigh);

    public sealed record ReferenceRangeDto(
    long? ReferenceRangeId,
    string Sex,              // "M","F","U"  (U = unisex/unknown)
    int? AgeMinDays,         // null = open
    int? AgeMaxDays,         // null = open
    decimal? NormalLow,
    decimal? NormalHigh,
    decimal? CriticalLow,
    decimal? CriticalHigh,
    string? UnitOverride,
    string? Method,
    string? Notes
)
    {
        public IReadOnlyList<ReferenceRangeDto>? Ranges { get; init; }
    }
}
