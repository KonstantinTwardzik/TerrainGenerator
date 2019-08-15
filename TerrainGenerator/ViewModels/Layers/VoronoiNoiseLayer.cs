using System;
using Topographer3D.Utilities;

namespace Topographer3D.ViewModels.Layers
{
    // VORONOI NOT IMPLEMENTED YET
    class VoronoiNoiseLayer : BaseLayer
    {
        //private VoronoiNoise voronoiNoise;

        public float VoronoiScale { get; set; }
        public int VoronoiOctaves { get; set; }
        public float VoronoiOctaveWeight { get; set; }
        public float VoronoiScaleX { get; set; }
        public float VoronoiScaleZ { get; set; }
        public int VoronoiSeed { get; set; }

        public VoronoiNoiseLayer(LayerManager layerManager, TerrainEngine terrainEngine) : base(layerManager, terrainEngine)
        {
            //voronoiNoise = new VoronoiNoise();
            InitProperties();
        }

        private void InitProperties()
        {
            LayerType = Layer.Voronoi;
            VoronoiScale = 1.0f;
            VoronoiOctaves = 6;
            VoronoiOctaveWeight = 0.6f;
            VoronoiScaleX = 0.5f;
            VoronoiScaleZ = 0.5f;
            VoronoiSeed = 500;
        }
    }
}
