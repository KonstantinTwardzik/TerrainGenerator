using System;
using System.Windows.Input;
using Topographer3D.ViewModels;

namespace Topographer3D.Commands
{
    internal class ChangeViewportQualityCommand : ICommand
    {
        private Viewport _viewport;

        public ChangeViewportQualityCommand(Viewport viewport)
        {
            _viewport = viewport;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _viewport.CanExecute;
        }

        public void Execute(object parameter)
        {
            int quality = Convert.ToInt32(parameter);
            _viewport.ChangeViewportQuality(quality);
        }
    }
}
