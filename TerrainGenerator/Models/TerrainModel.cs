using HelixToolkit.Wpf.SharpDX;
using Transform3D = System.Windows.Media.Media3D.Transform3D;

namespace Topographer3D.Models
{
    class TerrainModel
    {
        public MeshGeometry3D TerrainMeshMainGeometry3D { get; set; }
        public MeshGeometry3D TerrainMeshBorderGeometry3D { get; set; }
        public PhongMaterial TerrainMeshMainMaterial { get; set; }
        public PhongMaterial TerrainMeshBorderMaterial { get; set; }
        public Transform3D TerrainMeshTransform { get; set; }
        public TextureModel TerrainMeshMainTexture { get; set; }
        public TextureModel TerrainMeshBorderTexture { get; set; }


        public float[] TerrainHeights { get; set; }
        public float[] PreviousTerrainHeights { get; set; }

        public byte[] PreviousColors { get; set; }
        public byte[] CurrentColors { get; set; }

        public int TerrainSize { get; set; }
    }
}
