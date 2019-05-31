using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Topographer.Models
{
    class ColoringAlgorithm
    {
        private byte[] _bgr;
        private GradientStopCollection _gradientStopCollection;
        private double[] _heights;
        private double _offset;
        private int _terrainSize;
        private double _shift;
        private bool _invert;
        private double _min;
        private double _max;
        private double _range;

        public ColoringAlgorithm()
        {
            _bgr = new byte[3];
        }

        public void UpdateValues(GradientStopCollection gradientStopCollection, double[] heights, int terrainSize, double contrast, bool invert)
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
            _offset = (double)x / (double)max;
            calculateColor();
            return _bgr;
        }

        public void calculateMinMax()
        {
            _min = -0.01;
            _max = 0.01;
            double currentPoint;
            double neighbour1;
            double neighbour2;
            double neighbour3;
            double neighbour4;
            double neighbour5;
            double neighbour6;
            double neighbour7;
            double neighbour8;
            double weightedNeighbour;
            double check;

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

                    weightedNeighbour = (neighbour1 + neighbour2 + neighbour3 + neighbour4 + neighbour5 + neighbour6 + neighbour7 + neighbour8) / 8.0;

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
            double currentPoint = _heights[(x + z * _terrainSize)];
            _offset = 0;

            if (x > 0 && z > 0 && x < _terrainSize - 1 && z < _terrainSize - 1)
            {
                double neighbour1 = _heights[(x + z * _terrainSize) + 1];
                double neighbour2 = _heights[(x + z * _terrainSize) + _terrainSize];
                double neighbour3 = _heights[(x + z * _terrainSize) + _terrainSize + 1];
                double neighbour4 = _heights[(x + z * _terrainSize) + _terrainSize - 1];
                double neighbour5 = _heights[(x + z * _terrainSize) - 1];
                double neighbour6 = _heights[(x + z * _terrainSize) - _terrainSize];
                double neighbour7 = _heights[(x + z * _terrainSize) - _terrainSize + 1];
                double neighbour8 = _heights[(x + z * _terrainSize) - _terrainSize - 1];

                double weightedNeighbour = (neighbour1 + neighbour2 + neighbour3 + neighbour4 + neighbour5 + neighbour6 + neighbour7 + neighbour8) / 8.0;
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
            _offset = Math.Round((_offset - left.Offset) / (right.Offset - left.Offset), 2);
            _bgr[0] = (byte)((right.Color.B - left.Color.B) * _offset + left.Color.B);
            _bgr[1] = (byte)((right.Color.G - left.Color.G) * _offset + left.Color.G);
            _bgr[2] = (byte)((right.Color.R - left.Color.R) * _offset + left.Color.R);
            return;
        }
    }


}
