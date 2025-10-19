namespace HMS.Module.Lab.Service;
public interface ILabSampleService
{
    Task<long> CollectAsync(long labRequestId, string collector, CancellationToken ct);
    Task<bool> ReceiveAsync(long labSampleId, string receiver, CancellationToken ct);
}
