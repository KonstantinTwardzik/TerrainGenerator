using System;
using System.Linq;
using System.Windows.Media;
using Topographer3D.Utilities;

namespace Topographer3D.Models
{
    class DetailColorizationAlgorithm
    {
        private byte[] bgr;
        private GradientStopCollection gradientStopCollection;
        private float[] heights;
        private float offset;
        private int terrainSize;
        private float shift;
        private bool invert;
        private float min;
        private float max;
        private float range;
        private float colorRange;
        private Random random;

        public DetailColorizationAlgorithm()
        {
            bgr = new byte[3];
            random = new Random();
        }

        public void UpdateValues(GradientStopCollection gradientStopCollection, float[] heights, int terrainSize, float contrast, bool invert, float colorRange)
        {
            this.gradientStopCollection = gradientStopCollection;
            this.heights = heights;
            this.terrainSize = terrainSize;
            this.colorRange = colorRange;
            shift = contrast;
            this.invert = invert;
        }

        public byte[] ColorizeTerrain(int x, int z)
        {
            calculateOffset(x, z);
            calculateColor();
            return bgr;
        }

        public byte[] ColorizeBorder(int x, int max)
        {
            x %= max;
            offset = (float)x / (float)max;
            calculateColor();
            byte randomColorOffset = (byte)random.Next(16);
            bgr[0] += randomColorOffset;
            bgr[1] += randomColorOffset;
            bgr[2] += randomColorOffset;
            return bgr;
        }

        public void calculateMinMax()
        {
            min = -0.01f;
            max = 0.01f;
            float currentPoint;
            float check;

            for (int x = 0; x < terrainSize; x++)
            {
                for (int z = 0; z < terrainSize; z++)
                {
                    currentPoint = heights[(z + x * terrainSize)];

                    int[] neighbours = TerrainUtilities.GetNeighbours(x, z, terrainSize);

                    float weightedNeighbours = 0.0f;
                    for (int i = 0; i < neighbours.Length; i++)
                    {
                        weightedNeighbours += heights[neighbours[i]];
                    }
                    weightedNeighbours /= (float)neighbours.Length;

                    check = currentPoint - weightedNeighbours;

                    if (check < min)
                    {
                        min = check;
                    }
                    else if (check > max)
                    {
                        max = check;
                    }
                }
            }

            range = -min + max * colorRange;
        }

        public void calculateOffset(int x, int z)
        {
            float currentPoint = heights[(z + x * terrainSize)];
            offset = 0;

            int[] neighbours = TerrainUtilities.GetNeighbours(x, z, terrainSize);

            float weightedNeighbours = 0.0f;
            for (int i = 0; i < neighbours.Length; i++)
            {
                weightedNeighbours += heights[neighbours[i]];
            }
            weightedNeighbours /= (float)neighbours.Length;

            if (invert)
            {
                offset = (((weightedNeighbours - currentPoint) - min) / range) + shift;
            }
            else
            {
                offset = (((currentPoint - weightedNeighbours) - min) / range) + shift;
            }

        }

        private void calculateColor()
        {
            GradientStop[] stops = gradientStopCollection.OrderBy(_gradientStopCollection => _gradientStopCollection.Offset).ToArray();
            if (offset <= 0)
            {
                bgr[0] = stops[0].Color.B;
                bgr[1] = stops[0].Color.G;
                bgr[2] = stops[0].Color.R;
                return;
            }
            if (offset >= 1)
            {
                bgr[0] = stops[stops.Length - 1].Color.B;
                bgr[1] = stops[stops.Length - 1].Color.G;
                bgr[2] = stops[stops.Length - 1].Color.R;
                return;
            }
            GradientStop left = stops[0], right = null;
            foreach (GradientStop stop in stops)
            {
                if (stop.Offset >= offset)
                {
                    right = stop;
                    break;
                }
                left = stop;
            }
            offset = (float)Math.Round((offset - left.Offset) / (right.Offset - left.Offset), 2);
            bgr[0] = (byte)((right.Color.B - left.Color.B) * offset + left.Color.B);
            bgr[1] = (byte)((right.Color.G - left.Color.G) * offset + left.Color.G);
            bgr[2] = (byte)((right.Color.R - left.Color.R) * offset + left.Color.R);
            return;
        }
    }


}
