// Features/Lab/Models/Entities/myLabPreanalytical.cs

namespace HMS.Module.Lab.Features.Lab.Models.Entities;

public sealed class myLabPreanalytical
{
    public long LabPreanalyticalId { get; set; }
    public long LabRequestId { get; set; }           // 1:1 with request

    // Checkboxes / enums
    public bool IsDiabetic { get; set; }
    public bool TookAntibioticLast3Days { get; set; }
    public int? FastingHours { get; set; }           // 8..12 recommended
    public bool HasAllergy { get; set; }
    public string? AllergyNotes { get; set; }
    public string? ThyroidStatus { get; set; }       // "None" | "Hyper" | "Hypo"
    public bool HasAnemia { get; set; }
    public bool HasFattyLiver { get; set; }
    public bool HasHighCholesterol { get; set; }
    public bool Dialysis { get; set; }
    public bool CardiacAttackHistory { get; set; }
    public bool Pacemaker { get; set; }

    // Simple vitals (optional)
    public int? BloodPressureSys { get; set; }
    public int? BloodPressureDia { get; set; }
    public int? PulseBpm { get; set; }

    public string? Notes { get; set; }               // free text

    // Nav
    public myLabRequest LabRequest { get; set; } = default!;
    public DateTime CreatedAt { get;  set; }
    public string CreatedBy { get;  set; }= default!;
}
