namespace HMS.Communication.Domain.Abstractions;

public interface IProtocolSession
{
    Task OnBytesAsync(byte[] data, CancellationToken ct); // parse + ack/nak/eot as needed
}
