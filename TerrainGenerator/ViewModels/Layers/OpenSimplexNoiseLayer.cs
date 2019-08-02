using System.ComponentModel;
using Topographer3D.Models;
using Topographer3D.Utilities;

namespace Topographer3D.ViewModels.Layers
{
    class OpenSimplexNoiseLayer : BaseLayer
    {
        #region ATTRIBUTES & PROPERTIES
        private OpenSimplexNoiseAlgorithm _openSimplexNoise;
        private float[] TerrainPoints;
        private int TerrainSize;

        public float OSNScale { get; set; }
        public float OSNScaleX { get; set; }
        public float OSNScaleZ { get; set; }
        public int OSNOctaves { get; set; }
        public float OSNOctaveWeight { get; set; }
        public int OSNSeed { get; set; }
        public float OSNStrength { get; set; }

        #endregion

        #region INITIALIZATION
        public OpenSimplexNoiseLayer(LayerManager layerManager, TerrainEngine terrainEngine) : base(layerManager, terrainEngine)
        {
            _openSimplexNoise = new OpenSimplexNoiseAlgorithm();
            InitProperties();
        }

        private void InitProperties()
        {
            LayerType = Layer.OpenSimplex;
            OSNScale = 1.0f;
            OSNScaleX = 0.5f;
            OSNScaleZ = 0.5f;
            OSNOctaves = 6;
            OSNOctaveWeight = 0.6f;
            OSNSeed = 500;
            OSNStrength = 1.0f;
        }

        #endregion

        #region TERRAIN ENGINE PROCESSING
        public void StartOpenSimplexNoise(int TerrainSize, float[] TerrainPoints)
        {
            this.TerrainSize = TerrainSize;
            this.TerrainPoints = TerrainPoints;

            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += OpenSimplexNoiseCalculation;
            worker.ProgressChanged += OpenSimplexNoiseUpdate;
            worker.RunWorkerCompleted += OpenSimplexNoiseComplete;
            worker.RunWorkerAsync(100000);
        }

        private void OpenSimplexNoiseCalculation(object sender, DoWorkEventArgs e)
        {
            float octaveWeight = 1;
            float octaveMultiplier = 1;
            int sizeCompensator = 1;
            float[] helper = new float[TerrainPoints.Length];

            switch (TerrainSize)
            {
                case 16:
                    sizeCompensator = 256;
                    break;
                case 32:
                    sizeCompensator = 128;
                    break;
                case 64:
                    sizeCompensator = 64;
                    break;
                case 128:
                    sizeCompensator = 32;
                    break;

                case 512:
                    sizeCompensator = 8;
                    break;
                case 1024:
                    sizeCompensator = 4;
                    break;
                case 2048:
                    sizeCompensator = 2;
                    break;
                case 4096:
                    sizeCompensator = 1;
                    break;
            }

            for (int octaves = 0; octaves < OSNOctaves; octaves++)
            {
                for (int x = 0; x < TerrainSize; x++)
                {
                    for (int z = 0; z < TerrainSize; z++)
                    {
                        float value = 0;
                        float xValue = (float)((((0.00025f / OSNScale) / OSNScaleX) * (x * sizeCompensator) + OSNSeed) * octaveMultiplier);
                        float zValue = (float)((((0.00025f / OSNScale) / OSNScaleZ) * (z * sizeCompensator) + OSNSeed) * octaveMultiplier);
                        if (octaves == 0)
                        {
                            value = (float)(((_openSimplexNoise.Evaluate(xValue, zValue) * octaveWeight) + 1) / 2);
                        }
                        else
                        {
                            value = (float)((_openSimplexNoise.Evaluate(xValue, zValue) * octaveWeight) / 2);
                        }

                        helper[x + z * TerrainSize] += value * OSNStrength;
                    }
                }
                octaveWeight /= 2.0f - (OSNOctaveWeight - 0.5f);
                octaveMultiplier = octaves * 2.0f;

                int progressPercentage = (int)(((float)octaves / (float)OSNOctaves) * 100);
                (sender as BackgroundWorker).ReportProgress(progressPercentage);
            }

            for (int x = 0; x < TerrainSize; x++)
            {
                for (int z = 0; z < TerrainSize; z++)
                {
                    TerrainPoints[x + z * TerrainSize] = Application.Apply(TerrainPoints[x + z * TerrainSize], helper[x + z * TerrainSize], CurrentApplicationMode);
                }
            }
        }

        private void OpenSimplexNoiseUpdate(object sender, ProgressChangedEventArgs e)
        {
            ProgressPercentage = e.ProgressPercentage;
        }

        private void OpenSimplexNoiseComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            Processed();
            terrainEngine.SingleLayerCalculationComplete(this, TerrainPoints);
            Dispose();
        }

        #endregion

        #region DISPOSABLE
        protected override void Dispose()
        {
            TerrainPoints = null;
        }

        #endregion
    }
}
