using System;
using System.Windows.Input;
using Topographer.ViewModels;

namespace Topographer.Commands
{
    internal class DetailResolutionCommand : ICommand
    {
        private MainViewModel _mainViewModel;

        public DetailResolutionCommand(MainViewModel mainViewModel)
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
            int resolution = Convert.ToInt32(parameter);
            _mainViewModel.UpdateDetailResolution(resolution);
        }
    }
}
