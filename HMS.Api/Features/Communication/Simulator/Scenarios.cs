namespace HMS.Api.Features.Communication.Simulator;

public static class Scenarios
{
    // Returns a series of ASTM frames using <STX>/<ETX>/<CR> tokens; CommBench will turn them into real control chars
    public static string[] RocheGluNa(string deviceCode, string accession, int glu = 105, int na = 138)
    {
        var ts = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        return new[]
        {
            $"<STX>1H|\\^&|||{deviceCode}^cobas^E411|||||{ts}<ETX><CR>",
            $"<STX>2P|1<ETX><CR>",
            $"<STX>3O|1|{accession}||^^^GLU^SC||{ts}|||||N<ETX><CR>",
            $"<STX>4R|1|^^^GLU^SC|{glu}|mg/dL|70-110|N||F|{ts}<ETX><CR>",
            $"<STX>5O|2|{accession}||^^^NA^SC||{ts}|||||N<ETX><CR>",
            $"<STX>6R|1|^^^NA^SC|{na}|mmol/L|135-145|N||F|{ts}<ETX><CR>",
            $"<STX>7L|1|N<ETX><CR>",
        };
    }
}
