using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsProjectGDAL
{
    class Correlator
    {
        private static int i0;
        private static int j0;
        private static int iFanal;
        private static int jFanal;
        private static Rectangle rectSearch;

        private static void coordinates(Bitmap image, Bitmap model, Rectangle rect)
        {
            if (rect.X == 0 || rect.Y == 0)
            {
                i0 = 0;
                j0 = 0;
                iFanal = image.Width;
                jFanal = image.Height;
                rectSearch = new Rectangle(0, 0, image.Width, image.Height);
            }
            else
            {
                i0 = rect.X;
                j0 = rect.Y;
                iFanal = rect.X + rect.Width;
                jFanal = rect.Y + rect.Height;
                rectSearch = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
            }
        }

        //классическая корреляция с нормированием
        public static Point classicNorm(Colors color, Bitmap image, Bitmap model, Rectangle rect, ProgressBar progressBar1)
        {
            Point point = new Point();

            double F = 0;
            double F2 = 0;
            double F3 = 0;
            double F4 = 0;
            double Fmax = double.MinValue;

            coordinates(image, model, rect);

            //создание двумерного массива пикселей яркостей определенного
            //канала для более быстрых вычислений
            //----------------------------------------------------------------------------
            byte[,] searchByte = new byte[rectSearch.Width, rectSearch.Height];
            byte[,] modelByte = new byte[model.Width, model.Height];

            int i2 = 0;
            int j2 = 0;

            for (int i = i0; i < iFanal; i++, i2++)
            {
                for (int j = j0; j < jFanal; j++, j2++)
                {
                    searchByte[i2, j2] = (byte) (((UInt32)image.GetPixel(i, j).ToArgb()) & 0x000000FF);
                }
                j2 = 0;
            }

            for (int i = 0; i < model.Width; i++)
            {
                for (int j = 0; j < model.Height; j++)
                {
                    if (color == Colors.BLUE) modelByte[i, j] = ((byte)(((UInt32)model.GetPixel(i, j).ToArgb()) & 0x000000FF));
                    if (color == Colors.GREEN) modelByte[i, j] = ((byte)((((UInt32)model.GetPixel(i, j).ToArgb()) & 0x0000FF00) >> 8));
                    if (color == Colors.RED) modelByte[i, j] = ((byte)((((UInt32)model.GetPixel(i, j).ToArgb()) & 0x00FF0000) >> 16));
                }
            }
            //--------------------------------------------------------------------------

            progressBar1.Minimum = 0;
            progressBar1.Maximum = rectSearch.Width;
            progressBar1.Value = 0;

            for (int i = 0; i < rectSearch.Width; i++)
            {
                for (int j = 0; j < rectSearch.Height; j++)
                {
                    for (int i4 = 0; i4 < model.Width; i4++)
                    {
                        for (int j4 = 0; j4 < model.Height; j4++)
                        {
                            F2 += modelByte[i4, j4] * searchByte[i, j];
                            F3 += modelByte[i4, j4] * modelByte[i4, j4];
                            F4 += searchByte[i, j] * searchByte[i, j];
                        }
                    }
                    F = F2 / (Math.Pow(F3, 0.5) * Math.Pow(F4, 0.5));
                    if (F > Fmax)
                    {
                        Fmax = F;
                        point.X = rectSearch.X + i;
                        point.Y = rectSearch.Y + j;
                    }
                    F = 0;
                    F2 = 0;
                    F3 = 0;
                    F4 = 0;
                }
                ++progressBar1.Value;
            }
            progressBar1.Value = 0;
            return point;
        }

        //разностная корреляция с модулем
        public static Point diffAbs(Colors color, Bitmap image, Bitmap model, Rectangle rect, ProgressBar progressBar1)
        {
            Point point = new Point();

            int F = 0;
            int Fmin = int.MaxValue;

            coordinates(image, model, rect);

            progressBar1.Minimum = 0;
            progressBar1.Maximum = iFanal - i0;
            progressBar1.Value = 0;

            byte pixel = 0;

            for (int i = i0; i < iFanal; i++)
            {
                for (int j = j0; j < jFanal; j++)
                {
                    for (int i2 = 0; i2 < model.Width; i2++)
                    {
                        for (int j2 = 0; j2 < model.Height; j2++)
                        {
                            if (color == Colors.BLUE) pixel = ((byte)( ((UInt32) model.GetPixel(i2, j2).ToArgb()) & 0x000000FF));
                            if (color == Colors.GREEN) pixel = ((byte)( ( ((UInt32)model.GetPixel(i2, j2).ToArgb()) & 0x0000FF00) >> 8) );
                            if (color == Colors.RED) pixel = ((byte)((((UInt32)model.GetPixel(i2, j2).ToArgb()) & 0x00FF0000) >> 16));
                            F += Math.Abs(((byte)(((UInt32)image.GetPixel(i + i2, j + j2).ToArgb()) & 0x000000FF)) - pixel);
                        }
                    }
                    if (F < Fmin)
                    {
                        Fmin = F;
                        point.X = i;
                        point.Y = j;
                    }
                    F = 0;
                }
                ++progressBar1.Value;
            }
            progressBar1.Value = 0;
            return point;
        }

        //разностная корреляция с квадратом
        public static Point diffSqr(Colors color, Bitmap image, Bitmap model, Rectangle rect, ProgressBar progressBar1)
        {
            Point point = new Point();

            double F = 0;
            double Fmin = double.MaxValue;

            coordinates(image, model, rect);

            progressBar1.Minimum = 0;
            progressBar1.Maximum = iFanal - i0;
            progressBar1.Value = 0;

            byte pixel = 0;

            for (int i = i0; i < iFanal; i++)
            {
                for (int j = j0; j < jFanal; j++)
                {
                    for (int i2 = 0; i2 < model.Width; i2++)
                    {
                        for (int j2 = 0; j2 < model.Height; j2++)
                        {
                            if (color == Colors.BLUE) pixel = ((byte)(((UInt32)model.GetPixel(i2, j2).ToArgb()) & 0x000000FF));
                            if (color == Colors.GREEN) pixel = ((byte)((((UInt32)model.GetPixel(i2, j2).ToArgb()) & 0x0000FF00) >> 8));
                            if (color == Colors.RED) pixel = ((byte)((((UInt32)model.GetPixel(i2, j2).ToArgb()) & 0x00FF0000) >> 16));
                            F += Math.Pow((((byte)(((UInt32)image.GetPixel(i + i2, j + j2).ToArgb()) & 0x000000FF)) - pixel), 2);
                        }
                    }
                    if (F < Fmin)
                    {
                        Fmin = F;
                        point.X = i;
                        point.Y = j;
                    }
                    F = 0;
                }
                ++progressBar1.Value;
            }
            progressBar1.Value = 0;
            return point;
        }

    }
}
