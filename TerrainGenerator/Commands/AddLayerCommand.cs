using System;
using System.Windows.Input;
using Topographer3D.Utilities;
using Topographer3D.ViewModels;

namespace Topographer3D.Commands
{
    internal class AddLayerCommand : ICommand
    {
        private LayerManager _layerManager;

        public AddLayerCommand(LayerManager layerManager)
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
            Layer layer = (Layer)Enum.Parse(typeof(Layer), parameter.ToString());
            _layerManager.AddNewLayer(layer);

        }
    }
}
