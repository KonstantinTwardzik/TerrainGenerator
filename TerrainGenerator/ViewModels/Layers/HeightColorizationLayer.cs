using System;
using System.Collections.Generic;
using System.Drawing;
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
    class HeightColorizationLayer : BaseLayer
    {
        #region ATTRIBUTES & PROPERTIES
        private int TerrainSize;
        private float[] TerrainPoints;
        private Random _random = new Random();
        private OpenSimplexNoiseAlgorithm osn;

        private byte[] previousColorMap;
        private byte[] ColorMap;
        public MemoryStream TerrainMainColors { get; private set; }

        public float MinHeight { get; set; }
        public float MaxHeight { get; set; }
        public float ColorNoise { get; set; }
        public float HeightNoiseAmount { get; set; }
        public float HeightNoiseScale { get; set; }


        #region COLORS
        public Color Color10 { get; set; }
        public Color Color11 { get; set; }
        public Color Color12 { get; set; }
        public Color Color13 { get; set; }
        public Color Color14 { get; set; }
        public Color Color20 { get; set; }
        public Color Color21 { get; set; }
        public Color Color22 { get; set; }
        public Color Color23 { get; set; }
        public Color Color24 { get; set; }
        public Color Color30 { get; set; }
        public Color Color31 { get; set; }
        public Color Color32 { get; set; }
        public Color Color33 { get; set; }
        public Color Color34 { get; set; }
        public Color Color40 { get; set; }
        public Color Color41 { get; set; }
        public Color Color42 { get; set; }
        public Color Color43 { get; set; }
        public Color Color44 { get; set; }
        public Color Color50 { get; set; }
        public Color Color51 { get; set; }
        public Color Color52 { get; set; }
        public Color Color53 { get; set; }
        public Color Color54 { get; set; }
        public Color Color60 { get; set; }
        public Color Color61 { get; set; }
        public Color Color62 { get; set; }
        public Color Color63 { get; set; }
        public Color Color64 { get; set; }
        public Color Color70 { get; set; }
        public Color Color71 { get; set; }
        public Color Color72 { get; set; }
        public Color Color73 { get; set; }
        public Color Color74 { get; set; }

        public SolidColorBrush ColorBrush10 { get; set; }
        public SolidColorBrush ColorBrush11 { get; set; }
        public SolidColorBrush ColorBrush12 { get; set; }
        public SolidColorBrush ColorBrush13 { get; set; }
        public SolidColorBrush ColorBrush14 { get; set; }
        public SolidColorBrush ColorBrush20 { get; set; }
        public SolidColorBrush ColorBrush21 { get; set; }
        public SolidColorBrush ColorBrush22 { get; set; }
        public SolidColorBrush ColorBrush23 { get; set; }
        public SolidColorBrush ColorBrush24 { get; set; }
        public SolidColorBrush ColorBrush30 { get; set; }
        public SolidColorBrush ColorBrush31 { get; set; }
        public SolidColorBrush ColorBrush32 { get; set; }
        public SolidColorBrush ColorBrush33 { get; set; }
        public SolidColorBrush ColorBrush34 { get; set; }
        public SolidColorBrush ColorBrush40 { get; set; }
        public SolidColorBrush ColorBrush41 { get; set; }
        public SolidColorBrush ColorBrush42 { get; set; }
        public SolidColorBrush ColorBrush43 { get; set; }
        public SolidColorBrush ColorBrush44 { get; set; }
        public SolidColorBrush ColorBrush50 { get; set; }
        public SolidColorBrush ColorBrush51 { get; set; }
        public SolidColorBrush ColorBrush52 { get; set; }
        public SolidColorBrush ColorBrush53 { get; set; }
        public SolidColorBrush ColorBrush54 { get; set; }
        public SolidColorBrush ColorBrush60 { get; set; }
        public SolidColorBrush ColorBrush61 { get; set; }
        public SolidColorBrush ColorBrush62 { get; set; }
        public SolidColorBrush ColorBrush63 { get; set; }
        public SolidColorBrush ColorBrush64 { get; set; }
        public SolidColorBrush ColorBrush70 { get; set; }
        public SolidColorBrush ColorBrush71 { get; set; }
        public SolidColorBrush ColorBrush72 { get; set; }
        public SolidColorBrush ColorBrush73 { get; set; }
        public SolidColorBrush ColorBrush74 { get; set; }
        #endregion

        #region COLOR BOOLS
        public bool Color10Bool { get; set; }
        public bool Color11Bool { get; set; }
        public bool Color12Bool { get; set; }
        public bool Color13Bool { get; set; }
        public bool Color14Bool { get; set; }
        public bool Color20Bool { get; set; }
        public bool Color21Bool { get; set; }
        public bool Color22Bool { get; set; }
        public bool Color23Bool { get; set; }
        public bool Color24Bool { get; set; }
        public bool Color30Bool { get; set; }
        public bool Color31Bool { get; set; }
        public bool Color32Bool { get; set; }
        public bool Color33Bool { get; set; }
        public bool Color34Bool { get; set; }
        public bool Color40Bool { get; set; }
        public bool Color41Bool { get; set; }
        public bool Color42Bool { get; set; }
        public bool Color43Bool { get; set; }
        public bool Color44Bool { get; set; }
        public bool Color50Bool { get; set; }
        public bool Color51Bool { get; set; }
        public bool Color52Bool { get; set; }
        public bool Color53Bool { get; set; }
        public bool Color54Bool { get; set; }
        public bool Color60Bool { get; set; }
        public bool Color61Bool { get; set; }
        public bool Color62Bool { get; set; }
        public bool Color63Bool { get; set; }
        public bool Color64Bool { get; set; }
        public bool Color70Bool { get; set; }
        public bool Color71Bool { get; set; }
        public bool Color72Bool { get; set; }
        public bool Color73Bool { get; set; }
        public bool Color74Bool { get; set; }
        #endregion


        public IEnumerable<Mode> ColorApplicationModeEnum { get { return Enum.GetValues(typeof(Mode)).Cast<Mode>(); } }
        public Mode CurrentColorApplicationMode { get; set; }
        public bool ColorInvert { get; private set; }

        #endregion

        #region INITIALIZATION
        public HeightColorizationLayer(LayerManager layerManager, TerrainEngine terrainEngine) : base(layerManager, terrainEngine)
        {
            InitProperties();
            InitColors();
        }

        private void InitProperties()
        {
            LayerType = Layer.HeightColorization;
            HasApplicationMode = Visibility.Hidden;
            CurrentColorApplicationMode = Mode.Normal;
            osn = new OpenSimplexNoiseAlgorithm();

            MinHeight = 0.0f;
            MaxHeight = 1.0f;
            ColorNoise = 0.5f;
            HeightNoiseAmount = 0.5f;
            HeightNoiseScale = 0.5f;

            Color10Bool = true;
            Color11Bool = false;
            Color12Bool = false;
            Color13Bool = false;
            Color14Bool = false;
            Color20Bool = false;
            Color21Bool = false;
            Color22Bool = false;
            Color23Bool = false;
            Color24Bool = false;
            Color30Bool = false;
            Color31Bool = false;
            Color32Bool = false;
            Color33Bool = false;
            Color34Bool = false;
            Color40Bool = false;
            Color41Bool = false;
            Color42Bool = false;
            Color43Bool = false;
            Color44Bool = false;
            Color50Bool = false;
            Color51Bool = false;
            Color52Bool = false;
            Color53Bool = false;
            Color54Bool = false;
            Color60Bool = false;
            Color61Bool = false;
            Color62Bool = false;
            Color63Bool = false;
            Color64Bool = false;
        }

        public void InitColors()
        {
            Color10 = new Color();
            Color10 = Color.FromRgb(51, 58, 40);
            ColorBrush10 = new SolidColorBrush();
            ColorBrush10.Color = Color10;

            Color11 = new Color();
            Color11 = Color.FromRgb(142, 159, 77);
            ColorBrush11 = new SolidColorBrush();
            ColorBrush11.Color = Color11;

            Color12 = new Color();
            Color12 = Color.FromRgb(102, 115, 2);
            ColorBrush12 = new SolidColorBrush();
            ColorBrush12.Color = Color12;

            Color13 = new Color();
            Color13 = Color.FromRgb(137, 140, 32);
            ColorBrush13 = new SolidColorBrush();
            ColorBrush13.Color = Color13;

            Color14 = new Color();
            Color14 = Color.FromRgb(66, 49, 30);
            ColorBrush14 = new SolidColorBrush();
            ColorBrush14.Color = Color14;

            Color20 = new Color();
            Color20 = Color.FromRgb(232, 197, 159);
            ColorBrush20 = new SolidColorBrush();
            ColorBrush20.Color = Color20;

            Color21 = new Color();
            Color21 = Color.FromRgb(140, 79, 43);
            ColorBrush21 = new SolidColorBrush();
            ColorBrush21.Color = Color21;

            Color22 = new Color();
            Color22 = Color.FromRgb(220, 229, 56);
            ColorBrush22 = new SolidColorBrush();
            ColorBrush22.Color = Color22;

            Color23 = new Color();
            Color23 = Color.FromRgb(191, 236, 73);
            ColorBrush23 = new SolidColorBrush();
            ColorBrush23.Color = Color23;

            Color24 = new Color();
            Color24 = Color.FromRgb(242, 157, 82);
            ColorBrush24 = new SolidColorBrush();
            ColorBrush24.Color = Color24;

            Color30 = new Color();
            Color30 = Color.FromRgb(102, 115, 2);
            ColorBrush30 = new SolidColorBrush();
            ColorBrush30.Color = Color30;

            Color31 = new Color();
            Color31 = Color.FromRgb(123, 125, 127);
            ColorBrush31 = new SolidColorBrush();
            ColorBrush31.Color = Color31;

            Color32 = new Color();
            Color32 = Color.FromRgb(140, 119, 100);
            ColorBrush32 = new SolidColorBrush();
            ColorBrush32.Color = Color32;

            Color33 = new Color();
            Color33 = Color.FromRgb(50, 64, 1);
            ColorBrush33 = new SolidColorBrush();
            ColorBrush33.Color = Color11;

            Color34 = new Color();
            Color34 = Color.FromRgb(140, 138, 141);
            ColorBrush34 = new SolidColorBrush();
            ColorBrush34.Color = Color34;

            Color40 = new Color();
            Color40 = Color.FromRgb(147, 149, 152);
            ColorBrush40 = new SolidColorBrush();
            ColorBrush40.Color = Color40;

            Color41 = new Color();
            Color41 = Color.FromRgb(123, 125, 127);
            ColorBrush41 = new SolidColorBrush();
            ColorBrush41.Color = Color41;

            Color42 = new Color();
            Color42 = Color.FromRgb(247, 251, 255);
            ColorBrush42 = new SolidColorBrush();
            ColorBrush42.Color = Color42;

            Color43 = new Color();
            Color43 = Color.FromRgb(62, 63, 64);
            ColorBrush43 = new SolidColorBrush();
            ColorBrush43.Color = Color43;

            Color44 = new Color();
            Color44 = Color.FromRgb(201, 232, 231);
            ColorBrush44 = new SolidColorBrush();
            ColorBrush44.Color = Color44;

            Color50 = new Color();
            Color50 = Color.FromRgb(217, 197, 160);
            ColorBrush50 = new SolidColorBrush();
            ColorBrush50.Color = Color50;

            Color51 = new Color();
            Color51 = Color.FromRgb(242, 224, 189);
            ColorBrush51 = new SolidColorBrush();
            ColorBrush51.Color = Color51;

            Color52 = new Color();
            Color52 = Color.FromRgb(191, 169, 142);
            ColorBrush52 = new SolidColorBrush();
            ColorBrush52.Color = Color52;

            Color53 = new Color();
            Color53 = Color.FromRgb(241, 190, 121);
            ColorBrush53 = new SolidColorBrush();
            ColorBrush53.Color = Color53;

            Color54 = new Color();
            Color54 = Color.FromRgb(216, 145, 94);
            ColorBrush54 = new SolidColorBrush();
            ColorBrush54.Color = Color54;

            Color60 = new Color();
            Color60 = Color.FromRgb(235, 239, 242);
            ColorBrush60 = new SolidColorBrush();
            ColorBrush60.Color = Color11;

            Color61 = new Color();
            Color61 = Color.FromRgb(117, 156, 191);
            ColorBrush61 = new SolidColorBrush();
            ColorBrush61.Color = Color61;

            Color62 = new Color();
            Color62 = Color.FromRgb(139, 187, 217);
            ColorBrush62 = new SolidColorBrush();
            ColorBrush62.Color = Color62;

            Color63 = new Color();
            Color63 = Color.FromRgb(182, 219, 242);
            ColorBrush63 = new SolidColorBrush();
            ColorBrush63.Color = Color63;

            Color64 = new Color();
            Color64 = Color.FromRgb(206, 232, 242);
            ColorBrush64 = new SolidColorBrush();
            ColorBrush64.Color = Color64;

            Color70 = new Color();
            Color70 = Color.FromRgb(12, 108, 130);
            ColorBrush70 = new SolidColorBrush();
            ColorBrush70.Color = Color70;

            Color71 = new Color();
            Color71 = Color.FromRgb(216, 67, 120);
            ColorBrush71 = new SolidColorBrush();
            ColorBrush71.Color = Color71;

            Color72 = new Color();
            Color72 = Color.FromRgb(99, 67, 86);
            ColorBrush72 = new SolidColorBrush();
            ColorBrush72.Color = Color72;

            Color73 = new Color();
            Color73 = Color.FromRgb(50, 163, 83);
            ColorBrush73 = new SolidColorBrush();
            ColorBrush73.Color = Color73;

            Color74 = new Color();
            Color74 = Color.FromRgb(238, 101, 75);
            ColorBrush74 = new SolidColorBrush();
            ColorBrush74.Color = Color74;
        }
        #endregion

        #region TERRAIN ENGINE PROCESSING
        public void StartColorization(int TerrainSize, float[] TerrainPoints, byte[] previousColorMap)
        {
            // Not multithreaded yet!
            this.TerrainSize = TerrainSize;
            this.TerrainPoints = TerrainPoints;
            this.previousColorMap = previousColorMap;

            ColorizeCalculate();

            Processed();
            terrainEngine.SingleLayerCalculationComplete(this, TerrainMainColors, null, ColorMap);
        }

        public void ColorizeCalculate()
        {
            Color currentSelectedColor = GetCurrentColor();

            #region ColorMap                                                                       
            PixelFormat pixelFormat = PixelFormats.Bgr24;
            int rawStride = (TerrainSize * pixelFormat.BitsPerPixel + 7) / 8;
            ColorMap = new byte[rawStride * TerrainSize];

            int count = 0;
            for (int x = 0; x < TerrainSize; x++)
            {
                for (int z = 0; z < TerrainSize; z++)
                {
                    float randomizedHeight = TerrainPoints[z + x * TerrainSize] + (float)((osn.Evaluate(x * 0.2 * HeightNoiseScale, z * 0.2 * HeightNoiseScale) - 0.5) * HeightNoiseAmount * 0.15);
                    if (randomizedHeight >= MinHeight && randomizedHeight <= MaxHeight)
                    {

                        byte randomColorOffset = (byte)_random.Next(16);
                        byte[] BGR = new byte[3];
                        BGR[0] = currentSelectedColor.B;
                        BGR[1] = currentSelectedColor.G;
                        BGR[2] = currentSelectedColor.R;

                        if (BGR[0] + (byte)(randomColorOffset * ColorNoise) < 255)
                        {
                            BGR[0] += (byte)(randomColorOffset * ColorNoise);
                        }
                        if (BGR[1] + (byte)(randomColorOffset * ColorNoise) < 255)
                        {
                            BGR[1] += (byte)(randomColorOffset * ColorNoise);
                        }
                        if (BGR[2] + (byte)(randomColorOffset * ColorNoise) < 255)
                        {
                            BGR[2] += (byte)(randomColorOffset * ColorNoise);
                        }
                        for (int i = 0; i < 3; i++)
                        {
                            ColorMap[count] = BGR[i];
                            count++;
                        }
                    }
                    else
                    {
                        if (previousColorMap == null)
                        {
                            byte[] BGR = new byte[3];
                            BGR[0] = 155;
                            BGR[1] = 155;
                            BGR[2] = 155;
                            for (int i = 0; i < 3; i++)
                            {
                                ColorMap[count] = BGR[i];
                                count++;
                            }
                        }
                        else
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                ColorMap[count] = previousColorMap[count];
                                count++;
                            }
                        }

                    }
                }
            }


            BitmapSource bitmap = BitmapSource.Create(TerrainSize, TerrainSize, 96, 96, pixelFormat, null, ColorMap, rawStride);
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            TerrainMainColors = new MemoryStream();

            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            encoder.Save(TerrainMainColors);

            TerrainMainColors.Position = 0;
            #endregion
        }

        public Color GetCurrentColor()
        {
            Color currentSelectedColor = new Color();

            if (Color10Bool)
            {
                currentSelectedColor = Color10;
            }
            else if (Color11Bool)
            {
                currentSelectedColor = Color11;
            }
            else if (Color12Bool)
            {
                currentSelectedColor = Color12;
            }
            else if (Color13Bool)
            {
                currentSelectedColor = Color13;
            }
            else if (Color14Bool)
            {
                currentSelectedColor = Color14;
            }
            else if (Color20Bool)
            {
                currentSelectedColor = Color20;
            }
            else if (Color21Bool)
            {
                currentSelectedColor = Color21;
            }
            else if (Color22Bool)
            {
                currentSelectedColor = Color22;
            }
            else if (Color23Bool)
            {
                currentSelectedColor = Color23;
            }
            else if (Color24Bool)
            {
                currentSelectedColor = Color24;
            }
            else if (Color30Bool)
            {
                currentSelectedColor = Color30;
            }
            else if (Color31Bool)
            {
                currentSelectedColor = Color31;
            }
            else if (Color32Bool)
            {
                currentSelectedColor = Color32;
            }
            else if (Color33Bool)
            {
                currentSelectedColor = Color33;
            }
            else if (Color34Bool)
            {
                currentSelectedColor = Color34;
            }
            else if (Color40Bool)
            {
                currentSelectedColor = Color40;
            }
            else if (Color41Bool)
            {
                currentSelectedColor = Color41;
            }
            else if (Color42Bool)
            {
                currentSelectedColor = Color42;
            }
            else if (Color43Bool)
            {
                currentSelectedColor = Color43;
            }
            else if (Color44Bool)
            {
                currentSelectedColor = Color44;
            }
            else if (Color50Bool)
            {
                currentSelectedColor = Color50;
            }
            else if (Color51Bool)
            {
                currentSelectedColor = Color51;
            }
            else if (Color52Bool)
            {
                currentSelectedColor = Color52;
            }
            else if (Color53Bool)
            {
                currentSelectedColor = Color53;
            }
            else if (Color54Bool)
            {
                currentSelectedColor = Color54;
            }
            else if (Color60Bool)
            {
                currentSelectedColor = Color60;
            }
            else if (Color61Bool)
            {
                currentSelectedColor = Color61;
            }
            else if (Color62Bool)
            {
                currentSelectedColor = Color62;
            }
            else if (Color63Bool)
            {
                currentSelectedColor = Color63;
            }
            else if (Color64Bool)
            {
                currentSelectedColor = Color64;
            }
            else if (Color70Bool)
            {
                currentSelectedColor = Color70;
            }
            else if (Color71Bool)
            {
                currentSelectedColor = Color71;
            }
            else if (Color72Bool)
            {
                currentSelectedColor = Color72;
            }
            else if (Color73Bool)
            {
                currentSelectedColor = Color73;
            }
            else if (Color74Bool)
            {
                currentSelectedColor = Color74;
            }

            return currentSelectedColor;
        }
        #endregion

    }
}
