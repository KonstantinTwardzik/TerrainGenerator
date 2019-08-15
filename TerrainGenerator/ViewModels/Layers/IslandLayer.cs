using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Topographer3D.Utilities;

namespace Topographer3D.ViewModels.Layers
{
    class IslandLayer : BaseLayer
    {
        #region ATTRIBUTES & PROPERTIES
        private float[] TerrainPoints;
        private int TerrainSize;

        public float OutterHeight { get; set; }
        public float InnerHeight { get; set; }
        public float Size { get; set; }
        public IEnumerable<InterpolationMode> InterpolationModeEnum { get { return Enum.GetValues(typeof(InterpolationMode)).Cast<InterpolationMode>(); } }
        public InterpolationMode CurrentInterpolationMode { get; set; }

        #endregion

        #region INITIALIZATION
        public IslandLayer(LayerManager layerManager, TerrainEngine terrainEngine) : base(layerManager, terrainEngine)
        {
            InitProperties();
        }

        private void InitProperties()
        {
            LayerType = Layer.Island;
            OutterHeight = 0.0f;
            InnerHeight = 1.0f;
            Size = 0.5f;
            CurrentInterpolationMode = InterpolationMode.Smooth;
        }

        #endregion

        #region TerrainEngine Processing
        public void StartIsland(int TerrainSize, float[] TerrainPoints)
        {
            this.TerrainSize = TerrainSize;
            this.TerrainPoints = TerrainPoints;

            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += IslandCalculation;
            worker.ProgressChanged += IslandUpdate;
            worker.RunWorkerCompleted += IslandComplete;
            worker.RunWorkerAsync(100000);
        }

        private void IslandCalculation(object sender, DoWorkEventArgs e)
        {
            float value = 0;
            float heightRange = InnerHeight - OutterHeight;
            float highestValue = 1;
            float lowestValue = 0;
            float distance = 0;
            float midPosition = TerrainSize / 2;
            float maxDistance = midPosition * (Size + 0.5f);

            if (InnerHeight < OutterHeight)
            {
                highestValue = OutterHeight;
                lowestValue = InnerHeight;
            }
            else
            {
                highestValue = InnerHeight;
                lowestValue = OutterHeight;
            }

            for (int x = 0; x < TerrainSize; x++)
            {
                for (int z = 0; z < TerrainSize; z++)
                {
                    float currentPositionX = x - midPosition;
                    float currentPositionZ = z - midPosition;
                    distance = (float)Math.Sqrt((double)(currentPositionX * currentPositionX + currentPositionZ * currentPositionZ));

                    switch (CurrentInterpolationMode)
                    {
                        case InterpolationMode.Linear:
                            if (OutterHeight >= InnerHeight)
                            {
                                value = lowestValue - ((distance / maxDistance) * heightRange);
                            }
                            else
                            {
                                value = highestValue - ((distance / maxDistance) * heightRange);
                            }

                            if (value < lowestValue)
                            {
                                value = lowestValue;
                            }
                            else if (value > highestValue)
                            {
                                value = highestValue;
                            }
                            break;
                        case InterpolationMode.Smooth:
                            if (heightRange < 0)
                            {
                                value = highestValue - (float)((Math.Cos(Math.PI - (1 - ((distance / maxDistance))) * Math.PI)) / 2.0f + 0.5f) * -heightRange;
                            }
                            else
                            {
                                value = lowestValue + (float)((Math.Cos(Math.PI - (1 - ((distance / maxDistance))) * Math.PI)) / 2.0f + 0.5f) * heightRange;
                            }

                            if (distance >= maxDistance)
                            {
                                value = OutterHeight;
                            }
                            break;
                    }

                    TerrainPoints[x + z * TerrainSize] = Application.Apply(TerrainPoints[x + z * TerrainSize], value, CurrentApplicationMode);
                }

                int progressPercentage = (int)(((float)x / (float)TerrainSize) * 100);
                (sender as BackgroundWorker).ReportProgress(progressPercentage);
            }
        }

        private void IslandUpdate(object sender, ProgressChangedEventArgs e)
        {
            ProgressPercentage = e.ProgressPercentage;
        }

        private void IslandComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            Processed();
            terrainEngine.SingleLayerCalculationComplete(this, TerrainPoints);
        }

        #endregion

    }
}
