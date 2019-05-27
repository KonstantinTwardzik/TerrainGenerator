using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TerrainGenerator.Models;
using TerrainGenerator.Utilities;

namespace TerrainGenerator.ViewModels
{
    public class HeightLogic : INotifyPropertyChanged
    {
        #region Attributes
        private int _terrainSize;
        private double[] _terrainPointsUneroded;
        private TerrainPoints _terrainPoints;
        private OpenSimplexNoise _openSimplexNoise;
        private HydraulicErosion _hydraulicErosion;
        public bool isNoised;
        public bool isEroded;
        public bool isColored;

        //Open Simplex Noise
        private double _osnScale;
        private int _osnOctaves;
        private double _osnOctaveWeight;
        private double _osnScaleX;
        private double _osnScaleZ;
        private int _osnSeed;

        //Hydraulic Erosion
        private int _heIterations;
        private int _heMaxDropletLifetime;
        private int _heSeed;
        private int _heErosionRadius;
        private double _heInertia;
        private double _heSedimentCapacityFactor;
        private double _heMinSedimentCapacity;
        private double _heErodeSpeed;
        private double _heDepositSpeed;
        private double _heEvaporateSpeed;
        private double _heGravity;
        private bool _heErodeOver;
        #endregion

        #region Properties
        //Elementary Values
        public TerrainPoints TerrainPoints
        {
            get
            {
                return _terrainPoints;
            }
            set
            {
                _terrainPoints = value;
            }
        }
        public int TerrainSize
        {
            get
            {
                return _terrainSize;
            }
            set
            {
                _terrainSize = value;
            }
        }

        //OpenSimplexNoise
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

        //Hydraulic Erosion 
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
        public int HEMaxDropletLifetime
        {
            get
            {
                return _heMaxDropletLifetime;
            }
            set
            {
                _heMaxDropletLifetime = value;
                OnPropertyChanged("HEMaxDropletLifetime");
            }
        }
        public int HESeed
        {
            get
            {
                return _heSeed;
            }
            set
            {
                _heSeed = value;
                OnPropertyChanged("HESeed");
            }
        }
        public int HEErosionRadius
        {
            get
            {
                return _heErosionRadius;
            }
            set
            {
                _heErosionRadius = value;
                OnPropertyChanged("HEErosionRadius");
            }
        }
        public double HEInertia
        {
            get
            {
                return _heInertia;
            }
            set
            {
                _heInertia = value;
                OnPropertyChanged("HEInertia");
            }
        }
        public double HESedimentCapacityFactor
        {
            get
            {
                return _heSedimentCapacityFactor;
            }
            set
            {
                _heSedimentCapacityFactor = value;
                OnPropertyChanged("HESedimentCapacityFactor");
            }
        }
        public double HEMinSedimentCapacity
        {
            get
            {
                return _heMinSedimentCapacity;
            }
            set
            {
                _heMinSedimentCapacity = value;
                OnPropertyChanged("HEMinSedimentCapacity");
            }
        }
        public double HEErodeSpeed
        {
            get
            {
                return _heErodeSpeed;
            }
            set
            {
                _heErodeSpeed = value;
                OnPropertyChanged("HEErodeSpeed");
            }
        }
        public double HEDepositSpeed
        {
            get
            {
                return _heDepositSpeed;
            }
            set
            {
                _heDepositSpeed = value;
                OnPropertyChanged("HEDepositSpeed");
            }
        }
        public double HEEvaporateSpeed
        {
            get
            {
                return _heEvaporateSpeed;
            }
            set
            {
                _heEvaporateSpeed = value;
                OnPropertyChanged("HEEvaporateSpeed");
            }
        }
        public double HEGravity
        {
            get
            {
                return _heGravity;
            }
            set
            {
                _heGravity = value;
                OnPropertyChanged("HEGravity");
            }
        }
        public bool HEErodeOver
        {
            get
            {
                return _heErodeOver;
            }
            set
            {
                _heErodeOver = value;
                OnPropertyChanged("HEErodeOver");
            }
        }

        #endregion


        #region Initialization
        public HeightLogic()
        {
            InitHeights();
            _openSimplexNoise = new OpenSimplexNoise();
            _hydraulicErosion = new HydraulicErosion();
            isNoised = false;
            isEroded = false;
            isColored = false;

            //OpenSimplexNoise
            OSNScale = 0.5f;
            OSNOctaves = 6;
            OSNOctaveWeight = 0.6;
            OSNScaleX = 0.5f;
            OSNScaleZ = 0.5f;
            OSNSeed = 500;

            //Hydraulic Erosion
            HEIterations = 100000;
            HEMaxDropletLifetime = 15;
            HESeed = 1;
            HEErosionRadius = 1;
            HEInertia = 0.025;
            HESedimentCapacityFactor = 2;
            HEMinSedimentCapacity = 0.2;
            HEErodeSpeed = 0.15;
            HEDepositSpeed = 0.15;
            HEEvaporateSpeed = 0.5;
            HEGravity = 0.5;
            HEErodeOver = false;
        }

        public void InitHeights()
        {
            _terrainSize = 512;
            GenerateTerrainPoints();

        }

        private void GenerateTerrainPoints()
        {
            _terrainPoints.XPositions = new double[_terrainSize * _terrainSize];
            _terrainPoints.YPositions = new double[_terrainSize * _terrainSize];
            _terrainPoints.ZPositions = new double[_terrainSize * _terrainSize];
            _terrainPointsUneroded = new double[_terrainSize * _terrainSize];

            for (int x = 0; x < _terrainSize; x++)
            {
                for (int z = 0; z < _terrainSize; z++)
                {
                    _terrainPoints.XPositions[x + z * _terrainSize] = x;
                    _terrainPoints.YPositions[x + z * _terrainSize] = 0;
                    _terrainPoints.ZPositions[x + z * _terrainSize] = z;
                    _terrainPointsUneroded[x + z * _terrainSize] = 0;
                }
            }
        }
        #endregion

        public void ChangeDetailResolution(int terrainSize)
        {
            _terrainSize = terrainSize;
            GenerateTerrainPoints();
        }
        public void ResetHeights()
        {
            for (int x = 0; x < _terrainSize; x++)
            {
                for (int z = 0; z < _terrainSize; z++)
                {
                    _terrainPoints.YPositions[x + z * _terrainSize] = 0;
                }
            }

            isNoised = false;
        }

        public void OpenSimplexNoise()
        {
            double weight = 1;
            double octaveMultiplier = 1;
            double sizeCompensator = 1;

            switch (_terrainSize)
            {
                case 256:
                    sizeCompensator = 8;
                    break;
                case 512:
                    sizeCompensator = 4;
                    break;
                case 1024:
                    sizeCompensator = 2;
                    break;
                case 2048:
                    sizeCompensator = 1;
                    break;
            }
            ResetHeights();

            for (int o = 0; o < _osnOctaves; o++)
            {
                for (int x = 0; x < _terrainSize; x++)
                {
                    for (int z = 0; z < _terrainSize; z++)
                    {
                        double value = 0;
                        double xValue = ((((0.0005f / _osnScale) / _osnScaleX) * (x * sizeCompensator) + _osnSeed) * octaveMultiplier);
                        double zValue = ((((0.0005f / _osnScale) / _osnScaleZ) * (z * sizeCompensator) + _osnSeed) * octaveMultiplier);
                        if (o == 0)
                        {
                            value = (((_openSimplexNoise.Evaluate(xValue, zValue) * weight) + 1) / 2);
                        }
                        else
                        {
                            value = ((_openSimplexNoise.Evaluate(xValue, zValue) * weight) / 2);
                        }

                        _terrainPoints.YPositions[x + z * _terrainSize] += value;
                        _terrainPointsUneroded[x + z * _terrainSize] += value;
                    }
                }
                weight /= 2 - (_osnOctaveWeight - 0.5);
                octaveMultiplier = o * 2;
            }
            isNoised = true;
        }

        public void Erode()
        {
            if (!_heErodeOver)
            {
                for (int x = 0; x < _terrainSize; x++)
                {
                    for (int z = 0; z < _terrainSize; z++)
                    {
                        _terrainPoints.YPositions[x + z * _terrainSize] = _terrainPointsUneroded[x + z * _terrainSize];
                    }
                }
            }

            int seed = _heSeed;
            int iterations = _heIterations;
            int erosionRadius = _heErosionRadius;
            double inertia = _heInertia;
            double sedimentCapacityFactor = _heSedimentCapacityFactor;
            double minSedimentCapacity = 0.001 + 0.05 * _heMinSedimentCapacity;
            double erodeSpeed = _heErodeSpeed;
            double depositSpeed = _heDepositSpeed;
            double evaporateSpeed = 0.001 + 0.005 * _heEvaporateSpeed;
            double gravity = _heGravity;
            int maxDropletLifetime = _heMaxDropletLifetime;

            switch (_terrainSize)
            {
                case 256:

                    iterations *= 1;
                    erosionRadius *= 1;
                    inertia *= 1;
                    sedimentCapacityFactor *= 1;
                    minSedimentCapacity *= 1;
                    erodeSpeed *= 1;
                    depositSpeed *= 1;
                    evaporateSpeed *= 1;
                    gravity *= 1;
                    maxDropletLifetime *= 1;
                    break;
                case 512:
                    iterations *= 2;
                    erosionRadius *= 2;
                    inertia *= 2;
                    sedimentCapacityFactor *= 2;
                    minSedimentCapacity *= 2;
                    erodeSpeed *= 2;
                    depositSpeed *= 2;
                    evaporateSpeed *= 2;
                    gravity *= 2;
                    maxDropletLifetime *= 2;
                    break;
                case 1024:
                    iterations *= 4;
                    erosionRadius *= 4;
                    inertia *= 4;
                    sedimentCapacityFactor *= 4;
                    minSedimentCapacity *= 4;
                    erodeSpeed *= 4;
                    depositSpeed *= 4;
                    evaporateSpeed *= 4;
                    gravity *= 4;
                    maxDropletLifetime *= 4;
                    break;
                case 2048:
                    iterations *= 8;
                    erosionRadius *= 8;
                    inertia *= 8;
                    sedimentCapacityFactor *= 8;
                    minSedimentCapacity *= 8;
                    erodeSpeed *= 8;
                    depositSpeed *= 8;
                    evaporateSpeed *= 8;
                    gravity *= 8;
                    maxDropletLifetime *= 8;
                    break;

            }

            _hydraulicErosion.UpdateValues(seed, erosionRadius, inertia, sedimentCapacityFactor, minSedimentCapacity, erodeSpeed, depositSpeed, evaporateSpeed, gravity, maxDropletLifetime);
            _hydraulicErosion.Erode(_terrainPoints.YPositions, _terrainSize, iterations, false);


            isEroded = true;
        }

        public void ConvertToBitmapSource()
        {
            //System.Windows.Media.PixelFormat pixelFormat = PixelFormats.Bgr32;
            //int rawStride = (_terrainSize * pixelFormat.BitsPerPixel + 7) / 8;
            //byte[] rawImage = new byte[rawStride * _terrainSize];
            //BitmapSource image = BitmapSource.Create(_terrainSize, _terrainSize, 96, 96, pixelFormat, null, rawImage, rawStride);

            //Image image
        }
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
