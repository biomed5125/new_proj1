using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using HMS.CommBench.Service;

namespace HMS.CommBench.ViewModels
{
    public sealed class MainViewModel : INotifyPropertyChanged
    {
        private readonly CommApiClient _api;
        private readonly DispatcherTimer _pollTimer;
        private long? _lastInId;
        private long? _lastOutId;

        public ObservableCollection<CommApiClient.DeviceRow> Devices { get; } = new();
        private CommApiClient.DeviceRow? _selected;
        public CommApiClient.DeviceRow? SelectedDevice
        {
            get => _selected;
            set { _selected = value; OnPropertyChanged(); RaiseCanExec(); }
        }

        public ObservableCollection<TraceItem> InTraces { get; } = new();
        public ObservableCollection<TraceItem> OutTraces { get; } = new();

        public Relay RefreshCmd { get; }
        public Relay ConnectCmd { get; }
        public Relay DisconnectCmd { get; }
        public Relay SendDemoCmd { get; }

        private bool _isConnected;
        public bool IsConnected
        {
            get => _isConnected;
            private set { _isConnected = value; OnPropertyChanged(); RaiseCanExec(); }
        }

        public MainViewModel(CommApiClient api)
        {
            _api = api;

            RefreshCmd = new Relay(RefreshAsync);
            ConnectCmd = new Relay(() => { if (SelectedDevice != null) IsConnected = true; });
            DisconnectCmd = new Relay(() => { IsConnected = false; });
            SendDemoCmd = new Relay(SendDemoAsync, () => IsConnected && SelectedDevice != null);

            _pollTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _pollTimer.Tick += async (_, __) => await PollAsync();

            // initial load
            _ = RefreshAsync();
            _pollTimer.Start();
        }

        private async Task RefreshAsync()
        {
            try
            {
                var list = await _api.GetDevicesAsync();
                Devices.Clear();
                foreach (var d in list.OrderBy(x => x.Code))
                {
                    Devices.Add(d);
                }

                if (SelectedDevice == null)
                    SelectedDevice = Devices.FirstOrDefault();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load devices.\n{ex.Message}", "CommBench",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task SendDemoAsync()
        {
            if (SelectedDevice == null) return;

            // ask for accession
            var acc = Microsoft.VisualBasic.Interaction.InputBox(
                "Accession to post (e.g., ACC-xxxxxxxxx):",
                "Send Demo ASTM", "");

            if (string.IsNullOrWhiteSpace(acc)) return;

            // Send a single GLU result via the API helper endpoint
            try
            {
                var (file, size) = await _api.SendSimpleAsync(
                SelectedDevice.Id,
                acc.Trim(),
                testCode: "GLU",
                value: "105",
                unit: "mg/dL");


                OutTraces.Add(new TraceItem
                {
                    At = DateTimeOffset.Now,
                    Dir = "TX",
                    Text = $"send-simple -> {System.IO.Path.GetFileName(file)} ({size} bytes)"
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Send failed.\n{ex.Message}", "CommBench",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task PollAsync()
        {
            try
            {
                var ins = await _api.GetInboundAsync(_lastInId, 200, CancellationToken.None);
                if (ins.Count > 0)
                {
                    foreach (var r in ins)
                    {
                        InTraces.Add(new TraceItem
                        {
                            At = r.AtUtc.ToLocalTime(),
                            Dir = "RX",
                            Text = $"{r.DeviceCode}: {r.Text}"
                        });
                    }
                    _lastInId = ins.Max(x => x.Id);
                }

                var outs = await _api.GetOutboundAsync(_lastOutId, 200, CancellationToken.None);
                if (outs.Count > 0)
                {
                    foreach (var r in outs)
                    {
                        OutTraces.Add(new TraceItem
                        {
                            At = r.AtUtc.ToLocalTime(),
                            Dir = "TX",
                            Text = $"{r.DeviceCode}: {r.Text}"
                        });
                    }
                    _lastOutId = outs.Max(x => x.Id);
                }
            }
            catch
            {
                // swallow polling errors; UI keeps running
            }
        }

        private void RaiseCanExec()
        {
            SendDemoCmd.RaiseCanExecuteChanged();
            ConnectCmd.RaiseCanExecuteChanged();
            DisconnectCmd.RaiseCanExecuteChanged();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? p = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
    }
}
