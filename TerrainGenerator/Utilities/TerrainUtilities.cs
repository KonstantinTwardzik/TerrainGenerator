using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topographer3D.Utilities
{
    internal static class TerrainUtilities
    {
        internal static int[] GetNeighbours(int x, int z, int terrainSize)
        {
            int[] neighbours = new int[0];
            if (x == 0 && z == 0)
            {
                neighbours = new int[3];
                neighbours[0] = (z + x * terrainSize) + 1;
                neighbours[1] = (z + x * terrainSize) + terrainSize + 1;
                neighbours[2] = (z + x * terrainSize) + terrainSize;
            }
            else if (x == terrainSize - 1 && z == terrainSize - 1)
            {
                neighbours = new int[3];
                neighbours[0] = (z + x * terrainSize) - 1;
                neighbours[1] = (z + x * terrainSize) - terrainSize - 1;
                neighbours[2] = (z + x * terrainSize) - terrainSize;
            }
            else if (x == 0 && z == terrainSize - 1)
            {
                neighbours = new int[3];
                neighbours[0] = (z + x * terrainSize) + terrainSize;
                neighbours[1] = (z + x * terrainSize) + terrainSize - 1;
                neighbours[2] = (z + x * terrainSize) - 1;
            }
            else if (x == terrainSize - 1 && z == 0)
            {
                neighbours = new int[3];
                neighbours[0] = (z + x * terrainSize) - terrainSize;
                neighbours[1] = (z + x * terrainSize) - terrainSize + 1;
                neighbours[2] = (z + x * terrainSize) + 1;
            }
            else if (x == 0)
            {
                neighbours = new int[5];
                neighbours[0] = (z + x * terrainSize) - 1;
                neighbours[1] = (z + x * terrainSize) + 1;
                neighbours[2] = (z + x * terrainSize) + terrainSize + 1;
                neighbours[3] = (z + x * terrainSize) + terrainSize;
                neighbours[4] = (z + x * terrainSize) + terrainSize - 1;
            }
            else if (z == 0)
            {
                neighbours = new int[5];
                neighbours[0] = (z + x * terrainSize) - terrainSize;
                neighbours[1] = (z + x * terrainSize) - terrainSize + 1;
                neighbours[2] = (z + x * terrainSize) + 1;
                neighbours[3] = (z + x * terrainSize) + terrainSize + 1;
                neighbours[4] = (z + x * terrainSize) + terrainSize;
            }
            else if (x == terrainSize - 1)
            {
                neighbours = new int[5];
                neighbours[0] = (z + x * terrainSize) - 1;
                neighbours[1] = (z + x * terrainSize) - terrainSize - 1;
                neighbours[2] = (z + x * terrainSize) - terrainSize;
                neighbours[3] = (z + x * terrainSize) - terrainSize + 1;
                neighbours[4] = (z + x * terrainSize) + 1;
            }
            else if (z == terrainSize - 1)
            {
                neighbours = new int[5];
                neighbours[0] = (z + x * terrainSize) + terrainSize;
                neighbours[1] = (z + x * terrainSize) + terrainSize - 1;
                neighbours[2] = (z + x * terrainSize) - 1;
                neighbours[3] = (z + x * terrainSize) - terrainSize - 1;
                neighbours[4] = (z + x * terrainSize) - terrainSize;
            }
            else
            {
                neighbours = new int[8];
                neighbours[0] = (z + x * terrainSize) - 1;
                neighbours[1] = (z + x * terrainSize) - terrainSize - 1;
                neighbours[2] = (z + x * terrainSize) - terrainSize;
                neighbours[3] = (z + x * terrainSize) - terrainSize + 1;
                neighbours[4] = (z + x * terrainSize) + 1;
                neighbours[5] = (z + x * terrainSize) + terrainSize + 1;
                neighbours[6] = (z + x * terrainSize) + terrainSize;
                neighbours[7] = (z + x * terrainSize) + terrainSize - 1;
            }

            return neighbours;
        }

        internal static int[] GetPositionAndNeighbours(int x, int z, int terrainSize)
        {
            int[] neighbours = GetNeighbours(x, z, terrainSize);
            int[] PositionAndNeighbours = new int[neighbours.Length + 1];
            PositionAndNeighbours[0] = (z + x * terrainSize);
            for (int i = 0; i < neighbours.Length; i++)
            {
                PositionAndNeighbours[i + 1] = neighbours[i];
            }
            return PositionAndNeighbours;
        }

        internal static float Apply(float oldValue, float applyValue, ApplicationMode CurrentApplicationMode)
        {
            float newValue = 0;
            if (CurrentApplicationMode == ApplicationMode.Multiply)
            {
                newValue = oldValue * applyValue;
            }
            else if (CurrentApplicationMode == ApplicationMode.Add)
            {
                newValue = oldValue + applyValue;
            }
            else if (CurrentApplicationMode == ApplicationMode.Subtract)
            {
                newValue = oldValue - applyValue;
            }
            else if (CurrentApplicationMode == ApplicationMode.Darken)
            {
                if (oldValue > applyValue)
                {
                    newValue = applyValue;
                } else
                {
                    newValue = oldValue;
                }
            }
            else if (CurrentApplicationMode == ApplicationMode.Lighten)
            {
                if(oldValue < applyValue)
                {
                    newValue = applyValue;
                } else
                {
                    newValue = oldValue;
                }
            }
            else
            {
                newValue = applyValue;
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

        internal static int GetReversedSizeCompensator(int TerrainSize)
        {
            int sizeCompensator = 0;
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
            return sizeCompensator;
        }

        internal static int GetSizeCompensator(int TerrainSize)
        {
            int sizeCompensator = 0;
            switch (TerrainSize)
            {
                case 16:
                    sizeCompensator = 1;
                    break;
                case 32:
                    sizeCompensator = 2;
                    break;
                case 64:
                    sizeCompensator = 4;
                    break;
                case 128:
                    sizeCompensator = 8;
                    break;

                case 512:
                    sizeCompensator = 32;
                    break;
                case 1024:
                    sizeCompensator = 64;
                    break;
                case 2048:
                    sizeCompensator = 128;
                    break;
                case 4096:
                    sizeCompensator = 256;
                    break;
            }
            return sizeCompensator;
        }
    }
}

