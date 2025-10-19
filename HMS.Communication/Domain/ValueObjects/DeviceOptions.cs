// HMS.Communication/Domain/ValueObjects/DeviceOptions.cs
namespace HMS.Communication.Domain.ValueObjects;

public sealed class DeviceOptions
{
    public long DeviceId { get; init; } = 1;
    public string DeviceCode { get; init; } = "ROCHE1";
    public string Protocol { get; init; } = "ASTM";
    public string Transport { get; init; } = "File";      // File | Serial | Tcp
    public string? FilePath { get; init; }                // when Transport=File
    public string? TcpHost { get; init; }                 // when Transport=Tcp
    public int? TcpPort { get; init; }
    public SerialSettings? Serial { get; init; }          // when Transport=Serial
}
