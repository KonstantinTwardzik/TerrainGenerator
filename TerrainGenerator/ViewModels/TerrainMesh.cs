using System;
using System.Windows.Media.Media3D;
using System.ComponentModel;
using TerrainGenerator.Models;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TerrainGenerator.ViewModels
{
    internal class TerrainMesh : INotifyPropertyChanged
    {
        #region Attributes
        private MeshGeometry3D _meshGeometry3D;
        private HeightLogic _heightLogic;
        private int _terrainSize;
        private double _generalHeight = 0.5;
        private ImageBrush _imageBrush;
        #endregion

        #region Properties
        public double GeneralHeight
        {
            get
            {
                return _generalHeight;
            }
            set
            {
                _generalHeight = value;
            }
        }

        public MeshGeometry3D MeshGeometry3DProperty
        {
            get
            {
                return _meshGeometry3D;
            }
            set
            {
                _meshGeometry3D = value;
            }
        }

        public ImageBrush ImageBrush
        {
            get
            {
                return _imageBrush;
            }
            set
            {
                _imageBrush = value;
            }
        }
        #endregion

        public TerrainMesh(HeightLogic heightLogic)
        {
            _heightLogic = heightLogic;
            _meshGeometry3D = new MeshGeometry3D();
            InitMesh();
        }

        public void InitMesh()
        {
            _terrainSize = _heightLogic.TerrainSize;
            _meshGeometry3D.Positions.Clear();
            _meshGeometry3D.TriangleIndices.Clear();
            GeneratePositions();
            GenerateTriangleIndices();
            GenerateUVCoordinates();

            _imageBrush = new ImageBrush();
            _imageBrush.ImageSource = new BitmapImage(new Uri("../../Assets/Gradient.jpg", UriKind.Relative));
        }

        #region Generating 3D Mesh
        private void GeneratePositions()
        {
            Point3D point = new Point3D();
            // Terrain Points
            for (int x = 0; x < _terrainSize; x++)
            {
                for (int z = 0; z < _terrainSize; z++)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        if (i == 0)
                            point.X = ((double)x / ((double)_terrainSize - 1) - 0.5) * 2;
                        if (i == 1)
                            point.Y = 0;
                        if (i == 2)
                            point.Z = ((double)z / ((double)_terrainSize - 1) - 0.5) * 2;
                    }
                    MeshGeometry3DProperty.Positions.Add(point);
                }
            }

            // Border Points
            for (int z = 0; z < _terrainSize; z++)
            {
                int x = 0;
                for (int i = 0; i < 3; i++)
                {
                    if (i == 0)
                        point.X = ((double)x / ((double)_terrainSize - 1) - 0.5) * 2;
                    if (i == 1)
                        point.Y = 0;
                    if (i == 2)
                        point.Z = ((double)z / ((double)_terrainSize - 1) - 0.5) * 2;
                }
                MeshGeometry3DProperty.Positions.Add(point);

            }

            for (int x = 0; x < _terrainSize; x++)
            {
                int z = 0;
                for (int i = 0; i < 3; i++)
                {
                    if (i == 0)
                        point.X = ((double)x / ((double)_terrainSize - 1) - 0.5) * 2;
                    if (i == 1)
                        point.Y = 0;
                    if (i == 2)
                        point.Z = ((double)z / ((double)_terrainSize - 1) - 0.5) * 2;
                }
                MeshGeometry3DProperty.Positions.Add(point);
            }

            for (int x = 0; x < _terrainSize; x++)
            {
                int z = _terrainSize - 1;
                for (int i = 0; i < 3; i++)
                {
                    if (i == 0)
                        point.X = ((double)x / ((double)_terrainSize - 1) - 0.5) * 2;
                    if (i == 1)
                        point.Y = 0;
                    if (i == 2)
                        point.Z = ((double)z / ((double)_terrainSize - 1) - 0.5) * 2;
                }
                MeshGeometry3DProperty.Positions.Add(point);
            }

            for (int z = 0; z < _terrainSize; z++)
            {
                int x = _terrainSize - 1;
                for (int i = 0; i < 3; i++)
                {
                    if (i == 0)
                        point.X = ((double)x / ((double)_terrainSize - 1) - 0.5) * 2;
                    if (i == 1)
                        point.Y = 0;
                    if (i == 2)
                        point.Z = ((double)z / ((double)_terrainSize - 1) - 0.5) * 2;
                }
                MeshGeometry3DProperty.Positions.Add(point);
            }
        }

        private void GenerateTriangleIndices()
        {
            var value = 0;
            // Terrain Indices
            for (int i = 0; i < _terrainSize * _terrainSize - _terrainSize; i++)
            {
                for (int trianglePoint = 0; trianglePoint < 3; trianglePoint++)
                {
                    if (i % _terrainSize == 0)
                    {
                        break;
                    }
                    if (trianglePoint == 0)
                    {
                        value = i;
                    }
                    else if (trianglePoint == 1)
                    {
                        value = i + _terrainSize;

                    }
                    else if (trianglePoint == 2)
                    {
                        value = i + _terrainSize - 1;
                    }
                    MeshGeometry3DProperty.TriangleIndices.Add(value);
                }
                for (int trianglePoint = 0; trianglePoint < 3; trianglePoint++)
                {
                    if (i > 0 && ((i + 1) % _terrainSize) == 0)
                    {
                        break;
                    }
                    if (trianglePoint == 0)
                    {
                        value = i;
                    }
                    else if (trianglePoint == 1)
                    {
                        value = i + 1;

                    }
                    else if (trianglePoint == 2)
                    {
                        value = i + _terrainSize;
                    }
                    MeshGeometry3DProperty.TriangleIndices.Add(value);
                }
            }



            // Border Incdices
            for (int z = 0; z < _terrainSize - 1; z++)
            {
                for (int trianglePoint = 0; trianglePoint < 3; trianglePoint++)
                {
                    if (trianglePoint == 0)
                        value = z;
                    if (trianglePoint == 1)
                        value = z + (_terrainSize * _terrainSize);
                    if (trianglePoint == 2)
                        value = z + 1;
                    MeshGeometry3DProperty.TriangleIndices.Add(value);
                }

                for (int trianglePoint = 0; trianglePoint < 3; trianglePoint++)
                {
                    if (trianglePoint == 0)
                        value = z + 1;
                    if (trianglePoint == 1)
                        value = z + (_terrainSize * _terrainSize);
                    if (trianglePoint == 2)
                        value = z + 1 + (_terrainSize * _terrainSize);
                    MeshGeometry3DProperty.TriangleIndices.Add(value);
                }

                for (int trianglePoint = 0; trianglePoint < 3; trianglePoint++)
                {
                    if (trianglePoint == 0)
                        value = z + (_terrainSize * _terrainSize) - _terrainSize;
                    if (trianglePoint == 1)
                        value = z + 1 + (_terrainSize * _terrainSize) - _terrainSize;
                    if (trianglePoint == 2)
                        value = z + (_terrainSize * _terrainSize) + (3 * _terrainSize);
                    MeshGeometry3DProperty.TriangleIndices.Add(value);
                }

                for (int trianglePoint = 0; trianglePoint < 3; trianglePoint++)
                {
                    if (trianglePoint == 0)
                        value = z + 1 + (_terrainSize * _terrainSize) - _terrainSize;
                    if (trianglePoint == 1)
                        value = z + 1 + (_terrainSize * _terrainSize) + (3 * _terrainSize);
                    if (trianglePoint == 2)
                        value = z + (_terrainSize * _terrainSize) + (3 * _terrainSize);
                    MeshGeometry3DProperty.TriangleIndices.Add(value);
                }
            }

            for (int x = 0; x < _terrainSize - 1; x++)
            {
                for (int trianglePoint = 0; trianglePoint < 3; trianglePoint++)
                {
                    if (trianglePoint == 0)
                        value = x * _terrainSize;
                    if (trianglePoint == 1)
                        value = (x + 1) * _terrainSize;
                    if (trianglePoint == 2)
                        value = (_terrainSize * _terrainSize) + _terrainSize + x;
                    MeshGeometry3DProperty.TriangleIndices.Add(value);
                }

                for (int trianglePoint = 0; trianglePoint < 3; trianglePoint++)
                {
                    if (trianglePoint == 0)
                        value = (x + 1) * _terrainSize;
                    if (trianglePoint == 1)
                        value = (_terrainSize * _terrainSize) + _terrainSize + x + 1;
                    if (trianglePoint == 2)
                        value = (_terrainSize * _terrainSize) + _terrainSize + x;
                    MeshGeometry3DProperty.TriangleIndices.Add(value);
                }

                for (int trianglePoint = 0; trianglePoint < 3; trianglePoint++)
                {
                    if (trianglePoint == 0)
                        value = x * _terrainSize + (_terrainSize - 1);
                    if (trianglePoint == 1)
                        value = (_terrainSize * _terrainSize) + 2 * _terrainSize + x;
                    if (trianglePoint == 2)
                        value = (x + 1) * _terrainSize + (_terrainSize - 1);
                    MeshGeometry3DProperty.TriangleIndices.Add(value);
                }

                for (int trianglePoint = 0; trianglePoint < 3; trianglePoint++)
                {
                    if (trianglePoint == 0)
                        value = (x + 1) * _terrainSize + (_terrainSize - 1);
                    if (trianglePoint == 1)
                        value = (_terrainSize * _terrainSize) + 2 * _terrainSize + x;
                    if (trianglePoint == 2)
                        value = (_terrainSize * _terrainSize) + 2 * _terrainSize + x + 1;
                    MeshGeometry3DProperty.TriangleIndices.Add(value);
                }
            }
        }

        private void GenerateUVCoordinates()
        {
            Point point = new Point();

            for (int x = 0; x < _terrainSize; x++)
            {
                for (int z = 0; z < _terrainSize; z++)
                {
                    point.X = (double)x / (double)_terrainSize;
                    point.Y = (double)z / (double)_terrainSize;
                    MeshGeometry3DProperty.TextureCoordinates.Add(point);
                }
            }
        }
        #endregion



        public void UpdateMesh()
        {
            for (int i = 0; i < _terrainSize * _terrainSize; i++)
            {
                Point3D point = new Point3D();
                point = MeshGeometry3DProperty.Positions[i];
                point.Y = _heightLogic.TerrainPoints.YPositions[i] * GeneralHeight;
                MeshGeometry3DProperty.Positions[i] = point;
            }
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

            }
        }
        #endregion

    }
}
