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

        //Open Simplex Noise
        private double _osnScale;
        private int _osnOctaves;
        private double _osnOctaveWeight;
        private double _osnScaleX;
        private double _osnScaleZ;
        private int _osnSeed;

        //Hydraulic Erosion
        private int _heIterations;


        #endregion

        #region Properties
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

        public double OSNScale
        {
            get
            {
                return _osnScale;
            }
            set
            {
                _osnScale = value;
                OnPropertyChanged("OSNScale");
            }
        }

        public int OSNOctaves
        {
            get
            {
                return _osnOctaves;
            }
            set
            {
                _osnOctaves = value;
                OnPropertyChanged("OSNOctaves");
            }
        }

        public double OSNOctaveWeight
        {
            get
            {
                return _osnOctaveWeight;
            }
            set
            {
                _osnOctaveWeight = value;
                OnPropertyChanged("OSNOctaveWeight");
            }
        }


        public double OSNScaleX
        {
            get
            {
                return _osnScaleX;
            }
            set
            {
                _osnScaleX = value;
                OnPropertyChanged("OSNScaleX");
            }
        }

        public double OSNScaleZ
        {
            get
            {
                return _osnScaleZ;
            }
            set
            {
                _osnScaleZ = value;
                OnPropertyChanged("OSNScaleZ");
            }
        }

        public int OSNSeed
        {
            get
            {
                return _osnSeed;
            }
            set
            {
                _osnSeed = value;
                OnPropertyChanged("OSNSeed");
            }
        }

        public int HEIterations
        {
            get
            {
                return _heIterations;
            }
            set
            {
                _heIterations = value;
                OnPropertyChanged("HEIterations");
            }
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
            Res256 = true;
            Res512 = false;
            Res1024 = true;
            Res2048 = true;

            OSNScale = 0.5f;
            OSNOctaves = 6;
            OSNOctaveWeight = 0.6;
            OSNScaleX = 0.5f;
            OSNScaleZ = 0.5f;
            OSNSeed = 500;

            HEIterations = 100000;
        }

        private void InitLogic()
        {
            HeightLogicProperty = new HeightLogic(512);
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

        #region Button Handling
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

        public void QuitApplication()
        {
            System.Windows.Application.Current.Shutdown();
        }

        public void NewTerrain()
        {
            //System.Windows.Forms.Application.Restart();
            //System.Windows.Application.Current.Shutdown();
            _heightLogic.TerrainSize = 512;
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

            HeightLogicProperty.InitHeights(resolution);
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
            HeightLogicProperty.OpenSimplexNoise(_osnScale, _osnOctaves, _osnOctaveWeight, _osnScaleX, _osnScaleZ, _osnSeed);
            TerrainMeshProperty.UpdateMesh();
        }

        public void Erode()
        {
            HeightLogicProperty.Erode(_heIterations);
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
