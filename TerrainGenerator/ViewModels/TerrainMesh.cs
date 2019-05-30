using System;
using System.Windows.Media.Media3D;
using System.ComponentModel;
using TerrainGenerator.Models;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;

namespace TerrainGenerator.ViewModels
{
    internal class TerrainMesh : INotifyPropertyChanged
    {
        #region Attributes
        private MeshGeometry3D _meshGeometry3D;
        private TerrainSettings _heightLogic;
        private double _generalHeight;
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

        #region Initialization
        public TerrainMesh(TerrainSettings heightLogic)
        {
            _heightLogic = heightLogic;
            _meshGeometry3D = new MeshGeometry3D();
            _imageBrush = new ImageBrush();
            _generalHeight = 0.5;
            InitMesh();
        }

        public void InitMesh()
        {
            _meshGeometry3D.Positions.Clear();
            _meshGeometry3D.TriangleIndices.Clear();
            _meshGeometry3D.TextureCoordinates.Clear();
            GeneratePositions();
            GenerateTriangleIndices();
            GenerateUVCoordinates();
            GenerateDefaulTexture();
        }
        #endregion

        #region Generating 3D Model
        private void GeneratePositions()
        {
            Point3D point = new Point3D();
            // Terrain Points
            for (int x = 0; x < _heightLogic.TerrainSize; x++)
            {
                for (int z = 0; z < _heightLogic.TerrainSize; z++)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        if (i == 0)
                            point.X = ((double)x / ((double)_heightLogic.TerrainSize - 1) - 0.5) * 2;
                        if (i == 1)
                            point.Y = 0;
                        if (i == 2)
                            point.Z = ((double)z / ((double)_heightLogic.TerrainSize - 1) - 0.5) * 2;
                    }
                    MeshGeometry3DProperty.Positions.Add(point);
                }
            }

            // Border Points
            for (int z = 0; z < _heightLogic.TerrainSize; z++)
            {
                int x = 0;
                for (int i = 0; i < 3; i++)
                {
                    if (i == 0)
                        point.X = ((double)x / ((double)_heightLogic.TerrainSize - 1) - 0.5) * 2;
                    if (i == 1)
                        point.Y = 0;
                    if (i == 2)
                        point.Z = ((double)z / ((double)_heightLogic.TerrainSize - 1) - 0.5) * 2;
                }
                MeshGeometry3DProperty.Positions.Add(point);

            }

            for (int x = 0; x < _heightLogic.TerrainSize; x++)
            {
                int z = 0;
                for (int i = 0; i < 3; i++)
                {
                    if (i == 0)
                        point.X = ((double)x / ((double)_heightLogic.TerrainSize - 1) - 0.5) * 2;
                    if (i == 1)
                        point.Y = 0;
                    if (i == 2)
                        point.Z = ((double)z / ((double)_heightLogic.TerrainSize - 1) - 0.5) * 2;
                }
                MeshGeometry3DProperty.Positions.Add(point);
            }

            for (int x = 0; x < _heightLogic.TerrainSize; x++)
            {
                int z = _heightLogic.TerrainSize - 1;
                for (int i = 0; i < 3; i++)
                {
                    if (i == 0)
                        point.X = ((double)x / ((double)_heightLogic.TerrainSize - 1) - 0.5) * 2;
                    if (i == 1)
                        point.Y = 0;
                    if (i == 2)
                        point.Z = ((double)z / ((double)_heightLogic.TerrainSize - 1) - 0.5) * 2;
                }
                MeshGeometry3DProperty.Positions.Add(point);
            }

            for (int z = 0; z < _heightLogic.TerrainSize; z++)
            {
                int x = _heightLogic.TerrainSize - 1;
                for (int i = 0; i < 3; i++)
                {
                    if (i == 0)
                        point.X = ((double)x / ((double)_heightLogic.TerrainSize - 1) - 0.5) * 2;
                    if (i == 1)
                        point.Y = 0;
                    if (i == 2)
                        point.Z = ((double)z / ((double)_heightLogic.TerrainSize - 1) - 0.5) * 2;
                }
                MeshGeometry3DProperty.Positions.Add(point);
            }
        }

        private void GenerateTriangleIndices()
        {
            var value = 0;
            // Terrain Indices
            for (int i = 0; i < _heightLogic.TerrainSize * _heightLogic.TerrainSize - _heightLogic.TerrainSize; i++)
            {
                for (int trianglePoint = 0; trianglePoint < 3; trianglePoint++)
                {
                    if (i % _heightLogic.TerrainSize == 0)
                    {
                        break;
                    }
                    if (trianglePoint == 0)
                    {
                        value = i;
                    }
                    else if (trianglePoint == 1)
                    {
                        value = i + _heightLogic.TerrainSize;

                    }
                    else if (trianglePoint == 2)
                    {
                        value = i + _heightLogic.TerrainSize - 1;
                    }
                    MeshGeometry3DProperty.TriangleIndices.Add(value);
                }
                for (int trianglePoint = 0; trianglePoint < 3; trianglePoint++)
                {
                    if (i > 0 && ((i + 1) % _heightLogic.TerrainSize) == 0)
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
                        value = i + _heightLogic.TerrainSize;
                    }
                    MeshGeometry3DProperty.TriangleIndices.Add(value);
                }
            }



            // Border Incdices
            for (int z = 0; z < _heightLogic.TerrainSize - 1; z++)
            {
                for (int trianglePoint = 0; trianglePoint < 3; trianglePoint++)
                {
                    if (trianglePoint == 0)
                        value = z;
                    if (trianglePoint == 1)
                        value = z + (_heightLogic.TerrainSize * _heightLogic.TerrainSize);
                    if (trianglePoint == 2)
                        value = z + 1;
                    MeshGeometry3DProperty.TriangleIndices.Add(value);
                }

                for (int trianglePoint = 0; trianglePoint < 3; trianglePoint++)
                {
                    if (trianglePoint == 0)
                        value = z + 1;
                    if (trianglePoint == 1)
                        value = z + (_heightLogic.TerrainSize * _heightLogic.TerrainSize);
                    if (trianglePoint == 2)
                        value = z + 1 + (_heightLogic.TerrainSize * _heightLogic.TerrainSize);
                    MeshGeometry3DProperty.TriangleIndices.Add(value);
                }

                for (int trianglePoint = 0; trianglePoint < 3; trianglePoint++)
                {
                    if (trianglePoint == 0)
                        value = z + (_heightLogic.TerrainSize * _heightLogic.TerrainSize) - _heightLogic.TerrainSize;
                    if (trianglePoint == 1)
                        value = z + 1 + (_heightLogic.TerrainSize * _heightLogic.TerrainSize) - _heightLogic.TerrainSize;
                    if (trianglePoint == 2)
                        value = z + (_heightLogic.TerrainSize * _heightLogic.TerrainSize) + (3 * _heightLogic.TerrainSize);
                    MeshGeometry3DProperty.TriangleIndices.Add(value);
                }

                for (int trianglePoint = 0; trianglePoint < 3; trianglePoint++)
                {
                    if (trianglePoint == 0)
                        value = z + 1 + (_heightLogic.TerrainSize * _heightLogic.TerrainSize) - _heightLogic.TerrainSize;
                    if (trianglePoint == 1)
                        value = z + 1 + (_heightLogic.TerrainSize * _heightLogic.TerrainSize) + (3 * _heightLogic.TerrainSize);
                    if (trianglePoint == 2)
                        value = z + (_heightLogic.TerrainSize * _heightLogic.TerrainSize) + (3 * _heightLogic.TerrainSize);
                    MeshGeometry3DProperty.TriangleIndices.Add(value);
                }
            }

            for (int x = 0; x < _heightLogic.TerrainSize - 1; x++)
            {
                for (int trianglePoint = 0; trianglePoint < 3; trianglePoint++)
                {
                    if (trianglePoint == 0)
                        value = x * _heightLogic.TerrainSize;
                    if (trianglePoint == 1)
                        value = (x + 1) * _heightLogic.TerrainSize;
                    if (trianglePoint == 2)
                        value = (_heightLogic.TerrainSize * _heightLogic.TerrainSize) + _heightLogic.TerrainSize + x;
                    MeshGeometry3DProperty.TriangleIndices.Add(value);
                }

                for (int trianglePoint = 0; trianglePoint < 3; trianglePoint++)
                {
                    if (trianglePoint == 0)
                        value = (x + 1) * _heightLogic.TerrainSize;
                    if (trianglePoint == 1)
                        value = (_heightLogic.TerrainSize * _heightLogic.TerrainSize) + _heightLogic.TerrainSize + x + 1;
                    if (trianglePoint == 2)
                        value = (_heightLogic.TerrainSize * _heightLogic.TerrainSize) + _heightLogic.TerrainSize + x;
                    MeshGeometry3DProperty.TriangleIndices.Add(value);
                }

                for (int trianglePoint = 0; trianglePoint < 3; trianglePoint++)
                {
                    if (trianglePoint == 0)
                        value = x * _heightLogic.TerrainSize + (_heightLogic.TerrainSize - 1);
                    if (trianglePoint == 1)
                        value = (_heightLogic.TerrainSize * _heightLogic.TerrainSize) + 2 * _heightLogic.TerrainSize + x;
                    if (trianglePoint == 2)
                        value = (x + 1) * _heightLogic.TerrainSize + (_heightLogic.TerrainSize - 1);
                    MeshGeometry3DProperty.TriangleIndices.Add(value);
                }

                for (int trianglePoint = 0; trianglePoint < 3; trianglePoint++)
                {
                    if (trianglePoint == 0)
                        value = (x + 1) * _heightLogic.TerrainSize + (_heightLogic.TerrainSize - 1);
                    if (trianglePoint == 1)
                        value = (_heightLogic.TerrainSize * _heightLogic.TerrainSize) + 2 * _heightLogic.TerrainSize + x;
                    if (trianglePoint == 2)
                        value = (_heightLogic.TerrainSize * _heightLogic.TerrainSize) + 2 * _heightLogic.TerrainSize + x + 1;
                    MeshGeometry3DProperty.TriangleIndices.Add(value);
                }
            }
        }

        private void GenerateUVCoordinates()
        {
            Point point = new Point();
            for (int x = 0; x < _heightLogic.TerrainSize; x++)
            {
                for (int z = 0; z < _heightLogic.TerrainSize; z++)
                {
                    point.X = (double)x / (double)_heightLogic.TerrainSize;
                    point.Y = (double)z / (double)_heightLogic.TerrainSize;
                    MeshGeometry3DProperty.TextureCoordinates.Add(point);
                }
            }
        }

        private void GenerateDefaulTexture()
        {
            
            PixelFormat pixelFormat = PixelFormats.Bgr24;
            int rawStride = (pixelFormat.BitsPerPixel + 7) / 8;
            byte[] rawImage = new byte[rawStride];

            for (int i = 0; i < rawStride; i++)
            {
                rawImage[i] = 255;
            }

            BitmapSource bitmap = BitmapSource.Create(1, 1, 96, 96, pixelFormat, null, rawImage, rawStride);

            PngBitmapEncoder encoder = new PngBitmapEncoder();
            MemoryStream memoryStream = new MemoryStream();
            BitmapImage whiteTexture = new BitmapImage();

            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            encoder.Save(memoryStream);

            memoryStream.Position = 0;
            whiteTexture.BeginInit();
            whiteTexture.StreamSource = new MemoryStream(memoryStream.ToArray());
            whiteTexture.EndInit();
            whiteTexture.Freeze();

            _imageBrush.ImageSource = whiteTexture;
        }
        #endregion

        #region Updating 3D Model
        public void UpdateMesh()
        {
            Point3D point = new Point3D();
            for (int x = 0; x < _heightLogic.TerrainSize; x++)
            {
                for (int z = 0; z < _heightLogic.TerrainSize; z++)
                {
                    point = MeshGeometry3DProperty.Positions[x + z * _heightLogic.TerrainSize];
                    point.Y = _heightLogic.TerrainPoints[x + z * _heightLogic.TerrainSize] * GeneralHeight;
                    MeshGeometry3DProperty.Positions[x + z * _heightLogic.TerrainSize] = point;
                }
            }

            GenerateDefaulTexture();
        }

        public void UpdateTexture()
        {
            _imageBrush.ImageSource = _heightLogic.ColorMapImage;
        }
        #endregion
        
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
