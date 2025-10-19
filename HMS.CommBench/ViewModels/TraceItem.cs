using System;

namespace HMS.CommBench.ViewModels
{
    public sealed class TraceItem
    {
        public DateTimeOffset At { get; init; }
        public string Dir { get; init; } = "";
        public string Text { get; init; } = "";
    }
}
