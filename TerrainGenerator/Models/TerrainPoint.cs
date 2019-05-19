using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace TerrainGenerator.Models
{
    public class TerrainPoint
    {
        #region Attributes

        private int _xPosition;
        private int _zPosition;
        private double _height;
        private int[] _neighbours;
        #endregion

        #region Properties
        public int XPosition
        {
            get
            {
                return _xPosition;
            }
            set
            {
                _xPosition = value;
            }
        }

        public int ZPosition
        {
            get
            {
                return _zPosition;
            }
            set
            {
                _zPosition = value;
            }
        }

        public double Height
        {
            get
            {
                return _height;
            }
            set
            {
                _height = value;
            }
        }
        public int[] Neighbours
        {
            get
            {
                return _neighbours;
            }
            set
            {
                _neighbours = value;
            }
        }
        #endregion

        public TerrainPoint(int xPos, int zPos, double height, int[] neighbours)
        {
            _xPosition = xPos;
            _zPosition = zPos;
            _height = height;
            _neighbours = neighbours;
        }
    }
}
