﻿using System;
using System.Windows.Input;
using TerrainGenerator.ViewModels;

namespace TerrainGenerator.Commands
{
    internal class NewCommand : ICommand
    {
        private MainViewModel _mainViewModel;

        public NewCommand(MainViewModel mainViewModel)
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
            _mainViewModel.NewTerrain();
        }
    }
}
