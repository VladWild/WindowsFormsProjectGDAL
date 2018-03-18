using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsProjectGDAL
{
    class PyramidImage
    {
        private Bitmap original;

        private Bitmap pyramidDecimation;
        private Bitmap pyramidAvareging;
        private Bitmap pyramidGaussian;

        private int step;

        public PyramidImage(Bitmap original)
        {
            this.original = original;
        }

        private byte getRed(int x, int y)
        {
            return ((byte)((((UInt32)original.GetPixel(x, y).ToArgb()) & 0x00FF0000) >> 16));
        }

        private byte getGreen(int x, int y)
        {
            return ((byte)((((UInt32)original.GetPixel(x, y).ToArgb()) & 0x0000FF00) >> 8));
        }

        private byte getBlue(int x, int y)
        {
            return ((byte)(((UInt32)original.GetPixel(x, y).ToArgb()) & 0x000000FF));
        }

        public void decimationPixels(int step, ProgressBar progressBar1)
        {
            this.step = step;

            pyramidDecimation = new Bitmap(original.Width / step, original.Height / step);

            progressBar1.Maximum = pyramidDecimation.Height;

            byte red;
            byte green;
            byte blue;

            for (int i = 0; i < pyramidDecimation.Height; i++)
            {
                for (int j = 0; j < pyramidDecimation.Width; j++)
                {
                    red = getRed(j * step, i * step);
                    green = getGreen(j * step, i * step);
                    blue = getBlue(j * step, i * step);

                    pyramidDecimation.SetPixel(j, i, Color.FromArgb(red, green, blue));
                }
                progressBar1.Value += 1;
            }

            progressBar1.Value = 0;
        }

        public void avaregingPixels(int step, int size, ProgressBar progressBar1)
        {
            this.step = step;

            pyramidAvareging = new Bitmap(original.Width / step, original.Height / step);

            progressBar1.Maximum = pyramidAvareging.Height;

            int[] redPixels = new int[size * size];
            int[] greenPixels = new int[size * size];
            int[] bluePixels = new int[size * size];

            int redSum = 0;
            int greenSum = 0;
            int blueSum = 0;

            byte red;
            byte green;
            byte blue;

            int t;

            for (int i = 0; i < pyramidAvareging.Height; i++)
            {
                for (int j = 0; j < pyramidAvareging.Width; j++)
                {
                    if ((i + size < pyramidAvareging.Height) && (j + size < pyramidAvareging.Width))
                    {
                        t = 0;

                        for (int i2 = 0; i2 < size; i2++)
                        {
                            for (int j2 = 0; j2 < size; j2++, t++)
                            {
                                redPixels[t] = getRed(j * step + j2, i * step + i2);
                                greenPixels[t] = getGreen(j * step + j2, i * step + i2);
                                bluePixels[t] = getBlue(j * step + j2, i * step + i2);
                            }
                        }

                        for (int l = 0; l < redPixels.Length; l++)
                        {
                            redSum += redPixels[l];
                            greenSum += greenPixels[l];
                            blueSum += bluePixels[l];
                        }

                        red = (byte) (redSum / redPixels.Length);
                        green = (byte) (greenSum / greenPixels.Length);
                        blue = (byte) (blueSum / bluePixels.Length);

                        redSum = 0;
                        greenSum = 0;
                        blueSum = 0;

                        pyramidAvareging.SetPixel(j, i, Color.FromArgb(red, green, blue));
                    }
                }
                progressBar1.Value += 1;
            }
            progressBar1.Value = 0;
        }

        private double gaussian(double sko, int dx, int dy)
        {
            return Math.Exp(-((Math.Pow(dx, 2) + Math.Pow(dy, 2)) / (2 * Math.Pow(sko, 2)))) / (2 * Math.PI * Math.Pow(sko, 2));
        }

        public void gaussianFilter(int step, int size, double sko, ProgressBar progressBar1)
        {
            this.step = step;

            int matrixShift = (size - 1) / 2;
            int shift = matrixShift + 1;

            pyramidGaussian = new Bitmap(original.Width / step - matrixShift,
                original.Height / step - matrixShift);

            progressBar1.Maximum = pyramidGaussian.Height;

            double red = 0;
            double green = 0;
            double blue = 0;

            for (int i = 0; i < pyramidGaussian.Height; i++)
            {
                for (int j = 0; j < pyramidGaussian.Width; j++)
                {

                    for (int i2 = -matrixShift; i2 < matrixShift + 1; i2++)
                    {
                        for (int j2 = -matrixShift; j2 < matrixShift + 1; j2++)
                        {
                            red += (((double) getRed(j * step + shift + j2, i * step + shift + i2)) * gaussian(sko, i2, j2));
                            green += (((double) getGreen(j * step + shift + j2, i * step + shift + i2)) * gaussian(sko, i2, j2));
                            blue += (((double) getBlue(j * step + shift + j2, i * step + shift + i2)) * gaussian(sko, i2, j2));
                        }
                    }

                    if (red >= 256) red = 255;
                    if (green >= 256) green = 255;
                    if (blue >= 256) blue = 255;

                    pyramidGaussian.SetPixel(j, i, Color.FromArgb((int) red, (int) green, (int) blue));

                    red = 0;
                    green = 0;
                    blue = 0;
                }
                progressBar1.Value += 1;
            }
            progressBar1.Value = 0;
        }

        public Bitmap getOriginal()
        {
            return original;
        }


        public Bitmap getPyramidDecimation()
        {
            return pyramidDecimation;
        }

        public Bitmap getPyramidAvareging()
        {
            return pyramidAvareging;
        }

        public Bitmap getPyramidGaussian()
        {
            return pyramidGaussian;
        }

    }
}

