using System.ComponentModel;
using Topographer3D.Models;
using Topographer3D.Utilities;

namespace Topographer3D.ViewModels.Layers
{
    class HydraulicErosionLayer : BaseLayer
    {
        #region ATTRIBUTES & PROPERTIES
        private HydraulicErosionAlgorithm hydraulicErosion;
        private float[] TerrainPoints;
        private int TerrainSize;

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

        #endregion

        #region INITIALIZATION
        public HydraulicErosionLayer(LayerManager layerManager, TerrainEngine terrainEngine) : base(layerManager, terrainEngine)
        {
            hydraulicErosion = new HydraulicErosionAlgorithm();
            InitProperties();
        }

        private void InitProperties()
        {
            LayerType = Layer.Hydraulic;
            HasApplicationMode = System.Windows.Visibility.Hidden;
            HEIterations = 250000;
            HEMaxDropletLifetime = 15;
            HESeed = 1;
            HEErosionRadius = 1;
            HEInertia = 0.25f;
            HESedimentCapacityFactor = 2.0f;
            HEMinSedimentCapacity = 0.1f;
            HEErodeSpeed = 0.15f;
            HEDepositSpeed = 0.15f;
            HEEvaporateSpeed = 0.5f;
            HEGravity = 2;
        }

        #endregion

        #region TERRAIN ENGINE PROCESSING
        public void StartErosion(int TerrainSize, float[] TerrainPoints)
        {
            this.TerrainSize = TerrainSize;
            this.TerrainPoints = TerrainPoints;

            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += ErosionCalculation;
            worker.ProgressChanged += ErosionUpdate;
            worker.RunWorkerCompleted += ErosionComplete;
            worker.RunWorkerAsync(100000);
        }

        private void ErosionCalculation(object sender, DoWorkEventArgs e)
        {
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
                case 16:
                    iterations *= 1;
                    erosionRadius *= 1;
                    maxDropletLifetime *= 1;
                    inertia *= 0.5f;
                    sedimentCapacityFactor *= 0.125f;
                    minSedimentCapacity *= 0.125f;
                    erodeSpeed *= 0.125f;
                    depositSpeed *= 0.125f;
                    evaporateSpeed *= 0.125f;
                    gravity *= 0.125f;
                    break;
                case 32:
                    iterations *= 1;
                    erosionRadius *= 1;
                    maxDropletLifetime *= 1;
                    inertia *= 0.5f;
                    sedimentCapacityFactor *= 0.25f;
                    minSedimentCapacity *= 0.25f;
                    erodeSpeed *= 0.25f;
                    depositSpeed *= 0.25f;
                    evaporateSpeed *= 0.25f;
                    gravity *= 0.25f;
                    break;
                case 64:
                    iterations *= 1;
                    erosionRadius *= 1;
                    maxDropletLifetime *= 1;
                    inertia *= 0.5f;
                    sedimentCapacityFactor *= 0.5f;
                    minSedimentCapacity *= 0.5f;
                    erodeSpeed *= 0.5f;
                    depositSpeed *= 0.5f;
                    evaporateSpeed *= 0.5f;
                    gravity *= 0.5f;
                    break;
                case 128:
                    iterations *= 1;
                    erosionRadius *= 1;
                    maxDropletLifetime *= 1;
                    inertia *= 1;
                    sedimentCapacityFactor *= 1;
                    minSedimentCapacity *= 1;
                    erodeSpeed *= 1;
                    depositSpeed *= 1;
                    evaporateSpeed *= 1;
                    gravity *= 1;
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
                    erosionRadius *= 3;
                    inertia *= 3;
                    sedimentCapacityFactor *= 3;
                    minSedimentCapacity *= 3;
                    erodeSpeed *= 3;
                    depositSpeed *= 3;
                    evaporateSpeed *= 3;
                    gravity *= 3;
                    maxDropletLifetime *= 3;
                    break;
                case 2048:
                    iterations *= 8;
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
                case 4096:
                    iterations *= 16;
                    erosionRadius *= 5;
                    inertia *= 5;
                    sedimentCapacityFactor *= 5;
                    minSedimentCapacity *= 5;
                    erodeSpeed *= 5;
                    depositSpeed *= 5;
                    evaporateSpeed *= 5;
                    gravity *= 5;
                    maxDropletLifetime *= 5;
                    break;
            }

            hydraulicErosion.UpdateValues(seed, erosionRadius, inertia, sedimentCapacityFactor, minSedimentCapacity, erodeSpeed, depositSpeed, evaporateSpeed, gravity, maxDropletLifetime);
            hydraulicErosion.Initialize(TerrainPoints, TerrainSize, false);

            for (int iteration = 0; iteration < iterations; iteration++)
            {
                hydraulicErosion.Erode();

                if(iteration % 10000 == 0)
                {
                    int progressPercentage = (int)(((float)iteration / (float)iterations) * 100);
                    (sender as BackgroundWorker).ReportProgress(progressPercentage);
                }
            }
        }

        private void ErosionUpdate(object sender, ProgressChangedEventArgs e)
        {
            ProgressPercentage = e.ProgressPercentage;
        }

        private void ErosionComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            Processed();
            terrainEngine.SingleLayerCalculationComplete(this, TerrainPoints);
            Dispose();
        }

        #endregion

        #region Disposable
        protected override void Dispose()
        {
            TerrainPoints = null;
        }

        #endregion
    }
}
