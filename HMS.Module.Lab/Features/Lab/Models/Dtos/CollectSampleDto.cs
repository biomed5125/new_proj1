using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Module.Lab.Features.Lab.Models.Dtos
{
    public sealed class CollectSampleDto { 
        public DateTime? WhenUtc { get; set; } 
        public string? By { get; set; } 
    }

}
