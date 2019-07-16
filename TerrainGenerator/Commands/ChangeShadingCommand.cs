using System;
using System.Windows.Input;
using Topographer3D.ViewModels;

namespace Topographer3D.Commands
{
    internal class ChangeShadingCommand : ICommand
    {
        private Viewport _viewport;

        public ChangeShadingCommand(Viewport viewport)
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
            int shader = Convert.ToInt32(parameter);
            _viewport.ChangeShading(shader);
        }
    }
}
