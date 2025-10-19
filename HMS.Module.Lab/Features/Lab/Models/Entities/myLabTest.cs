// HMS.Module.Lab/Features/Lab/Models/Entities/myLabTest.cs
namespace HMS.Module.Lab.Features.Lab.Models.Entities;

public sealed class myLabTest
{
    public long LabTestId { get; set; }
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Unit { get; set; }
    public decimal Price { get; set; }
    public int TatMinutes { get; set; }
    public decimal? RefLow { get; set; }
    public decimal? RefHigh { get; set; }

    // existing FK
    public long SpecimenTypeId { get; set; }
    public mySpecimenType SpecimenType { get; set; } = default!;

    // 🔹 NEW: stability/holding times (null = unspecified)
    public int? StabilityRoomHours { get; set; }         // e.g., 8
    public int? StabilityRefrigeratedHours { get; set; } // e.g., 48
    public int? StabilityFrozenDays { get; set; }        // e.g., 30

    // 🔹 Optional 1:many ranges; keep also a “default” pointer if you like
    public long? DefaultReferenceRangeId { get; set; }
    public myReferenceRange? DefaultReferenceRange { get; set; }
    public ICollection<myReferenceRange> ReferenceRanges { get; set; } = new List<myReferenceRange>();

    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }




    //public long LabTestId { get; set; }
    //public string Code { get; set; } = default!;
    //public string Name { get; set; } = default!;    
    //public string? Unit { get; set; }    
    //public decimal Price { get; set; }         // for billing/dashboards
    //public int TatMinutes { get; set; } = 60;
    //public bool IsActive { get; set; } = true;
    //// NEW(nullable)
    //public decimal? RefLow { get; set; }
    //public decimal? RefHigh { get; set; }

    //// audit
    //public DateTime CreatedAt { get; set; }
    //public string? CreatedBy { get; set; }
    //public DateTime? UpdatedAt { get; set; }
    //public string? UpdatedBy { get; set; }
    //public bool IsDeleted { get; set; }

    //public long? DefaultReferenceRangeId { get; set; }
    //public myReferenceRange? DefaultReferenceRange { get; set; }
    //public long SpecimenTypeId { get; set; }
    //public mySpecimenType SpecimenType { get; set; } = default!;
}
