using System.ComponentModel;

namespace Topographer3D.ViewModels.Layers
{
    class SlopeLayer : BaseLayer
    {
        #region Attributes
        private float[] TerrainPoints;
        private int TerrainSize;

        #endregion

        #region Properties
        public float StartHeight { get; set; }
        public float EndHeight { get; set; }
        public bool UseXDirection { get; set; }


        #endregion

        #region Initialization
        public SlopeLayer(LayerManager layerManager, TerrainEngine terrainEngine) : base(layerManager, terrainEngine)
        {
            InitProperties();
        }

        private void InitProperties()
        {
            LayerType = Layer.Slope;
            StartHeight = 0.0f;
            EndHeight = 1.0f;
            UseXDirection = true;
        }

        #endregion

        #region TerrainEngine Processing
        public void StartSlope(int TerrainSize, float[] TerrainPoints)
        {
            this.TerrainSize = TerrainSize;
            this.TerrainPoints = TerrainPoints;

            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += SlopeCalculation;
            worker.ProgressChanged += ProgressChanged;
            worker.RunWorkerCompleted += CalculationCompleted;
            worker.RunWorkerAsync(100000);
        }

        private void SlopeCalculation(object sender, DoWorkEventArgs e)
        {
            float value = 0;
            float difference = 0;
            float lowestValue = 0;
            if (EndHeight <= StartHeight)
            {
                lowestValue = EndHeight;
                difference = StartHeight - EndHeight;

            }
            else
            {
                lowestValue = StartHeight;
                difference = EndHeight - StartHeight;
            }

            for (int x = 0; x < TerrainSize; x++)
            {
                for (int z = 0; z < TerrainSize; z++)
                {

                    if (UseXDirection)
                    {
                        value = lowestValue + (((float)x / (float)TerrainSize) * difference);
                    } else
                    {
                        value = lowestValue + (((float)z / (float)TerrainSize) * difference);
                    }
                    TerrainPoints[x + z * TerrainSize] = ApplyMode(TerrainPoints[x + z * TerrainSize], value);
                }

                int progressPercentage = (int)(((float)x / (float)TerrainSize) * 100);
                (sender as BackgroundWorker).ReportProgress(progressPercentage);
            }
        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressPercentage = e.ProgressPercentage;
        }


        private void CalculationCompleted(object sender, RunWorkerCompletedEventArgs e)
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

        protected override void Dispose()
        {
            TerrainPoints = null;
        }


        #endregion


    }
}
