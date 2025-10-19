namespace HMS.Communication.Abstractions
{
    /// <summary>
    /// Represents a communication driver for a laboratory analyzer.
    /// </summary>
    public interface IAnalyzerDriver
    {
        string Name { get; }

        /// <summary>
        /// Builds a basic ASTM or ASCII message for simulation or bench testing.
        /// </summary>
        //byte[] BuildSimpleResult(string accession, string testCode, string value, string unit);

        /// <summary>
        /// Optional: Initialize or reset analyzer context.
        /// </summary>
        //void Initialize();

        /// <summary>
        /// Optional: Custom command builder (ASTM, SUIT, etc.)
        /// </summary>
        byte[] BuildSimpleResult(string accession, string testCode, string value, string unit,
                             string deviceCode = "DEV", string? mode = null);
    }
}
