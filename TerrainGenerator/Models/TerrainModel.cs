using HelixToolkit.Wpf.SharpDX;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Transform3D = System.Windows.Media.Media3D.Transform3D;

namespace Topographer3D.Models
{
    class TerrainModel
    {
        private int _terrainSize;
        private float[] _terrainHeights;
        private float[] _previousTerrainHeights;
        private byte[] _terrainColors;
        private byte[] _previousTerrainColors;
        private int rawStride;
        private PixelFormat pixelFormat = PixelFormats.Bgr24;

        public int TerrainSize
        {
            get
            {
                return _terrainSize;
            }
            set
            {
                _terrainSize = value;
                _terrainHeights = new float[value * value];
                _previousTerrainHeights = new float[value * value];
                rawStride = (TerrainSize * pixelFormat.BitsPerPixel + 7) / 8;

                _terrainColors = new byte[rawStride * _terrainSize];
                _previousTerrainColors = new byte[rawStride * _terrainSize];
            }
        }

        public MeshGeometry3D TerrainMeshMainGeometry3D { get; set; }
        public MeshGeometry3D TerrainMeshBorderGeometry3D { get; set; }
        public PhongMaterial TerrainMeshMainMaterial { get; set; }
        public PhongMaterial TerrainMeshBorderMaterial { get; set; }
        public Transform3D TerrainMeshTransform { get; set; }
        public TextureModel TerrainMeshMainTexture { get; set; }
        public TextureModel TerrainMeshBorderTexture { get; set; }

        public float[] TerrainHeights
        {
            get
            {
                return _terrainHeights;
            }
            set
            {
                _terrainHeights = value;
            }
        }
        public float[] PrevTerrainHeights { get { return _previousTerrainHeights; } set { _previousTerrainHeights = value; } }

        public byte[] TerrainColors { get { return _terrainColors; } set { _terrainColors = value; } }
        public byte[] PreviousTerrainColors { get { return _previousTerrainColors; } set { _previousTerrainColors = value; } }

        public BitmapImage HeightMapImage { get; set; }
        public BitmapImage ColorMapImage { get; set; }
    }
}
