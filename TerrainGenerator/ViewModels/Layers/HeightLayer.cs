using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topographer3D.ViewModels.Layers
{
    class HeightLayer : BaseLayer
    {
        #region Attributes
        private float[] TerrainPoints;
        private int TerrainSize;

        #endregion

        #region Properties
        public float Height { get; set; }


        #endregion

        #region Initialization
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

        #region TerrainEngine Processing
        public void StartHeight(int TerrainSize, float[] TerrainPoints)
        {
            this.TerrainSize = TerrainSize;
            this.TerrainPoints = TerrainPoints;

            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.RunWorkerAsync(100000);
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int x = 0; x < TerrainSize; x++)
            {
                for (int z = 0; z < TerrainSize; z++)
                {
                    TerrainPoints[x + z * TerrainSize] = ApplyMode(TerrainPoints[x + z * TerrainSize], Height);
                }

                int progressPercentage = (int)(((float)x / (float)TerrainSize) * 100);
                (sender as BackgroundWorker).ReportProgress(progressPercentage);
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

        protected override void Dispose()
        {
            TerrainPoints = null;
        }


        #endregion


    }
}
