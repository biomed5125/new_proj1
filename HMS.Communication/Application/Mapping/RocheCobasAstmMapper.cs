namespace HMS.Communication.Application.Mapping;

public interface IInstrumentToLisMapper
{
    string Map(long deviceId, string instrumentCode);
}

public sealed class RocheCobasAstmMapper : IInstrumentToLisMapper
{
    public string Map(long deviceId, string instrumentCode) => instrumentCode;
}
