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
        private OpenSimplexNoise osn;
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
            osn = new OpenSimplexNoise();
            InitHeights(terrainSize);
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
                    _terrainPoints[x + z * _terrainSize] = new TerrainPoint(x, z, 0, FindNeighbours());
                }
            }
        }

        private int[] FindNeighbours()
        {

            return new int[3];
        }

        public void OpenSimplexNoise(float perlinScale, int perlinOctaves, float perlinScaleX, float perlinScaleZ, int perlinSeed)
        {
            for (int x = 0; x < _terrainSize; x++)
            {
                for (int z = 0; z < _terrainSize; z++)
                {
                    double xValue = ((0.0025f / perlinScale) / perlinScaleX) * x + perlinSeed;
                    double zValue = ((0.0025f / perlinScale) / perlinScaleZ) * z + perlinSeed;
                    double value = osn.Evaluate(xValue, zValue);
                    if (value < 0)
                        value = 0;
                    _terrainPoints[x + z * _terrainSize].Height = value;
                }
            }
        }
    }

}
