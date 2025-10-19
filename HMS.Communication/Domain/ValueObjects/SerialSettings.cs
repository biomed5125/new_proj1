// HMS.Communication/Domain/ValueObjects/SerialSettings.cs
namespace HMS.Communication.Domain.ValueObjects;

public sealed class SerialSettings
{
    public string PortName { get; init; } = "COM3";
    public int Baud { get; init; } = 9600;
    public string Parity { get; init; } = "None";    // None, Odd, Even, Mark, Space
    public int DataBits { get; init; } = 8;
    public string StopBits { get; init; } = "One";   // None, One, Two, OnePointFive
}
