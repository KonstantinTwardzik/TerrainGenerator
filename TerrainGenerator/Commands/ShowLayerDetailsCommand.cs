using System;
using System.Windows.Input;
using Topographer3D.ViewModels.Layers;

namespace Topographer3D.Commands
{
    internal class ShowLayerDetailsCommand : ICommand
    {
        private BaseLayer _layer;

        public ShowLayerDetailsCommand(BaseLayer layer)
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
            _layer.ShowLayerDetails();
        }
    }
}
