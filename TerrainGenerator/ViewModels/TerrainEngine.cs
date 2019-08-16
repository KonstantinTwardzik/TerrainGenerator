using System;
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
        private int currentLayerPosition;

        // TERRAIN VALUES
        public float[] TerrainHeights
        {
            get { return terrainModel.TerrainHeights; }
            set { terrainModel.TerrainHeights = value; }
        }
        public float[] PreviousTerrainHeights
        {
            get { return terrainModel.PreviousTerrainHeights; }
            set { terrainModel.PreviousTerrainHeights = value; }
        }

        public byte[] PreviousColors
        {
            get { return terrainModel.PreviousColors; }
            set { terrainModel.PreviousColors = value; }
        }

        public byte[] CurrentColors
        {
            get { return terrainModel.CurrentColors; }
            set { terrainModel.CurrentColors = value; }
        }

        public int TerrainSize
        {
            get { return terrainModel.TerrainSize; }
            set { terrainModel.TerrainSize = value; }
        }


        // MAP EXPORT
        public BitmapImage HeightMapImage { get; set; }
        public BitmapImage ColorMapImage { get; set; }

        #endregion

        #region INITIALIZATION
        public TerrainEngine(TerrainModel terrainModel)
        {
            this.terrainModel = terrainModel;
            currentLayerPosition = 0;
            TerrainSize = 512;
            FlattenTerrain();
        }

        internal void InitLogic(LayerManager layerManager, MainViewModel mainViewModel)
        {
            this.layerManager = layerManager;
            this.mainViewModel = mainViewModel;
        }

        internal void FlattenTerrain()
        {
            TerrainHeights = new float[TerrainSize * TerrainSize];
            PreviousTerrainHeights = new float[TerrainSize * TerrainSize];

            for (int x = 0; x < TerrainSize; x++)
            {
                for (int z = 0; z < TerrainSize; z++)
                {
                    TerrainHeights[z + x * TerrainSize] = 0;
                }
            }
        }

        #endregion

        #region LAYER HANDLING

        public void StartCalculationToLayer(BaseLayer layer)
        {
            layerManager.SetStatusBar(true);
            if (layer.Position < currentLayerPosition || currentLayerPosition == 0)
            {
                Console.WriteLine("layer.Position < currentLayerPosition || currentLayerPosition == 0");
                ResetTerrainEngine();
                currentLayerPosition = layer.Position;
                SingleLayerCalculationStart(0);
            }

            else if (layer.Position == currentLayerPosition && layerManager.Layers[currentLayerPosition - 1].IsProcessed == true)
            {
                Console.WriteLine("layer.Position == currentLayerPosition && layerManager.Layers[currentLayerPosition - 1].IsProcessed == true");
                if (layer.LayerType != Layer.DetailColorization)
                {
                    for (int x = 0; x < TerrainSize; x++)
                    {
                        for (int z = 0; z < TerrainSize; z++)
                        {
                            TerrainHeights[z + x * TerrainSize] = PreviousTerrainHeights[z + x * TerrainSize];
                        }
                    }
                }
                SingleLayerCalculationStart(layer.Position);
            }
            else if (layer.Position > currentLayerPosition)
            {
                Console.WriteLine("layer.Position > currentLayerPosition");
                for (int x = 0; x < TerrainSize; x++)
                {
                    for (int z = 0; z < TerrainSize; z++)
                    {
                        PreviousTerrainHeights[z + x * TerrainSize] = TerrainHeights[z + x * TerrainSize];
                    }
                }
                if(CurrentColors != null)
                {
                    PreviousColors = new byte[CurrentColors.Length];
                    for (int i = 0; i < CurrentColors.Length; i++)
                    {
                        PreviousColors[i] = CurrentColors[i];
                        
                    }

                }


                int startCalculationPosition = currentLayerPosition + 1;
                currentLayerPosition = layer.Position;
                SingleLayerCalculationStart(startCalculationPosition);
            }
        }

        private void SingleLayerCalculationStart(int layerPosition)
        {
            BaseLayer currentLayer = layerManager.Layers[layerPosition];

            switch (currentLayer.LayerType)
            {
                case Layer.Height:
                    HeightLayer heightLayer = currentLayer as HeightLayer;
                    heightLayer.StartHeight(TerrainSize, TerrainHeights);
                    break;
                case Layer.Slope:
                    SlopeLayer slopeLayer = currentLayer as SlopeLayer;
                    slopeLayer.StartSlope(TerrainSize, TerrainHeights);
                    break;
                case Layer.Island:
                    IslandLayer islandLayer = currentLayer as IslandLayer;
                    islandLayer.StartIsland(TerrainSize, TerrainHeights);
                    break;
                case Layer.OpenSimplex:
                    OpenSimplexNoiseLayer OSNLayer = currentLayer as OpenSimplexNoiseLayer;
                    OSNLayer.StartOpenSimplexNoise(TerrainSize, TerrainHeights);
                    break;

                case Layer.Hydraulic:
                    HydraulicErosionLayer hydraulicErosionLayer = currentLayer as HydraulicErosionLayer;
                    hydraulicErosionLayer.StartErosion(TerrainSize, TerrainHeights);
                    break;

                case Layer.DetailColorization:
                    DetailColorizationLayer detailColorizationLayer = currentLayer as DetailColorizationLayer;
                    detailColorizationLayer.StartColorization(TerrainSize, TerrainHeights);
                    break;
                case Layer.HeightColorization:
                    HeightColorizationLayer heightColorizationLayer = currentLayer as HeightColorizationLayer;
                    heightColorizationLayer.StartColorization(TerrainSize, TerrainHeights, PreviousColors);
                    break;
            }

        }

        internal void SingleLayerCalculationComplete(BaseLayer layer, float[] terrainHeights)
        {
            if (layer.Position < currentLayerPosition)
            {
                for (int x = 0; x < TerrainSize; x++)
                {
                    for (int z = 0; z < TerrainSize; z++)
                    {
                        PreviousTerrainHeights[z + x * TerrainSize] = terrainHeights[z + x * TerrainSize];
                    }
                }
            }
            //TerrainHeights = terrainHeights;
            mainViewModel.UpdateMesh();
            StartNextLayerCalculation(layer);
        }

        internal void SingleLayerCalculationComplete(BaseLayer layer, MemoryStream terrainMainColors, MemoryStream terrainBorderColors, byte[] currentColors)
        {
            if (layer.Position < currentLayerPosition)
            {
                PreviousColors = new byte[currentColors.Length];
                
                for (int i = 0; i < currentColors.Length; i++)
                {
                    PreviousColors[i] = currentColors[i];
                }
            }
            CurrentColors = new byte[currentColors.Length];
            for (int i = 0; i < currentColors.Length; i++)
            {
                CurrentColors[i] = currentColors[i];
            }
            mainViewModel.UpdateTextures(terrainMainColors, terrainBorderColors);
            CreateAlbedoMap(terrainMainColors);
            StartNextLayerCalculation(layer);
        }

        internal void SingleLayerCalculationComplete(BaseLayer layer, MemoryStream terrainMainColors, byte[] currentColors)
        {
            if (layer.Position < currentLayerPosition)
            {
                PreviousColors = new byte[currentColors.Length];

                for (int i = 0; i < currentColors.Length; i++)
                {
                    PreviousColors[i] = currentColors[i];
                }
            }
            CurrentColors = new byte[currentColors.Length];
            for (int i = 0; i < currentColors.Length; i++)
            {
                CurrentColors[i] = currentColors[i];
            }
            mainViewModel.UpdateTexture(terrainMainColors);
            CreateAlbedoMap(terrainMainColors);
            StartNextLayerCalculation(layer);
        }

        private void StartNextLayerCalculation(BaseLayer layer)
        {
            int currentLayer = layer.Position + 1;
            if (currentLayer <= currentLayerPosition)
            {
                SingleLayerCalculationStart(currentLayer);

            }
            else
            {
                layerManager.SetStatusBar(false);
            }
        }

        internal void ResetTerrainEngine()
        {
            currentLayerPosition = 0;
            foreach (BaseLayer layer in layerManager.Layers)
            {
                layer.Unprocessed();
            }
            FlattenTerrain();
            mainViewModel.UpdateMesh();
            mainViewModel.ResetTextures();
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
