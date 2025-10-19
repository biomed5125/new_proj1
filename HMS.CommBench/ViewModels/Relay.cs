using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HMS.CommBench.ViewModels
{
    /// <summary>ICommand that supports both sync and async executes, plus RaiseCanExecuteChanged.</summary>
    public sealed class Relay : ICommand
    {
        private readonly Func<Task>? _executeAsync;
        private readonly Action? _executeSync;
        private readonly Func<bool>? _canExecute;

        public Relay(Action execute, Func<bool>? canExecute = null)
        {
            _executeSync = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public Relay(Func<Task> executeAsync, Func<bool>? canExecute = null)
        {
            _executeAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;

        public async void Execute(object? parameter)
        {
            if (_executeSync is not null) { _executeSync(); return; }
            if (_executeAsync is not null) await _executeAsync();
        }

        public event EventHandler? CanExecuteChanged;
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
