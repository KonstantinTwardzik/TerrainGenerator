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
        private bool _res256;
        private bool _res512;
        private bool _res1024;
        private bool _res2048;

        #endregion

        #region Properties
        public TerrainMesh TerrainMeshProperty
        {
            get; set;
        }

        public HeightLogic HeightLogicProperty
        {
            get; set;
        }

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

        public float PerlinScale
        {
            get; set;
        }

        public int PerlinOctaves
        {
            get; set;
        }

        public float PerlinScaleX
        {
            get; set;
        }

        public float PerlinScaleZ
        {
            get; set;
        }

        public int PerlinSeed
        {
            get; set;
        }
        #endregion

        public MainViewModel()
        {
            InitLogic();
            InitCommands();
            InitProperties();

        }

        private void InitProperties()
        {
            _res256 = true;
            _res512 = false;
            _res1024 = true;
            _res2048 = true;

            PerlinScale = 0.5f;
            PerlinOctaves = 3;
            PerlinScaleX = 0.5f;
            PerlinScaleZ = 0.5f;
            PerlinSeed = 500;
        }

        private void InitLogic()
        {
            HeightLogicProperty = new HeightLogic(512);
            TerrainMeshProperty = new TerrainMesh(HeightLogicProperty);
        }

        private void InitCommands()
        {
            CalculateNoiseCommand = new CalculateNoiseCommand(this);
            QuitCommand = new QuitCommand(this);
            NewCommand = new NewCommand(this);
            UpdateMeshCommand = new UpdateMeshCommand(this);
            DetailResolutionCommand = new DetailResolutionCommand(this);
        }

        #region Button Handling
        public bool CanExecute
        {
            get { return true; }
        }

        public ICommand CalculateNoiseCommand
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

        public void QuitApplication()
        {
            System.Windows.Application.Current.Shutdown();
        }

        public void NewTerrain()
        {
            System.Windows.Forms.Application.Restart();
            System.Windows.Application.Current.Shutdown();
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
            HeightLogicProperty.InitHeights(resolution);
            TerrainMeshProperty.InitMesh();
        }

        public void CalculateNoise()
        {
            HeightLogicProperty.OpenSimplexNoise(PerlinScale, PerlinOctaves, PerlinScaleX, PerlinScaleZ, PerlinSeed);
            TerrainMeshProperty.UpdateMesh();
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
