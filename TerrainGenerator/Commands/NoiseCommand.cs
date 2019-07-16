using System;
using System.Windows.Input;
using Topographer3D.ViewModels;

namespace Topographer3D.Commands
{
    internal class NoiseCommand : ICommand
    {
        private MainViewModel _mainViewModel;

        public NoiseCommand(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value;  }
        }

        public bool CanExecute(object parameter)
        {
            return _mainViewModel.CanExecute;
        }

        public void Execute(object parameter)
        {
            _mainViewModel.Noise();
        }
    }
}
