﻿using HelixToolkit.Wpf.SharpDX;
using Media3D = System.Windows.Media.Media3D;
using Vector3D = System.Windows.Media.Media3D.Vector3D;
using Transform3D = System.Windows.Media.Media3D.Transform3D;
using Color = System.Windows.Media.Color;
using Vector3 = SharpDX.Vector3;
using Colors = System.Windows.Media.Colors;
using System;
using System.IO;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SharpDX;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using Topographer3D.Commands;

namespace Topographer3D.ViewModels
{
    internal class Viewport : ObservableObject, IDisposable
    {
        #region Properties

        private TerrainSettings terrainSettings;
        private ViewportCamera viewportCamera;
        private MeshBuilder terrainMesh;
        private MeshBuilder borderMesh;

        public IEffectsManager EffectsManager
        {
            get; protected set;
        }

        public float HeightMultiplicator { get; set; }
        public Vector3D UpDirection { get; set; } = new Vector3D(0, 1, 0);
        public Vector3D DirectionalLightDirection { get; private set; }
        public Color DirectionalLightColor { get; private set; }
        public Color AmbientLightColor { get; private set; }
        public MeshGeometry3D TerrainMeshMainGeometry3D { get; private set; }
        public MeshGeometry3D TerrainMeshBorderGeometry3D { get; private set; }
        public PhongMaterial TerrainMeshMainMaterial { get; private set; }
        public PhongMaterial TerrainMeshBorderMaterial { get; private set; }
        public Transform3D TerrainMeshMainTransform { get; private set; }
        public Transform3D TerrainMeshBorderTransform { get; private set; }
        public TextureModel TerrainMeshMainTexture { get; private set; }
        public TextureModel TerrainMeshBorderTexture { get; private set; }

        public double MinZoom { get; private set; }
        public double MaxZoom { get; private set; }

        #endregion

        #region Initialization
        public Viewport(TerrainSettings terrainSettings, ViewportCamera viewportCamera)
        {
            EffectsManager = new DefaultEffectsManager();
            this.terrainSettings = terrainSettings;
            this.viewportCamera = viewportCamera;
            HeightMultiplicator = 1.0f;
            MinZoom = 2;
            MaxZoom = 5;

            //Lighting            
            AmbientLightColor = Colors.DimGray;
            DirectionalLightColor = Colors.White;
            DirectionalLightDirection = new Vector3D(-2, -5, -2);

            InitCommands();
            InitMesh();
        }



        public void InitMesh()
        {
            if (TerrainMeshMainGeometry3D != null)
            {
                TerrainMeshMainGeometry3D.Positions.Clear();
                TerrainMeshMainGeometry3D.TriangleIndices.Clear();
                TerrainMeshMainGeometry3D.TextureCoordinates.Clear();
                TerrainMeshBorderGeometry3D.Positions.Clear();
                TerrainMeshBorderGeometry3D.TriangleIndices.Clear();
                TerrainMeshBorderGeometry3D.TextureCoordinates.Clear();
            }

            GenerateMainMesh();
            GenerateBorderMesh();
            GenerateDefaultTexture();
        }

        private void InitCommands()
        {
            PerspectiveCommand = new PerspectiveCommand(this);
            OrthographicCommand = new OrthographicCommand(this);

    }
        #endregion

        #region Generating 3D Model

        private void GenerateMainMesh()
        {
            terrainMesh = new MeshBuilder();
            GenerateTerrainPositions();
            GenerateTerrainTriangleIndices();
            GenerateTerrainUVCoordinates();
            TerrainMeshMainGeometry3D = terrainMesh.ToMeshGeometry3D();
            TerrainMeshMainMaterial = new PhongMaterial();
            TerrainMeshMainMaterial.EnableFlatShading = true;
            TerrainMeshMainMaterial.RenderDiffuseMap = true;
            TerrainMeshMainTransform = new Media3D.TranslateTransform3D(0, 0, 0);
        }

        private void GenerateBorderMesh()
        {
            borderMesh = new MeshBuilder();
            GenerateBorderPositions();
            GenerateBorderTriangleIndices();
            GenerateBorderUVCoordinates();
            TerrainMeshBorderGeometry3D = borderMesh.ToMeshGeometry3D();
            TerrainMeshBorderMaterial = new PhongMaterial();
            TerrainMeshBorderMaterial.EnableFlatShading = true;
            TerrainMeshBorderMaterial.RenderDiffuseMap = true;
            TerrainMeshBorderTransform = new Media3D.TranslateTransform3D(0, 0, 0);
        }

        private void GenerateTerrainPositions()
        {
            Vector3 point = new Vector3();
            // Terrain Points
            for (int x = 0; x < terrainSettings.TerrainSize; x++)
            {
                for (int z = 0; z < terrainSettings.TerrainSize; z++)
                {
                    point.X = (float)((float)x / ((float)terrainSettings.TerrainSize - 1) - 0.5) * 2;
                    point.Y = 0;
                    point.Z = (float)((float)z / ((float)terrainSettings.TerrainSize - 1) - 0.5) * 2;
                    terrainMesh.Positions.Add(point);
                    //terrainMesh.Normals.Add(new Vector3(0, 1, 0));
                }
            }
        }

        private void GenerateTerrainTriangleIndices()
        {
            var value = 0;
            // Terrain Indices
            for (int i = 0; i < terrainSettings.TerrainSize * terrainSettings.TerrainSize - terrainSettings.TerrainSize; i++)
            {
                for (int trianglePoint = 0; trianglePoint < 3; trianglePoint++)
                {
                    if (i % terrainSettings.TerrainSize == 0)
                    {
                        break;
                    }
                    if (trianglePoint == 0)
                    {
                        value = i;
                    }
                    else if (trianglePoint == 1)
                    {
                        value = i + terrainSettings.TerrainSize;

                    }
                    else if (trianglePoint == 2)
                    {
                        value = i + terrainSettings.TerrainSize - 1;
                    }
                    terrainMesh.TriangleIndices.Add(value);
                }
                for (int trianglePoint = 0; trianglePoint < 3; trianglePoint++)
                {
                    if (i > 0 && ((i + 1) % terrainSettings.TerrainSize) == 0)
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
                        value = i + terrainSettings.TerrainSize;
                    }
                    terrainMesh.TriangleIndices.Add(value);
                }
            }
        }

        private void GenerateTerrainUVCoordinates()
        {
            Vector2 point = new Vector2();
            for (int x = 0; x < terrainSettings.TerrainSize; x++)
            {
                for (int z = 0; z < terrainSettings.TerrainSize; z++)
                {
                    point.X = (float)((float)x / (float)terrainSettings.TerrainSize);
                    point.Y = (float)((float)z / (float)terrainSettings.TerrainSize);
                    terrainMesh.TextureCoordinates.Add(point);
                }
            }
        }

        private void GenerateBorderPositions()
        {
            Vector3 point = new Vector3(-1, 0, 0);

            // Border Points
            for (int z = 0; z < terrainSettings.TerrainSize; z++)
            {
                point.Z = (float)((float)z / ((float)terrainSettings.TerrainSize - 1) - 0.5) * 2;
                borderMesh.Positions.Add(point);
                borderMesh.Positions.Add(point);
            }

            point.Z = 1;
            for (int x = 0; x < terrainSettings.TerrainSize; x++)
            {
                point.X = (float)((float)x / ((float)terrainSettings.TerrainSize - 1) - 0.5) * 2;
                borderMesh.Positions.Add(point);
                borderMesh.Positions.Add(point);
            }

            point.X = 1;
            for (int z = terrainSettings.TerrainSize - 1; z >= 0; z--)
            {
                point.Z = (float)((float)z / ((float)terrainSettings.TerrainSize - 1) - 0.5) * 2;
                borderMesh.Positions.Add(point);
                borderMesh.Positions.Add(point);
            }

            point.Z = -1;
            for (int x = terrainSettings.TerrainSize - 1; x >= 0; x--)
            {
                point.X = (float)((float)x / ((float)terrainSettings.TerrainSize - 1) - 0.5) * 2;
                borderMesh.Positions.Add(point);
                borderMesh.Positions.Add(point);
            }
        }

        private void GenerateBorderTriangleIndices()
        {
            int value = 0;
            // Border Indices
            for (int i = 0; i < borderMesh.Positions.Count - 2; i++)
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
                        borderMesh.TriangleIndices.Add(value);
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
                        borderMesh.TriangleIndices.Add(value);
                    }
                }
            }
        }

        private void GenerateBorderUVCoordinates()
        {
            Vector2 point = new Vector2();
            //Border
            for (int x = 0; x < borderMesh.Positions.Count; x++)
            {
                if (x % 2 == 0)
                {
                    point.X = 0.0f;
                    point.Y = 0.0f;
                }
                else
                {
                    point.Y = 1.0f;
                    point.X = 1.0f;
                }
                borderMesh.TextureCoordinates.Add(point);
            }
        }

        public void GenerateDefaultTexture()
        {
            //Create White Image 
            PixelFormat pixelFormat = PixelFormats.Bgr24;
            int rawStride = (pixelFormat.BitsPerPixel + 7) / 8;
            byte[] rawImage = new byte[rawStride];
            for (int i = 0; i < rawStride; i++)
            {
                rawImage[i] = 255;
            }

            //Convert to memorystream
            BitmapSource bitmap = BitmapSource.Create(1, 1, 96, 96, pixelFormat, null, rawImage, rawStride);
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            MemoryStream memoryStream = new MemoryStream();           
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            encoder.Save(memoryStream);
            memoryStream.Position = 0;

            //Apply to Materials
            TerrainMeshMainTexture = new TextureModel(memoryStream);
            TerrainMeshBorderTexture = new TextureModel(memoryStream);
            TerrainMeshMainMaterial.DiffuseMap = TerrainMeshMainTexture;
            TerrainMeshBorderMaterial.DiffuseMap = TerrainMeshBorderTexture;
        }
        #endregion

        #region Updating 3D Model
        public void UpdateMesh()
        {
            Vector3 point = new Vector3();

            //Update Terrainmesh
            for (int x = 0; x < terrainSettings.TerrainSize; x++)
            {
                for (int z = 0; z < terrainSettings.TerrainSize; z++)
                {
                    point = terrainMesh.Positions[x + (z * terrainSettings.TerrainSize)];
                    point.Y = (float)(terrainSettings.TerrainPoints[x + z * terrainSettings.TerrainSize] * HeightMultiplicator);
                    terrainMesh.Positions[x + z * terrainSettings.TerrainSize] = point;
                }
            }
            TerrainMeshMainGeometry3D = terrainMesh.ToMeshGeometry3D();

            //for (int x = 0; x < terrainSettings.TerrainSize; x++)
            //{
            //    for (int z = 0; z < terrainSettings.TerrainSize; z++)
            //    {
            //        terrainMesh.Normals[x + z * terrainSettings.TerrainSize] = new Vector3((float)terrainSettings.TerrainPoints[z + x * terrainSettings.TerrainSize], (float)terrainSettings.TerrainPoints[x + z * terrainSettings.TerrainSize], (float)terrainSettings.TerrainPoints[z + x * terrainSettings.TerrainSize]);
            //        terrainMesh.Normals[x + z * terrainSettings.TerrainSize].Normalize();
            //    }
            //}
                                 
            //Update Bordermesh
            for (int z = 0; z < terrainSettings.TerrainSize; z++)
            {
                point = borderMesh.Positions[(z * 2) + 1];
                point.Y = (float)(terrainSettings.TerrainPoints[z] * HeightMultiplicator);
                borderMesh.Positions[(z * 2) + 1] = point;
            }

            for (int x = 0; x < terrainSettings.TerrainSize; x++)
            {
                point = borderMesh.Positions[(x * 2) + 1 + (2 * terrainSettings.TerrainSize)];
                point.Y = (float)(terrainSettings.TerrainPoints[x * terrainSettings.TerrainSize + terrainSettings.TerrainSize - 1] * HeightMultiplicator);
                borderMesh.Positions[(x * 2) + 1 + (2 * terrainSettings.TerrainSize)] = point;
            }

            for (int z = terrainSettings.TerrainSize; z > 0; z--)
            {
                point = borderMesh.Positions[(z * 2) - 1 + (4 * terrainSettings.TerrainSize)];
                point.Y = (float)(terrainSettings.TerrainPoints[(terrainSettings.TerrainSize * terrainSettings.TerrainSize) - z] * HeightMultiplicator);
                borderMesh.Positions[(z * 2) - 1 + (4 * terrainSettings.TerrainSize)] = point;
            }

            for (int x = terrainSettings.TerrainSize; x > 0; x--)
            {
                point = borderMesh.Positions[(x * 2) - 1 + (6 * terrainSettings.TerrainSize)];
                point.Y = (float)(terrainSettings.TerrainPoints[(terrainSettings.TerrainSize - x) * terrainSettings.TerrainSize] * HeightMultiplicator);
                borderMesh.Positions[(x * 2) - 1 + (6 * terrainSettings.TerrainSize)] = point;
            }
            TerrainMeshBorderGeometry3D = borderMesh.ToMeshGeometry3D();
        }

        public void UpdateTexture()
        {
            TerrainMeshMainTexture = new TextureModel(terrainSettings.TerrainMainColors);
            TerrainMeshMainMaterial.DiffuseMap = TerrainMeshMainTexture;
            TerrainMeshBorderTexture = new TextureModel(terrainSettings.TerrainBorderColors);
            TerrainMeshBorderMaterial.DiffuseMap = TerrainMeshBorderTexture;
        }
        #endregion

        #region Viewport Settings 
        public void SetOrthographicView()
        {
            viewportCamera.SetOrthographicCam();
            MinZoom = 8;
            MaxZoom = 20;
        }

        public void SetPerspectiveView()
        {
            viewportCamera.SetPerspectiveCam();
            MinZoom = 2;
            MaxZoom = 5;
        }
        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~Viewport()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
                if (EffectsManager != null)
                {
                    var effectManager = EffectsManager as IDisposable;
                    Disposer.RemoveAndDispose(ref effectManager);
                }
                disposedValue = true;
                GC.SuppressFinalize(this);
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

        #region ICommands
        public bool CanExecute { get { return true; } }
        public ICommand PerspectiveCommand { get; private set; }
        public ICommand OrthographicCommand { get; private set; }
        #endregion
    }

    public abstract class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName]string info = "")
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        protected bool SetValue<T>(ref T backingField, T value, [CallerMemberName]string propertyName = "")
        {
            if (object.Equals(backingField, value))
            {
                return false;
            }

            backingField = value;
            this.OnPropertyChanged(propertyName);
            return true;
        }
    }
}
