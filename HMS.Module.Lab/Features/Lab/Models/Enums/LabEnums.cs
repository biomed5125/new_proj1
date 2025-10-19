// File: Features/Lab/Models/Enums/LabEnums.cs
namespace HMS.Module.Lab.Features.Lab.Models.Enums;

public enum LabRequestStatus
{
    Requested = 0,
    InProgress = 1,
    Completed = 2,
    Cancelled = 3
}

public enum LabSampleStatus
{
    Collected = 0,
    Received = 1,
    Rejected = 2,
    Labeled = 3
}

public enum LabResultStatus
{
    Entered = 0,
    Final = 1,
    Corrected = 2,
}

public enum ResultFlag
{
    None = 0,
    Low = 1,
    High = 2,
    CriticalLow = 3,
    CriticalHigh = 4,
    Normal = 5,
}

public enum SampleType
{
    Serum = 0,
    Plasma = 1,
    WholeBlood = 2,
    Urine = 3,
    CSF = 4,
    Stool = 5,
    Semen = 6
}