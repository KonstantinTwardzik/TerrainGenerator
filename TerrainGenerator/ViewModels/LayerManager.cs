﻿using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Topographer3D.Commands;
using Topographer3D.Utilities;
using Topographer3D.ViewModels.Layers;

namespace Topographer3D.ViewModels
{
    class LayerManager : ObservableObject
    {
        #region ATTRIBUTES & PROPERTIES
        private TerrainEngine terrainEngine;

        //Status Bar Bottom
        private string LoadingColor = "#E86E48";
        private string FinishedColor = "#72C1F2";
        private bool TerrainEngineIsOccupied;

        // LAYERS
        public ObservableCollection<BaseLayer> Layers { get; private set; }
        public ObservableCollection<BaseLayer> LayerDetails { get; private set; }

        // VIEW SUPPORT
        public bool IsLayerSelection { get; private set; }
        public bool IsShowDetails { get; private set; }
        public Visibility IsLayerSelectionVisibility { get; private set; }
        public Visibility IsShowDetailsVisibility { get; private set; }
        public double ViewHeight
        {
            set
            {
                LayerDetailHeight = value - 100;
            }
        }
        public double LayerDetailHeight { get; private set; }
        public string StatusBarColor { get; set; }
        public string StatusBarText { get; set; }

        #endregion

        #region INITIALIZATION
        public LayerManager(TerrainEngine terrainEngine)
        {
            this.terrainEngine = terrainEngine;
            InitProperties();
            InitCommands();
        }

        private void InitProperties()
        {
            Layers = new ObservableCollection<BaseLayer>();
            LayerDetails = new ObservableCollection<BaseLayer>();
            ShowLayerAdding();
            SetStatusBar(false);
        }

        private void InitCommands()
        {
            AddLayerCommand = new AddLayerCommand(this);
            ShowLayerSelectionCommand = new ShowLayerSelectionCommand(this);
        }

        #endregion

        #region LAYER LOGIC
        public void AddNewLayer(Layer layerType)
        {

            BaseLayer newLayer = null;
            switch (layerType)
            {
                case Layer.Height:
                    HeightLayer newHeightLayer = new HeightLayer(this, terrainEngine);
                    newHeightLayer.Name = GetName(Layer.Height, Layers.Count);
                    newHeightLayer.ImagePath = "pack://application:,,,/Topographer3D;component/Assets/Icons/RaiseIcon.png";
                    newHeightLayer.Position = Layers.Count;
                    Layers.Add(newHeightLayer);
                    ShowLayerDetails(newHeightLayer);
                    newLayer = newHeightLayer;
                    break;
                case Layer.Slope:
                    SlopeLayer newSlopeLayer = new SlopeLayer(this, terrainEngine);
                    newSlopeLayer.Name = GetName(Layer.Slope, Layers.Count);
                    newSlopeLayer.ImagePath = "pack://application:,,,/Topographer3D;component/Assets/Icons/RaiseIcon.png";
                    newSlopeLayer.Position = Layers.Count;
                    Layers.Add(newSlopeLayer);
                    ShowLayerDetails(newSlopeLayer);
                    newLayer = newSlopeLayer;
                    break;
                case Layer.Island:
                    IslandLayer newIslandLayer = new IslandLayer(this, terrainEngine);
                    newIslandLayer.Name = GetName(Layer.Island, Layers.Count);
                    newIslandLayer.ImagePath = "pack://application:,,,/Topographer3D;component/Assets/Icons/RaiseIcon.png";
                    newIslandLayer.Position = Layers.Count;
                    Layers.Add(newIslandLayer);
                    ShowLayerDetails(newIslandLayer);
                    newLayer = newIslandLayer;
                    break;
                case Layer.OpenSimplex:
                    OpenSimplexNoiseLayer newOSNLayer = new OpenSimplexNoiseLayer(this, terrainEngine);
                    newOSNLayer.Name = GetName(Layer.OpenSimplex, Layers.Count);
                    newOSNLayer.ImagePath = "pack://application:,,,/Topographer3D;component/Assets/Icons/RaiseIcon.png";
                    newOSNLayer.Position = Layers.Count;
                    Layers.Add(newOSNLayer);
                    ShowLayerDetails(newOSNLayer);
                    newLayer = newOSNLayer;
                    break;
                case Layer.CellNoise:
                    CellNoiseLayer newCellNoiseLayer = new CellNoiseLayer(this, terrainEngine);
                    newCellNoiseLayer.Name = GetName(Layer.CellNoise, Layers.Count);
                    newCellNoiseLayer.ImagePath = "pack://application:,,,/Topographer3D;component/Assets/Icons/RaiseIcon.png";
                    newCellNoiseLayer.Position = Layers.Count;
                    Layers.Add(newCellNoiseLayer);
                    ShowLayerDetails(newCellNoiseLayer);
                    newLayer = newCellNoiseLayer;
                    break;

                case Layer.Hydraulic:
                    HydraulicErosionLayer newHydrauliceErosionLayer = new HydraulicErosionLayer(this, terrainEngine);
                    newHydrauliceErosionLayer.Name = GetName(Layer.Hydraulic, Layers.Count);
                    newHydrauliceErosionLayer.ImagePath = "pack://application:,,,/Topographer3D;component/Assets/Icons/ErodeIcon.png";
                    newHydrauliceErosionLayer.Position = Layers.Count;
                    Layers.Add(newHydrauliceErosionLayer);
                    ShowLayerDetails(newHydrauliceErosionLayer);
                    newLayer = newHydrauliceErosionLayer;
                    break;

                case Layer.DetailColorization:
                    DetailColorizationLayer newDetailColorizationLayer = new DetailColorizationLayer(this, terrainEngine);
                    newDetailColorizationLayer.Name = GetName(Layer.DetailColorization, Layers.Count);
                    newDetailColorizationLayer.ImagePath = "pack://application:,,,/Topographer3D;component/Assets/Icons/ColorizeIcon.png";
                    newDetailColorizationLayer.Position = Layers.Count;
                    Layers.Add(newDetailColorizationLayer);
                    ShowLayerDetails(newDetailColorizationLayer);
                    newLayer = newDetailColorizationLayer;
                    break;
                case Layer.HeightColorization:
                    HeightColorizationLayer newHeightColorizationLayer = new HeightColorizationLayer(this, terrainEngine);
                    newHeightColorizationLayer.Name = GetName(Layer.HeightColorization, Layers.Count);
                    newHeightColorizationLayer.ImagePath = "pack://application:,,,/Topographer3D;component/Assets/Icons/ColorizeIcon.png";
                    newHeightColorizationLayer.Position = Layers.Count;
                    Layers.Add(newHeightColorizationLayer);
                    ShowLayerDetails(newHeightColorizationLayer);
                    newLayer = newHeightColorizationLayer;
                    break;
            }

            if (newLayer.Position == 0)
            {
                newLayer.HasApplicationMode = Visibility.Hidden;
                newLayer.CurrentApplicationMode = (ApplicationMode)100;
            }

        }

        public void DeleteLayer(BaseLayer layer)
        {
            if (!TerrainEngineIsOccupied)
            {
                Layers.Remove(layer);
                UpdateLayerView();
            }
        }

        public void DeleteAllLayers()
        {
            if (!TerrainEngineIsOccupied)
            {
                Layers.Clear();
                UpdateLayerView();
            }
        }

        public void MoveLayer(BaseLayer layer, bool IsForward)
        {
            if (!TerrainEngineIsOccupied)
            {
                bool changedSequence = false;
                if (IsForward)
                {
                    if (layer.Position != 0)
                    {
                        Layers.RemoveAt(layer.Position);
                        Layers.Insert(layer.Position - 1, layer);
                        changedSequence = true;
                    }

                }
                else
                {
                    if (layer.Position < Layers.Count - 1)
                    {
                        Layers.RemoveAt(layer.Position);
                        Layers.Insert(layer.Position + 1, layer);
                        changedSequence = true;
                    }
                }

                if ((layer.Position == 0 || layer.Position == 1) && changedSequence)
                {
                    Layers[0].HasApplicationMode = Visibility.Hidden;
                    Layers[0].CurrentApplicationMode = (ApplicationMode)100;
                    if (Layers[1].LayerType != Layer.DetailColorization && Layers[1].LayerType != Layer.HeightColorization && Layers[1].LayerType != Layer.Hydraulic)
                    {
                        Layers[1].HasApplicationMode = Visibility.Visible;
                        Layers[1].CurrentApplicationMode = ApplicationMode.Add;
                    }
                }

                UpdateLayerView();
            }
        }

        private void UpdateLayerView()
        {
            for (int i = 0; i < Layers.Count(); i++)
            {
                Layers[i].Name = GetName(Layers[i].LayerType, i);
                Layers[i].Position = i;
            }

            if (Layers.Count != 0)
            {
                ShowLayerDetails(Layers[Layers.Count - 1]);
            }
            else
            {
                ShowLayerAdding();
            }

            terrainEngine.ResetTerrainEngine();
        }

        private string GetName(Layer layerType, int layerPositon)
        {
            string name = "";
            switch (layerType)
            {
                case Layer.Height:
                    name = "Height";
                    break;
                case Layer.Slope:
                    name = "Slope";
                    break;
                case Layer.Island:
                    name = "Island";
                    break;
                case Layer.OpenSimplex:
                    name = "Open Simplex";
                    break;
                case Layer.CellNoise:
                    name = "Cell Noise";
                    break;
                case Layer.Hydraulic:
                    name = "Hydraulic";
                    break;
                case Layer.DetailColorization:
                    name = "Detail Color";
                    break;
                case Layer.HeightColorization:
                    name = "Height Color";
                    break;

            }
            name = (layerPositon + 1) + " - " + name;
            return name;
        }

        public void Calculate(BaseLayer layer)
        {
            if (!TerrainEngineIsOccupied)
            {
                layer.Unprocessed();
                terrainEngine.StartCalculationToLayer(layer);
            }
        }

        #endregion

        #region VIEWS LOGIC
        public void ShowLayerAdding()
        {
            LayerDetails.Clear();
            IsShowDetails = false;
            IsShowDetailsVisibility = Visibility.Hidden;
            IsLayerSelection = true;
            IsLayerSelectionVisibility = Visibility.Visible;
        }

        public void ShowLayerDetails(BaseLayer layer)
        {
            LayerDetails.Clear();
            LayerDetails.Add(layer);
            IsLayerSelection = false;
            IsLayerSelectionVisibility = Visibility.Hidden;
            IsShowDetails = true;
            IsShowDetailsVisibility = Visibility.Visible;
        }

        public void SetStatusBar(bool IsOccupied)
        {
            if (IsOccupied)
            {
                StatusBarColor = LoadingColor;
                StatusBarText = "Terrain Engine Calculating ...";
                TerrainEngineIsOccupied = true;
            }
            else
            {
                StatusBarColor = FinishedColor;
                StatusBarText = "Terrain Engine Waiting ...";
                TerrainEngineIsOccupied = false;
            }
        }

        #endregion

        #region ICOMMANDS
        public bool CanExecute { get { return true; } }
        public ICommand AddLayerCommand { get; private set; }
        public ICommand ShowLayerSelectionCommand { get; private set; }
        #endregion
    }
}
