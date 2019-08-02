using System;
using System.Linq;
using System.Windows.Media;

namespace Topographer3D.Models
{
    class DetailColoringAlgorithm
    {
        private byte[] _bgr;
        private GradientStopCollection _gradientStopCollection;
        private float[] _heights;
        private float _offset;
        private int _terrainSize;
        private float _shift;
        private bool _invert;
        private float _min;
        private float _max;
        private float _range;
        private Random _random;

        public DetailColoringAlgorithm()
        {
            _bgr = new byte[3];
            _random = new Random();
        }

        public void UpdateValues(GradientStopCollection gradientStopCollection, float[] heights, int terrainSize, float contrast, bool invert)
        {
            _gradientStopCollection = gradientStopCollection;
            _heights = heights;
            _terrainSize = terrainSize;
            _shift = contrast;
            _invert = invert;

        }

        public byte[] ColorizeTerrain(int x, int z)
        {
            calculateOffset(x, z);
            calculateColor();
            return _bgr;
        }

        public byte[] ColorizeBorder(int x, int max)
        {
            x %= max;
            _offset = (float)x / (float)max;
            calculateColor();
            byte randomColorOffset = (byte)_random.Next(16);
            _bgr[0] += randomColorOffset;
            _bgr[1] += randomColorOffset;
            _bgr[2] += randomColorOffset;
            return _bgr;
        }

        public void calculateMinMax()
        {
            _min = -0.01f;
            _max = 0.01f;
            float currentPoint;
            float neighbour1;
            float neighbour2;
            float neighbour3;
            float neighbour4;
            float neighbour5;
            float neighbour6;
            float neighbour7;
            float neighbour8;
            float weightedNeighbour;
            float check;

            for (int x = 1; x < _terrainSize - 1; x++)
            {
                for (int z = 1; z < _terrainSize - 1; z++)
                {
                    currentPoint = _heights[(x + z * _terrainSize)];
                    neighbour1 = _heights[(x + z * _terrainSize) + 1];
                    neighbour2 = _heights[(x + z * _terrainSize) + _terrainSize];
                    neighbour3 = _heights[(x + z * _terrainSize) + _terrainSize + 1];
                    neighbour4 = _heights[(x + z * _terrainSize) + _terrainSize - 1];
                    neighbour5 = _heights[(x + z * _terrainSize) - 1];
                    neighbour6 = _heights[(x + z * _terrainSize) - _terrainSize];
                    neighbour7 = _heights[(x + z * _terrainSize) - _terrainSize + 1];
                    neighbour8 = _heights[(x + z * _terrainSize) - _terrainSize - 1];

                    weightedNeighbour = (neighbour1 + neighbour2 + neighbour3 + neighbour4 + neighbour5 + neighbour6 + neighbour7 + neighbour8) / 8.0f;

                    check = currentPoint - weightedNeighbour;

                    if (check < _min)
                    {
                        _min = check;
                        //Console.WriteLine("Min:" + _min);
                    }
                    else if (check > _max)
                    {
                        _max = check;
                        //Console.WriteLine("Max:" + _max);
                    }
                }
            }

            _range = -_min + _max;
        }

        public void calculateOffset(int x, int z)
        {
            float currentPoint = _heights[(x + z * _terrainSize)];
            _offset = 0;

            if (x > 0 && z > 0 && x < _terrainSize - 1 && z < _terrainSize - 1)
            {
                float neighbour1 = _heights[(x + z * _terrainSize) + 1];
                float neighbour2 = _heights[(x + z * _terrainSize) + _terrainSize];
                float neighbour3 = _heights[(x + z * _terrainSize) + _terrainSize + 1];
                float neighbour4 = _heights[(x + z * _terrainSize) + _terrainSize - 1];
                float neighbour5 = _heights[(x + z * _terrainSize) - 1];
                float neighbour6 = _heights[(x + z * _terrainSize) - _terrainSize];
                float neighbour7 = _heights[(x + z * _terrainSize) - _terrainSize + 1];
                float neighbour8 = _heights[(x + z * _terrainSize) - _terrainSize - 1];

                float weightedNeighbour = (neighbour1 + neighbour2 + neighbour3 + neighbour4 + neighbour5 + neighbour6 + neighbour7 + neighbour8) / 8.0f;
                if (_invert)
                {
                    _offset = (((weightedNeighbour - currentPoint) - _min) / _range) + _shift;
                }
                else
                {
                    _offset = (((currentPoint - weightedNeighbour) - _min) / _range) + _shift;
                }
            }


        }

        private void calculateColor()
        {
            GradientStop[] stops = _gradientStopCollection.OrderBy(_gradientStopCollection => _gradientStopCollection.Offset).ToArray();
            if (_offset <= 0)
            {
                _bgr[0] = stops[0].Color.B;
                _bgr[1] = stops[0].Color.G;
                _bgr[2] = stops[0].Color.R;
                return;
            }
            if (_offset >= 1)
            {
                _bgr[0] = stops[stops.Length - 1].Color.B;
                _bgr[1] = stops[stops.Length - 1].Color.G;
                _bgr[2] = stops[stops.Length - 1].Color.R;
                return;
            }
            GradientStop left = stops[0], right = null;
            foreach (GradientStop stop in stops)
            {
                if (stop.Offset >= _offset)
                {
                    right = stop;
                    break;
                }
                left = stop;
            }
            _offset = (float)Math.Round((_offset - left.Offset) / (right.Offset - left.Offset), 2);
            _bgr[0] = (byte)((right.Color.B - left.Color.B) * _offset + left.Color.B);
            _bgr[1] = (byte)((right.Color.G - left.Color.G) * _offset + left.Color.G);
            _bgr[2] = (byte)((right.Color.R - left.Color.R) * _offset + left.Color.R);
            return;
        }
    }


}
