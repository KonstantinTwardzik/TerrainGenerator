using System;
using System.Windows.Input;
using Topographer3D.ViewModels;

namespace Topographer3D.Commands
{
    internal class ShowLayerSelectionCommand : ICommand
    {
        private LayerManager _layerManager;

        public ShowLayerSelectionCommand(LayerManager layerManager)
        {
            _layerManager = layerManager;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _layerManager.CanExecute;
        }

        public void Execute(object parameter)
        {
            _layerManager.ShowLayerAdding();
        }
    }
}
