//using HMS.SharedServices.IdGeneration;
//using Microsoft.EntityFrameworkCore;
//using HMS.SharedKernel.Ids;
//using System.Collections.Concurrent;

//namespace HMS.SharedServices.IdGeneration
//{
//    public sealed class BusinessIdGenerator : IBusinessIdGenerator
//    {
//        // In-memory counters per name (dev/demo). For production, switch to SQL SEQUENCEs.
//        private static readonly ConcurrentDictionary<string, long> _seq = new();

//        private static long Next(string name) =>
//            _seq.AddOrUpdate(name, 1, (_, current) => current + 1);

//        // Luhn checksum for MRN tail (optional, but you referenced it)
//        private static int Luhn(string digits)
//        {
//            int sum = 0, alt = 0;
//            for (int i = digits.Length - 1; i >= 0; i--)
//            {
//                int n = digits[i] - '0';
//                if ((alt++ & 1) == 1) { n *= 2; if (n > 9) n -= 9; }
//                sum += n;
//            }
//            return (10 - (sum % 10)) % 10;
//        }

//        // ---- Generators ----
//        public string NewMrn()
//        {
//            var num = Next("Seq_MRN").ToString().PadLeft(10, '0'); // 10 digits + luhn
//            return $"P{num}{Luhn(num)}";                           // e.g., P0000123456
//        }

//        public string NewEncounterNo(DateTime utc)
//        {
//            var seq = Next("Seq_Encounter").ToString().PadLeft(6, '0');
//            return $"E{utc:yyyyMMdd}-{seq}";                       // E20250814-000123
//        }

//        public string NewAppointmentNo(DateTime utc)
//        {
//            var seq = Next("Seq_Appointment").ToString().PadLeft(6, '0');
//            return $"AP{utc:yyyyMMdd}-{seq}";                      // AP20250814-000123
//        }

//        public string NewLabOrderNo(DateTime utc)
//        {
//            var seq = Next("Seq_LabOrder").ToString().PadLeft(6, '0');
//            return $"LR{utc:yyyyMMdd}-{seq}";                      // LR20250814-000123
//        }

//        // DICOM-friendly (<=16 chars). Example: SYYMMDD###### (13)
//        public string NewAccessionNumber(DateTime utc)
//        {
//            var seq = Next("Seq_Accession").ToString().PadLeft(6, '0');
//            return $"S{utc:yyMMdd}{seq}";                          // S250814000123
//        }

//        public string NewInvoiceNo(DateTime utc)
//        {
//            var seq = Next("Seq_Invoice").ToString().PadLeft(6, '0');
//            return $"INV{utc:yyyy}-{seq}";                         // INV2025-000123
//        }
//    }
//}
