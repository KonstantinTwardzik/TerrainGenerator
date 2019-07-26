using System;
using System.Windows.Input;
using Topographer3D.Commands;
using System.Windows;
using System.Windows.Forms;
using Topographer3D.Utilities;

namespace Topographer3D.ViewModels
{
    internal class MainViewModel : ObservableObject
    {
        #region Attributes 
        private string maxPath;
        private string maxFullPath;
        private bool dragable;
        private double oldLeft;
        private double oldTop;
        private double oldWidth;
        private double oldHeight;

        #endregion

        #region Properties
        //MVVM 
        public Viewport Viewport { get; private set; }
        public TerrainEngine TerrainEngine { get; private set; }
        public ViewportCamera ViewportCamera { get; private set; }
        public LayerManager LayerManager { get; private set; }

        //DetailResolution
        public bool Res16 { get; private set; }
        public bool Res32 { get; private set; }
        public bool Res64 { get; private set; }
        public bool Res128 { get; private set; }
        public bool Res512 { get; private set; }
        public bool Res1024 { get; private set; }
        public bool Res2048 { get; private set; }
        public bool Res4096 { get; private set; }

        //HeightMultiplicator
        public bool Height25 { get; private set; }
        public bool Height50 { get; private set; }
        public bool Height75 { get; private set; }
        public bool Height100 { get; private set; }
        public bool Height125 { get; private set; }
        public bool Height150 { get; private set; }

        public string MaxImagePath { get; private set; }
        #endregion

        #region Initialization
        public MainViewModel()
        {
            InitLogic();
            InitCommands();
            InitProperties();
        }

        private void InitLogic()
        {
            TerrainEngine = new TerrainEngine();
            ViewportCamera = new ViewportCamera();
            Viewport = new Viewport(TerrainEngine, ViewportCamera);
            LayerManager = new LayerManager(TerrainEngine);
            TerrainEngine.InitLogic(LayerManager, this);
        }

        private void InitProperties()
        {
            // Window Size values
            dragable = true;
            oldLeft = App.Current.MainWindow.Left;
            oldTop = App.Current.MainWindow.Top;
            oldWidth = App.Current.MainWindow.Width;
            oldHeight = App.Current.MainWindow.Height;

            Res16 = true;
            Res32 = true;
            Res64 = true;
            Res128 = true;
            Res512 = false;
            Res1024 = true;
            Res2048 = true;
            Res4096 = true;

            Height25 = true;
            Height50 = true;
            Height75 = true;
            Height100 = false;
            Height125 = true;
            Height150 = true;

            maxPath = "pack://application:,,,/Topographer3D;component/Assets/Icons/Maximize.png";
            maxFullPath = "pack://application:,,,/Topographer3D;component/Assets/Icons/MaximizeFullscreen.png";
            MaxImagePath = maxPath;
        }

        private void InitCommands()
        {
            DragCommand = new DragCommand(this);
            MinimizeCommand = new MinimizeCommand(this);
            MaximizeCommand = new MaximizeCommand(this);
            QuitCommand = new QuitCommand(this);
            NewCommand = new NewCommand(this);
            ExportCommand = new ExportCommand(this);
            UpdateMeshCommand = new UpdateMeshCommand(this);
            ChangeHeightCommand = new ChangeHeightCommand(this);
            DetailResolutionCommand = new DetailResolutionCommand(this);
            HelpCommand = new HelpCommand(this);

        }
        #endregion

        #region Button Handling
        public void QuitApplication()
        {
            System.Windows.Application.Current.Shutdown();
        }

        public void DragWindow()
        {
            if(dragable)
            {
                App.Current.MainWindow.DragMove();
            }
        }

        public void MinimizeApplication()
        {
            App.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        public void MaximizeApplication()
        {
            if (!dragable)
            {
                MaxImagePath = maxPath;
                dragable = true;

                App.Current.MainWindow.Left = oldLeft;
                App.Current.MainWindow.Top = oldTop;
                App.Current.MainWindow.Width = oldWidth;
                App.Current.MainWindow.Height = oldHeight;
                App.Current.MainWindow.ResizeMode = ResizeMode.CanResizeWithGrip;
            }
            else
            {
                MaxImagePath = maxFullPath;
                dragable = false;

                //safe old values
                oldLeft = App.Current.MainWindow.Left;
                oldTop = App.Current.MainWindow.Top;
                oldWidth = App.Current.MainWindow.Width;
                oldHeight = App.Current.MainWindow.Height;


                // set new values
                App.Current.MainWindow.Left = 0;
                App.Current.MainWindow.Top = 0;
                App.Current.MainWindow.Width = Screen.PrimaryScreen.WorkingArea.Width;
                App.Current.MainWindow.Height = Screen.PrimaryScreen.WorkingArea.Height;
                App.Current.MainWindow.ResizeMode = ResizeMode.NoResize;
            }

        }

        public void NewTerrain()
        {
            TerrainEngine.TerrainSize = 512;
            UpdateDetailResolution(512);
            TerrainEngine.ResetHeights();
            ChangeMesh();
            InitProperties();
            TerrainEngine.InitProperties();
        }

        public void ChangeMesh()
        {
            Viewport.UpdateMesh();
            Viewport.GenerateDefaultTexture();
        }

        public void ChangeHeight(float heightMulitplicator)
        {
            switch (heightMulitplicator)
            {
                case 25:
                    Height25 = false;
                    Height50 = true;
                    Height75 = true;
                    Height100 = true;
                    Height125 = true;
                    Height150 = true;
                    break;
                case 50:
                    Height25 = true;
                    Height50 = false;
                    Height75 = true;
                    Height100 = true;
                    Height125 = true;
                    Height150 = true;
                    break;
                case 75:
                    Height25 = true;
                    Height50 = true;
                    Height75 = false;
                    Height100 = true;
                    Height125 = true;
                    Height150 = true;
                    break;
                case 100:
                    Height25 = true;
                    Height50 = true;
                    Height75 = true;
                    Height100 = false;
                    Height125 = true;
                    Height150 = true;
                    break;
                case 125:
                    Height25 = true;
                    Height50 = true;
                    Height75 = true;
                    Height100 = true;
                    Height125 = false;
                    Height150 = true;
                    break;
                case 150:
                    Height25 = true;
                    Height50 = true;
                    Height75 = true;
                    Height100 = true;
                    Height125 = true;
                    Height150 = false;
                    break;
            }

            Viewport.HeightMultiplicator = heightMulitplicator / 100.0f;
            Viewport.UpdateMesh();
        }

        public void UpdateDetailResolution(int resolution)
        {
            switch (resolution)
            {
                case 16:
                    Res16 = false;
                    Res32 = true;
                    Res64 = true;
                    Res128 = true;
                    Res512 = true;
                    Res1024 = true;
                    Res2048 = true;
                    Res4096 = true;
                    break;
                case 32:
                    Res16 = true;
                    Res32 = false;
                    Res64 = true;
                    Res128 = true;
                    Res512 = true;
                    Res1024 = true;
                    Res2048 = true;
                    Res4096 = true;
                    break;
                case 64:
                    Res16 = true;
                    Res32 = true;
                    Res64 = false;
                    Res128 = true;
                    Res512 = true;
                    Res1024 = true;
                    Res2048 = true;
                    Res4096 = true;
                    break;
                case 128:
                    Res16 = true;
                    Res32 = true;
                    Res64 = true;
                    Res128 = false;
                    Res512 = true;
                    Res1024 = true;
                    Res2048 = true;
                    Res4096 = true;
                    break;
                case 512:
                    Res16 = true;
                    Res32 = true;
                    Res64 = true;
                    Res128 = true;
                    Res512 = false;
                    Res1024 = true;
                    Res2048 = true;
                    Res4096 = true;
                    break;
                case 1024:
                    Res16 = true;
                    Res32 = true;
                    Res64 = true;
                    Res128 = true;
                    Res512 = true;
                    Res1024 = false;
                    Res2048 = true;
                    Res4096 = true;
                    break;
                case 2048:
                    Res16 = true;
                    Res32 = true;
                    Res64 = true;
                    Res128 = true;
                    Res512 = true;
                    Res1024 = true;
                    Res2048 = false;
                    Res4096 = true;
                    break;
                case 4096:
                    Res16 = true;
                    Res32 = true;
                    Res64 = true;
                    Res128 = true;
                    Res512 = true;
                    Res1024 = true;
                    Res2048 = true;
                    Res4096 = false;
                    break;
            }

            TerrainEngine.ChangeDetailResolution(resolution);
            Viewport.InitMesh();
            //GetPreviousState();
        }

        //public void GetPreviousState()
        //{
        //    if (TerrainEngine.isNoised && TerrainEngine.isEroded && TerrainEngine.isColored)
        //    {
        //        Noise();
        //        Erode();
        //        Colorize();
        //    }
        //    else if (TerrainEngine.isNoised && TerrainEngine.isEroded)
        //    {
        //        Noise();
        //        Erode();
        //    }
        //    else if (TerrainEngine.isNoised)
        //    {
        //        Noise();
        //    }
        //}

        public void OpenWebsite()
        {
            System.Diagnostics.Process.Start("https://github.com/KonstantinTwardzik/Topographer3D/wiki");
        }

        //public void Erode()
        //{
        //    TerrainEngine.Erode();
        //    ChangeMesh();
        //}

        //public void Colorize()
        //{
        //    TerrainEngine.Colorize();
        //    Viewport.UpdateTexture();
        //}

        //public void GenerateAll()
        //{
        //    Noise();
        //    Erode();
        //    Colorize();
        //}

        public void ExportMaps()
        {
            TerrainEngine.CreateHeightMap();
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "png (.png) | *.png",
                FilterIndex = 1
            };
            Nullable<bool> result = saveFileDialog.ShowDialog();
            if (result == true)
            {
                TerrainEngine.ExportMaps(saveFileDialog.FileName);
            }


        }

        public static Point GetMousePosition()
        {
            System.Drawing.Point point = Control.MousePosition;
            return new Point(point.X, point.Y);
        }
        #endregion

        #region ICommands
        public bool CanExecute { get { return true; } }
        public ICommand DragCommand { get; private set; }
        public ICommand MinimizeCommand { get; private set; }
        public ICommand MaximizeCommand { get; private set; }
        public ICommand QuitCommand { get; private set; }
        public ICommand NewCommand { get; private set; }
        public ICommand UpdateMeshCommand { get; private set; }
        public ICommand ChangeHeightCommand { get; private set; }
        public ICommand DetailResolutionCommand { get; private set; }
        public ICommand HelpCommand { get; private set; }
        public ICommand ExportCommand { get; private set; }
        #endregion
    }
}
