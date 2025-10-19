
namespace HMS.SharedKernel.Base
{
    public sealed class SeedRun
    {
        public long SeedRunId { get; set; }
        public string Name { get; set; } = default!;    // e.g. "Lab:Demo:Catalog"
        public string Version { get; set; } = "v1";     // bump to re-apply logic
        public DateTime AppliedAtUtc { get; set; } = DateTime.UtcNow;
    }

}
