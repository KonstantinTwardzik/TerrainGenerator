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
                neighbours[1] = (z + x * terrainSize) + terrainSize;
                neighbours[2] = (z + x * terrainSize) + terrainSize - 1;
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
                neighbours[3] = (z + x * terrainSize) - terrainSize + 1;
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

        internal static float Apply(float oldValue, float applyValue, ApplicationMode CurrentApplicationMode)
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
    }
}

