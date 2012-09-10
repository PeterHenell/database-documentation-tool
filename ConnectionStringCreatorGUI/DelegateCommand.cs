using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ConnectionStringCreatorGUI
{
    public class DelegateCommand<T> : ICommand
    {
        Action<T> _executeDelegate;

        public DelegateCommand(Action<T> executeDelegate)
        {
            _executeDelegate = executeDelegate;
        }

        public void Execute(T parameter)
        {
            _executeDelegate(parameter);
        }

        public bool CanExecute(object parameter) { return true; }
        public event EventHandler CanExecuteChanged;


        public void Execute(object parameter)
        {
            _executeDelegate((T)parameter);
        }
    }

    public class DelegateCommand : ICommand
    {
        Action _executeDelegate;

        public DelegateCommand(Action executeDelegate)
        {
            _executeDelegate = executeDelegate;
        }

        public void Execute()
        {
            _executeDelegate();
        }

        public bool CanExecute(object parameter) { return true; }
        public event EventHandler CanExecuteChanged;


        public void Execute(object parameter)
        {
            _executeDelegate();
        }
    }

}
