using System.ComponentModel;
using Topographer3D.Utilities;

namespace Topographer3D.ViewModels.Layers
{
    class HeightLayer : BaseLayer
    {
        #region ATTRIBUTES & PROPERTIES
        private float[] TerrainPoints;
        private int TerrainSize;
        public float Height { get; set; }

        #endregion

        #region INITIALIZATION
        public HeightLayer(LayerManager layerManager, TerrainEngine terrainEngine) : base(layerManager, terrainEngine)
        {
            InitProperties();
        }

        private void InitProperties()
        {
            LayerType = Layer.Height;
            Height = 0.5f;
        }

        #endregion

        #region TERRAIN ENGINE PROCESSING
        public void StartHeight(int TerrainSize, float[] TerrainPoints)
        {
            this.TerrainSize = TerrainSize;
            this.TerrainPoints = TerrainPoints;

            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += HeightCalculation;
            worker.ProgressChanged += HeightUpdate;
            worker.RunWorkerCompleted += HeightFinished;
            worker.RunWorkerAsync(100000);
        }

        private void HeightCalculation(object sender, DoWorkEventArgs e)
        {
            for (int x = 0; x < TerrainSize; x++)
            {
                for (int z = 0; z < TerrainSize; z++)
                {
                    TerrainPoints[z + x * TerrainSize] = Application.Apply(TerrainPoints[z + x * TerrainSize], Height, CurrentApplicationMode);
                }

                int progressPercentage = (int)(((float)x / (float)TerrainSize) * 100);
                (sender as BackgroundWorker).ReportProgress(progressPercentage);
            }
        }

        private void HeightUpdate(object sender, ProgressChangedEventArgs e)
        {
            ProgressPercentage = e.ProgressPercentage;
        }

        private void HeightFinished(object sender, RunWorkerCompletedEventArgs e)
        {
            Processed();
            terrainEngine.SingleLayerCalculationComplete(this, TerrainPoints);
            Dispose();
        }

        #endregion

        #region DISPOSABLE SUPPORT
        protected override void Dispose()
        {
            TerrainPoints = null;
        }

        #endregion
    }
}
