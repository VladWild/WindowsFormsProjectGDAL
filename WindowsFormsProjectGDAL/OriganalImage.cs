﻿using OSGeo.GDAL;
using OSGeo.OGR;
using OSGeo.OSR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace WindowsFormsProjectGDAL
{
    public class OriganalImage
    {
        private const int numberImages = 4;

        private Dataset dataSet;

        private Band blue;
        private Band green;
        private Band red;

        private int width;
        private int height;

        private int bandCount;

        private byte[] blueBuffer;
        private byte[] greenBuffer;
        private byte[] redBuffer;

        private Bitmap blueBitmap;
        private Bitmap greenBitmap;
        private Bitmap redBitmap;
        private Bitmap rgbBitmap;

        public OriganalImage(String filename, ProgressBar progressBar1)
        {
            dataSet = Gdal.Open(filename, Access.GA_ReadOnly);

            blue = dataSet.GetRasterBand(3);
            green = dataSet.GetRasterBand(2);
            red = dataSet.GetRasterBand(1);

            width = dataSet.RasterXSize;
            height = dataSet.RasterYSize;
            bandCount = dataSet.RasterCount;

            blueBuffer = new byte[width * height];
            greenBuffer = new byte[width * height];
            redBuffer = new byte[width * height];

            blue.ReadRaster(0, 0, width, height, blueBuffer, width, height, 0, 0);
            green.ReadRaster(0, 0, width, height, greenBuffer, width, height, 0, 0);
            red.ReadRaster(0, 0, width, height, redBuffer, width, height, 0, 0);

            progressBar1.Maximum = numberImages * height;

            createBlueBitmap(progressBar1);
            createGreenBitmap(progressBar1);
            createRedBitmap(progressBar1);
            createRGBBitmap(progressBar1);

            progressBar1.Value = 0;

        }

        public OriganalImage(Bitmap image, ProgressBar progressBar1)
        {
            width = image.Width;
            height = image.Height;

            progressBar1.Maximum = image.Width * image.Height;

            copyBlueBitmap(image, progressBar1);
            copyGreenBitmap(image, progressBar1);
            copyRedBitmap(image, progressBar1);
            copyRGBBitmap(image);

            progressBar1.Value = 0;
        }

        //методы для 1 конструктора
        //--------------------------------------------------------------------------------

        private void createBlueBitmap(ProgressBar progressBar1)
        {
            blueBitmap = new Bitmap(width, height);

            int t = 0;

            for (var i = 0; i < blueBitmap.Height; i++)
            {
                for (var j = 0; j < blueBitmap.Width; j++)
                {
                    blueBitmap.SetPixel(j, i, Color.FromArgb(255, 0, 0, blueBuffer[t++]));
                }
                progressBar1.Value += 1;
            }

        }

        private void createGreenBitmap(ProgressBar progressBar1)
        {
            greenBitmap = new Bitmap(width, height);

            int t = 0;

            for (var i = 0; i < greenBitmap.Height; i++)
            {
                for (var j = 0; j < greenBitmap.Width; j++)
                {
                    greenBitmap.SetPixel(j, i, Color.FromArgb(255, 0, greenBuffer[t++], 0));
                }
                progressBar1.Value += 1;
            }

        }

        private void createRedBitmap(ProgressBar progressBar1)
        {
            redBitmap = new Bitmap(width, height);

            int t = 0;

            for (var i = 0; i < redBitmap.Height; i++)
            {
                for (var j = 0; j < redBitmap.Width; j++)
                {
                    redBitmap.SetPixel(j, i, Color.FromArgb(255, redBuffer[t++], 0, 0));
                }
                progressBar1.Value += 1;
            }
                
        }

        private void createRGBBitmap(ProgressBar progressBar1)
        {
            rgbBitmap = new Bitmap(width, height);

            int t = 0;

            for (var i = 0; i < redBitmap.Height; i++)
            {
                for (var j = 0; j < redBitmap.Width; j++, t++)
                {
                    rgbBitmap.SetPixel(j, i, Color.FromArgb(255, redBuffer[t], greenBuffer[t], blueBuffer[t]));
                }
                progressBar1.Value += 1;
            }
        }

        //методы для 2 конструктора
        //-----------------------------------------------------------------------------------

        private void copyBlueBitmap(Bitmap image, ProgressBar progressBar1)
        {
            blueBitmap = new Bitmap(width, height);

            for (var i = 0; i < blueBitmap.Height; i++)
            {
                for (var j = 0; j < blueBitmap.Width; j++)
                {
                    blueBitmap.SetPixel(j, i, Color.FromArgb(255, 0, 0,
                        (int) ((UInt32)(image.GetPixel(j, i).ToArgb()) & 0x000000FF)));
                }
                progressBar1.Value += 1;
            }
        }

        private void copyGreenBitmap(Bitmap image, ProgressBar progressBar1)
        {
            greenBitmap = new Bitmap(width, height);

            for (var i = 0; i < greenBitmap.Height; i++)
            {
                for (var j = 0; j < greenBitmap.Width; j++)
                {
                    greenBitmap.SetPixel(j, i, Color.FromArgb(255, 0, 
                        (int) (((UInt32)(image.GetPixel(j, i).ToArgb()) & 0x0000FF00) >> 8), 0));
                }
                progressBar1.Value += 1;
            }
        }

        private void copyRedBitmap(Bitmap image, ProgressBar progressBar1)
        {
            redBitmap = new Bitmap(width, height);

            for (var i = 0; i < redBitmap.Height; i++)
            {
                for (var j = 0; j < redBitmap.Width; j++)
                {
                    redBitmap.SetPixel(j, i, Color.FromArgb(255, 
                        (int) (((UInt32)(image.GetPixel(j, i).ToArgb()) & 0x00FF0000) >> 16), 0, 0));
                }
                progressBar1.Value += 1;
            }
        }

        private void copyRGBBitmap(Bitmap image)
        {
            rgbBitmap = (Bitmap) image.Clone();
        }

        //получение изображения и его каналов
        //--------------------------------------------------------------------------------

        public Bitmap getBlueBitmap()
        {
            return blueBitmap;
        }

        public Bitmap getGreenBitmap()
        {
            return greenBitmap;
        }

        public Bitmap getRedBitmap()
        {
            return redBitmap;
        }

        public Bitmap getRGBBitmap()
        {
            return rgbBitmap;
        }


    }
}
