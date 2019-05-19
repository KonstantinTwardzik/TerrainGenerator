using System;
using TerrainGenerator.Models;
using TerrainGenerator.Utilities;

namespace TerrainGenerator.ViewModels
{
    public class HeightLogic
    {
        #region Attributes
        private int _terrainSize;
        private TerrainPoint[] _terrainPoints;
        private OpenSimplexNoise _openSimplexNoise;
        private HydraulicErosion _hydraulicErosion;
        public bool isNoised;
        public bool isEroded;
        public bool isColored;
        #endregion

        #region Properties
        public TerrainPoint[] TerrainPoints
        {
            get
            {
                return _terrainPoints;
            }
            set
            {
                _terrainPoints = value;
            }
        }

        public int TerrainSize
        {
            get
            {
                return _terrainSize;
            }
            set
            {
                _terrainSize = value;
            }
        }
        #endregion

        public HeightLogic(int terrainSize)
        {
            InitHeights(terrainSize);
            _openSimplexNoise = new OpenSimplexNoise();
            _hydraulicErosion = new HydraulicErosion();
            isNoised = false;
            isEroded = false;
            isColored = false;
        }

        public void InitHeights(int terrainSize)
        {
            _terrainSize = terrainSize;
            GenerateTerrainPoints();
        }

        private void GenerateTerrainPoints()
        {
            _terrainPoints = new TerrainPoint[_terrainSize * _terrainSize];
            for (int x = 0; x < _terrainSize; x++)
            {
                for (int z = 0; z < _terrainSize; z++)
                {
                    _terrainPoints[x + z * _terrainSize] = new TerrainPoint(x, z, 0);
                }
            }
        }

        public void ResetHeights()
        {
            for (int x = 0; x < _terrainSize; x++)
            {

                for (int z = 0; z < _terrainSize; z++)
                {
                    _terrainPoints[x + z * _terrainSize].Height = 0;
                }
            }

            isNoised = false;
        }

        public void OpenSimplexNoise(double perlinScale, int perlinOctaves, double perlinOctaveWeight, double perlinScaleX, double perlinScaleZ, int perlinSeed)
        {
            double weight = 1;
            double octaveMultiplier = 1;
            double sizeCompensator = 1;

            switch (_terrainSize)
            {
                case 256:
                    sizeCompensator = 8;
                    break;
                case 512:
                    sizeCompensator = 4;
                    break;
                case 1024:
                    sizeCompensator = 2;
                    break;
                case 2048:
                    sizeCompensator = 1;
                    break;

            }
            ResetHeights();

            for (int o = 0; o < perlinOctaves; o++)
            {
                for (int x = 0; x < _terrainSize; x++)
                {
                    for (int z = 0; z < _terrainSize; z++)
                    {
                        double value = 0;
                        double xValue = ((((0.0005f / perlinScale) / perlinScaleX) * (x * sizeCompensator) + perlinSeed) * octaveMultiplier);
                        double zValue = ((((0.0005f / perlinScale) / perlinScaleZ) * (z * sizeCompensator) + perlinSeed) * octaveMultiplier);
                        if (o == 0)
                        {
                            value = (((_openSimplexNoise.Evaluate(xValue, zValue) * weight) + 1) / 2);
                        }
                        else
                        {
                            value = ((_openSimplexNoise.Evaluate(xValue, zValue) * weight) / 2);
                        }

                        _terrainPoints[x + z * _terrainSize].Height += value;
                    }
                }
                weight /= 2 - (perlinOctaveWeight - 0.5);
                octaveMultiplier = o * 2;
            }
            isNoised = true;
        }

        public void Erode(int heIterations)
        {
            double[] map = new double[_terrainPoints.Length];
            for (int x = 0; x < _terrainSize; x++)
            {
                for (int z = 0; z < _terrainSize; z++)
                {
                    map[x + z * _terrainSize] = _terrainPoints[x + z * _terrainSize].Height;
                }
            }


            double[] values = _hydraulicErosion.Erode(map, _terrainSize, heIterations, false);

            for (int x = 0; x < _terrainSize; x++)
            {
                for (int z = 0; z < _terrainSize; z++)
                {
                    _terrainPoints[x + z * _terrainSize].Height = values[x + z * _terrainSize];
                }
            }

            isEroded = true;
        }
    }

}
