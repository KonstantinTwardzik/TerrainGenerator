using System;
using System.Windows.Input;
using Topographer3D.ViewModels.Layers;

namespace Topographer3D.Commands
{
    internal class DeleteLayerCommand : ICommand
    {
        private BaseLayer _layer;

        public DeleteLayerCommand(BaseLayer layer)
        {
            _layer = layer;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _layer.CanExecute;
        }

        public void Execute(object parameter)
        {
            _layer.Delete();
        }
    }
}
