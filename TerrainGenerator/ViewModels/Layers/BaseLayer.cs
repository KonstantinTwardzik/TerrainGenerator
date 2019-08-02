using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Topographer3D.Commands;
using Topographer3D.Utilities;

namespace Topographer3D.ViewModels.Layers
{
    abstract class BaseLayer : ObservableObject
    {
        #region ATTRIBUTES & PROPERTIES
        public LayerManager LayerManager { get; private set; }
        protected TerrainEngine terrainEngine;
        private string LoadingColor = "#E86E48";
        private string FinishedColor = "#72C1F2";

        public int Position { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public int ProgressPercentage { get; set; }
        public string ProgressBarColor { get; set; }
        public bool IsProcessed { get; set; }
        public Layer LayerType { get; set; }
        public Visibility HasApplicationMode { get; set; }
        public ApplicationMode CurrentApplicationMode { get; set; }
        public IEnumerable<ApplicationMode> ApplicationModeEnum { get { return Enum.GetValues(typeof(ApplicationMode)).Cast<ApplicationMode>(); } }

        #endregion

        #region INITIALIZATION
        public BaseLayer(LayerManager layerManager, TerrainEngine terrainEngine)
        {
            LayerManager = layerManager;
            this.terrainEngine = terrainEngine;
            InitProperties();
            InitCommands();
        }

        private void InitProperties()
        {
            Unprocessed();
            CurrentApplicationMode = ApplicationMode.Normal;
            HasApplicationMode = Visibility.Visible;
        }

        private void InitCommands()
        {
            ShowLayerDetailsCommand = new ShowLayerDetailsCommand(this);
            CalculateCommand = new CalculateCommand(this);
            DeleteLayerCommand = new DeleteLayerCommand(this);
            MoveLayerCommand = new MoveLayerCommand(this);
        }

        #endregion

        #region TERRAIN ENGINE PROCESSING
        internal void Processed()
        {
            ProgressPercentage = 100;
            ProgressBarColor = FinishedColor;
            IsProcessed = true;
        }

        internal void Unprocessed()
        {
            ProgressPercentage = 0;
            ProgressBarColor = LoadingColor;
            IsProcessed = false;
        }

        public void Calculate()
        {
            Unprocessed();
            LayerManager.Calculate(this);
        }

        public void Delete()
        {
            LayerManager.DeleteLayer(this);
        }

        public void ShowLayerDetails()
        {
            LayerManager.ShowLayerDetails(this);
            Calculate();
        }

        public void MoveLayer(bool IsForward)
        {
            LayerManager.MoveLayer(this, IsForward);
        }

        #endregion

        #region DISPOSABLE SUPPORT
        protected abstract void Dispose();

        #endregion

        #region ICOMMANDS
        public bool CanExecute { get { return true; } }
        public ICommand ShowLayerDetailsCommand { get; private set; }
        public ICommand CalculateCommand { get; private set; }
        public ICommand DeleteLayerCommand { get; private set; }
        public ICommand MoveLayerCommand { get; private set; }
        #endregion

    }




}

