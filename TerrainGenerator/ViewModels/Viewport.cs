using HelixToolkit.Wpf.SharpDX;
using Media3D = System.Windows.Media.Media3D;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Vector3D = System.Windows.Media.Media3D.Vector3D;
using Transform3D = System.Windows.Media.Media3D.Transform3D;
using Color = System.Windows.Media.Color;
using Vector3 = SharpDX.Vector3;
using Colors = System.Windows.Media.Colors;
using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Topographer3D.ViewModels
{
    internal class Viewport : ObservableObject, IDisposable
    {
        #region Properties
        public const string Orthographic = "Orthographic Camera";

        public const string Perspective = "Perspective Camera";

        private string cameraModel;

        private Camera camera;

        private string subTitle;

        private string title;

        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                SetValue(ref title, value, "Title");
            }
        }

        public string SubTitle
        {
            get
            {
                return subTitle;
            }
            set
            {
                SetValue(ref subTitle, value, "SubTitle");
            }
        }

        public List<string> CameraModelCollection { get; private set; }

        public string CameraModel
        {
            get
            {
                return cameraModel;
            }
            set
            {
                if (SetValue(ref cameraModel, value, "CameraModel"))
                {
                    OnCameraModelChanged();
                }
            }
        }

        public Camera Camera
        {
            get
            {
                return camera;
            }

            protected set
            {
                SetValue(ref camera, value, "Camera");
                CameraModel = value is PerspectiveCamera
                                       ? Perspective
                                       : value is OrthographicCamera ? Orthographic : null;
            }
        }
        private IEffectsManager effectsManager;
        public IEffectsManager EffectsManager
        {
            get { return effectsManager; }
            protected set
            {
                SetValue(ref effectsManager, value);
            }
        }

        protected OrthographicCamera defaultOrthographicCamera = new OrthographicCamera { Position = new System.Windows.Media.Media3D.Point3D(0, 0, 5), LookDirection = new System.Windows.Media.Media3D.Vector3D(-0, -0, -5), UpDirection = new System.Windows.Media.Media3D.Vector3D(0, 1, 0), NearPlaneDistance = 1, FarPlaneDistance = 100 };

        protected PerspectiveCamera defaultPerspectiveCamera = new PerspectiveCamera { Position = new System.Windows.Media.Media3D.Point3D(0, 0, 5), LookDirection = new System.Windows.Media.Media3D.Vector3D(-0, -0, -5), UpDirection = new System.Windows.Media.Media3D.Vector3D(0, 1, 0), NearPlaneDistance = 0.5, FarPlaneDistance = 150 };

        public event EventHandler CameraModelChanged;
        public double GeneralHeight { get; set; }
        public Vector3D UpDirection { get; set; } = new Vector3D(0, 1, 0);
        public Vector3D DirectionalLightDirection { get; private set; }
        public Color DirectionalLightColor { get; private set; }
        public Color AmbientLightColor { get; private set; }
        public MeshGeometry3D TerrainMeshMainGeometry3DProperty { get; private set; }
        public MeshGeometry3D TerrainMeshBorderGeometry3DProperty { get; private set; }
        public PhongMaterial TerrainMeshMainMaterial { get; private set; }
        public PhongMaterial TerrainMeshBorderMaterial { get; private set; }
        public Transform3D TerrainMeshMainTransform { get; private set; }
        public Transform3D TerrainMeshBorderTransform { get; private set; }
        #endregion

        #region Initialization
        public Viewport(TerrainSettings heightLogic)
        {           
            // camera models
            CameraModelCollection = new List<string>()
            {
                Orthographic,
                Perspective,
            };

            // on camera changed callback
            CameraModelChanged += (s, e) =>
            {
                if (cameraModel == Orthographic)
                {
                    if (!(Camera is OrthographicCamera))
                        Camera = defaultOrthographicCamera;
                }
                else if (cameraModel == Perspective)
                {
                    if (!(Camera is PerspectiveCamera))
                        Camera = defaultPerspectiveCamera;
                }
                else
                {
                    throw new HelixToolkitException("Camera Model Error.");
                }
            };

            // default camera model
            CameraModel = Perspective;

            Title = "Demo (HelixToolkitDX)";
            SubTitle = "Default Base View Model";

            EffectsManager = new DefaultEffectsManager();



            //_terrainSettings = heightLogic;
            //_terrainMeshGeometry3D = new MeshGeometry3D();
            //_terrainImageBrush = new ImageBrush();
            //_borderMeshGeometry3D = new MeshGeometry3D();
            //_borderImageBrush = new ImageBrush();
            //_generalHeight = 0.75;
            //InitMesh();

            // camera setup
            Camera = new PerspectiveCamera
            {
                Position = new Point3D(3, 3, 5),
                LookDirection = new Vector3D(-3, -3, -5),
                UpDirection = new Vector3D(0, 1, 0),
                FarPlaneDistance = 5000000
            };

            // setup lighting            
            AmbientLightColor = Colors.DimGray;
            DirectionalLightColor = Colors.White;
            DirectionalLightDirection = new Vector3D(-2, -5, -2);

            var b1 = new MeshBuilder();
            b1.AddSphere(new Vector3(0, 0, 0), 0.5);
            b1.AddBox(new Vector3(0, 0, 0), 1, 0.5, 2, BoxFaces.All);

            TerrainMeshMainGeometry3DProperty = b1.ToMeshGeometry3D();
            TerrainMeshMainMaterial = PhongMaterials.Gray;
            TerrainMeshMainTransform = new Media3D.TranslateTransform3D(0, 0, 0);
        }

        protected virtual void OnCameraModelChanged()
        {
            var eh = CameraModelChanged;
            if (eh != null)
            {
                eh(this, new EventArgs());
            }
        }

        public static MemoryStream LoadFileToMemory(string filePath)
        {
            using (var file = new FileStream(filePath, FileMode.Open))
            {
                var memory = new MemoryStream();
                file.CopyTo(memory);
                return memory;
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

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

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~Viewport()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        //public void InitMesh()
        //{
        //    _terrainMeshGeometry3D.Positions.Clear();
        //    _terrainMeshGeometry3D.TriangleIndices.Clear();
        //    _terrainMeshGeometry3D.TextureCoordinates.Clear();
        //    _borderMeshGeometry3D.Positions.Clear();
        //    _borderMeshGeometry3D.TriangleIndices.Clear();
        //    _borderMeshGeometry3D.TextureCoordinates.Clear();

        //    GenerateTerrainPositions();
        //    GenerateTerrainTriangleIndices();
        //    GenerateTerrainUVCoordinates();
        //    GenerateDefaultTexture();
        //    GenerateBorderPositions();
        //    GenerateBorderTriangleIndices();
        //    GenerateBorderUVCoordinates();
        //}
        #endregion

        //#region Generating 3D Model
        //private void GenerateTerrainPositions()
        //{
        //    Point3D point = new Point3D();
        //    // Terrain Points
        //    for (int x = 0; x < _terrainSettings.TerrainSize; x++)
        //    {
        //        for (int z = 0; z < _terrainSettings.TerrainSize; z++)
        //        {
        //            for (int i = 0; i < 3; i++)
        //            {
        //                if (i == 0)
        //                    point.X = ((double)x / ((double)_terrainSettings.TerrainSize - 1) - 0.5) * 2;
        //                if (i == 1)
        //                    point.Y = 0;
        //                if (i == 2)
        //                    point.Z = ((double)z / ((double)_terrainSettings.TerrainSize - 1) - 0.5) * 2;
        //            }
        //            TerrainMeshMainGeometry3DProperty.Positions.Add(point);
        //        }
        //    }
        //}

        //private void GenerateTerrainTriangleIndices()
        //{
        //    var value = 0;
        //    // Terrain Indices
        //    for (int i = 0; i < _terrainSettings.TerrainSize * _terrainSettings.TerrainSize - _terrainSettings.TerrainSize; i++)
        //    {
        //        for (int trianglePoint = 0; trianglePoint < 3; trianglePoint++)
        //        {
        //            if (i % _terrainSettings.TerrainSize == 0)
        //            {
        //                break;
        //            }
        //            if (trianglePoint == 0)
        //            {
        //                value = i;
        //            }
        //            else if (trianglePoint == 1)
        //            {
        //                value = i + _terrainSettings.TerrainSize;

        //            }
        //            else if (trianglePoint == 2)
        //            {
        //                value = i + _terrainSettings.TerrainSize - 1;
        //            }
        //            TerrainMeshMainGeometry3DProperty.TriangleIndices.Add(value);
        //        }
        //        for (int trianglePoint = 0; trianglePoint < 3; trianglePoint++)
        //        {
        //            if (i > 0 && ((i + 1) % _terrainSettings.TerrainSize) == 0)
        //            {
        //                break;
        //            }
        //            if (trianglePoint == 0)
        //            {
        //                value = i;
        //            }
        //            else if (trianglePoint == 1)
        //            {
        //                value = i + 1;

        //            }
        //            else if (trianglePoint == 2)
        //            {
        //                value = i + _terrainSettings.TerrainSize;
        //            }
        //            TerrainMeshMainGeometry3DProperty.TriangleIndices.Add(value);
        //        }
        //    }
        //}

        //private void GenerateTerrainUVCoordinates()
        //{
        //    Point point = new Point();
        //    for (int x = 0; x < _terrainSettings.TerrainSize; x++)
        //    {
        //        for (int z = 0; z < _terrainSettings.TerrainSize; z++)
        //        {
        //            point.X = (double)x / (double)_terrainSettings.TerrainSize;
        //            point.Y = (double)z / (double)_terrainSettings.TerrainSize;
        //            TerrainMeshMainGeometry3DProperty.TextureCoordinates.Add(point);
        //        }
        //    }
        //}

        //private void GenerateBorderPositions()
        //{
        //    Point3D point = new Point3D(-1, 0, 0);

        //    // Border Points
        //    for (int z = 0; z < _terrainSettings.TerrainSize; z++)
        //    {
        //        point.Z = ((double)z / ((double)_terrainSettings.TerrainSize - 1) - 0.5) * 2;
        //        TerrainMeshBorderGeometry3DProperty.Positions.Add(point);
        //        TerrainMeshBorderGeometry3DProperty.Positions.Add(point);
        //    }

        //    point.Z = 1;
        //    for (int x = 0; x < _terrainSettings.TerrainSize; x++)
        //    {
        //        point.X = ((double)x / ((double)_terrainSettings.TerrainSize - 1) - 0.5) * 2;
        //        TerrainMeshBorderGeometry3DProperty.Positions.Add(point);
        //        TerrainMeshBorderGeometry3DProperty.Positions.Add(point);
        //    }

        //    point.X = 1;
        //    for (int z = _terrainSettings.TerrainSize - 1; z >= 0; z--)
        //    {
        //        point.Z = ((double)z / ((double)_terrainSettings.TerrainSize - 1) - 0.5) * 2;
        //        TerrainMeshBorderGeometry3DProperty.Positions.Add(point);
        //        TerrainMeshBorderGeometry3DProperty.Positions.Add(point);
        //    }

        //    point.Z = -1;
        //    for (int x = _terrainSettings.TerrainSize - 1; x >= 0; x--)
        //    {
        //        point.X = ((double)x / ((double)_terrainSettings.TerrainSize - 1) - 0.5) * 2;
        //        TerrainMeshBorderGeometry3DProperty.Positions.Add(point);
        //        TerrainMeshBorderGeometry3DProperty.Positions.Add(point);
        //    }
        //}

        //private void GenerateBorderTriangleIndices()
        //{
        //    int value = 0;
        //    // Border Indices
        //    for (int i = 0; i < _borderMeshGeometry3D.Positions.Count; i++)
        //    {

        //        if (i % 2 == 0)
        //        {
        //            for (int trianglePoint = 0; trianglePoint < 3; trianglePoint++)
        //            {
        //                if (trianglePoint == 0)
        //                    value = i + 2;
        //                if (trianglePoint == 1)
        //                    value = i + 1;
        //                if (trianglePoint == 2)
        //                    value = i;
        //                TerrainMeshBorderGeometry3DProperty.TriangleIndices.Add(value);
        //            }
        //        }
        //        else
        //        {
        //            for (int trianglePoint = 0; trianglePoint < 3; trianglePoint++)
        //            {
        //                if (trianglePoint == 0)
        //                    value = i + 1;
        //                if (trianglePoint == 1)
        //                    value = i + 2;
        //                if (trianglePoint == 2)
        //                    value = i;
        //                TerrainMeshBorderGeometry3DProperty.TriangleIndices.Add(value);
        //            }
        //        }

        //    }
        //}

        //private void GenerateBorderUVCoordinates()
        //{
        //    Point point = new Point();
        //    //Border
        //    for (int x = 0; x < TerrainMeshBorderGeometry3DProperty.Positions.Count; x++)
        //    {
        //        if (x % 2 == 0)
        //        {
        //            point.X = 0.0;
        //            point.Y = 0.0;
        //        }
        //        else
        //        {
        //            point.Y = 1.0;
        //            point.X = 1.0;
        //        }
        //        TerrainMeshBorderGeometry3DProperty.TextureCoordinates.Add(point);
        //    }
        //}

        //public void GenerateDefaultTexture()
        //{

        //    PixelFormat pixelFormat = PixelFormats.Bgr24;
        //    int rawStride = (pixelFormat.BitsPerPixel + 7) / 8;
        //    byte[] rawImage = new byte[rawStride];

        //    for (int i = 0; i < rawStride; i++)
        //    {
        //        rawImage[i] = 255;
        //    }

        //    BitmapSource bitmap = BitmapSource.Create(1, 1, 96, 96, pixelFormat, null, rawImage, rawStride);

        //    PngBitmapEncoder encoder = new PngBitmapEncoder();
        //    MemoryStream memoryStream = new MemoryStream();
        //    BitmapImage defaultTexture = new BitmapImage();

        //    encoder.Frames.Add(BitmapFrame.Create(bitmap));
        //    encoder.Save(memoryStream);

        //    memoryStream.Position = 0;
        //    defaultTexture.BeginInit();
        //    defaultTexture.StreamSource = new MemoryStream(memoryStream.ToArray());
        //    defaultTexture.EndInit();
        //    defaultTexture.Freeze();

        //    _terrainImageBrush.ImageSource = defaultTexture;
        //    _borderImageBrush.ImageSource = defaultTexture;
        //}
        //#endregion

        //#region Updating 3D Model
        //public void UpdateMesh()
        //{
        //    Point3D point = new Point3D();

        //    //Terrain
        //    for (int x = 0; x < _terrainSettings.TerrainSize; x++)
        //    {
        //        for (int z = 0; z < _terrainSettings.TerrainSize; z++)
        //        {
        //            point = TerrainMeshMainGeometry3DProperty.Positions[x + z * _terrainSettings.TerrainSize];
        //            point.Y = _terrainSettings.TerrainPoints[x + z * _terrainSettings.TerrainSize] * GeneralHeight;
        //            TerrainMeshMainGeometry3DProperty.Positions[x + z * _terrainSettings.TerrainSize] = point;
        //        }
        //    }

        //    //Border
        //    for (int z = 0; z < _terrainSettings.TerrainSize; z++)
        //    {
        //        point = TerrainMeshBorderGeometry3DProperty.Positions[(z * 2) + 1];
        //        point.Y = _terrainSettings.TerrainPoints[z] * GeneralHeight;
        //        TerrainMeshBorderGeometry3DProperty.Positions[(z * 2) + 1] = point;
        //    }

        //    for (int x = 0; x < _terrainSettings.TerrainSize; x++)
        //    {
        //        point = TerrainMeshBorderGeometry3DProperty.Positions[(x * 2) + 1 + (2 * _terrainSettings.TerrainSize)];
        //        point.Y = _terrainSettings.TerrainPoints[x * _terrainSettings.TerrainSize + _terrainSettings.TerrainSize - 1] * GeneralHeight;
        //        TerrainMeshBorderGeometry3DProperty.Positions[(x * 2) + 1 + (2 * _terrainSettings.TerrainSize)] = point;
        //    }

        //    for (int z = _terrainSettings.TerrainSize; z > 0; z--)
        //    {
        //        point = TerrainMeshBorderGeometry3DProperty.Positions[(z * 2) - 1 + (4 * _terrainSettings.TerrainSize)];
        //        point.Y = _terrainSettings.TerrainPoints[(_terrainSettings.TerrainSize * _terrainSettings.TerrainSize) - z] * GeneralHeight;
        //        TerrainMeshBorderGeometry3DProperty.Positions[(z * 2) - 1 + (4 * _terrainSettings.TerrainSize)] = point;
        //    }

        //    for (int x = _terrainSettings.TerrainSize; x > 0; x--)
        //    {
        //        point = TerrainMeshBorderGeometry3DProperty.Positions[(x * 2) - 1 + (6 * _terrainSettings.TerrainSize)];
        //        point.Y = _terrainSettings.TerrainPoints[(_terrainSettings.TerrainSize - x) * _terrainSettings.TerrainSize] * GeneralHeight;
        //        TerrainMeshBorderGeometry3DProperty.Positions[(x * 2) - 1 + (6 * _terrainSettings.TerrainSize)] = point;
        //    }



        //}
        //public void UpdateTexture()
        //{
        //    _terrainImageBrush.ImageSource = _terrainSettings.ColorMapImage;
        //    _borderImageBrush.ImageSource = _terrainSettings.BorderMapImage;
        //}
        //#endregion
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
