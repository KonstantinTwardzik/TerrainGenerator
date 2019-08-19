using System;
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
        public int VoronoiOctaves { get; set; }
        public float VoronoiOctaveWeight { get; set; }
        public float VoronoiScaleX { get; set; }
        public float VoronoiScaleZ { get; set; }
        public int VoronoiSeed { get; set; }

        public CellNoiseLayer(LayerManager layerManager, TerrainEngine terrainEngine) : base(layerManager, terrainEngine)
        {
            InitProperties();
        }

        private void InitProperties()
        {
            LayerType = Layer.CellNoise;
            Quantity = 100;
            rng = new Random();
        }

        public void StartCellNoise(int TerrainSize, float[] TerrainPoints)
        {
            this.TerrainSize = TerrainSize;
            this.TerrainPoints = TerrainPoints;

            int cellQuantity = (int)Math.Sqrt((float)Quantity);
            int cellSize = TerrainSize / cellQuantity;

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
                    int[] neighbourCells = //TerrainUtilities.GetNeighbours;
                    float m_dist = 100f;
                    for (int i = 0; i < neighbourCells.Length; i++)
                    {
                        int lengthX = 0;
                        int lengthZ = 0;
                        if (x > featurePoints[i, 0])
                        {
                            lengthX = x - featurePoints[i, 0];
                        }
                        else
                        {
                            lengthX = featurePoints[i, 0] - x;
                        }
                        if (z > featurePoints[i, 1])
                        {
                            lengthZ = z - featurePoints[i, 1];
                        }
                        else
                        {
                            lengthZ = featurePoints[i, 1] - z;
                        }

                        float dist = (float)Math.Sqrt((double)(lengthX * lengthX + lengthZ * lengthZ));
                        m_dist = Math.Min(m_dist, dist);
                    }
                    TerrainPoints[z + x * TerrainSize] = m_dist / 100;
                }

            }


            Processed();
            terrainEngine.SingleLayerCalculationComplete(this, TerrainPoints);
        }
    }
}





























