using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Topographer3D.Utilities;

namespace Topographer3D.ViewModels.Layers
{
    class SlopeLayer : BaseLayer
    {
        #region ATTRIBUTES & PROPERTIES
        private float[] TerrainPoints;
        private int TerrainSize;
        private int startPosition;
        private int endPosition;

        public float StartHeight { get; set; }
        public float EndHeight { get; set; }
        public int StartPosition
        {
            get
            {
                return startPosition;
            }
            set
            {
                if (value < EndPosition)
                    startPosition = value;
            }
        }
        public int EndPosition
        {
            get
            {
                return endPosition;
            }
            set
            {
                if (value > StartPosition)
                    endPosition = value;
            }
        }
        public Direction CurrentDirection { get; set; }
        public IEnumerable<Direction> DirectionEnum { get { return Enum.GetValues(typeof(Direction)).Cast<Direction>(); } }
        public IEnumerable<InterpolationMode> InterpolationModeEnum { get { return Enum.GetValues(typeof(InterpolationMode)).Cast<InterpolationMode>(); } }
        public InterpolationMode CurrentInterpolationMode { get; set; }

        #endregion

        #region INITIALIZATION
        public SlopeLayer(LayerManager layerManager, TerrainEngine terrainEngine) : base(layerManager, terrainEngine)
        {
            InitProperties();
        }

        private void InitProperties()
        {
            LayerType = Layer.Slope;
            StartHeight = 0.0f;
            EndHeight = 1.0f;
            StartPosition = 0;
            EndPosition = 100;
            CurrentDirection = Direction.X_Axis;
            CurrentInterpolationMode = InterpolationMode.Smooth;
        }

        #endregion

        #region TERRAIN ENGINE PROCESSING
        public void StartSlope(int TerrainSize, float[] TerrainPoints)
        {
            this.TerrainSize = TerrainSize;
            this.TerrainPoints = TerrainPoints;

            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += SlopeCalculation;
            worker.ProgressChanged += SlopeUpdate;
            worker.RunWorkerCompleted += SlopeComplete;
            worker.RunWorkerAsync(100000);
        }

        private void SlopeCalculation(object sender, DoWorkEventArgs e)
        {
            float value = 0;
            float heightRange = StartHeight - EndHeight;
            float startPosition = (float)StartPosition / 100.0f * TerrainSize;
            float endPosition = (float)EndPosition / 100.0f * TerrainSize;
            float positionRange = endPosition - startPosition;

            if (positionRange < 0)
            {
                positionRange = -positionRange;
            }
            if (heightRange < 0)
            {
                heightRange = -heightRange;
            }

            for (int x = 0; x < TerrainSize; x++)
            {
                for (int z = 0; z < TerrainSize; z++)
                {
                    switch (CurrentDirection)
                    {
                        case Direction.X_Axis:
                            switch (CurrentInterpolationMode)
                            {
                                case InterpolationMode.Linear:
                                    if (x < startPosition)
                                    {
                                        value = StartHeight;
                                    }
                                    else if (x > endPosition)
                                    {
                                        value = EndHeight;
                                    }
                                    else
                                    {
                                        if (StartHeight < EndHeight)
                                        {
                                            value = StartHeight + ((((float)x - startPosition) / positionRange) * heightRange);
                                        }
                                        else
                                        {
                                            value = StartHeight - ((((float)x - startPosition) / positionRange) * heightRange);
                                        }
                                    }
                                    break;
                                case InterpolationMode.Smooth:
                                    if (x < startPosition)
                                    {
                                        value = StartHeight;
                                    }
                                    else if (x > endPosition)
                                    {
                                        value = EndHeight;
                                    }
                                    else
                                    {
                                        if (StartHeight < EndHeight)
                                        {
                                            value = StartHeight + (float)(Math.Cos(Math.PI - ((x - startPosition) / positionRange) * Math.PI) / 2.0f + 0.5f) * heightRange;
                                        }
                                        else
                                        {
                                            value = StartHeight - (float)(Math.Cos(Math.PI - ((x - startPosition) / positionRange) * Math.PI) / 2.0f + 0.5f) * heightRange;
                                        }
                                    }
                                    break;
                            }
                            break;
                        case Direction.Z_Axis:
                            switch (CurrentInterpolationMode)
                            {
                                case InterpolationMode.Linear:
                                    if (z < startPosition)
                                    {
                                        value = StartHeight;
                                    }
                                    else if (z > endPosition)
                                    {
                                        value = EndHeight;
                                    }
                                    else
                                    {
                                        if (StartHeight < EndHeight)
                                        {
                                            value = StartHeight + ((((float)z - startPosition) / positionRange) * heightRange);
                                        }
                                        else
                                        {
                                            value = StartHeight - ((((float)z - startPosition) / positionRange) * heightRange);
                                        }

                                    }
                                    break;
                                case InterpolationMode.Smooth:
                                    if (z < startPosition)
                                    {
                                        value = StartHeight;
                                    }
                                    else if (z > endPosition)
                                    {
                                        value = EndHeight;
                                    }
                                    else
                                    {
                                        if (StartHeight < EndHeight)
                                        {
                                            value = StartHeight + (float)(Math.Cos(Math.PI - ((z - startPosition) / positionRange) * Math.PI) / 2.0f + 0.5f) * heightRange;
                                        }
                                        else
                                        {
                                            value = StartHeight - (float)(Math.Cos(Math.PI - ((z - startPosition) / positionRange) * Math.PI) / 2.0f + 0.5f) * heightRange;
                                        }
                                    }
                                    break;
                            }
                            break;

                    }
                    TerrainPoints[x + z * TerrainSize] = Application.Apply(TerrainPoints[x + z * TerrainSize], value, CurrentApplicationMode);
                }
                int progressPercentage = (int)(((float)x / (float)TerrainSize) * 100);
                (sender as BackgroundWorker).ReportProgress(progressPercentage);
            }
        }

        private void SlopeUpdate(object sender, ProgressChangedEventArgs e)
        {
            ProgressPercentage = e.ProgressPercentage;
        }

        private void SlopeComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            Processed();
            terrainEngine.SingleLayerCalculationComplete(this, TerrainPoints);
        }

        #endregion
    }
}
