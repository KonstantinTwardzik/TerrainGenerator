using System;
using System.Windows.Media.Media3D;
using System.ComponentModel;
using Topographer.Models;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;

namespace Topographer.ViewModels
{
    internal class TerrainMesh
    {
        #region Attributes
        private TerrainSettings _terrainSettings;
        private double _generalHeight;
        private MeshGeometry3D _terrainMeshGeometry3D;
        private ImageBrush _terrainImageBrush;
        private MeshGeometry3D _borderMeshGeometry3D;
        private ImageBrush _borderImageBrush;
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

        public MeshGeometry3D TerrainMeshGeometry3DProperty
        {
            get
            {
                return _terrainMeshGeometry3D;
            }
            set
            {
                _terrainMeshGeometry3D = value;
            }
        }

        public ImageBrush TerrainImageBrush
        {
            get
            {
                return _terrainImageBrush;
            }
            set
            {
                _terrainImageBrush = value;
            }
        }

        public MeshGeometry3D BorderMeshGeometry3DProperty
        {
            get
            {
                return _borderMeshGeometry3D;
            }
            set
            {
                _borderMeshGeometry3D = value;
            }
        }

        public ImageBrush BorderImageBrush
        {
            get
            {
                return _borderImageBrush;
            }
            set
            {
                _borderImageBrush = value;
            }
        }
        #endregion

        #region Initialization
        public TerrainMesh(TerrainSettings heightLogic)
        {
            _terrainSettings = heightLogic;
            _terrainMeshGeometry3D = new MeshGeometry3D();
            _terrainImageBrush = new ImageBrush();
            _borderMeshGeometry3D = new MeshGeometry3D();
            _borderImageBrush = new ImageBrush();
            _generalHeight = 0.75;
            InitMesh();
        }

        public void InitMesh()
        {
            _terrainMeshGeometry3D.Positions.Clear();
            _terrainMeshGeometry3D.TriangleIndices.Clear();
            _terrainMeshGeometry3D.TextureCoordinates.Clear();
            _borderMeshGeometry3D.Positions.Clear();
            _borderMeshGeometry3D.TriangleIndices.Clear();
            _borderMeshGeometry3D.TextureCoordinates.Clear();

            GenerateTerrainPositions();
            GenerateTerrainTriangleIndices();
            GenerateTerrainUVCoordinates();
            GenerateDefaultTexture();
            GenerateBorderPositions();
            GenerateBorderTriangleIndices();
            GenerateBorderUVCoordinates();
        }
        #endregion

        #region Generating 3D Model
        private void GenerateTerrainPositions()
        {
            Point3D point = new Point3D();
            // Terrain Points
            for (int x = 0; x < _terrainSettings.TerrainSize; x++)
            {
                for (int z = 0; z < _terrainSettings.TerrainSize; z++)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        if (i == 0)
                            point.X = ((double)x / ((double)_terrainSettings.TerrainSize - 1) - 0.5) * 2;
                        if (i == 1)
                            point.Y = 0;
                        if (i == 2)
                            point.Z = ((double)z / ((double)_terrainSettings.TerrainSize - 1) - 0.5) * 2;
                    }
                    TerrainMeshGeometry3DProperty.Positions.Add(point);
                }
            }
        }

        private void GenerateTerrainTriangleIndices()
        {
            var value = 0;
            // Terrain Indices
            for (int i = 0; i < _terrainSettings.TerrainSize * _terrainSettings.TerrainSize - _terrainSettings.TerrainSize; i++)
            {
                for (int trianglePoint = 0; trianglePoint < 3; trianglePoint++)
                {
                    if (i % _terrainSettings.TerrainSize == 0)
                    {
                        break;
                    }
                    if (trianglePoint == 0)
                    {
                        value = i;
                    }
                    else if (trianglePoint == 1)
                    {
                        value = i + _terrainSettings.TerrainSize;

                    }
                    else if (trianglePoint == 2)
                    {
                        value = i + _terrainSettings.TerrainSize - 1;
                    }
                    TerrainMeshGeometry3DProperty.TriangleIndices.Add(value);
                }
                for (int trianglePoint = 0; trianglePoint < 3; trianglePoint++)
                {
                    if (i > 0 && ((i + 1) % _terrainSettings.TerrainSize) == 0)
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
                        value = i + _terrainSettings.TerrainSize;
                    }
                    TerrainMeshGeometry3DProperty.TriangleIndices.Add(value);
                }
            }
        }

        private void GenerateTerrainUVCoordinates()
        {
            Point point = new Point();
            for (int x = 0; x < _terrainSettings.TerrainSize; x++)
            {
                for (int z = 0; z < _terrainSettings.TerrainSize; z++)
                {
                    point.X = (double)x / (double)_terrainSettings.TerrainSize;
                    point.Y = (double)z / (double)_terrainSettings.TerrainSize;
                    TerrainMeshGeometry3DProperty.TextureCoordinates.Add(point);
                }
            }
        }

        private void GenerateBorderPositions()
        {
            Point3D point = new Point3D(-1, 0, 0);

            // Border Points
            for (int z = 0; z < _terrainSettings.TerrainSize; z++)
            {
                point.Z = ((double)z / ((double)_terrainSettings.TerrainSize - 1) - 0.5) * 2;
                BorderMeshGeometry3DProperty.Positions.Add(point);
                BorderMeshGeometry3DProperty.Positions.Add(point);
            }

            point.Z = 1;
            for (int x = 0; x < _terrainSettings.TerrainSize; x++)
            {
                point.X = ((double)x / ((double)_terrainSettings.TerrainSize - 1) - 0.5) * 2;
                BorderMeshGeometry3DProperty.Positions.Add(point);
                BorderMeshGeometry3DProperty.Positions.Add(point);
            }

            point.X = 1;
            for (int z = _terrainSettings.TerrainSize - 1; z >= 0; z--)
            {
                point.Z = ((double)z / ((double)_terrainSettings.TerrainSize - 1) - 0.5) * 2;
                BorderMeshGeometry3DProperty.Positions.Add(point);
                BorderMeshGeometry3DProperty.Positions.Add(point);
            }

            point.Z = -1;
            for (int x = _terrainSettings.TerrainSize - 1; x >= 0; x--)
            {
                point.X = ((double)x / ((double)_terrainSettings.TerrainSize - 1) - 0.5) * 2;
                BorderMeshGeometry3DProperty.Positions.Add(point);
                BorderMeshGeometry3DProperty.Positions.Add(point);
            }
        }

        private void GenerateBorderTriangleIndices()
        {
            int value = 0;
            // Border Indices
            for (int i = 0; i < _borderMeshGeometry3D.Positions.Count; i++)
            {

                if (i % 2 == 0)
                {
                    for (int trianglePoint = 0; trianglePoint < 3; trianglePoint++)
                    {
                        if (trianglePoint == 0)
                            value = i + 2;
                        if (trianglePoint == 1)
                            value = i + 1;
                        if (trianglePoint == 2)
                            value = i;
                        BorderMeshGeometry3DProperty.TriangleIndices.Add(value);
                    }
                }
                else
                {
                    for (int trianglePoint = 0; trianglePoint < 3; trianglePoint++)
                    {
                        if (trianglePoint == 0)
                            value = i + 1;
                        if (trianglePoint == 1)
                            value = i + 2;
                        if (trianglePoint == 2)
                            value = i;
                        BorderMeshGeometry3DProperty.TriangleIndices.Add(value);
                    }
                }

            }
        }

        private void GenerateBorderUVCoordinates()
        {
            Point point = new Point();
            //Border
            for (int x = 0; x < BorderMeshGeometry3DProperty.Positions.Count; x++)
            {
                if (x % 2 == 0)
                {
                    point.X = 0.0;
                    point.Y = 0.0;
                }
                else
                {
                    point.Y = 1.0;
                    point.X = 1.0;
                }
                BorderMeshGeometry3DProperty.TextureCoordinates.Add(point);
            }
        }

        public void GenerateDefaultTexture()
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
            BitmapImage defaultTexture = new BitmapImage();

            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            encoder.Save(memoryStream);

            memoryStream.Position = 0;
            defaultTexture.BeginInit();
            defaultTexture.StreamSource = new MemoryStream(memoryStream.ToArray());
            defaultTexture.EndInit();
            defaultTexture.Freeze();

            _terrainImageBrush.ImageSource = defaultTexture;
            _borderImageBrush.ImageSource = defaultTexture;
        }
        #endregion

        #region Updating 3D Model
        public void UpdateMesh()
        {
            Point3D point = new Point3D();

            //Terrain
            for (int x = 0; x < _terrainSettings.TerrainSize; x++)
            {
                for (int z = 0; z < _terrainSettings.TerrainSize; z++)
                {
                    point = TerrainMeshGeometry3DProperty.Positions[x + z * _terrainSettings.TerrainSize];
                    point.Y = _terrainSettings.TerrainPoints[x + z * _terrainSettings.TerrainSize] * GeneralHeight;
                    TerrainMeshGeometry3DProperty.Positions[x + z * _terrainSettings.TerrainSize] = point;
                }
            }

            //Border
            for (int z = 0; z < _terrainSettings.TerrainSize; z++)
            {
                point = BorderMeshGeometry3DProperty.Positions[(z * 2) + 1];
                point.Y = _terrainSettings.TerrainPoints[z] * GeneralHeight;
                BorderMeshGeometry3DProperty.Positions[(z * 2) + 1] = point;
            }

            for (int x = 0; x < _terrainSettings.TerrainSize; x++)
            {
                point = BorderMeshGeometry3DProperty.Positions[(x * 2) + 1 + (2 * _terrainSettings.TerrainSize)];
                point.Y = _terrainSettings.TerrainPoints[x * _terrainSettings.TerrainSize + _terrainSettings.TerrainSize - 1] * GeneralHeight;
                BorderMeshGeometry3DProperty.Positions[(x * 2) + 1 + (2 * _terrainSettings.TerrainSize)] = point;
            }

            for (int z = _terrainSettings.TerrainSize; z > 0; z--)
            {
                point = BorderMeshGeometry3DProperty.Positions[(z * 2) - 1 + (4 * _terrainSettings.TerrainSize)];
                point.Y = _terrainSettings.TerrainPoints[(_terrainSettings.TerrainSize * _terrainSettings.TerrainSize) - z] * GeneralHeight;
                BorderMeshGeometry3DProperty.Positions[(z * 2) - 1 + (4 * _terrainSettings.TerrainSize)] = point;
            }

            for (int x = _terrainSettings.TerrainSize; x > 0; x--)
            {
                point = BorderMeshGeometry3DProperty.Positions[(x * 2) - 1 + (6 * _terrainSettings.TerrainSize)];
                point.Y = _terrainSettings.TerrainPoints[(_terrainSettings.TerrainSize - x) * _terrainSettings.TerrainSize] * GeneralHeight;
                BorderMeshGeometry3DProperty.Positions[(x * 2) - 1 + (6 * _terrainSettings.TerrainSize)] = point;
            }



        }
        public void UpdateTexture()
        {
            _terrainImageBrush.ImageSource = _terrainSettings.ColorMapImage;
            _borderImageBrush.ImageSource = _terrainSettings.BorderMapImage;
        }
        #endregion
    }
}
