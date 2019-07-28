using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Topographer3D.Commands;
using Topographer3D.Models;
using Topographer3D.Utilities;
using Topographer3D.ViewModels.Layers;

namespace Topographer3D.ViewModels
{
    class LayerManager : ObservableObject
    {
        #region Attributes
        private TerrainEngine terrainEngine;

        #endregion

        #region Properties 
        public ObservableCollection<BaseLayer> Layers { get; private set; }
        public ObservableCollection<BaseLayer> LayerDetails { get; private set; }
        public bool IsLayerSelection { get; private set; }
        public bool IsShowDetails { get; private set; }
        public Visibility IsLayerSelectionVisibility { get; private set; }
        public Visibility IsShowDetailsVisibility { get; private set; }
        public DataTemplate ItemTemplateDetail { get; private set; }

        //Status Bar Bottom
        private string LoadingColor = "#E86E48";
        private string FinishedColor = "#72C1F2";
        public string StatusBarColor { get; set; }
        public string StatusBarText { get; set; }


        #endregion

        #region Initialization
        public LayerManager(TerrainEngine terrainEngine)
        {
            this.terrainEngine = terrainEngine;
            InitProperties();
            InitCommands();
            ShowLayerAdding();
        }

        private void InitProperties()
        {
            Layers = new ObservableCollection<BaseLayer>();
            LayerDetails = new ObservableCollection<BaseLayer>();
            StatusBarColor = FinishedColor;
            StatusBarText = "Terrain Engine Waiting ...";
        }

        private void InitCommands()
        {
            AddLayerCommand = new AddLayerCommand(this);
            ShowLayerSelectionCommand = new ShowLayerSelectionCommand(this);
        }
        #endregion

        #region Button Handling
        public void AddNewLayer(Layer layerType)
        {
            switch (layerType)
            {
                case Layer.Height:
                    HeightLayer newHeightLayer = new HeightLayer(this, terrainEngine);
                    newHeightLayer.Name = GetName(Layer.Height, Layers.Count);
                    newHeightLayer.ImagePath = "pack://application:,,,/Topographer3D;component/Assets/Icons/RaiseIcon.png";
                    newHeightLayer.Position = Layers.Count;
                    Layers.Add(newHeightLayer);
                    ShowLayerDetails(newHeightLayer);
                    break;
                case Layer.Slope:
                    SlopeLayer newSlopeLayer = new SlopeLayer(this, terrainEngine);
                    newSlopeLayer.Name = GetName(Layer.Slope, Layers.Count);
                    newSlopeLayer.ImagePath = "pack://application:,,,/Topographer3D;component/Assets/Icons/RaiseIcon.png";
                    newSlopeLayer.Position = Layers.Count;
                    Layers.Add(newSlopeLayer);
                    ShowLayerDetails(newSlopeLayer);
                    break;
                case Layer.Island:
                    IslandLayer newIslandLayer = new IslandLayer(this, terrainEngine);
                    newIslandLayer.Name = GetName(Layer.Island, Layers.Count);
                    newIslandLayer.ImagePath = "pack://application:,,,/Topographer3D;component/Assets/Icons/RaiseIcon.png";
                    newIslandLayer.Position = Layers.Count;
                    Layers.Add(newIslandLayer);
                    ShowLayerDetails(newIslandLayer);
                    break;
                case Layer.OpenSimplex:
                    OpenSimplexNoiseLayer newOSNLayer = new OpenSimplexNoiseLayer(this, terrainEngine);
                    newOSNLayer.Name = GetName(Layer.OpenSimplex, Layers.Count);
                    newOSNLayer.ImagePath = "pack://application:,,,/Topographer3D;component/Assets/Icons/RaiseIcon.png";
                    newOSNLayer.Position = Layers.Count;
                    Layers.Add(newOSNLayer);
                    ShowLayerDetails(newOSNLayer);
                    break;
                case Layer.Voronoi:
                    VoronoiNoiseLayer newVoronoiLayer = new VoronoiNoiseLayer(this, terrainEngine);
                    newVoronoiLayer.Name = GetName(Layer.Voronoi, Layers.Count);
                    newVoronoiLayer.ImagePath = "pack://application:,,,/Topographer3D;component/Assets/Icons/RaiseIcon.png";
                    newVoronoiLayer.Position = Layers.Count;
                    Layers.Add(newVoronoiLayer);
                    ShowLayerDetails(newVoronoiLayer);
                    break;

                case Layer.Hydraulic:
                    HydraulicErosionLayer newHydrauliceErosionLayer = new HydraulicErosionLayer(this, terrainEngine);
                    newHydrauliceErosionLayer.Name = GetName(Layer.Hydraulic, Layers.Count);
                    newHydrauliceErosionLayer.ImagePath = "pack://application:,,,/Topographer3D;component/Assets/Icons/ErodeIcon.png";
                    newHydrauliceErosionLayer.Position = Layers.Count;
                    Layers.Add(newHydrauliceErosionLayer);
                    ShowLayerDetails(newHydrauliceErosionLayer);
                    break;
            }
        }

        public void DeleteLayer(BaseLayer layer)
        {
            Layers.Remove(layer);
            UpdateLayerView();

        }

        public void MoveLayer(BaseLayer layer, bool IsForward)
        {
            if (IsForward)
            {
                if (layer.Position != 0)
                {
                    Layers.RemoveAt(layer.Position);
                    Layers.Insert(layer.Position - 1, layer);
                }

            }
            else
            {
                if (layer.Position < Layers.Count - 1)
                {
                    Layers.RemoveAt(layer.Position);
                    Layers.Insert(layer.Position + 1, layer);
                }

            }

            UpdateLayerView();
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
            StartCompleteCalculation();
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
                case Layer.Voronoi:
                    name = "Voronoi";
                    break;
                case Layer.Hydraulic:
                    name = "Hydraulic";
                    break;

            }
            name = (layerPositon + 1) + " - " + name;
            return name;
        }

        private void StartCompleteCalculation()
        {
            if(Layers.Count != 0)
            {
                terrainEngine.StartCalculation(Layers[Layers.Count - 1]);
            } else
            {
                terrainEngine.ResetHeights();
            }
        }

        public void Calculate(BaseLayer layer)
        {
            terrainEngine.StartCalculation(layer);
        }

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
            }
            else
            {
                StatusBarColor = FinishedColor;
                StatusBarText = "Terrain Engine Waiting ...";
            }
        }

        #endregion

        #region ICommands
        public bool CanExecute { get { return true; } }
        public ICommand AddLayerCommand { get; private set; }
        public ICommand ShowLayerSelectionCommand { get; private set; }
        #endregion
    }

    enum Layer
    {
        Height,
        Slope,
        Island,
        OpenSimplex,
        Voronoi,
        Hydraulic,
        DisplacementColorization
    };
}
