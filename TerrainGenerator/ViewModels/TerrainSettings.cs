using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TerrainGenerator.Models;
using Color = System.Windows.Media.Color;

namespace TerrainGenerator.ViewModels
{
    public class TerrainSettings : INotifyPropertyChanged
    {
        #region Attributes
        private int _terrainSize;
        private double[] _terrainPoints;
        private double[] _terrainPointsUneroded;
        private OpenSimplexNoise _openSimplexNoise;
        private HydraulicErosion _hydraulicErosion;
        private ColoringAlgorithm _coloringAlgorithm;
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

        //Texturing
        private BitmapImage _colorMapImage;
        private BitmapImage _heightMapImage;
        private LinearGradientBrush _gradient1;
        private LinearGradientBrush _gradient2;
        private LinearGradientBrush _gradient3;
        private LinearGradientBrush _gradient4;
        private LinearGradientBrush _gradient5;
        private LinearGradientBrush _gradient6;
        private LinearGradientBrush _gradient7;
        private double _colorShift;
        private bool _colorInvert;
        #endregion

        #region Properties
        //Elementary Values
        public double[] TerrainPoints
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

        // Coloring
        public BitmapImage HeightMapImage
        {
            get
            {
                return _heightMapImage;
            }
            set
            {
                value = _heightMapImage;
            }
        }
        public BitmapImage ColorMapImage
        {
            get
            {
                return _colorMapImage;
            }
            set
            {
                value = _colorMapImage;
            }
        }
        public double ColorShift
        {
            get
            {
                return _colorShift;
            }
            set
            {
                _colorShift = value;
                OnPropertyChanged("ColorShift");
            }
        }
        public bool ColorInvert
        {
            get
            {
                return _colorInvert;
            }
            set
            {
                _colorInvert = value;
                OnPropertyChanged("ColorInvert");
            }
        }
        public LinearGradientBrush Gradient1
        {
            get
            {
                return _gradient1;
            }
            set
            {
                _gradient1 = value;
            }
        }
        public LinearGradientBrush Gradient2
        {
            get
            {
                return _gradient2;
            }
            set
            {
                _gradient2 = value;
            }
        }
        public LinearGradientBrush Gradient3
        {
            get
            {
                return _gradient3;
            }
            set
            {
                _gradient3 = value;
            }
        }
        public LinearGradientBrush Gradient4
        {
            get
            {
                return _gradient4;
            }
            set
            {
                _gradient4 = value;
            }
        }
        public LinearGradientBrush Gradient5
        {
            get
            {
                return _gradient5;
            }
            set
            {
                _gradient5 = value;
            }
        }
        public LinearGradientBrush Gradient6
        {
            get
            {
                return _gradient6;
            }
            set
            {
                _gradient6 = value;
            }
        }
        public LinearGradientBrush Gradient7
        {
            get
            {
                return _gradient7;
            }
            set
            {
                _gradient7 = value;
            }
        }
        #endregion

        #region Initialization
        public TerrainSettings()
        {
            InitHeights();
            InitAttributes();
            InitProperties();
            InitGradients();
        }

        public void InitAttributes()
        {
            _openSimplexNoise = new OpenSimplexNoise();
            _hydraulicErosion = new HydraulicErosion();
            _coloringAlgorithm = new ColoringAlgorithm();

            isNoised = false;
            isEroded = false;
            isColored = false;

            // Coloring
            _colorMapImage = new BitmapImage();
            _heightMapImage = new BitmapImage();
        }

        public void InitProperties()
        {
            //OpenSimplexNoise
            OSNScale = 0.5f;
            OSNOctaves = 6;
            OSNOctaveWeight = 0.6;
            OSNScaleX = 0.5f;
            OSNScaleZ = 0.5f;
            OSNSeed = 500;

            //Hydraulic Erosion
            HEIterations = 150000;
            HEMaxDropletLifetime = 15;
            HESeed = 1;
            HEErosionRadius = 1;
            HEInertia = 0.025;
            HESedimentCapacityFactor = 2;
            HEMinSedimentCapacity = 0.2;
            HEErodeSpeed = 0.15;
            HEDepositSpeed = 0.15;
            HEEvaporateSpeed = 0.5;
            HEGravity = 2;
            HEErodeOver = false;

            //Coloring
            ColorShift = 0.0;
            ColorInvert = false;
        }

        public void InitHeights()
        {
            _terrainSize = 512;
            GenerateTerrainPoints();
        }

        public void InitGradients()
        {
            #region Gradient1
            _gradient1 = new LinearGradientBrush();
            _gradient1.StartPoint = new System.Windows.Point(0, 0);
            _gradient1.EndPoint = new System.Windows.Point(1, 0);
            _gradient1.GradientStops = new GradientStopCollection();

            Color color10 = new Color();
            color10 = Color.FromRgb(31, 38, 20);
            Color color11 = new Color();
            color11 = Color.FromRgb(51, 64, 24);
            Color color12 = new Color();
            color12 = Color.FromRgb(106, 115, 41);
            Color color13 = new Color();
            color13 = Color.FromRgb(137, 140, 32);
            Color color14 = new Color();
            color14 = Color.FromRgb(66, 49, 30);

            GradientStop stop10 = new GradientStop();
            stop10.Offset = 0;
            stop10.Color = color11;
            GradientStop stop11 = new GradientStop();
            stop11.Offset = 0.1;
            stop11.Color = color12;
            GradientStop stop12 = new GradientStop();
            stop12.Offset = 0.2;
            stop12.Color = color14;
            GradientStop stop13 = new GradientStop();
            stop13.Offset = 0.3;
            stop13.Color = color10;
            GradientStop stop14 = new GradientStop();
            stop14.Offset = 0.4;
            stop14.Color = color14;
            GradientStop stop15 = new GradientStop();
            stop15.Offset = 0.5;
            stop15.Color = color13;
            GradientStop stop16 = new GradientStop();
            stop16.Offset = 0.6;
            stop16.Color = color10;
            GradientStop stop17 = new GradientStop();
            stop17.Offset = 0.7;
            stop17.Color = color11;
            GradientStop stop18 = new GradientStop();
            stop18.Offset = 0.8;
            stop18.Color = color14;
            GradientStop stop19 = new GradientStop();
            stop19.Offset = 0.9;
            stop19.Color = color12;
            GradientStop stop110 = new GradientStop();
            stop110.Offset = 1.0;
            stop110.Color = color11;

            _gradient1.GradientStops.Add(stop10);
            _gradient1.GradientStops.Add(stop11);
            _gradient1.GradientStops.Add(stop12);
            _gradient1.GradientStops.Add(stop13);
            _gradient1.GradientStops.Add(stop15);
            _gradient1.GradientStops.Add(stop16);
            _gradient1.GradientStops.Add(stop17);
            _gradient1.GradientStops.Add(stop18);
            _gradient1.GradientStops.Add(stop19);
            _gradient1.GradientStops.Add(stop110);
            #endregion

            #region Gradient2
            _gradient2 = new LinearGradientBrush();
            _gradient2.StartPoint = new System.Windows.Point(0, 0);
            _gradient2.EndPoint = new System.Windows.Point(1, 0);
            _gradient2.GradientStops = new GradientStopCollection();

            Color color20 = new Color();
            color20 = Color.FromRgb(232, 197, 159);
            Color color21 = new Color();
            color21 = Color.FromRgb(140, 79, 43);
            Color color22 = new Color();
            color22 = Color.FromRgb(220, 119, 56);
            Color color23 = new Color();
            color23 = Color.FromRgb(191, 116, 73);
            Color color24 = new Color();
            color24 = Color.FromRgb(242, 157, 82);

            GradientStop stop20 = new GradientStop();
            stop20.Offset = 0;
            stop20.Color = color20;
            GradientStop stop21 = new GradientStop();
            stop21.Offset = 0.1;
            stop21.Color = color23;
            GradientStop stop22 = new GradientStop();
            stop22.Offset = 0.2;
            stop22.Color = color22;
            GradientStop stop23 = new GradientStop();
            stop23.Offset = 0.3;
            stop23.Color = color21;
            GradientStop stop24 = new GradientStop();
            stop24.Offset = 0.4;
            stop24.Color = color24;
            GradientStop stop25 = new GradientStop();
            stop25.Offset = 0.5;
            stop25.Color = color22;
            GradientStop stop26 = new GradientStop();
            stop26.Offset = 0.6;
            stop26.Color = color23;
            GradientStop stop27 = new GradientStop();
            stop27.Offset = 0.7;
            stop27.Color = color20;
            GradientStop stop28 = new GradientStop();
            stop28.Offset = 0.8;
            stop28.Color = color24;
            GradientStop stop29 = new GradientStop();
            stop29.Offset = 0.9;
            stop29.Color = color22;
            GradientStop stop210 = new GradientStop();
            stop210.Offset = 1;
            stop210.Color = color23;

            _gradient2.GradientStops.Add(stop20);
            _gradient2.GradientStops.Add(stop21);
            _gradient2.GradientStops.Add(stop22);
            _gradient2.GradientStops.Add(stop23);
            _gradient2.GradientStops.Add(stop24);
            _gradient2.GradientStops.Add(stop25);
            _gradient2.GradientStops.Add(stop26);
            _gradient2.GradientStops.Add(stop27);
            _gradient2.GradientStops.Add(stop28);
            _gradient2.GradientStops.Add(stop29);
            _gradient2.GradientStops.Add(stop210);
            #endregion

            #region Gradient3
            _gradient3 = new LinearGradientBrush();
            _gradient3.StartPoint = new System.Windows.Point(0, 0);
            _gradient3.EndPoint = new System.Windows.Point(1, 0);
            _gradient3.GradientStops = new GradientStopCollection();

            Color color30 = new Color();
            color30 = Color.FromRgb(62, 63, 64);
            Color color31 = new Color();
            color31 = Color.FromRgb(123, 125, 127);
            Color color32 = new Color();
            color32 = Color.FromRgb(72, 89, 34);
            Color color33 = new Color();
            color33 = Color.FromRgb(38, 38, 20);
            Color color34 = new Color();
            color34 = Color.FromRgb(147, 149, 152);

            GradientStop stop30 = new GradientStop();
            stop30.Offset = 0;
            stop30.Color = color30;
            GradientStop stop31 = new GradientStop();
            stop31.Offset = 0.1;
            stop31.Color = color33;
            GradientStop stop32 = new GradientStop();
            stop32.Offset = 0.2;
            stop32.Color = color32;
            GradientStop stop33 = new GradientStop();
            stop33.Offset = 0.3;
            stop33.Color = color31;
            GradientStop stop34 = new GradientStop();
            stop34.Offset = 0.4;
            stop34.Color = color34;
            GradientStop stop35 = new GradientStop();
            stop35.Offset = 0.5;
            stop35.Color = color32;
            GradientStop stop36 = new GradientStop();
            stop36.Offset = 0.6;
            stop36.Color = color33;
            GradientStop stop37 = new GradientStop();
            stop37.Offset = 0.7;
            stop37.Color = color30;
            GradientStop stop38 = new GradientStop();
            stop38.Offset = 0.8;
            stop38.Color = color34;
            GradientStop stop39 = new GradientStop();
            stop39.Offset = 0.9;
            stop39.Color = color32;
            GradientStop stop310 = new GradientStop();
            stop310.Offset = 1;
            stop310.Color = color33;

            _gradient3.GradientStops.Add(stop30);
            _gradient3.GradientStops.Add(stop31);
            _gradient3.GradientStops.Add(stop32);
            _gradient3.GradientStops.Add(stop33);
            _gradient3.GradientStops.Add(stop34);
            _gradient3.GradientStops.Add(stop35);
            _gradient3.GradientStops.Add(stop36);
            _gradient3.GradientStops.Add(stop37);
            _gradient3.GradientStops.Add(stop38);
            _gradient3.GradientStops.Add(stop39);
            _gradient3.GradientStops.Add(stop310);
            #endregion

            #region Gradient4
            _gradient4 = new LinearGradientBrush();
            _gradient4.StartPoint = new System.Windows.Point(0, 0);
            _gradient4.EndPoint = new System.Windows.Point(1, 0);
            _gradient4.GradientStops = new GradientStopCollection();

            Color color40 = new Color();
            color40 = Color.FromRgb(147, 149, 152);
            Color color41 = new Color();
            color41 = Color.FromRgb(123, 125, 127);
            Color color42 = new Color();
            color42 = Color.FromRgb(247, 251, 255);
            Color color43 = new Color();
            color43 = Color.FromRgb(62, 63, 64);
            Color color44 = new Color();
            color44 = Color.FromRgb(201, 232, 231);

            GradientStop stop40 = new GradientStop();
            stop40.Offset = 0;
            stop40.Color = color40;
            GradientStop stop41 = new GradientStop();
            stop41.Offset = 0.1;
            stop41.Color = color41;
            GradientStop stop42 = new GradientStop();
            stop42.Offset = 0.2;
            stop42.Color = color42;
            GradientStop stop43 = new GradientStop();
            stop43.Offset = 0.3;
            stop43.Color = color41;
            GradientStop stop44 = new GradientStop();
            stop44.Offset = 0.4;
            stop44.Color = color43;
            GradientStop stop45 = new GradientStop();
            stop45.Offset = 0.5;
            stop45.Color = color42;
            GradientStop stop46 = new GradientStop();
            stop46.Offset = 0.6;
            stop46.Color = color44;
            GradientStop stop47 = new GradientStop();
            stop47.Offset = 0.7;
            stop47.Color = color42;
            GradientStop stop48 = new GradientStop();
            stop48.Offset = 0.8;
            stop48.Color = color41;
            GradientStop stop49 = new GradientStop();
            stop49.Offset = 0.9;
            stop49.Color = color43;
            GradientStop stop410 = new GradientStop();
            stop410.Offset = 1;
            stop410.Color = color42;

            _gradient4.GradientStops.Add(stop40);
            _gradient4.GradientStops.Add(stop41);
            _gradient4.GradientStops.Add(stop42);
            _gradient4.GradientStops.Add(stop43);
            _gradient4.GradientStops.Add(stop44);
            _gradient4.GradientStops.Add(stop45);
            _gradient4.GradientStops.Add(stop46);
            _gradient4.GradientStops.Add(stop47);
            _gradient4.GradientStops.Add(stop48);
            _gradient4.GradientStops.Add(stop49);
            _gradient4.GradientStops.Add(stop410);
            #endregion

            #region Gradient5
            _gradient5 = new LinearGradientBrush();
            _gradient5.StartPoint = new System.Windows.Point(0, 0);
            _gradient5.EndPoint = new System.Windows.Point(1, 0);
            _gradient5.GradientStops = new GradientStopCollection();

            Color color50 = new Color();
            color50 = Color.FromRgb(217, 197, 160);
            Color color51 = new Color();
            color51 = Color.FromRgb(242, 224, 189);
            Color color52 = new Color();
            color52 = Color.FromRgb(191, 169, 142);
            Color color53 = new Color();
            color53 = Color.FromRgb(241, 190, 121);
            Color color54 = new Color();
            color54 = Color.FromRgb(216, 145, 94);

            GradientStop stop50 = new GradientStop();
            stop50.Offset = 0;
            stop50.Color = color50;
            GradientStop stop51 = new GradientStop();
            stop51.Offset = 0.1;
            stop51.Color = color51;
            GradientStop stop52 = new GradientStop();
            stop52.Offset = 0.2;
            stop52.Color = color53;
            GradientStop stop53 = new GradientStop();
            stop53.Offset = 0.3;
            stop53.Color = color51;
            GradientStop stop54 = new GradientStop();
            stop54.Offset = 0.4;
            stop54.Color = color50;
            GradientStop stop55 = new GradientStop();
            stop55.Offset = 0.5;
            stop55.Color = color52;
            GradientStop stop56 = new GradientStop();
            stop56.Offset = 0.6;
            stop56.Color = color54;
            GradientStop stop57 = new GradientStop();
            stop57.Offset = 0.7;
            stop57.Color = color52;
            GradientStop stop58 = new GradientStop();
            stop58.Offset = 0.8;
            stop58.Color = color51;
            GradientStop stop59 = new GradientStop();
            stop59.Offset = 0.9;
            stop59.Color = color53;
            GradientStop stop510 = new GradientStop();
            stop510.Offset = 1;
            stop510.Color = color52;

            _gradient5.GradientStops.Add(stop50);
            _gradient5.GradientStops.Add(stop51);
            _gradient5.GradientStops.Add(stop52);
            _gradient5.GradientStops.Add(stop53);
            _gradient5.GradientStops.Add(stop54);
            _gradient5.GradientStops.Add(stop55);
            _gradient5.GradientStops.Add(stop56);
            _gradient5.GradientStops.Add(stop57);
            _gradient5.GradientStops.Add(stop58);
            _gradient5.GradientStops.Add(stop59);
            _gradient5.GradientStops.Add(stop510);
            #endregion

            #region Gradient6
            _gradient6 = new LinearGradientBrush();
            _gradient6.StartPoint = new System.Windows.Point(0, 0);
            _gradient6.EndPoint = new System.Windows.Point(1, 0);
            _gradient6.GradientStops = new GradientStopCollection();

            Color color60 = new Color();
            color60 = Color.FromRgb(0, 191, 189);
            Color color61 = new Color();
            color61 = Color.FromRgb(117, 156, 191);
            Color color62 = new Color();
            color62 = Color.FromRgb(139, 187, 217);
            Color color63 = new Color();
            color63 = Color.FromRgb(182, 219, 242);
            Color color64 = new Color();
            color64 = Color.FromRgb(206, 232, 242);

            GradientStop stop60 = new GradientStop();
            stop60.Offset = 0;
            stop60.Color = color60;
            GradientStop stop61 = new GradientStop();
            stop61.Offset = 0.1;
            stop61.Color = color61;
            GradientStop stop62 = new GradientStop();
            stop62.Offset = 0.2;
            stop62.Color = color63;
            GradientStop stop63 = new GradientStop();
            stop63.Offset = 0.3;
            stop63.Color = color61;
            GradientStop stop64 = new GradientStop();
            stop64.Offset = 0.4;
            stop64.Color = color63;
            GradientStop stop65 = new GradientStop();
            stop65.Offset = 0.5;
            stop65.Color = color62;
            GradientStop stop66 = new GradientStop();
            stop66.Offset = 0.6;
            stop66.Color = color64;
            GradientStop stop67 = new GradientStop();
            stop67.Offset = 0.7;
            stop67.Color = color62;
            GradientStop stop68 = new GradientStop();
            stop68.Offset = 0.8;
            stop68.Color = color61;
            GradientStop stop69 = new GradientStop();
            stop69.Offset = 0.9;
            stop69.Color = color63;
            GradientStop stop610 = new GradientStop();
            stop610.Offset = 1;
            stop610.Color = color62;

            _gradient6.GradientStops.Add(stop60);
            _gradient6.GradientStops.Add(stop61);
            _gradient6.GradientStops.Add(stop62);
            _gradient6.GradientStops.Add(stop63);
            _gradient6.GradientStops.Add(stop64);
            _gradient6.GradientStops.Add(stop65);
            _gradient6.GradientStops.Add(stop66);
            _gradient6.GradientStops.Add(stop67);
            _gradient6.GradientStops.Add(stop68);
            _gradient6.GradientStops.Add(stop69);
            _gradient6.GradientStops.Add(stop610);
            #endregion

            #region Gradient7
            _gradient7 = new LinearGradientBrush();
            _gradient7.StartPoint = new System.Windows.Point(0, 0);
            _gradient7.EndPoint = new System.Windows.Point(1, 0);
            _gradient7.GradientStops = new GradientStopCollection();

            Color color70 = new Color();
            color70 = Color.FromRgb(168, 8, 126);
            Color color71 = new Color();
            color71 = Color.FromRgb(202, 96, 55);
            Color color72 = new Color();
            color72 = Color.FromRgb(92, 255, 255);
            Color color73 = new Color();
            color73 = Color.FromRgb(79, 255, 123);
            Color color74 = new Color();
            color74 = Color.FromRgb(242, 224, 255);

            GradientStop stop70 = new GradientStop();
            stop70.Offset = 0;
            stop70.Color = color70;
            GradientStop stop71 = new GradientStop();
            stop71.Offset = 0.1;
            stop71.Color = color71;
            GradientStop stop72 = new GradientStop();
            stop72.Offset = 0.2;
            stop72.Color = color73;
            GradientStop stop73 = new GradientStop();
            stop73.Offset = 0.3;
            stop73.Color = color71;
            GradientStop stop74 = new GradientStop();
            stop74.Offset = 0.4;
            stop74.Color = color73;
            GradientStop stop75 = new GradientStop();
            stop75.Offset = 0.5;
            stop75.Color = color72;
            GradientStop stop76 = new GradientStop();
            stop76.Offset = 0.6;
            stop76.Color = color74;
            GradientStop stop77 = new GradientStop();
            stop77.Offset = 0.7;
            stop77.Color = color72;
            GradientStop stop78 = new GradientStop();
            stop78.Offset = 0.8;
            stop78.Color = color71;
            GradientStop stop79 = new GradientStop();
            stop79.Offset = 0.9;
            stop79.Color = color73;
            GradientStop stop710 = new GradientStop();
            stop710.Offset = 1;
            stop710.Color = color72;

            _gradient7.GradientStops.Add(stop70);
            _gradient7.GradientStops.Add(stop71);
            _gradient7.GradientStops.Add(stop72);
            _gradient7.GradientStops.Add(stop73);
            _gradient7.GradientStops.Add(stop74);
            _gradient7.GradientStops.Add(stop75);
            _gradient7.GradientStops.Add(stop76);
            _gradient7.GradientStops.Add(stop77);
            _gradient7.GradientStops.Add(stop78);
            _gradient7.GradientStops.Add(stop79);
            _gradient7.GradientStops.Add(stop710);
            #endregion
        }

        private void GenerateTerrainPoints()
        {
            _terrainPoints = new double[_terrainSize * _terrainSize];
            _terrainPointsUneroded = new double[_terrainSize * _terrainSize];

            for (int x = 0; x < _terrainSize; x++)
            {
                for (int z = 0; z < _terrainSize; z++)
                {
                    _terrainPoints[x + z * _terrainSize] = 0;
                    _terrainPointsUneroded[x + z * _terrainSize] = 0;
                }
            }
        }
        #endregion

        #region Terrain Generation
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
                    _terrainPoints[x + z * _terrainSize] = 0;
                    _terrainPointsUneroded[x + z * _terrainSize] = 0;
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

                        _terrainPoints[x + z * _terrainSize] += value;
                        _terrainPointsUneroded[x + z * _terrainSize] += value;
                    }
                }
                weight /= 2 - (_osnOctaveWeight - 0.5);
                octaveMultiplier = o * 2;
            }
            isNoised = true;
            isEroded = false;
            isColored = false;
        }

        public void Erode()
        {
            if (!_heErodeOver && isEroded == true)
            {
                for (int x = 0; x < _terrainSize; x++)
                {
                    for (int z = 0; z < _terrainSize; z++)
                    {
                        _terrainPoints[x + z * _terrainSize] = _terrainPointsUneroded[x + z * _terrainSize];
                    }
                }
                System.Console.WriteLine("Terrain zurückgesetzt");
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
            _hydraulicErosion.Erode(_terrainPoints, _terrainSize, iterations, false);

            isEroded = true;
            isColored = false;
        }

        public void Colorize()
        {
            GradientStopCollection currentSelectedGradient = _gradient7.GradientStops;
            PixelFormat pixelFormat = PixelFormats.Bgr24;
            int rawStride = (_terrainSize * pixelFormat.BitsPerPixel + 7) / 8;
            byte[] rawImage = new byte[rawStride * _terrainSize];
            _coloringAlgorithm.UpdateValues(currentSelectedGradient, _terrainPoints, _terrainSize, _colorShift, _colorInvert);
            _coloringAlgorithm.calculateMinMax();
            int count = 0;
            for (int x = 0; x < _terrainSize; x++)
            {
                for (int z = 0; z < _terrainSize; z++)
                {
                    byte[] RGB = _coloringAlgorithm.Colorize(x, z);
                    for (int i = 0; i < 3; i++)
                    {
                        rawImage[count] = RGB[i];
                        count++;
                    }

                }
            }
            BitmapSource bitmap = BitmapSource.Create(_terrainSize, _terrainSize, 96, 96, pixelFormat, null, rawImage, rawStride);
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            MemoryStream memoryStream = new MemoryStream();
            _colorMapImage = new BitmapImage();

            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            encoder.Save(memoryStream);

            memoryStream.Position = 0;
            _colorMapImage.BeginInit();
            _colorMapImage.StreamSource = new MemoryStream(memoryStream.ToArray());
            _colorMapImage.EndInit();
            _colorMapImage.Freeze();
        }

        public void GenerateMaps()
        {
            //Heightmap Generation
            System.Windows.Media.PixelFormat pixelFormat = PixelFormats.Gray32Float;
            int rawStride = (_terrainSize * pixelFormat.BitsPerPixel + 7) / 8;
            byte[] rawImage = new byte[rawStride * _terrainSize];

            // Filling the rawImage with Data
            int count = 0;
            for (int x = 0; x < _terrainSize; x++)
            {
                for (int z = 0; z < _terrainSize; z++)
                {
                    byte[] bytes = BitConverter.GetBytes((float)_terrainPoints[x + z * _terrainSize]);
                    for (int i = 0; i < 4; i++)
                    {
                        if (count >= rawImage.Length)
                        {
                            break;
                        }
                        rawImage[count] = bytes[i];
                        count++;
                    }
                }
            }


            BitmapSource bitmap = BitmapSource.Create(_terrainSize, _terrainSize, 96, 96, pixelFormat, null, rawImage, rawStride);

            PngBitmapEncoder encoder = new PngBitmapEncoder();
            MemoryStream memoryStream = new MemoryStream();
            _heightMapImage = new BitmapImage();

            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            encoder.Save(memoryStream);

            memoryStream.Position = 0;
            _heightMapImage.BeginInit();
            _heightMapImage.StreamSource = new MemoryStream(memoryStream.ToArray());
            _heightMapImage.EndInit();
            _heightMapImage.Freeze();
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
