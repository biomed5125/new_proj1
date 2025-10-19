using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Module.Lab.Features.Lab.Models.Dtos
{
    public sealed class CbcReportDto
    {
        // header
        public string OrderNo { get; set; } = default!;
        public string? Accession { get; set; }
        public string? PatientName { get; set; }
        public string? PatientId { get; set; }
        public DateTime CollectedAt { get; set; }
        public DateTime ReportedAt { get; set; }

        // key CBC values (common Sysmex-style panel)
        public decimal WBC { get; set; }          // 10^9/L
        public decimal RBC { get; set; }          // 10^12/L
        public decimal HGB { get; set; }          // g/dL
        public decimal HCT { get; set; }          // %
        public decimal MCV { get; set; }          // fL
        public decimal MCH { get; set; }          // pg
        public decimal MCHC { get; set; }         // g/dL
        public decimal RDW_CV { get; set; }       // %
        public decimal PLT { get; set; }          // 10^9/L
        public decimal MPV { get; set; }          // fL

        // 5-part differential (absolute + percent optional)
        public decimal NEUT_Pct { get; set; }     // %
        public decimal LYMPH_Pct { get; set; }    // %
        public decimal MONO_Pct { get; set; }     // %
        public decimal EO_Pct { get; set; }       // %
        public decimal BASO_Pct { get; set; }     // %

        // reference ranges (simple – per analyte)
        public Dictionary<string, (decimal? low, decimal? high)> Ref { get; set; } = new();

        // histograms/smoothed counts (32–64 bins work well)
        public float[] WbcHist { get; set; } = Array.Empty<float>();
        public float[] RbcHist { get; set; } = Array.Empty<float>();
        public float[] PltHist { get; set; } = Array.Empty<float>();

        public string? MorphologyNote { get; set; }
    }

}
