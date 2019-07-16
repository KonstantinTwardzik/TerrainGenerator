using System;
using System.Windows.Input;
using Topographer3D.ViewModels;

namespace Topographer3D.Commands
{
    internal class ChangeViewCommand : ICommand
    {
        private ViewportCamera _viewportCamera;

        public ChangeViewCommand(ViewportCamera viewportCamera)
        {
            _viewportCamera = viewportCamera;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _viewportCamera.CanExecute;
        }

        public void Execute(object parameter)
        {
            int view = Convert.ToInt32(parameter);
            _viewportCamera.SetView(view);
        }
    }
}
