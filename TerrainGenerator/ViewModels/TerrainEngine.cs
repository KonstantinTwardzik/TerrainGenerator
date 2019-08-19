using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Topographer3D.Models;
using Topographer3D.Utilities;
using Topographer3D.ViewModels.Layers;

namespace Topographer3D.ViewModels
{
    class TerrainEngine : ObservableObject
    {
        #region ATTRIBUTES & PROPERTIES
        // LOGIC
        private LayerManager layerManager;
        private MainViewModel mainViewModel;
        private TerrainModel terrainModel;
        private int requestedLayerPosition;

        // TERRAIN MODEL PROPERTIES
        public float[] TerrainHeights
        {
            get { return terrainModel.TerrainHeights; }
            set { terrainModel.TerrainHeights = value; }
        }
        public float[] PreviousTerrainHeights
        {
            get { return terrainModel.PrevTerrainHeights; }
            set { terrainModel.PrevTerrainHeights = value; }
        }
        public byte[] PreviousTerrainColors
        {
            get { return terrainModel.PreviousTerrainColors; }
            set { terrainModel.PreviousTerrainColors = value; }
        }
        public byte[] TerrainColors
        {
            get { return terrainModel.TerrainColors; }
            set { terrainModel.TerrainColors = value; }
        }
        public int TerrainSize
        {
            get { return terrainModel.TerrainSize; }
            set { terrainModel.TerrainSize = value; }
        }
        public BitmapImage HeightMapImage
        {
            get { return terrainModel.HeightMapImage; }
            set { terrainModel.HeightMapImage = value; }
        }
        public BitmapImage ColorMapImage
        {
            get { return terrainModel.ColorMapImage; }
            set { terrainModel.ColorMapImage = value; }
        }
        #endregion

        #region INITIALIZATION
        public TerrainEngine(TerrainModel terrainModel)
        {
            this.terrainModel = terrainModel;
            requestedLayerPosition = 0;
            TerrainSize = 512;
            FlattenTerrain();
        }

        internal void InitLogic(LayerManager layerManager, MainViewModel mainViewModel)
        {
            this.layerManager = layerManager;
            this.mainViewModel = mainViewModel;
        }
        #endregion

        #region RESETTING TERRAIN
        // Reset all States and Data to Default Values
        internal void ResetTerrainEngine()
        {
            requestedLayerPosition = 0;
            foreach (BaseLayer layer in layerManager.Layers)
            {
                layer.Unprocessed();
            }
            FlattenTerrain();
        }

        // Set all TerrainHeights to 0
        internal void FlattenTerrain()
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += FlattenCalculation;
            worker.RunWorkerCompleted += FlattenFinished;
            worker.RunWorkerAsync(100000);
        }

        private void FlattenCalculation(object sender, DoWorkEventArgs e)
        {
            for (int x = 0; x < TerrainSize; x++)
            {
                for (int z = 0; z < TerrainSize; z++)
                {
                    TerrainHeights[z + x * TerrainSize] = 0;
                }
            }
        }

        private void FlattenFinished(object sender, RunWorkerCompletedEventArgs e)
        {
            TerrainHeights = new float[TerrainSize * TerrainSize];
            PreviousTerrainHeights = new float[TerrainSize * TerrainSize];
            mainViewModel.UpdateMesh();
            mainViewModel.ResetTextures();
        }
        #endregion

        #region LAYER HANDLING

        public void StartCalculationToLayer(BaseLayer layer)
        {
            layerManager.SetStatusBar(true);

            if (layer.Position < requestedLayerPosition || requestedLayerPosition == 0)
            {
                ResetTerrainEngine();
                requestedLayerPosition = layer.Position;
                SingleLayerCalculationStart(layerManager.Layers[0]);
            }
            else if (layer.Position == requestedLayerPosition && layerManager.Layers[requestedLayerPosition - 1].IsProcessed == true)
            {
                if (layer.LayerType != Layer.DetailColorization || layer.LayerType != Layer.HeightColorization)
                {
                    UpdateHeights(PreviousTerrainHeights);
                }
                SingleLayerCalculationStart(layer);
            }
            else if (layer.Position > requestedLayerPosition)
            {
                UpdatePreviousHeights(TerrainHeights);
                if (TerrainColors != null)
                {
                    UpdatePreviousColors(TerrainColors);
                }
                int startLayerPosition = requestedLayerPosition + 1;
                requestedLayerPosition = layer.Position;
                SingleLayerCalculationStart(layerManager.Layers[startLayerPosition]);
            }
        }

        private void SingleLayerCalculationStart(BaseLayer layer)
        {
            switch (layer.LayerType)
            {
                // Elevation Layers
                case Layer.Height:
                    HeightLayer heightLayer = layer as HeightLayer;
                    heightLayer.StartHeight(TerrainSize, TerrainHeights);
                    break;
                case Layer.Slope:
                    SlopeLayer slopeLayer = layer as SlopeLayer;
                    slopeLayer.StartSlope(TerrainSize, TerrainHeights);
                    break;
                case Layer.Island:
                    IslandLayer islandLayer = layer as IslandLayer;
                    islandLayer.StartIsland(TerrainSize, TerrainHeights);
                    break;
                case Layer.OpenSimplex:
                    OpenSimplexNoiseLayer OSNLayer = layer as OpenSimplexNoiseLayer;
                    OSNLayer.StartOpenSimplexNoise(TerrainSize, TerrainHeights);
                    break;
                case Layer.CellNoise:
                    CellNoiseLayer CellNoiseLayer = currentLayer as CellNoiseLayer;
                    CellNoiseLayer.StartCellNoise(TerrainSize, TerrainHeights);
                    break;

                // Erosion Layers
                case Layer.Hydraulic:
                    HydraulicErosionLayer hydraulicErosionLayer = layer as HydraulicErosionLayer;
                    hydraulicErosionLayer.StartErosion(TerrainSize, TerrainHeights);
                    break;

                // Colorization Layers
                case Layer.DetailColorization:
                    DetailColorizationLayer detailColorizationLayer = layer as DetailColorizationLayer;
                    detailColorizationLayer.StartColorization(TerrainSize, TerrainHeights);
                    break;
                case Layer.HeightColorization:
                    HeightColorizationLayer heightColorizationLayer = layer as HeightColorizationLayer;
                    heightColorizationLayer.StartColorization(TerrainSize, TerrainHeights, PreviousTerrainColors);
                    break;
            }
        }

        internal void SingleLayerCalculationComplete(BaseLayer layer, float[] terrainHeights)
        {
            if (layer.Position < requestedLayerPosition)
            {
                UpdatePreviousHeights(terrainHeights);
            }
            UpdateHeights(terrainHeights);
            mainViewModel.UpdateMesh();
            StartNextLayerCalculation(layer);
        }

        internal void SingleLayerCalculationComplete(BaseLayer layer, MemoryStream terrainMainColors, MemoryStream terrainBorderColors, byte[] currentColors)
        {
            if (layer.Position < requestedLayerPosition)
            {
                UpdatePreviousColors(currentColors);
            }
            if(terrainBorderColors != null)
            {
                mainViewModel.UpdateTextures(terrainMainColors, terrainBorderColors);
            } else
            {
                mainViewModel.UpdateTexture(terrainMainColors);
            }
            UpdateColors(currentColors);
            CreateAlbedoMap(terrainMainColors);
            StartNextLayerCalculation(layer);
        }

        //Checks if next Layer should be Calculated and starts calculation if so.
        private void StartNextLayerCalculation(BaseLayer layer)
        {
            int nextLayer = layer.Position + 1;
            if (nextLayer <= requestedLayerPosition)
            {
                SingleLayerCalculationStart(layerManager.Layers[nextLayer]);
            }
            else
            {
                layerManager.SetStatusBar(false);
            }
        }
      
        // Updates the Heights Array
        private void UpdateHeights(float[] heights)
        {
            for (int x = 0; x < TerrainSize; x++)
            {
                for (int z = 0; z < TerrainSize; z++)
                {
                    TerrainHeights[z + x * TerrainSize] = heights[z + x * TerrainSize];
                }
            }
        }

        // Updates the PreviousHeights Array
        private void UpdatePreviousHeights(float[] heights)
        {
            for (int x = 0; x < TerrainSize; x++)
            {
                for (int z = 0; z < TerrainSize; z++)
                {
                    PreviousTerrainHeights[z + x * TerrainSize] = heights[z + x * TerrainSize];
                }
            }
        }

        // Updates the Colors Array
        private void UpdateColors(byte[] colors)
        {
            for (int i = 0; i < colors.Length; i++)
            {
                TerrainColors[i] = colors[i];
            }
        }

        // Updates the PreviousColors Array
        private void UpdatePreviousColors(byte[] colors)
        {
            for (int i = 0; i < colors.Length; i++)
            {
                PreviousTerrainColors[i] = colors[i];
            }
        }

        #endregion

        #region EXPORT MAPS
        public void CreateHeightMap()
        {
            //Heightmap
            System.Windows.Media.PixelFormat pixelFormat = PixelFormats.Gray32Float;
            int rawStride = (TerrainSize * pixelFormat.BitsPerPixel + 7) / 8;
            byte[] rawImage = new byte[rawStride * TerrainSize];

            // Filling the rawImage with Data
            int count = 0;
            for (int x = 0; x < TerrainSize; x++)
            {
                for (int z = 0; z < TerrainSize; z++)
                {
                    byte[] bytes = BitConverter.GetBytes((float)TerrainHeights[z + x * TerrainSize]);
                    for (int i = 0; i < 4; i++)
                    {
                        if (count >= rawImage.Length)
                        {
                            break;
                        }
                        rawImage[count] = bytes[i];
                        count++;
                    }
                }
            }
            BitmapSource bitmap = BitmapSource.Create(TerrainSize, TerrainSize, 96, 96, pixelFormat, null, rawImage, rawStride);
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            MemoryStream memoryStream = new MemoryStream();
            HeightMapImage = new BitmapImage();

            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            encoder.Save(memoryStream);

            memoryStream.Position = 0;
            HeightMapImage.BeginInit();
            HeightMapImage.StreamSource = new MemoryStream(memoryStream.ToArray());
            HeightMapImage.EndInit();
            HeightMapImage.Freeze();
        }

        public void CreateAlbedoMap(MemoryStream terrainMainColors)
        {
            MemoryStream memoryStream = terrainMainColors;
            ColorMapImage = new BitmapImage();

            terrainMainColors.Position = 0;
            ColorMapImage.BeginInit();
            ColorMapImage.StreamSource = new MemoryStream(memoryStream.ToArray());
            ColorMapImage.EndInit();
            ColorMapImage.Freeze();
        }

        public void ExportMaps(String filePath)
        {
            int index = filePath.LastIndexOf(@".");

            if (HeightMapImage.IsFrozen)
            {
                String heightFilePath = filePath.Insert(index, "_height");
                Save(HeightMapImage, heightFilePath);
            }

            if (ColorMapImage != null && ColorMapImage.IsFrozen)
            {
                String albedoFilePath = filePath.Insert(index, "_albedo");
                Save(ColorMapImage, albedoFilePath);
            }
        }

        public void Save(BitmapSource image, string filePath)
        {
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));

            using (var fileStream = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
            {
                encoder.Save(fileStream);
            }
        }

        #endregion
    }
}
