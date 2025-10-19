namespace HMS.Communication.Domain.Entities;
public class RouterRule
{
    public long Id { get; private set; }
    public bool IsEnabled { get; private set; } = true;
    public long? DeviceId { get; private set; }           // null = any device
    public string? RecordType { get; private set; }       // e.g., "O","R"
    public string? TestCodeRegex { get; private set; }    // optional regex for instrument test code
    public string Target { get; private set; } = "";      // "OrderDispatch" | "ResultIngest"
    public int Priority { get; private set; } = 100;      // lower wins
    public bool IsDeleted { get; set; } = false;
    private RouterRule() { }
    public RouterRule(bool enabled, long? deviceId, string? recordType, string? regex, string target, int priority = 100)
    {
        IsEnabled = enabled; DeviceId = deviceId; RecordType = recordType;
        TestCodeRegex = regex; Target = target; Priority = priority;
    }
}
