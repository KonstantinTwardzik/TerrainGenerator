using System;
using System.ComponentModel;
using Topographer3D.Utilities;

namespace Topographer3D.ViewModels.Layers
{
    // VORONOI NOT IMPLEMENTED YET
    class CellNoiseLayer : BaseLayer
    {
        private float[] TerrainPoints;
        private int TerrainSize;
        private Random rng;

        public int Quantity { get; set; }
        public int Seed { get; set; }

        public CellNoiseLayer(LayerManager layerManager, TerrainEngine terrainEngine) : base(layerManager, terrainEngine)
        {
            InitProperties();
        }

        private void InitProperties()
        {
            LayerType = Layer.CellNoise;
            Quantity = 100;
            Seed = 100;

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
            rng = new Random(Seed);
            int cellQuantity = (int)Math.Sqrt((float)Quantity);

            int cellSize = TerrainSize / cellQuantity;
            cellQuantity += 1;
            int completeSize = cellSize * cellQuantity;

            int[,] featurePoints = new int[cellQuantity * cellQuantity, 2];
            for (int CellX = 0; CellX < cellQuantity; CellX++)
            {
                for (int CellZ = 0; CellZ < cellQuantity; CellZ++)
                {
                    int randomX = rng.Next(CellX * cellSize, (CellX + 1) * cellSize);
                    int randomZ = rng.Next(CellZ * cellSize, (CellZ + 1) * cellSize);
                    featurePoints[CellZ + CellX * cellQuantity, 0] = randomX;
                    featurePoints[CellZ + CellX * cellQuantity, 1] = randomZ;
                }
            }

            for (int x = 0; x < TerrainSize; x++)
            {
                for (int z = 0; z < TerrainSize; z++)
                {
                    int[] CellAndNeighbourCells = TerrainUtilities.GetPositionAndNeighbours(x / cellSize, z / cellSize, cellQuantity);
                    float m_dist = 100f;
                    for (int i = 0; i < CellAndNeighbourCells.Length; i++)
                    {
                        int lengthX = 0;
                        int lengthZ = 0;
                        if (x > featurePoints[CellAndNeighbourCells[i], 0])
                        {
                            lengthX = x - featurePoints[CellAndNeighbourCells[i], 0];
                        }
                        else
                        {
                            lengthX = featurePoints[CellAndNeighbourCells[i], 0] - x;
                        }
                        if (z > featurePoints[CellAndNeighbourCells[i], 1])
                        {
                            lengthZ = z - featurePoints[CellAndNeighbourCells[i], 1];
                        }
                        else
                        {
                            lengthZ = featurePoints[CellAndNeighbourCells[i], 1] - z;
                        }

                        float dist = (float)Math.Sqrt((double)(lengthX * lengthX + lengthZ * lengthZ));
                        m_dist = Math.Min(m_dist, dist);
                    }
                    float value = m_dist / 100;
                    TerrainPoints[z + x * TerrainSize] = TerrainUtilities.Apply(TerrainPoints[z + x * TerrainSize], value, CurrentApplicationMode);
                    //DRAW GRID INTO 3D MODEL
                    //if (x % (int)cellSize == 0 || z % (int)cellSize == 0)
                    //{
                    //    TerrainPoints[z + x * TerrainSize] = 0;
                    //}
                }
                int progressPercentage = (int)(((float)x / (float)TerrainSize) * 100);
                (sender as BackgroundWorker).ReportProgress(progressPercentage);
            }

            for (int x = 0; x < TerrainSize; x++)
            {
                for (int z = 0; z < TerrainSize; z++)
                {

                    if (x > completeSize)
                    {
                        //float value = TerrainPoints[z+x*];
                        //TerrainPoints[z + x * TerrainSize] = TerrainUtilities.Apply(TerrainPoints[z + x * TerrainSize], value, CurrentApplicationMode);
                        TerrainPoints[z + x * TerrainSize] = 0;
                    }
                    if (z > completeSize)
                    {
                        //float value = TerrainPoints[];
                        //TerrainPoints[z + x * TerrainSize] = TerrainUtilities.Apply(TerrainPoints[z + x * TerrainSize], value, CurrentApplicationMode);
                        TerrainPoints[z + x * TerrainSize] = 0;
                    }


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





























