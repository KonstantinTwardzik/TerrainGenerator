using System.ComponentModel;
using System.Threading;
using Topographer3D.Models;

namespace Topographer3D.ViewModels.Layers
{
  class OpenSimplexNoiseLayer : BaseLayer
    {
        #region Attributes
        private OpenSimplexNoise _openSimplexNoise;
        private float[] TerrainPoints;
        private int TerrainSize;
        private int sizeCompensator;

        #endregion

        #region Properties
        public float OSNScale { get; set; }
        public float OSNScaleX { get; set; }
        public float OSNScaleZ { get; set; }
        public int OSNOctaves { get; set; }
        public float OSNOctaveWeight { get; set; }
        public int OSNSeed { get; set; }
        public float OSNStrength { get; set; }

        #endregion

        #region Initialization
        public OpenSimplexNoiseLayer(LayerManager layerManager, TerrainEngine terrainEngine) : base(layerManager, terrainEngine)
        {
            _openSimplexNoise = new OpenSimplexNoise();
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

        #region TerrainEngine Processing
        public void StartOpenSimplexNoise(int TerrainSize, float[] TerrainPoints, int sizeCompensator)
        {
            this.TerrainSize = TerrainSize;
            this.TerrainPoints = TerrainPoints;
            this.sizeCompensator = sizeCompensator;

            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.RunWorkerAsync(100000);
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            float octaveWeight = 1;
            float octaveMultiplier = 1;
            float[] helper = new float[TerrainPoints.Length];

            for (int octaves = 0; octaves < OSNOctaves; octaves++)
            {
                for (int x = 0; x < TerrainSize; x++)
                {
                    for (int z = 0; z < TerrainSize; z++)
                    {
                        float value = 0;
                        float xValue = (float)((((0.0005f / OSNScale) / OSNScaleX) * (x * sizeCompensator) + OSNSeed) * octaveMultiplier);
                        float zValue = (float)((((0.0005f / OSNScale) / OSNScaleZ) * (z * sizeCompensator) + OSNSeed) * octaveMultiplier);
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
                    TerrainPoints[x + z * TerrainSize] = ApplyMode(TerrainPoints[x + z * TerrainSize], helper[x + z * TerrainSize]);
                }
            }


        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressPercentage = e.ProgressPercentage;
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Processed();
            terrainEngine.WorkerComplete(this, TerrainPoints);
            Dispose();
        }

        private float ApplyMode(float oldValue, float applyValue)
        {
            float newValue = 0;
            switch (CurrentApplicationMode)
            {
                case ApplicationMode.Normal:
                    newValue = applyValue;
                    break;
                case ApplicationMode.Add:
                    newValue = oldValue + applyValue;
                    break;
                case ApplicationMode.Multiply:
                    newValue = oldValue * applyValue;
                    break;
                case ApplicationMode.Subtract:
                    newValue = oldValue - applyValue;
                    break;
            }
            if (newValue < 0)
            {
                newValue = 0;
            }
            else if (newValue > 1)
            {
                newValue = 1;
            }
            return newValue;
        }

        #endregion

        #region Button Handling
        protected override void Dispose()
        {
            TerrainPoints = null;
        }



        #endregion
    }
}
