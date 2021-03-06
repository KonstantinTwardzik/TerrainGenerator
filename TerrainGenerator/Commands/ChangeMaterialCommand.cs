﻿using System;
using System.Windows.Input;
using Topographer3D.ViewModels;

namespace Topographer3D.Commands
{
    internal class ChangeMaterialCommand : ICommand
    {
        private Viewport _viewport;

        public ChangeMaterialCommand(Viewport viewport)
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
            int material = Convert.ToInt32(parameter);
            _viewport.ChangeMaterial(material);
        }
    }
}
