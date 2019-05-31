using System;
using System.Windows.Input;
using TerrainGenerator.Commands;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Forms;

namespace TerrainGenerator.ViewModels
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        #region Attributes 
        private TerrainMesh _terrainMesh;
        private TerrainSettings _terrainSettings;
        private bool _res256;
        private bool _res512;
        private bool _res1024;
        private bool _res2048;
        #endregion

        #region Properties
        //MVVM 
        public TerrainMesh TerrainMeshProperty
        {
            get
            {
                return _terrainMesh;
            }
            set
            {
                _terrainMesh = value;
            }
        }
        public TerrainSettings TerrainSettingsProperty
        {
            get
            {
                return _terrainSettings;
            }
            set
            {
                _terrainSettings = value;
            }
        }

        //DetailResolution
        public bool Res256
        {
            get
            {
                return _res256;
            }
            set
            {
                _res256 = value;
                OnPropertyChanged("Res256");
            }
        }
        public bool Res512
        {
            get
            {
                return _res512;
            }
            set
            {
                _res512 = value;
                OnPropertyChanged("Res512");
            }
        }
        public bool Res1024
        {
            get
            {
                return _res1024;
            }
            set
            {
                _res1024 = value;
                OnPropertyChanged("Res1024");
            }
        }
        public bool Res2048
        {
            get
            {
                return _res2048;
            }
            set
            {
                _res2048 = value;
                OnPropertyChanged("Res2048");
            }
        }
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
        }

        private void InitLogic()
        {
            TerrainSettingsProperty = new TerrainSettings();
            TerrainMeshProperty = new TerrainMesh(TerrainSettingsProperty);
        }

        private void InitCommands()
        {
            NoiseCommand = new NoiseCommand(this);
            ErodeCommand = new ErodeCommand(this);
            ColorizeCommand = new ColorizeCommand(this);
            GenerateAllCommand = new GenerateAllCommand(this);
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

        public void NewTerrain()
        {
            _terrainSettings.TerrainSize = 512;
            UpdateDetailResolution(512);
            _terrainSettings.ResetHeights();
            _terrainMesh.UpdateMesh();
            InitProperties();
        }

        public void ChangeMesh()
        {
            TerrainMeshProperty.UpdateMesh();
            TerrainMeshProperty.GenerateDefaultTexture();
        }

        public void ChangeHeight()
        {
            TerrainMeshProperty.UpdateMesh();
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
            TerrainMeshProperty.InitMesh();
            GetPreviousState();
        }

        public void GetPreviousState()
        {
            if (_terrainSettings.isNoised &&  _terrainSettings.isEroded && _terrainSettings.isColored)
            {
                Noise();
                Erode();
                Colorize();
            }
            else if (_terrainSettings.isNoised && _terrainSettings.isEroded)
            {
                Noise();
                Erode();
            }
            else if (_terrainSettings.isNoised)
            {
                Noise();
            }
        }

        public void OpenWebsite()
        {
            System.Diagnostics.Process.Start("https://github.com/KonstantinTwardzik/TerrainGenerator");
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
            TerrainMeshProperty.UpdateTexture();
        }

        public void GenerateAll()
        {
            Noise();
            Erode();
            Colorize();
        }

        public void ExportMaps()
        {
            //SaveFileDialog saveFileDialog = new SaveFileDialog();
            //saveFileDialog.Filter = "*.PNG";
            //saveFileDialog.ShowDialog();
            
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
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

            }
        }
        #endregion
    }
}
