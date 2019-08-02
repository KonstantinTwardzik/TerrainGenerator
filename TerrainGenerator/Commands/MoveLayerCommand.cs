using System;
using System.Windows.Input;
using Topographer3D.ViewModels;
using Topographer3D.ViewModels.Layers;

namespace Topographer3D.Commands
{
    internal class MoveLayerCommand : ICommand
    {
        private BaseLayer _baseLayer;

        public MoveLayerCommand(BaseLayer baseLayer)
        {
            _baseLayer = baseLayer;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _baseLayer.CanExecute;
        }

        public void Execute(object parameter)
        {
            bool ToFront = Convert.ToBoolean(parameter);
            _baseLayer.MoveLayer(ToFront);

        }
    }
}
