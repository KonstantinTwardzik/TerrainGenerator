using System;
using System.Windows.Input;
using Topographer3D.Commands;
using System.ComponentModel;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf.SharpDX;
using System.Windows.Forms;

namespace Topographer3D.ViewModels
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        #region Attributes 
        private string _maxPath;
        private string _maxFullPath;
        #endregion

        #region Properties
        //MVVM 
        public Viewport ViewportProperty { get; set; }
        public TerrainSettings TerrainSettingsProperty { get; set; }

        //DetailResolution
        public bool Res256 { get; set; }
        public bool Res512 { get; set; }
        public bool Res1024 { get; set; }
        public bool Res2048 { get; set; }
        public string MaxImagePath { get; set; }
        #endregion

        #region Initialization
        public MainViewModel()
        {
            InitLogic();
            InitCommands();
            InitProperties();
        }

        private void InitProperties()
        {
            Res256 = true;
            Res512 = false;
            Res1024 = true;
            Res2048 = true;

            _maxPath = "pack://application:,,,/Topographer3D;component/Assets/Icons/Maximize.png";
            _maxFullPath = "pack://application:,,,/Topographer3D;component/Assets/Icons/MaximizeFullscreen.png";
            MaxImagePath = _maxPath;
        }

        private void InitLogic()
        {
            TerrainSettingsProperty = new TerrainSettings();
            ViewportProperty = new Viewport(TerrainSettingsProperty);
        }

        private void InitCommands()
        {
            NoiseCommand = new NoiseCommand(this);
            ErodeCommand = new ErodeCommand(this);
            ColorizeCommand = new ColorizeCommand(this);
            GenerateAllCommand = new GenerateAllCommand(this);
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
            App.Current.MainWindow.DragMove();
        }

        public void MinimizeApplication()
        {
            App.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        public void MaximizeApplication()
        {
            if (App.Current.MainWindow.WindowState == WindowState.Maximized)
            {
                MaxImagePath = _maxPath;
                App.Current.MainWindow.WindowState = WindowState.Normal;
            }
            else if (App.Current.MainWindow.WindowState == WindowState.Normal)
            {
                MaxImagePath = _maxFullPath;
                App.Current.MainWindow.WindowState = WindowState.Maximized;
            }
        }

        public void NewTerrain()
        {
            TerrainSettingsProperty.TerrainSize = 512;
            UpdateDetailResolution(512);
            TerrainSettingsProperty.ResetHeights();
            ChangeMesh();
            InitProperties();
            TerrainSettingsProperty.InitProperties();
        }

        public void ChangeMesh()
        {
            //ViewportProperty.UpdateMesh();
            //ViewportProperty.GenerateDefaultTexture();
        }

        public void ChangeHeight()
        {
            //ViewportProperty.UpdateMesh();
        }

        public void UpdateDetailResolution(int resolution)
        {
            switch (resolution)
            {
                case 256:
                    Res256 = false;
                    Res512 = true;
                    Res1024 = true;
                    Res2048 = true;
                    break;
                case 512:
                    Res256 = true;
                    Res512 = false;
                    Res1024 = true;
                    Res2048 = true;
                    break;
                case 1024:
                    Res256 = true;
                    Res512 = true;
                    Res1024 = false;
                    Res2048 = true;
                    break;
                case 2048:
                    Res256 = true;
                    Res512 = true;
                    Res1024 = true;
                    Res2048 = false;
                    break;
            }

            TerrainSettingsProperty.ChangeDetailResolution(resolution);
            //ViewportProperty.InitMesh();
            GetPreviousState();
        }

        public void GetPreviousState()
        {
            if (TerrainSettingsProperty.isNoised && TerrainSettingsProperty.isEroded && TerrainSettingsProperty.isColored)
            {
                Noise();
                Erode();
                Colorize();
            }
            else if (TerrainSettingsProperty.isNoised && TerrainSettingsProperty.isEroded)
            {
                Noise();
                Erode();
            }
            else if (TerrainSettingsProperty.isNoised)
            {
                Noise();
            }
        }

        public void OpenWebsite()
        {
            System.Diagnostics.Process.Start("https://github.com/KonstantinTwardzik/Topographer3D/wiki");
        }

        public void Noise()
        {
            TerrainSettingsProperty.OpenSimplexNoise();
            ChangeMesh();
        }

        public void Erode()
        {
            TerrainSettingsProperty.Erode();
            ChangeMesh();
        }

        public void Colorize()
        {
            TerrainSettingsProperty.Colorize();
            //ViewportProperty.UpdateTexture();
        }

        public void GenerateAll()
        {
            Noise();
            Erode();
            Colorize();
        }

        public void ExportMaps()
        {
            TerrainSettingsProperty.CreateHeightMap();
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "png (.png) | *.png",
                FilterIndex = 1
            };
            Nullable<bool> result = saveFileDialog.ShowDialog();
            if (result == true)
            {
                TerrainSettingsProperty.ExportMaps(saveFileDialog.FileName);
            }


        }

        public static Point GetMousePosition()
        {
            System.Drawing.Point point = Control.MousePosition;
            return new Point(point.X, point.Y);
        }
        #endregion

        #region ICommands
        public bool CanExecute
        {
            get { return true; }
        }

        public ICommand NoiseCommand
        {
            get;
            private set;
        }

        public ICommand ErodeCommand
        {
            get;
            private set;
        }

        public ICommand ColorizeCommand
        {
            get;
            private set;
        }

        public ICommand GenerateAllCommand
        {
            get;
            private set;
        }

        public ICommand DragCommand
        {
            get;
            private set;
        }

        public ICommand MinimizeCommand
        {
            get;
            private set;
        }

        public ICommand MaximizeCommand
        {
            get;
            private set;
        }

        public ICommand QuitCommand
        {
            get;
            private set;
        }

        public ICommand NewCommand
        {
            get;
            private set;
        }

        public ICommand UpdateMeshCommand
        {
            get;
            private set;
        }

        public ICommand ChangeHeightCommand
        {
            get;
            private set;
        }

        public ICommand DetailResolutionCommand
        {
            get;
            private set;
        }

        public ICommand HelpCommand
        {
            get;
            private set;
        }

        public ICommand ExportCommand
        {
            get;
            private set;
        }
        #endregion

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        //private void OnPropertyChanged(string propertyName)
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}
        #endregion
    }
}
