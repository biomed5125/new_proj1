using HMS.SharedKernel.Ids;
using System.Security.Cryptography;

namespace HMS.Communication.Infrastructure.Persistence.BusinessId
{
    /// <summary>
    /// Time-based business numbers for the Communication module.
    /// Does NOT use DB sequences (per your constraint).
    /// Adds a small random suffix to avoid same-ms collisions.
    /// </summary>
    public static class BusinessIdExtensions
    {
        private static string NewTimeId(string prefix, DateTime utc)
        {
            int rand = RandomNumberGenerator.GetInt32(0, 1000); // 000-999
            return $"{prefix}{utc:yyyyMMddHHmmssfff}{rand:D3}";
        }

        public static string NewCommDeviceNo(this IBusinessIdGenerator _, DateTime utc)
            => NewTimeId("DEV", utc); // e.g., DEV20250829...

        public static string NewCommInboundNo(this IBusinessIdGenerator _, DateTime utc)
            => NewTimeId("CMI", utc); // inbound message

        public static string NewCommOutboundNo(this IBusinessIdGenerator _, DateTime utc)
            => NewTimeId("CMO", utc); // outbound message

        public static string NewCommDeadLetterNo(this IBusinessIdGenerator _, DateTime utc)
            => NewTimeId("DLR", utc);
    }
}
