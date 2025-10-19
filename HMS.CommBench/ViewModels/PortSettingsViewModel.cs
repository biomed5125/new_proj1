using System.IO.Ports;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HMS.CommBench.ViewModels;

public sealed class PortSettingsViewModel : INotifyPropertyChanged
{
    private int _baud = 9600;
    private Parity _parity = Parity.None;
    private int _dataBits = 8;
    private StopBits _stopBits = StopBits.One;

    public int Baud { get => _baud; set { _baud = value; OnPropertyChanged(); } }
    public Parity Parity { get => _parity; set { _parity = value; OnPropertyChanged(); } }
    public int DataBits { get => _dataBits; set { _dataBits = value; OnPropertyChanged(); } }
    public StopBits StopBits { get => _stopBits; set { _stopBits = value; OnPropertyChanged(); } }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? p = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
}
