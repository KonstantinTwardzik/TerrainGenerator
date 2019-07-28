using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Topographer3D.Models;
using Topographer3D.Utilities;
using Topographer3D.ViewModels.Layers;
using Color = System.Windows.Media.Color;

namespace Topographer3D.ViewModels
{
    class TerrainEngine : ObservableObject
    {
        #region Attributes
        private LayerManager layerManager;
        private MainViewModel mainViewModel;

        private int currentLayerPosition;

        //Texturing
        private BitmapImage _colorMapImage;
        private BitmapImage _heightMapImage;
        private BitmapImage _borderMapImage;
        #endregion

        #region Properties
        //Elementary Values
        public float[] TerrainPoints { get; set; }
        public int TerrainSize { get; set; }


        // Coloring
        public BitmapImage HeightMapImage { get; set; }
        public BitmapImage ColorMapImage { get; set; }
        public BitmapImage BorderMapImage { get; set; }
        #endregion

        #region Initialization
        public TerrainEngine()
        {
            InitHeights();
            InitAttributes();
            InitProperties();
        }

        internal void InitLogic(LayerManager layerManager, MainViewModel mainViewModel)
        {
            this.layerManager = layerManager;
            this.mainViewModel = mainViewModel;
        }

        public void InitAttributes()
        {
            // Coloring
            _colorMapImage = new BitmapImage();
            _heightMapImage = new BitmapImage();
            currentLayerPosition = 0;
        }

        public void InitProperties()
        {
            TerrainSize = 512;


        }

        public void InitHeights()
        {
            TerrainSize = 512;
            GenerateTerrainPoints();
        }

        private void GenerateTerrainPoints()
        {
            TerrainPoints = new float[TerrainSize * TerrainSize];

            for (int x = 0; x < TerrainSize; x++)
            {
                for (int z = 0; z < TerrainSize; z++)
                {
                    TerrainPoints[x + z * TerrainSize] = 0;
                }
            }
        }
        #endregion

        #region Terrain Generation

        public void StartCalculation(BaseLayer layer)
        {
            if (layer.Position <= currentLayerPosition || currentLayerPosition == 0)
            {
                foreach (BaseLayer layer0 in layerManager.Layers)
                {
                    layer0.Unprocessed();
                }

                currentLayerPosition = layer.Position;
                ResetHeights();
                StartLayerCalculation(0);
            }
            else
            {
                int startCalculatioPosition = currentLayerPosition + 1;
                currentLayerPosition = layer.Position;
                StartLayerCalculation(startCalculatioPosition);
            }
            layerManager.SetStatusBar(true);


        }

        internal void WorkerComplete(BaseLayer layer, float[] terrainPoints)
        {
            TerrainPoints = terrainPoints;
            mainViewModel.ChangeMesh();
            int currentLayer = layer.Position + 1;
            if (currentLayer <= currentLayerPosition)
            {
                StartLayerCalculation(currentLayer);
            }
            else
            {
                layerManager.SetStatusBar(false);
            }

        }

        private void StartLayerCalculation(int layerPosition)
        {
            BaseLayer currentLayer = layerManager.Layers[layerPosition];

            switch (currentLayer.LayerType)
            {
                case Layer.Height:
                    HeightLayer heightLayer = currentLayer as HeightLayer;
                    heightLayer.StartHeight(TerrainSize, TerrainPoints);
                    break;
                case Layer.Slope:
                    SlopeLayer slopeLayer = currentLayer as SlopeLayer;
                    slopeLayer.StartSlope(TerrainSize, TerrainPoints);
                    break;
                case Layer.Island:
                    IslandLayer islandLayer = currentLayer as IslandLayer;
                    islandLayer.StartIsland(TerrainSize, TerrainPoints);
                    break;
                case Layer.OpenSimplex:
                    OpenSimplexNoiseLayer OSNLayer = currentLayer as OpenSimplexNoiseLayer;
                    OSNLayer.StartOpenSimplexNoise(TerrainSize, TerrainPoints);
                    break;

                case Layer.Hydraulic:
                    HydraulicErosionLayer hydraulicErosionLayer = currentLayer as HydraulicErosionLayer;
                    hydraulicErosionLayer.StartErosion(TerrainSize, TerrainPoints);
                    break;
            }

        }

        public void ChangeDetailResolution(int terrainSize)
        {
            TerrainSize = terrainSize;
            GenerateTerrainPoints();
        }

        public void ResetHeights()
        {
            for (int x = 0; x < TerrainSize; x++)
            {
                for (int z = 0; z < TerrainSize; z++)
                {
                    TerrainPoints[x + z * TerrainSize] = 0;
                }
            }
            mainViewModel.ChangeMesh();
        }

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
                    byte[] bytes = BitConverter.GetBytes((float)TerrainPoints[x + z * TerrainSize]);
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
            _heightMapImage = new BitmapImage();

            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            encoder.Save(memoryStream);

            memoryStream.Position = 0;
            _heightMapImage.BeginInit();
            _heightMapImage.StreamSource = new MemoryStream(memoryStream.ToArray());
            _heightMapImage.EndInit();
            _heightMapImage.Freeze();
        }

        public void ExportMaps(String filePath)
        {
            int index = filePath.LastIndexOf(@".");

            if (_heightMapImage.IsFrozen)
            {
                String heightFilePath = filePath.Insert(index, "_height");
                Save(_heightMapImage, heightFilePath);
            }

            if (_colorMapImage.IsFrozen)
            {
                String albedoFilePath = filePath.Insert(index, "_albedo");
                Save(_colorMapImage, albedoFilePath);
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
