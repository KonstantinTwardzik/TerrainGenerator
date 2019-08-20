using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Topographer3D.Utilities;

namespace Topographer3D.ViewModels.Layers
{
    // VORONOI NOT IMPLEMENTED YET
    class CellNoiseLayer : BaseLayer
    {
        private float[] TerrainPoints;
        private int TerrainSize;
        private CellNoiseAlgortihm cellNoiseAlgorithm;

        public int Quantity { get; set; }
        public int Seed { get; set; }
        public int Distance { get; set; }
        public float Height { get; set; }
        public int Octaves { get; set; }




        public CellNoiseLayer(LayerManager layerManager, TerrainEngine terrainEngine) : base(layerManager, terrainEngine)
        {
            InitProperties();
            cellNoiseAlgorithm = new CellNoiseAlgortihm();
        }

        private void InitProperties()
        {
            LayerType = Layer.CellNoise;
            Quantity = 100;
            Seed = 100;
            Height = 0.3f;
            Distance = 1;
            Octaves = 1;
        }

        public void StartCellNoise(int TerrainSize, float[] TerrainPoints)
        {
            this.TerrainSize = TerrainSize;
            this.TerrainPoints = TerrainPoints;

            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += CellNoiseCalculation;
            worker.ProgressChanged += CellNoiseUpdate;
            worker.RunWorkerCompleted += CellNoiseComplete;
            worker.RunWorkerAsync(100000);
        }

        private void CellNoiseCalculation(object sender, DoWorkEventArgs e)
        {
            float[] helper = new float[TerrainPoints.Length];

            for (int octave = 1; octave <= Octaves; octave++)
            {
                float height = Height * (1 / (float)octave);
                int quantity = Quantity * octave + 1;
                cellNoiseAlgorithm.PrepareValues(Seed, TerrainSize, quantity, Distance, octave);

                // Calculate Heights depending on Distance to the next FeaturePoint
                for (int x = 0; x < TerrainSize; x++)
                {
                    for (int z = 0; z < TerrainSize; z++)
                    {
                        float value = cellNoiseAlgorithm.Evaluate(x, z);
                        value *= height;
                        helper[z + x * TerrainSize] = TerrainUtilities.Apply(helper[z + x * TerrainSize], value, ApplicationMode.Add);
                    }
                }
                int progressPercentage = (int)(((float)octave / (float)Octaves) * 100);
                (sender as BackgroundWorker).ReportProgress(progressPercentage);
            }

            for (int x = 0; x < TerrainSize; x++)
            {
                for (int z = 0; z < TerrainSize; z++)
                {
                    TerrainPoints[z + x * TerrainSize] = TerrainUtilities.Apply(TerrainPoints[z + x * TerrainSize], helper[z + x * TerrainSize], CurrentApplicationMode);
                }
            }

        }

        private void CellNoiseUpdate(object sender, ProgressChangedEventArgs e)
        {
            ProgressPercentage = e.ProgressPercentage;
        }

        private void CellNoiseComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            Processed();
            terrainEngine.SingleLayerCalculationComplete(this, TerrainPoints);
        }

    }
}





























