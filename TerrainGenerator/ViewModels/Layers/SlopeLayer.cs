using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Topographer3D.ViewModels.Layers
{
    class SlopeLayer : BaseLayer
    {
        #region Attributes
        private float[] TerrainPoints;
        private int TerrainSize;
        private int startPosition;
        private int endPosition;

        #endregion

        #region Properties
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

        #region Initialization
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

        #region TerrainEngine Processing
        public void StartSlope(int TerrainSize, float[] TerrainPoints)
        {
            this.TerrainSize = TerrainSize;
            this.TerrainPoints = TerrainPoints;

            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += SlopeCalculation;
            worker.ProgressChanged += ProgressChanged;
            worker.RunWorkerCompleted += CalculationCompleted;
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
                    TerrainPoints[x + z * TerrainSize] = ApplyMode(TerrainPoints[x + z * TerrainSize], value);
                }
                int progressPercentage = (int)(((float)x / (float)TerrainSize) * 100);
                (sender as BackgroundWorker).ReportProgress(progressPercentage);
            }
        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressPercentage = e.ProgressPercentage;
        }

        private void CalculationCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Processed();
            terrainEngine.WorkerComplete(this, TerrainPoints);
            Dispose();
        }

        private float ApplyMode(float oldValue, float applyValue)
        {
            float newValue = 0;
            switch (CurrentApplicationMode)
            {
                case ApplicationMode.Normal:
                    newValue = applyValue;
                    break;
                case ApplicationMode.Add:
                    newValue = oldValue + applyValue;
                    break;
                case ApplicationMode.Multiply:
                    newValue = oldValue * applyValue;
                    break;
                case ApplicationMode.Subtract:
                    newValue = oldValue - applyValue;
                    break;
            }
            if (newValue < 0)
            {
                newValue = 0;
            }
            else if (newValue > 1)
            {
                newValue = 1;
            }
            return newValue;
        }

        protected override void Dispose()
        {
            TerrainPoints = null;
        }

        #endregion
    }

    public enum Direction
    {
        X_Axis,
        Z_Axis
    }
}
