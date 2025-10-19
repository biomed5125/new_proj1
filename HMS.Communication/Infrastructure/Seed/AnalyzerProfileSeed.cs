using HMS.Communication.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HMS.Communication.Infrastructure.Persistence.Seed
{
    public static class AnalyzerProfileSeed
    {
        public static void Seed(ModelBuilder mb)
        {
            mb.Entity<AnalyzerProfile>().HasData(
                new AnalyzerProfile
                {
                    AnalyzerProfileId = 1,
                    Name = "Roche Cobas e411",
                    Protocol = "ASTM",
                    DriverClass = "RocheCobasDriver",
                    PortSettings = "{ \"Baud\":9600, \"Bits\":8, \"Parity\":\"None\", \"Stop\":1 }",
                    DefaultMode = "Elecsys",
                    Notes = "Host test code required; half-duplex ASTM E1381/E1394."
                },
                new AnalyzerProfile
                {
                    AnalyzerProfileId = 2,
                    Name = "Roche Cobas c311",
                    Protocol = "ASTM",
                    DriverClass = "RocheCobasDriver",
                    PortSettings = "{ \"Baud\":9600, \"Bits\":8, \"Parity\":\"None\", \"Stop\":1 }",
                    DefaultMode = "Cobas",
                    Notes = "Chemistry module using ASTM E1381/E1394."
                },
                new AnalyzerProfile
                {
                    AnalyzerProfileId = 3,
                    Name = "Sysmex XP-300",
                    Protocol = "SUIT",
                    DriverClass = "SysmexSuitDriver",
                    PortSettings = "{ \"Baud\":9600, \"Bits\":8, \"Parity\":\"None\", \"Stop\":1 }",
                    DefaultMode = "Sysmex",
                    Notes = "Implements SUIT v8.0; frame 0-7 rotation; 15 s timeout."
                },
                new AnalyzerProfile
                {
                    AnalyzerProfileId = 4,
                    Name = "Fuji Dri-Chem NX500",
                    Protocol = "ASCII",
                    DriverClass = "FujiDryChemDriver",
                    PortSettings = "{ \"Baud\":9600, \"Bits\":8, \"Parity\":\"None\", \"Stop\":1 }",
                    DefaultMode = "Fuji",
                    Notes = "Simple ASCII text frames; ACK/NAK only."
                }
            );
        }
    }
}
