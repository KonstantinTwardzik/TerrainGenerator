﻿using System;
using System.Windows.Input;
using TerrainGenerator.ViewModels;

namespace TerrainGenerator.Commands
{
    internal class ExportCommand : ICommand
    {
        private MainViewModel _mainViewModel;

        public ExportCommand(MainViewModel mainViewModel)
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
            _mainViewModel.ExportMaps();
        }
    }
}