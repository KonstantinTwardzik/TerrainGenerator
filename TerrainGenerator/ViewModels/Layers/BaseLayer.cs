using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Topographer3D.Commands;
using Topographer3D.Utilities;

namespace Topographer3D.ViewModels.Layers
{
    abstract class BaseLayer : ObservableObject
    {
        #region Attributes
        protected LayerManager layerManager;
        protected TerrainEngine terrainEngine;
        private string LoadingColor = "#E86E48";
        private string FinishedColor = "#3A9EEB";

        #endregion

        #region Properties
        public int Position { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public int ProgressPercentage { get; set; }
        public string ProgressBarColor { get; set; }
        public bool IsProcessed { get; set; }
        public Layer LayerType { get; set; }

        public ApplicationMode CurrentApplicationMode { get; set; }
        public IEnumerable<ApplicationMode> ApplicationModeEnum { get { return Enum.GetValues(typeof(ApplicationMode)).Cast<ApplicationMode>(); } }


        #endregion

        #region Initialization
        public BaseLayer(LayerManager layerManager, TerrainEngine terrainEngine)
        {
            this.layerManager = layerManager;
            this.terrainEngine = terrainEngine;
            InitProperties();
            InitCommands();
        }

        private void InitProperties()
        {
            Unprocessed();
            CurrentApplicationMode = ApplicationMode.Normal;
        }

        private void InitCommands()
        {
            ShowLayerDetailsCommand = new ShowLayerDetailsCommand(this);
            CalculateCommand = new CalculateCommand(this);
            DeleteLayerCommand = new DeleteLayerCommand(this);
        }

        #endregion

        #region Button Handling
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
            layerManager.Calculate(this);
        }

        public void Delete()
        {
            layerManager.DeleteLayer(this);
        }

        public void ShowLayerDetails()
        {
            layerManager.ShowLayerDetails(this);
            Calculate();
        }

        #endregion

        #region Abstract Functions
        protected abstract void Dispose();

        #endregion

        #region ICommands
        public bool CanExecute { get { return true; } }
        public ICommand ShowLayerDetailsCommand { get; private set; }
        public ICommand CalculateCommand { get; private set; }
        public ICommand DeleteLayerCommand { get; private set; }
        #endregion

    }

    public enum ApplicationMode
    {
        Normal,
        Add,
        Subtract,
        Multiply,

    }


}

