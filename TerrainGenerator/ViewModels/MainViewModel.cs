using System;
using System.Windows.Input;
using TerrainGenerator.Commands;
using System.Threading.Tasks;
using System.ComponentModel;

namespace TerrainGenerator.ViewModels
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        #region Attributes 
        private TerrainMesh _terrainMesh;
        private HeightLogic _heightLogic;
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
        public HeightLogic HeightLogicProperty
        {
            get
            {
                return _heightLogic;
            }
            set
            {
                _heightLogic = value;
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
            HeightLogicProperty = new HeightLogic();
            TerrainMeshProperty = new TerrainMesh(HeightLogicProperty);
        }

        private void InitCommands()
        {
            NoiseCommand = new NoiseCommand(this);
            ErodeCommand = new ErodeCommand(this);
            QuitCommand = new QuitCommand(this);
            NewCommand = new NewCommand(this);
            UpdateMeshCommand = new UpdateMeshCommand(this);
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
            //System.Windows.Forms.Application.Restart();
            //System.Windows.Application.Current.Shutdown();
            _heightLogic.TerrainSize = 512;
            UpdateDetailResolution(512);
            _heightLogic.ResetHeights();
            _terrainMesh.UpdateMesh();
            InitProperties();
        }

        public void UpdateMesh()
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

            HeightLogicProperty.ChangeDetailResolution(resolution);
            TerrainMeshProperty.InitMesh();

            if (_heightLogic.isNoised)
            {
                Noise();
            }
            if (_heightLogic.isEroded)
            {
                Erode();
            }

        }

        public void OpenWebsite()
        {
            System.Diagnostics.Process.Start("https://github.com/KonstantinTwardzik/TerrainGenerator");
        }

        public void Noise()
        {
            HeightLogicProperty.OpenSimplexNoise();
            TerrainMeshProperty.UpdateMesh();
            HeightLogicProperty.ConvertToBitmapSource();
        }

        public void Erode()
        {
            HeightLogicProperty.Erode();
            TerrainMeshProperty.UpdateMesh();
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
