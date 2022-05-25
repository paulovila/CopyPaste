using System;
using System.Windows.Input;

namespace kahua.host.uno.ui.behaviors
{
    internal class RelayCommand<T> : ICommand where T : struct
    {
        private Action<T> a;
        private Func<T, bool> p;

        public RelayCommand(Action<T> a, Func<T, bool> p)
        {
            this.a = a;
            this.p = p;
        }

        public event EventHandler CanExecuteChanged;
        public bool CanExecute(object parameter) => p((T)parameter);
        public void Execute(object parameter) => a((T)parameter);
    }
}