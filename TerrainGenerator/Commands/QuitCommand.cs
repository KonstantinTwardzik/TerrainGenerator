using System;
using System.Windows.Input;
using Topographer.ViewModels;

namespace Topographer.Commands
{
    internal class QuitCommand : ICommand
    {
        private MainViewModel _mainViewModel;

        public QuitCommand(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _mainViewModel.CanExecute;
        }

        public void Execute(object parameter)
        {
            _mainViewModel.QuitApplication();
        }
    }
}
