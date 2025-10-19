using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Module.Lab.Features.Lab.Models.Entities
{
    public enum ThyroidStatus {None = 0 , Hyper = 1, Hypo = 2 };
    public sealed class myLabPatientProfile
    {
        public long LabPatientId { get; set; }

        // Checkboxes / enums
        public bool Diabetic { get; set; }
        public ThyroidStatus Thyroid { get; set; }       // "None" | "Hyper" | "Hypo"
        public bool ChronicAnemia { get; set; }
        public bool Dialysis { get; set; }
        public bool Pacemaker { get; set; }
        public bool CardiacHistory { get; set; }

        public bool Allergy { get; set; }
        public string? AllergyNotes { get; set; }

        public bool FattyLiver { get; set; }
        public bool HighCholesterol { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public string? UpdatedBy {  get; set; }

        // Nav
        public myLabPatient LabPatient { get; set; } = default!;
    }
}
