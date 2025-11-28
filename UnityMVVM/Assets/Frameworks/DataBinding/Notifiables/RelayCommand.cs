using System;
using System.Runtime.CompilerServices;

namespace MVVMToolkit.DataBinding
{
    public sealed class RelayCommand : IRelayCommand
    {
        #nullable enable

        private readonly Action<object?> execute;
        private readonly Func<object?, bool>? canExecute;

        public event EventHandler? CanExecuteChanged;

        public RelayCommand(Action<object?> execute)
        {
            if (execute == null)
                throw new ArgumentNullException();

            this.execute = execute;
        }

        public RelayCommand(Action<object?> execute, Func<object?, bool> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException();

            if (canExecute == null)
                throw new ArgumentNullException();

            this.execute = execute;
            this.canExecute = canExecute;
        }

        public void NotifyCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanExecute(object? parameter)
        {
            return canExecute?.Invoke(parameter) != false;
        }

        public void Execute(object? parameter)
        {
            execute(parameter);
        }
    }
}
