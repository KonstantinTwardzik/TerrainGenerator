using System;
using Topographer3D.Models;

namespace Topographer3D.ViewModels.Layers
{
    class ThermalErosionLayer : BaseLayer
    {
        private HydraulicErosion hydraulicErosion;

        //Hydraulic Erosion 
        public int HEIterations { get; set; }
        public int HEMaxDropletLifetime { get; set; }
        public int HESeed { get; set; }
        public int HEErosionRadius { get; set; }
        public float HEInertia { get; set; }
        public float HESedimentCapacityFactor { get; set; }
        public float HEMinSedimentCapacity { get; set; }
        public float HEErodeSpeed { get; set; }
        public float HEDepositSpeed { get; set; }
        public float HEEvaporateSpeed { get; set; }
        public float HEGravity { get; set; }
        public bool HEErodeOver { get; set; }

        public ThermalErosionLayer(LayerManager layerManager, TerrainEngine terrainEngine) : base(layerManager, terrainEngine)
        {
            hydraulicErosion = new HydraulicErosion();
            InitProperties();
        }

        private void InitProperties()
        {
            //Hydraulic Erosion
            HEIterations = 150000;
            HEMaxDropletLifetime = 15;
            HESeed = 1;
            HEErosionRadius = 1;
            HEInertia = 0.25f;
            HESedimentCapacityFactor = 1.5f;
            HEMinSedimentCapacity = 0.4f;
            HEErodeSpeed = 0.15f;
            HEDepositSpeed = 0.15f;
            HEEvaporateSpeed = 0.5f;
            HEGravity = 2;
            HEErodeOver = false;
        }

        public void Erode(int TerrainSize, float[] TerrainPoints, int sizeCompensator)
        {
            //if (!HEErodeOver && isEroded == true)
            //{
            //    for (int x = 0; x < TerrainSize; x++)
            //    {
            //        for (int z = 0; z < TerrainSize; z++)
            //        {
            //            //TerrainPoints[x + z * TerrainSize] = (float)TerrainPointsUneroded[x + z * TerrainSize];
            //        }
            //    }
            //}

            int seed = HESeed;
            int iterations = HEIterations;
            int erosionRadius = HEErosionRadius;
            float inertia = 0.2f * HEInertia;
            float sedimentCapacityFactor = HESedimentCapacityFactor;
            float minSedimentCapacity = 0.001f + 0.025f * HEMinSedimentCapacity;
            float erodeSpeed = HEErodeSpeed;
            float depositSpeed = 0.5f * HEDepositSpeed;
            float evaporateSpeed = 0.001f + 0.005f * HEEvaporateSpeed;
            float gravity = HEGravity;
            int maxDropletLifetime = HEMaxDropletLifetime;

            switch (TerrainSize)
            {
                case 256:
                    iterations *= 1;
                    erosionRadius *= 1;
                    inertia *= 1;
                    sedimentCapacityFactor *= 1;
                    minSedimentCapacity *= 1;
                    erodeSpeed *= 1;
                    depositSpeed *= 1;
                    evaporateSpeed *= 1;
                    gravity *= 1;
                    maxDropletLifetime *= 1;
                    break;
                case 512:
                    iterations *= 2;
                    erosionRadius *= 2;
                    inertia *= 2;
                    sedimentCapacityFactor *= 2;
                    minSedimentCapacity *= 2;
                    erodeSpeed *= 2;
                    depositSpeed *= 2;
                    evaporateSpeed *= 2;
                    gravity *= 2;
                    maxDropletLifetime *= 2;
                    break;
                case 1024:
                    iterations *= 4;
                    erosionRadius *= 4;
                    inertia *= 4;
                    sedimentCapacityFactor *= 4;
                    minSedimentCapacity *= 4;
                    erodeSpeed *= 4;
                    depositSpeed *= 4;
                    evaporateSpeed *= 4;
                    gravity *= 4;
                    maxDropletLifetime *= 4;
                    break;
                case 2048:
                    iterations *= 8;
                    erosionRadius *= 8;
                    inertia *= 8;
                    sedimentCapacityFactor *= 8;
                    minSedimentCapacity *= 8;
                    erodeSpeed *= 8;
                    depositSpeed *= 8;
                    evaporateSpeed *= 8;
                    gravity *= 8;
                    maxDropletLifetime *= 8;
                    break;
            }

            hydraulicErosion.UpdateValues(seed, erosionRadius, inertia, sedimentCapacityFactor, minSedimentCapacity, erodeSpeed, depositSpeed, evaporateSpeed, gravity, maxDropletLifetime);
            hydraulicErosion.Erode(TerrainPoints, TerrainSize, iterations, false);

        }

        protected override void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
