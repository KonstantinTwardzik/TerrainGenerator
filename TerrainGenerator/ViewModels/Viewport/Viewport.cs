﻿using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using SharpDX;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Shaders;
using Vector3 = SharpDX.Vector3;
using Media3D = System.Windows.Media.Media3D;
using Transform3D = System.Windows.Media.Media3D.Transform3D;
using Color = System.Windows.Media.Color;
using Colors = System.Windows.Media.Colors;
using Topographer3D.Commands;
using Topographer3D.Utilities;
using Topographer3D.Models;

namespace Topographer3D.ViewModels
{
    internal class Viewport : ObservableObject, IDisposable
    {
        #region PROPERTIES
        // Logic
        private TerrainModel terrainModel;
        private TerrainEngine terrainEngine;
        private ViewportCamera viewportCamera;
        private MeshBuilder terrainMesh;
        private MeshBuilder borderMesh;

        // Terrain Model Properties
        public int TerrainSize
        {
            get { return terrainModel.TerrainSize; }
            set { terrainModel.TerrainSize = value; }
        }
        public MeshGeometry3D TerrainMeshMainGeometry3D
        {
            get { return terrainModel.TerrainMeshMainGeometry3D; }
            set { terrainModel.TerrainMeshMainGeometry3D = value; }
        }
        public MeshGeometry3D TerrainMeshBorderGeometry3D
        {
            get { return terrainModel.TerrainMeshBorderGeometry3D; }
            set { terrainModel.TerrainMeshBorderGeometry3D = value; }
        }
        public PhongMaterial TerrainMeshMainMaterial
        {
            get { return terrainModel.TerrainMeshMainMaterial; }
            set { terrainModel.TerrainMeshMainMaterial = value; }
        }
        public PhongMaterial TerrainMeshBorderMaterial
        {
            get { return terrainModel.TerrainMeshBorderMaterial; }
            set { terrainModel.TerrainMeshBorderMaterial = value; }
        }
        public Transform3D TerrainMeshTransform
        {
            get { return terrainModel.TerrainMeshTransform; }
            set { terrainModel.TerrainMeshTransform = value; }
        }
        public TextureModel TerrainMeshMainTexture
        {
            get { return terrainModel.TerrainMeshMainTexture; }
            set { terrainModel.TerrainMeshMainTexture = value; }
        }
        public TextureModel TerrainMeshBorderTexture
        {
            get { return terrainModel.TerrainMeshBorderTexture; }
            set { terrainModel.TerrainMeshBorderTexture = value; }
        }
        public float[] TerrainHeights
        {
            get { return terrainModel.TerrainHeights; }
        }

        // Other Stuff
        public IEffectsManager EffectsManager
        {
            get; protected set;
        }
        public float HeightMultiplicator { get; set; }
        public Color DirectionalLightColor { get; private set; }

        public FXAALevel FXAA { get; private set; }
        public MSAALevel MSAA { get; private set; }
        public SSAOQuality SSAO { get; private set; }
        public double MinZoom { get; private set; }
        public double MaxZoom { get; private set; }

        public bool Low { get; private set; }
        public bool Medium { get; private set; }
        public bool High { get; private set; }
        public bool Ultra { get; private set; }

        #endregion

        #region INITIALIZATION
        public Viewport(TerrainEngine terrainSettings, ViewportCamera viewportCamera, TerrainModel terrainModel)
        {
            this.terrainModel = terrainModel;
            EffectsManager = new DefaultEffectsManager();
            this.terrainEngine = terrainSettings;
            this.viewportCamera = viewportCamera;

            InitProperties();
            InitCommands();
            InitModel();
            InitDefaultViewportSettings();
            ChangeViewMode(0);
        }

        public void InitModel()
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

            GenerateMain();
            GenerateBorder();
            GenerateDefaultTexture();

        }

        private void InitCommands()
        {
            ChangeViewModeCommand = new ChangeViewModeCommand(this);
            ChangeMaterialCommand = new ChangeMaterialCommand(this);
            ChangeViewportQualityCommand = new ChangeViewportQualityCommand(this);
            ChangeShadingCommand = new ChangeShadingCommand(this);
        }

        private void InitProperties()
        {
            MSAA = new MSAALevel();
            FXAA = new FXAALevel();
            HeightMultiplicator = 1.0f;
            DirectionalLightColor = Colors.White;
            ChangeViewportQuality(2);
        }

        public void InitDefaultViewportSettings()
        {
            if (TerrainSize <= 256)
            {
                ChangeMaterial(0);
                ChangeShading(0);
            }
            else
            {
                ChangeMaterial(2);
                ChangeShading(1);
            }


        }
        #endregion

        #region INITIALIZING 3D MODEL

        private void GenerateMain()
        {
            terrainMesh = new MeshBuilder();
            GenerateTerrainPositions();
            GenerateTerrainTriangleIndices();
            GenerateTerrainUVCoordinates();
            TerrainMeshMainGeometry3D = terrainMesh.ToMeshGeometry3D();
            TerrainMeshMainMaterial = new PhongMaterial();

            TerrainMeshMainMaterial.RenderDiffuseMap = true;
            TerrainMeshTransform = new Media3D.TranslateTransform3D(0, 0, 0);
        }

        private void GenerateBorder()
        {
            borderMesh = new MeshBuilder();
            GenerateBorderPositions();
            GenerateBorderTriangleIndices();
            GenerateBorderUVCoordinates();
            TerrainMeshBorderGeometry3D = borderMesh.ToMeshGeometry3D();
            TerrainMeshBorderMaterial = new PhongMaterial();
            TerrainMeshBorderMaterial.RenderDiffuseMap = true;
        }

        private void GenerateTerrainPositions()
        {
            Vector3 point = new Vector3();
            // Terrain Points
            for (int x = 0; x < TerrainSize; x++)
            {
                for (int z = 0; z < TerrainSize; z++)
                {
                    point.X = (float)((float)x / ((float)TerrainSize - 1) - 0.5) * 2;
                    point.Y = 0;
                    point.Z = (float)((float)z / ((float)TerrainSize - 1) - 0.5) * 2;
                    terrainMesh.Positions.Add(point);
                    terrainMesh.Normals.Add(new Vector3(0, 1, 0));

                }
            }
        }

        private void GenerateTerrainTriangleIndices()
        {
            var value = 0;
            // Terrain Indices
            for (int i = 0; i < TerrainSize * TerrainSize - TerrainSize; i++)
            {
                for (int trianglePoint = 0; trianglePoint < 3; trianglePoint++)
                {
                    if (i % TerrainSize == 0)
                    {
                        break;
                    }
                    if (trianglePoint == 0)
                    {
                        value = i;
                    }
                    else if (trianglePoint == 1)
                    {
                        value = i + TerrainSize;

                    }
                    else if (trianglePoint == 2)
                    {
                        value = i + TerrainSize - 1;
                    }
                    terrainMesh.TriangleIndices.Add(value);
                }
                for (int trianglePoint = 0; trianglePoint < 3; trianglePoint++)
                {
                    if (i > 0 && ((i + 1) % TerrainSize) == 0)
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
                        value = i + TerrainSize;
                    }
                    terrainMesh.TriangleIndices.Add(value);
                }
            }
        }

        private void GenerateTerrainUVCoordinates()
        {
            Vector2 point = new Vector2();
            for (int x = 0; x < TerrainSize; x++)
            {
                for (int z = 0; z < TerrainSize; z++)
                {
                    point.Y = (float)((float)x / (float)TerrainSize);
                    point.X = (float)((float)z / (float)TerrainSize);
                    terrainMesh.TextureCoordinates.Add(point);
                }
            }
        }

        private void GenerateBorderPositions()
        {
            Vector3 point = new Vector3(-1, 0, 0);

            // Border Points
            for (int z = 0; z < TerrainSize; z++)
            {
                point.Z = (float)((float)z / ((float)TerrainSize - 1) - 0.5) * 2;
                borderMesh.Positions.Add(point);
                borderMesh.Positions.Add(point);
                borderMesh.Normals.Add(new Vector3(-1, 0, 0));
                borderMesh.Normals.Add(new Vector3(-1, 0, 0));
            }

            point.Z = 1;
            for (int x = 0; x < TerrainSize; x++)
            {
                point.X = (float)((float)x / ((float)TerrainSize - 1) - 0.5) * 2;
                borderMesh.Positions.Add(point);
                borderMesh.Positions.Add(point);
                borderMesh.Normals.Add(new Vector3(0, 0, 1));
                borderMesh.Normals.Add(new Vector3(0, 0, 1));
            }

            point.X = 1;
            for (int z = TerrainSize - 1; z >= 0; z--)
            {
                point.Z = (float)((float)z / ((float)TerrainSize - 1) - 0.5) * 2;
                borderMesh.Positions.Add(point);
                borderMesh.Positions.Add(point);
                borderMesh.Normals.Add(new Vector3(1, 0, 0));
                borderMesh.Normals.Add(new Vector3(1, 0, 0));
            }

            point.Z = -1;
            for (int x = TerrainSize - 1; x >= 0; x--)
            {
                point.X = (float)((float)x / ((float)TerrainSize - 1) - 0.5) * 2;
                borderMesh.Positions.Add(point);
                borderMesh.Positions.Add(point);
                borderMesh.Normals.Add(new Vector3(0, 0, -1));
                borderMesh.Normals.Add(new Vector3(0, 0, -1));
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
                rawImage[i] = 170;
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

        #region UPDATING 3D MODEL

        public void UpdateMesh()
        {
            Vector3 point = new Vector3();
            //Updates Terrainmesh

            for (int x = 0; x < TerrainSize; x++)
            {
                for (int z = 0; z < TerrainSize; z++)
                {
                    point = terrainMesh.Positions[z + (x * TerrainSize)];
                    point.Y = (TerrainHeights[z + x * TerrainSize] * HeightMultiplicator);
                    terrainMesh.Positions[z + x * TerrainSize] = point;
                }
            }
            TerrainMeshMainGeometry3D = terrainMesh.ToMeshGeometry3D();

            for (int x = 1; x < TerrainSize - 1; x++)
            {
                for (int z = 1; z < TerrainSize - 1; z++)
                {
                    Vector3 neighbour0 = terrainMesh.Positions[z + (x * TerrainSize) - 1];
                    Vector3 neighbour1 = terrainMesh.Positions[z + (x * TerrainSize) + 1];
                    Vector3 neighbour2 = terrainMesh.Positions[z + (x * TerrainSize) - TerrainSize];
                    Vector3 neighbour3 = terrainMesh.Positions[z + (x * TerrainSize) + TerrainSize];
                    Vector3 vec0 = neighbour0 - neighbour1;
                    Vector3 vec1 = neighbour2 - neighbour3;
                    terrainMesh.Normals[z + x * TerrainSize] = Vector3.Cross(vec0, vec1);
                }
            }

            //Updates Bordermesh
            for (int z = 0; z < TerrainSize; z++)
            {
                point = borderMesh.Positions[(z * 2) + 1];
                point.Y = (TerrainHeights[z] * HeightMultiplicator);
                borderMesh.Positions[(z * 2) + 1] = point;
            }

            for (int x = 0; x < TerrainSize; x++)
            {
                point = borderMesh.Positions[(x * 2) + 1 + (2 * TerrainSize)];
                point.Y = (TerrainHeights[x * TerrainSize + TerrainSize - 1] * HeightMultiplicator);
                borderMesh.Positions[(x * 2) + 1 + (2 * TerrainSize)] = point;
            }

            for (int z = TerrainSize; z > 0; z--)
            {
                point = borderMesh.Positions[(z * 2) - 1 + (4 * TerrainSize)];
                point.Y = (TerrainHeights[(TerrainSize * TerrainSize) - z] * HeightMultiplicator);
                borderMesh.Positions[(z * 2) - 1 + (4 * TerrainSize)] = point;
            }

            for (int x = TerrainSize; x > 0; x--)
            {
                point = borderMesh.Positions[(x * 2) - 1 + (6 * TerrainSize)];
                point.Y = (TerrainHeights[(TerrainSize - x) * TerrainSize] * HeightMultiplicator);
                borderMesh.Positions[(x * 2) - 1 + (6 * TerrainSize)] = point;
            }
            TerrainMeshBorderGeometry3D = borderMesh.ToMeshGeometry3D();
        }

        // Updates Terrain and Border Texture
        public void UpdateTextures(MemoryStream terrainMainColors, MemoryStream terrainBorderColors)
        {
            TerrainMeshMainTexture = new TextureModel(terrainMainColors);
            TerrainMeshBorderTexture = new TextureModel(terrainBorderColors);
            TerrainMeshMainMaterial.DiffuseMap = TerrainMeshMainTexture;
            TerrainMeshBorderMaterial.DiffuseMap = TerrainMeshBorderTexture;
        }

        // Updates Only Terrain Texture
        public void UpdateTexture(MemoryStream terrainMainColors)
        {
            TerrainMeshMainTexture = new TextureModel(terrainMainColors);
            TerrainMeshMainMaterial.DiffuseMap = TerrainMeshMainTexture;
        }

        #endregion

        #region VIEWPORT SETTINGS
        // Changes Viewport Quality and Updates View
        public void ChangeViewportQuality(int quality)
        {
            switch (quality)
            {
                // Low
                case 0:
                    Low = false;
                    Medium = true;
                    High = true;
                    Ultra = true;
                    FXAA = FXAALevel.None;
                    MSAA = MSAALevel.Disable;
                    SSAO = SSAOQuality.Low;
                    break;

                // Medium
                case 1:
                    Low = true;
                    Medium = false;
                    High = true;
                    Ultra = true;
                    FXAA = FXAALevel.Low;
                    MSAA = MSAALevel.Two;
                    SSAO = SSAOQuality.Low;
                    break;

                // High
                case 2:
                    Low = true;
                    Medium = true;
                    High = false;
                    Ultra = true;
                    FXAA = FXAALevel.High;
                    MSAA = MSAALevel.Four;
                    SSAO = SSAOQuality.High;
                    break;

                // Ultra
                case 3:
                    Low = true;
                    Medium = true;
                    High = true;
                    Ultra = false;
                    FXAA = FXAALevel.Ultra;
                    MSAA = MSAALevel.Maximum;
                    SSAO = SSAOQuality.High;
                    break;
            }
        }

        // Switchs between Orthographic and Perspective Camera
        public void ChangeViewMode(int viewMode)
        {
            switch (viewMode)
            {
                // Orthographic Mode
                case 0:
                    viewportCamera.SetOrthographicCam();
                    MinZoom = 8;
                    MaxZoom = 20;
                    break;

                // Perspective Mode
                case 1:
                    viewportCamera.SetPerspectiveCam();
                    MinZoom = 2;
                    MaxZoom = 5;
                    break;
            }
        }

        // Changes Materialproperties of Terrain
        public void ChangeMaterial(int material)
        {
            switch (material)
            {
                // Super Glossy
                case 0:
                    TerrainMeshMainMaterial.SpecularColor = new Color4(0.75f, 0.75f, 0.75f, 1.0f);
                    TerrainMeshBorderMaterial.SpecularColor = new Color4(0.75f, 0.75f, 0.75f, 1.0f);
                    TerrainMeshMainMaterial.SpecularShininess = 100;
                    TerrainMeshBorderMaterial.SpecularShininess = 100;
                    break;
                // Glossy
                case 1:
                    TerrainMeshMainMaterial.SpecularColor = new Color4(0.5f, 0.5f, 0.5f, 1.0f);
                    TerrainMeshBorderMaterial.SpecularColor = new Color4(0.5f, 0.5f, 0.5f, 1.0f);
                    TerrainMeshMainMaterial.SpecularShininess = 25;
                    TerrainMeshBorderMaterial.SpecularShininess = 25;
                    break;
                // Matte
                case 2:
                    TerrainMeshMainMaterial.SpecularColor = Color4.Black;
                    TerrainMeshBorderMaterial.SpecularColor = Color4.Black;
                    TerrainMeshMainMaterial.SpecularShininess = 0;
                    TerrainMeshBorderMaterial.SpecularShininess = 0;
                    break;
            }
        }

        // Switchs between Flat- and Phongshading
        public void ChangeShading(int shadingMode)
        {
            switch (shadingMode)
            {
                // Flat Shading
                case 0:

                    TerrainMeshMainMaterial.EnableFlatShading = true;
                    TerrainMeshBorderMaterial.EnableFlatShading = true;
                    TerrainMeshMainMaterial.DiffuseMapSampler = DefaultSamplers.PointSamplerWrap;
                    TerrainMeshBorderMaterial.DiffuseMapSampler = DefaultSamplers.PointSamplerWrap;
                    break;

                //Phong Shading
                case 1:
                    TerrainMeshMainMaterial.EnableFlatShading = false;
                    TerrainMeshBorderMaterial.EnableFlatShading = false;
                    TerrainMeshMainMaterial.DiffuseMapSampler = DefaultSamplers.LinearSamplerWrapAni16;
                    TerrainMeshBorderMaterial.DiffuseMapSampler = DefaultSamplers.LinearSamplerWrapAni16;
                    break;
            }
        }

        #endregion

        #region ICOMMANDS
        public bool CanExecute { get { return true; } }
        public ICommand ChangeViewModeCommand { get; private set; }
        public ICommand ChangeMaterialCommand { get; private set; }
        public ICommand ChangeViewportQualityCommand { get; private set; }
        public ICommand ChangeLightingModeCommand { get; private set; }
        public ICommand ChangeShadingCommand { get; private set; }
        #endregion

        #region IDISPOSABLE SUPPORT
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
    }
}
