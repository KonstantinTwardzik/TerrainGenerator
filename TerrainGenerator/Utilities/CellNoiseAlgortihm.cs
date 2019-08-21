using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topographer3D.Utilities
{

    internal class CellNoiseAlgortihm
    {
        private Random rng;
        private int sizeCompensator;
        private int cellSize;
        private int cellQuantity;
        private int[,] featurePoints;
        private int evaluationFunction;
        private int octaveHelper;



        internal void PrepareValues(int seed, int terrainSize, int quantity, int evaluationFunction, int octave)
        {
            // Preparing the Random Seed and other Stuff
            this.evaluationFunction = evaluationFunction;
            this.octaveHelper = octave *2;
            rng = new Random(seed);
            sizeCompensator = TerrainUtilities.GetSizeCompensator(terrainSize);
            CreateGrid(terrainSize, (float)quantity);
        }

        internal void CreateGrid(int terrainSize, float quantity)
        {
            // Creating Grid
            cellQuantity = (int)Math.Sqrt(quantity);
            cellSize = terrainSize / cellQuantity;
            cellQuantity +=octaveHelper*2;

            if (cellSize > 3)
            {
                // Creating Randomly distribtuted FeaturePoints
                featurePoints = new int[cellQuantity * cellQuantity, 2];
                for (int CellX = 0; CellX < cellQuantity; CellX++)
                {
                    for (int CellZ = 0; CellZ < cellQuantity; CellZ++)
                    {
                        int randomX = rng.Next(CellX * cellSize, (CellX + 1) * cellSize);
                        int randomZ = rng.Next(CellZ * cellSize, (CellZ + 1) * cellSize);
                        randomX -= cellSize * octaveHelper;
                        randomZ -= cellSize * octaveHelper;
                        featurePoints[CellZ + CellX * cellQuantity, 0] = randomX;
                        featurePoints[CellZ + CellX * cellQuantity, 1] = randomZ;
                    }
                }
            }
        }

        internal float Evaluate(int x, int z)
        {
            float value = 0;
            if (cellSize > 3)
            {
                int[] cellAndNeighbourCells = TerrainUtilities.GetPositionAndNeighbours((x + cellSize * octaveHelper) / cellSize, (z + cellSize * octaveHelper) / cellSize, cellQuantity);

                float[] distances = new float[cellAndNeighbourCells.Length];
                for (int i = 0; i < cellAndNeighbourCells.Length; i++)
                {
                    // Euclidian Distance
                    int lengthX = 0;
                    int lengthZ = 0;
                    if (x > featurePoints[cellAndNeighbourCells[i], 0])
                    {
                        lengthX = x - featurePoints[cellAndNeighbourCells[i], 0];
                    }
                    else
                    {
                        lengthX = featurePoints[cellAndNeighbourCells[i], 0] - x;
                    }
                    if (z > featurePoints[cellAndNeighbourCells[i], 1])
                    {
                        lengthZ = z - featurePoints[cellAndNeighbourCells[i], 1];
                    }
                    else
                    {
                        lengthZ = featurePoints[cellAndNeighbourCells[i], 1] - z;
                    }
                    float currentDistance = (float)Math.Sqrt((double)(lengthX * lengthX + lengthZ * lengthZ));

                    distances[i] = currentDistance;
                }
                Array.Sort(distances);

                value = distances[evaluationFunction - 1] / (sizeCompensator * 2);
            }


            return value;
        }
    }
}
