﻿using System;
using System.Windows.Input;
using Topographer3D.ViewModels;

namespace Topographer3D.Commands
{
    internal class ChangeHeightCommand : ICommand
    {
        private MainViewModel _mainViewModel;

        public ChangeHeightCommand(MainViewModel mainViewModel)
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
            float heightMultiplicator = (float)Convert.ToDouble(parameter);
            _mainViewModel.ChangeHeight(heightMultiplicator);
        }
    }
}
