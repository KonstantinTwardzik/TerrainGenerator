using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Topographer3D.Models;
using Topographer3D.Utilities;
using Color = System.Windows.Media.Color;

namespace Topographer3D.ViewModels.Layers
{
    class DetailColorizationLayer : BaseLayer
    {
        #region ATTRIUBTES & PROPERTIES
        private DetailColoringAlgorithm _coloringAlgorithm;
        private int TerrainSize;
        private float[] TerrainPoints;

        private LinearGradientBrush _gradientBorder;
        private Color color0Border;
        private Color color1Border;
        private Color color2Border;
        private Color color3Border;
        private Color color4Border;

        public MemoryStream TerrainMainColors { get; private set; }
        public MemoryStream TerrainBorderColors { get; private set; }

        public LinearGradientBrush Gradient1 { get; set; }
        public LinearGradientBrush Gradient2 { get; set; }
        public LinearGradientBrush Gradient3 { get; set; }
        public LinearGradientBrush Gradient4 { get; set; }
        public LinearGradientBrush Gradient5 { get; set; }
        public LinearGradientBrush Gradient6 { get; set; }
        public LinearGradientBrush Gradient7 { get; set; }

        public bool Gradient1RB { get; set; }
        public bool Gradient2RB { get; set; }
        public bool Gradient3RB { get; set; }
        public bool Gradient4RB { get; set; }
        public bool Gradient5RB { get; set; }
        public bool Gradient6RB { get; set; }
        public bool Gradient7RB { get; set; }

        public float ColorShift { get; set; }

        public IEnumerable<ColorApplicationMode> ColorApplicationModeEnum { get { return Enum.GetValues(typeof(ColorApplicationMode)).Cast<ColorApplicationMode>(); } }
        public ColorApplicationMode CurrentColorApplicationMode { get; set; }

        #endregion

        #region INITIALIZATION
        public DetailColorizationLayer(LayerManager layerManager, TerrainEngine terrainEngine) : base(layerManager, terrainEngine)
        {
            _coloringAlgorithm = new DetailColoringAlgorithm();

            _gradientBorder = new LinearGradientBrush();
            _gradientBorder.StartPoint = new System.Windows.Point(0, 0);
            _gradientBorder.EndPoint = new System.Windows.Point(1, 0);
            _gradientBorder.GradientStops = new GradientStopCollection();
            color0Border = new Color();
            color1Border = new Color();
            color2Border = new Color();
            color3Border = new Color();
            color4Border = new Color();

            InitProperties();
            InitGradients();
        }

        private void InitProperties()
        {
            LayerType = Layer.DetailColorization;
            HasApplicationMode = Visibility.Hidden;
            ColorShift = 0.0f;
            CurrentColorApplicationMode = ColorApplicationMode.Normal;
            Gradient1RB = true;
            Gradient2RB = false;
            Gradient3RB = false;
            Gradient4RB = false;
            Gradient5RB = false;
            Gradient6RB = false;
            Gradient7RB = false;
        }

        public void InitGradients()
        {
            #region Gradient1
            Gradient1 = new LinearGradientBrush();
            Gradient1.StartPoint = new System.Windows.Point(0, 0);
            Gradient1.EndPoint = new System.Windows.Point(1, 0);
            Gradient1.GradientStops = new GradientStopCollection();

            Color color10 = new Color();
            color10 = Color.FromRgb(51, 58, 40);
            Color color11 = new Color();
            color11 = Color.FromRgb(142, 159, 77);
            Color color12 = new Color();
            color12 = Color.FromRgb(102, 115, 2);
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

            Gradient1.GradientStops.Add(stop10);
            Gradient1.GradientStops.Add(stop11);
            Gradient1.GradientStops.Add(stop12);
            Gradient1.GradientStops.Add(stop13);
            Gradient1.GradientStops.Add(stop15);
            Gradient1.GradientStops.Add(stop16);
            Gradient1.GradientStops.Add(stop17);
            Gradient1.GradientStops.Add(stop18);
            Gradient1.GradientStops.Add(stop19);
            Gradient1.GradientStops.Add(stop110);
            #endregion

            #region Gradient2
            Gradient2 = new LinearGradientBrush();
            Gradient2.StartPoint = new System.Windows.Point(0, 0);
            Gradient2.EndPoint = new System.Windows.Point(1, 0);
            Gradient2.GradientStops = new GradientStopCollection();

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

            Gradient2.GradientStops.Add(stop20);
            Gradient2.GradientStops.Add(stop21);
            Gradient2.GradientStops.Add(stop22);
            Gradient2.GradientStops.Add(stop23);
            Gradient2.GradientStops.Add(stop24);
            Gradient2.GradientStops.Add(stop25);
            Gradient2.GradientStops.Add(stop26);
            Gradient2.GradientStops.Add(stop27);
            Gradient2.GradientStops.Add(stop28);
            Gradient2.GradientStops.Add(stop29);
            Gradient2.GradientStops.Add(stop210);
            #endregion

            #region Gradient3
            Gradient3 = new LinearGradientBrush();
            Gradient3.StartPoint = new System.Windows.Point(0, 0);
            Gradient3.EndPoint = new System.Windows.Point(1, 0);
            Gradient3.GradientStops = new GradientStopCollection();

            Color color30 = new Color();
            color30 = Color.FromRgb(102, 115, 2);
            Color color31 = new Color();
            color31 = Color.FromRgb(123, 125, 127);
            Color color32 = new Color();
            color32 = Color.FromRgb(140, 119, 100);
            Color color33 = new Color();
            color33 = Color.FromRgb(50, 64, 1);
            Color color34 = new Color();
            color34 = Color.FromRgb(140, 138, 141);

            GradientStop stop30 = new GradientStop();
            stop30.Offset = 0;
            stop30.Color = color30;
            GradientStop stop31 = new GradientStop();
            stop31.Offset = 0.1;
            stop31.Color = color33;
            GradientStop stop32 = new GradientStop();
            stop32.Offset = 0.2;
            stop32.Color = color31;
            GradientStop stop33 = new GradientStop();
            stop33.Offset = 0.3;
            stop33.Color = color30;
            GradientStop stop34 = new GradientStop();
            stop34.Offset = 0.4;
            stop34.Color = color34;
            GradientStop stop35 = new GradientStop();
            stop35.Offset = 0.5;
            stop35.Color = color31;
            GradientStop stop36 = new GradientStop();
            stop36.Offset = 0.6;
            stop36.Color = color32;
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

            Gradient3.GradientStops.Add(stop30);
            Gradient3.GradientStops.Add(stop31);
            Gradient3.GradientStops.Add(stop32);
            Gradient3.GradientStops.Add(stop33);
            Gradient3.GradientStops.Add(stop34);
            Gradient3.GradientStops.Add(stop35);
            Gradient3.GradientStops.Add(stop36);
            Gradient3.GradientStops.Add(stop37);
            Gradient3.GradientStops.Add(stop38);
            Gradient3.GradientStops.Add(stop39);
            Gradient3.GradientStops.Add(stop310);
            #endregion

            #region Gradient4
            Gradient4 = new LinearGradientBrush();
            Gradient4.StartPoint = new System.Windows.Point(0, 0);
            Gradient4.EndPoint = new System.Windows.Point(1, 0);
            Gradient4.GradientStops = new GradientStopCollection();

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

            Gradient4.GradientStops.Add(stop40);
            Gradient4.GradientStops.Add(stop41);
            Gradient4.GradientStops.Add(stop42);
            Gradient4.GradientStops.Add(stop43);
            Gradient4.GradientStops.Add(stop44);
            Gradient4.GradientStops.Add(stop45);
            Gradient4.GradientStops.Add(stop46);
            Gradient4.GradientStops.Add(stop47);
            Gradient4.GradientStops.Add(stop48);
            Gradient4.GradientStops.Add(stop49);
            Gradient4.GradientStops.Add(stop410);
            #endregion

            #region Gradient5
            Gradient5 = new LinearGradientBrush();
            Gradient5.StartPoint = new System.Windows.Point(0, 0);
            Gradient5.EndPoint = new System.Windows.Point(1, 0);
            Gradient5.GradientStops = new GradientStopCollection();

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
            GradientStop stop51 = new GradientStop
            {
                Offset = 0.1,
                Color = color51
            };
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

            Gradient5.GradientStops.Add(stop50);
            Gradient5.GradientStops.Add(stop51);
            Gradient5.GradientStops.Add(stop52);
            Gradient5.GradientStops.Add(stop53);
            Gradient5.GradientStops.Add(stop54);
            Gradient5.GradientStops.Add(stop55);
            Gradient5.GradientStops.Add(stop56);
            Gradient5.GradientStops.Add(stop57);
            Gradient5.GradientStops.Add(stop58);
            Gradient5.GradientStops.Add(stop59);
            Gradient5.GradientStops.Add(stop510);
            #endregion

            #region Gradient6
            Gradient6 = new LinearGradientBrush();
            Gradient6.StartPoint = new System.Windows.Point(0, 0);
            Gradient6.EndPoint = new System.Windows.Point(1, 0);
            Gradient6.GradientStops = new GradientStopCollection();

            Color color60 = new Color();
            color60 = Color.FromRgb(235, 239, 242);
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

            Gradient6.GradientStops.Add(stop60);
            Gradient6.GradientStops.Add(stop61);
            Gradient6.GradientStops.Add(stop62);
            Gradient6.GradientStops.Add(stop63);
            Gradient6.GradientStops.Add(stop64);
            Gradient6.GradientStops.Add(stop65);
            Gradient6.GradientStops.Add(stop66);
            Gradient6.GradientStops.Add(stop67);
            Gradient6.GradientStops.Add(stop68);
            Gradient6.GradientStops.Add(stop69);
            Gradient6.GradientStops.Add(stop610);
            #endregion

            #region Gradient7
            Gradient7 = new LinearGradientBrush();
            Gradient7.StartPoint = new System.Windows.Point(0, 0);
            Gradient7.EndPoint = new System.Windows.Point(1, 0);
            Gradient7.GradientStops = new GradientStopCollection();

            Color color70 = new Color();
            color70 = Color.FromRgb(12, 108, 130);
            Color color71 = new Color();
            color71 = Color.FromRgb(216, 67, 120);
            Color color72 = new Color();
            color72 = Color.FromRgb(99, 67, 86);
            Color color73 = new Color();
            color73 = Color.FromRgb(50, 163, 83);
            Color color74 = new Color();
            color74 = Color.FromRgb(238, 101, 75);

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

            Gradient7.GradientStops.Add(stop70);
            Gradient7.GradientStops.Add(stop71);
            Gradient7.GradientStops.Add(stop72);
            Gradient7.GradientStops.Add(stop73);
            Gradient7.GradientStops.Add(stop74);
            Gradient7.GradientStops.Add(stop75);
            Gradient7.GradientStops.Add(stop76);
            Gradient7.GradientStops.Add(stop77);
            Gradient7.GradientStops.Add(stop78);
            Gradient7.GradientStops.Add(stop79);
            Gradient7.GradientStops.Add(stop710);
            #endregion
        }

        #endregion

        #region TERRAIN ENGINE PROCESSING
        public void StartColorization(int TerrainSize, float[] TerrainPoints)
        {
            // Not multithreaded yet!
            this.TerrainSize = TerrainSize;
            this.TerrainPoints = TerrainPoints;


            ColorizeCalculate();

            Processed();
            terrainEngine.SingleLayerCalculationComplete(this, TerrainMainColors, TerrainBorderColors);
            Dispose();
        }

        public void ColorizeCalculate()
        {
            GradientStopCollection currentSelectedGradient;

            #region Gradient Selector
            if (Gradient1RB)
            {
                currentSelectedGradient = Gradient1.GradientStops;
                ChangeBorderGradient(89, 75, 66, 38, 27, 20, 115, 97, 81, 38, 27, 20, 13, 0, 0);
            }
            else if (Gradient2RB)
            {
                currentSelectedGradient = Gradient2.GradientStops;
                ChangeBorderGradient(232, 197, 159, 140, 79, 43, 220, 119, 56, 191, 116, 73, 232, 157, 82);
            }
            else if (Gradient3RB)
            {
                currentSelectedGradient = Gradient3.GradientStops;
                ChangeBorderGradient(100, 98, 101, 138, 130, 116, 82, 70, 61, 96, 82, 71, 138, 123, 112);
            }
            else if (Gradient4RB)
            {
                currentSelectedGradient = Gradient4.GradientStops;
                ChangeBorderGradient(140, 142, 145, 123, 125, 127, 227, 231, 235, 62, 63, 64, 99, 106, 115);
            }
            else if (Gradient5RB)
            {
                currentSelectedGradient = Gradient5.GradientStops;
                ChangeBorderGradient(217, 197, 160, 232, 214, 179, 191, 169, 142, 239, 224, 172, 134, 105, 73);
            }
            else if (Gradient6RB)
            {
                currentSelectedGradient = Gradient6.GradientStops;
                ChangeBorderGradient(235, 239, 239, 117, 156, 191, 139, 187, 217, 182, 219, 239, 206, 232, 239);
            }
            else if (Gradient7RB)
            {
                currentSelectedGradient = Gradient7.GradientStops;
                ChangeBorderGradient(12, 108, 130, 216, 67, 120, 99, 67, 86, 50, 163, 83, 238, 101, 75);
            }
            else
            {
                return;
            }
            #endregion

            bool ColorInvert = false; 
            if (CurrentColorApplicationMode == ColorApplicationMode.Normal)
            {
                ColorInvert = true;
            }

            #region ColorMap                                                                       
            PixelFormat pixelFormat = PixelFormats.Bgr24;
            int rawStride = (TerrainSize * pixelFormat.BitsPerPixel + 7) / 8;
            byte[] rawImage = new byte[rawStride * TerrainSize];

            _coloringAlgorithm.UpdateValues(currentSelectedGradient, TerrainPoints, TerrainSize, ColorShift, ColorInvert);
            _coloringAlgorithm.calculateMinMax();

            int count = 0;
            for (int x = 0; x < TerrainSize; x++)
            {
                for (int z = 0; z < TerrainSize; z++)
                {
                    byte[] RGB = _coloringAlgorithm.ColorizeTerrain(x, z);
                    for (int i = 0; i < 3; i++)
                    {
                        rawImage[count] = RGB[i];
                        count++;
                    }
                }
            }


            BitmapSource bitmap = BitmapSource.Create(TerrainSize, TerrainSize, 96, 96, pixelFormat, null, rawImage, rawStride);
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            TerrainMainColors = new MemoryStream();

            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            encoder.Save(TerrainMainColors);

            TerrainMainColors.Position = 0;
            #endregion

            #region BorderMap
            int width = 500;
            int height = 1;
            int rawStrideBorder = (width * pixelFormat.BitsPerPixel + 7) / 8;
            byte[] rawImageBorder = new byte[rawStrideBorder * height];
            _coloringAlgorithm.UpdateValues(_gradientBorder.GradientStops, TerrainPoints, TerrainSize, ColorShift, ColorInvert);
            int count2 = 0;
            for (int x = 0; x < width * height; x++)
            {
                byte[] RGB = _coloringAlgorithm.ColorizeBorder(x * 4, width);
                for (int i = 0; i < 3; i++)
                {
                    rawImageBorder[count2] = RGB[i];
                    count2++;
                }
            }

            BitmapSource bitmapBorder = BitmapSource.Create(width, height, 96, 96, pixelFormat, null, rawImageBorder, rawStrideBorder);
            PngBitmapEncoder encoderBorder = new PngBitmapEncoder();
            TerrainBorderColors = new MemoryStream();

            encoderBorder.Frames.Add(BitmapFrame.Create(bitmapBorder));
            encoderBorder.Save(TerrainBorderColors);

            TerrainBorderColors.Position = 0;
            #endregion

        }

        private void ChangeBorderGradient(byte r0, byte g0, byte b0, byte r1, byte g1, byte b1, byte r2, byte g2, byte b2, byte r3, byte g3, byte b3, byte r4, byte g4, byte b4)
        {
            #region GradientBorder
            _gradientBorder.GradientStops.Clear();

            color0Border = Color.FromRgb(r0, g0, b0);
            color1Border = Color.FromRgb(r1, g1, b1);
            color2Border = Color.FromRgb(r2, g2, b2);
            color3Border = Color.FromRgb(r3, g3, b3);
            color4Border = Color.FromRgb(r4, g4, b4);

            GradientStop stop0Border = new GradientStop();
            stop0Border.Offset = 0;
            stop0Border.Color = color0Border;
            GradientStop stop1Border = new GradientStop();
            stop1Border.Offset = 0.1;
            stop1Border.Color = color0Border;
            GradientStop stop2Border = new GradientStop();
            stop2Border.Offset = 0.15;
            stop2Border.Color = color1Border;
            GradientStop stop3Border = new GradientStop();
            stop3Border.Offset = 0.4;
            stop3Border.Color = color1Border;
            GradientStop stop4Border = new GradientStop();
            stop4Border.Offset = 0.45;
            stop4Border.Color = color2Border;
            GradientStop stop5Border = new GradientStop();
            stop5Border.Offset = 0.6;
            stop5Border.Color = color2Border;
            GradientStop stop6Border = new GradientStop();
            stop6Border.Offset = 0.65;
            stop6Border.Color = color3Border;
            GradientStop stop7Border = new GradientStop();
            stop7Border.Offset = 0.72;
            stop7Border.Color = color3Border;
            GradientStop stop8Border = new GradientStop();
            stop8Border.Offset = 0.78;
            stop8Border.Color = color4Border;
            GradientStop stop9Border = new GradientStop();
            stop9Border.Offset = 0.85;
            stop9Border.Color = color4Border;
            GradientStop stop10Border = new GradientStop();
            stop10Border.Offset = 0.9;
            stop10Border.Color = color0Border;
            GradientStop stop11Border = new GradientStop();
            stop11Border.Offset = 1.0;
            stop11Border.Color = color0Border;

            _gradientBorder.GradientStops.Add(stop0Border);
            _gradientBorder.GradientStops.Add(stop1Border);
            _gradientBorder.GradientStops.Add(stop2Border);
            _gradientBorder.GradientStops.Add(stop3Border);
            _gradientBorder.GradientStops.Add(stop4Border);
            _gradientBorder.GradientStops.Add(stop5Border);
            _gradientBorder.GradientStops.Add(stop6Border);
            _gradientBorder.GradientStops.Add(stop7Border);
            _gradientBorder.GradientStops.Add(stop8Border);
            _gradientBorder.GradientStops.Add(stop9Border);
            _gradientBorder.GradientStops.Add(stop10Border);
            _gradientBorder.GradientStops.Add(stop11Border);
            #endregion
        }

        #endregion

        #region DISPOSABLE
        protected override void Dispose()
        {
            TerrainPoints = null;
        }

        #endregion
    }
}
