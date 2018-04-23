using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsProjectGDAL
{
    class Correlator
    {

        private struct info
        {
            public double mean;
            public double sko;
        }

        private static Rectangle rectSearch;
        private static byte[,] searchByte;
        private static byte[,] modelByte;

        private static double[,] function;
        private static Point pointPosition = new Point();

        //возвращает прямоугольник области поиска
        private static Rectangle getRectSearch(Bitmap image, Rectangle rect)
        {
            if (rect.X == 0 || rect.Y == 0)
            {
                return new Rectangle(0, 0, image.Width, image.Height);
            }
            else
            {
                return new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
            }
        }

        //получение двумерного массива значения яркостей пикселей в области поиска
        private static void getSearchByteArray(Colors color, Bitmap image, Rectangle rect)
        {
            searchByte = new byte[rectSearch.Width, rectSearch.Height];

            int i2 = 0;
            int j2 = 0;

            for (int i = rect.X; i < rect.X + rectSearch.Width; i++, i2++)
            {
                for (int j = rect.Y; j < rect.Y + rectSearch.Height; j++, j2++)
                {
                    if (color == Colors.BLUE) searchByte[i2, j2] = (byte) (((UInt32)image.GetPixel(i, j).ToArgb()) & 0x000000FF);
                    if (color == Colors.GREEN) searchByte[i2, j2] = ((byte)((((UInt32)image.GetPixel(i, j).ToArgb()) & 0x0000FF00) >> 8));
                    if (color == Colors.RED) searchByte[i2, j2] = ((byte)((((UInt32)image.GetPixel(i, j).ToArgb()) & 0x00FF0000) >> 16));
                }
                j2 = 0;
            }

        }

        //получение двумерного массива значений яркостей пикселей модели
        private static void getModelByteArray(Colors color, Bitmap model, Rectangle rect)
        {
            modelByte = new byte[model.Width, model.Height];

            for (int i = 0; i < model.Width; i++)
            {
                for (int j = 0; j < model.Height; j++)
                {
                    if (color == Colors.BLUE) modelByte[i, j] = ((byte)(((UInt32)model.GetPixel(i, j).ToArgb()) & 0x000000FF));
                    if (color == Colors.GREEN) modelByte[i, j] = ((byte)((((UInt32)model.GetPixel(i, j).ToArgb()) & 0x0000FF00) >> 8));
                    if (color == Colors.RED) modelByte[i, j] = ((byte)((((UInt32)model.GetPixel(i, j).ToArgb()) & 0x00FF0000) >> 16));
                }
            }
        }

        private static void setingProgressBar(ProgressBar progressBar1, Bitmap model)
        {
            progressBar1.Minimum = 0;
            progressBar1.Maximum = rectSearch.Width - model.Width;
            progressBar1.Value = 0;
        }

        //вычисление ско яркости фрагментов изображений
        private static double sko(byte[,] array, int i0, int j0, int Width, int Heigth, double mean)
        {
            double sum = 0;
            double sko = 0;
            double n = Width * Heigth - 1;

            for (int i = i0; i < i0 + Width; i++)
            {
                for (int j = j0; j < j0 + Heigth; j++)
                {
                    sum += Math.Pow(array[i, j] - mean, 2);
                }
            }

            sko = sum / n;
            sko = Math.Pow(sko, 0.5);

            return sko;
        }

        //вычисление математического ожидания и ско яркости из фрагментов массива
        private static info meanSko(byte[,] array, int i0, int j0, int Width, int Heigth)
        {
            info info;

            double sum = 0;
            double sko = 0;
            double mean = 0;
            double n = Width * Heigth;

            for (int i = i0; i < i0 + Width; i++)
            {
                for (int j = j0; j < j0 + Heigth; j++)
                {
                    mean += array[i, j];
                    sum += array[i, j] * array[i, j];
                }
            }

            mean /= n;
            sko = sum / n - mean * mean;
            sko = Math.Pow(sko, 0.5);

            info.mean = mean;
            info.sko = sko;

            return info;
        }

        //классическая корреляция с нормированием
        public static Point classicNorm(Colors color, Colors colorModel, Bitmap image, Bitmap model, 
            Rectangle rect, ProgressBar progressBar1, Stopwatch sWatch, Label label23)
        {
            function = new double[image.Width - model.Width, image.Height - model.Width];   //массив значений корреляционной функции
            List<double> list = new List<double>();     //временно

            Point point = new Point();

            double F = 0;
            double F2 = 0;
            double F3 = 0;
            double F4 = 0;
            double Fmax = double.MinValue;

            rectSearch = getRectSearch(image, rect);

            getSearchByteArray(color, image, rect);
            getModelByteArray(colorModel, model, rect);

            setingProgressBar(progressBar1, model);

            sWatch.Start();
            for (int i = 0; i < rectSearch.Width - model.Width; i++)
            {
                for (int j = 0; j < rectSearch.Height - model.Height; j++)
                {
                    for (int i2 = 0; i2 < model.Width; i2++)
                    {
                        for (int j2 = 0; j2 < model.Height; j2++)
                        {
                            F2 += modelByte[i2, j2] * searchByte[i + i2, j + j2];
                            F3 += modelByte[i2, j2] * modelByte[i2, j2];
                            F4 += searchByte[i + i2, j + j2] * searchByte[i + i2, j + j2];
                        }
                    }
                    F = F2 / (Math.Pow(F3, 0.5) * Math.Pow(F4, 0.5));
                    function[i, j] = F;     //корреляционная функция
                    list.Add(F);    //временно
                    if (F > Fmax)
                    {
                        Fmax = F;
                        point.X = rectSearch.X + i;
                        point.Y = rectSearch.Y + j;
                        pointPosition.X = j;        //положение x - максимума в корреляционной функции
                        pointPosition.Y = i;        //положение y - максимума в корреляционной функции
                    }
                    F = 0;
                    F2 = 0;
                    F3 = 0;
                    F4 = 0;
                }
                ++progressBar1.Value;
            }
            sWatch.Stop();
            progressBar1.Value = 0;
            label23.Text = "max = " + Fmax.ToString();
            list.Clear();
            return point;
        }

        //разностная корреляция с модулем
        public static Point diffAbs(Colors color, Colors colorModel, Bitmap image, Bitmap model, 
            Rectangle rect, ProgressBar progressBar1, Stopwatch sWatch, Label label23)
        {
            function = new double[image.Width - model.Width, image.Height - model.Width];   //массив значений корреляционной функции

            Point point = new Point();

            double F = 0;
            double Fmin = double.MaxValue;

            rectSearch = getRectSearch(image, rect);

            getSearchByteArray(color, image, rect);
            getModelByteArray(colorModel, model, rect);

            setingProgressBar(progressBar1, model);

            sWatch.Start();
            for (int i = 0; i < rectSearch.Width - model.Width; i++)
            {
                for (int j = 0; j < rectSearch.Height - model.Height; j++)
                {
                    for (int i2 = 0; i2 < model.Width; i2++)
                    {
                        for (int j2 = 0; j2 < model.Height; j2++)
                        {
                            F += Math.Abs(searchByte[i + i2, j + j2] - modelByte[i2, j2]);
                        }
                    }
                    function[i, j] = F;     //корреляционная функция
                    if (F < Fmin)
                    {
                        Fmin = F;
                        point.X = rectSearch.X + i;
                        point.Y = rectSearch.Y + j;
                        pointPosition.X = j;        //положение x - минимума в корреляционной функции
                        pointPosition.Y = i;        //положение y - минимума в корреляционной функции
                    }
                    F = 0;
                }
                ++progressBar1.Value;
            }
            sWatch.Stop();
            progressBar1.Value = 0;
            label23.Text = "min = " + Fmin.ToString();
            return point;
        }

        //разностная корреляция с квадратом
        public static Point diffSqr(Colors color, Colors colorModel, Bitmap image, Bitmap model, 
            Rectangle rect, ProgressBar progressBar1, Stopwatch sWatch, Label label23)
        {
            function = new double[image.Width - model.Width, image.Height - model.Width];   //массив значений корреляционной функции

            Point point = new Point();

            double F = 0;
            double Fmin = double.MaxValue;

            rectSearch = getRectSearch(image, rect);

            getSearchByteArray(color, image, rect);
            getModelByteArray(colorModel, model, rect);

            setingProgressBar(progressBar1, model);

            sWatch.Start();
            for (int i = 0; i < rectSearch.Width - model.Width; i++)
            {
                for (int j = 0; j < rectSearch.Height - model.Height; j++)
                {
                    for (int i2 = 0; i2 < model.Width; i2++)
                    {
                        for (int j2 = 0; j2 < model.Height; j2++)
                        {
                            F += (searchByte[i + i2, j + j2] - modelByte[i2, j2]) * 
                                (searchByte[i + i2, j + j2] - modelByte[i2, j2]);
                        }
                    }
                    function[i, j] = F;     //корреляционная функция
                    if (F < Fmin)
                    {
                        Fmin = F;
                        point.X = rectSearch.X + i;
                        point.Y = rectSearch.Y + j;
                        pointPosition.X = j;        //положение x - минимума в корреляционной функции
                        pointPosition.Y = i;        //положение y - минимума в корреляционной функции
                    }
                    F = 0;
                }
                ++progressBar1.Value;
            }
            sWatch.Stop();
            progressBar1.Value = 0;
            label23.Text = "min = " + Fmin.ToString();
            return point;
        }

        //вычисление математического ожидания яркости из фрагмента массива
        private static double mean(byte [,] array, int i0, int j0, int Width, int Heigth)
        {
            double mean = 0;
            double n = Width * Heigth;

            for (int i = i0; i < i0 + Width; i++)
            {
                for (int j = j0; j < j0 + Heigth; j++)
                {
                    mean += array[i, j]; 
                }
            }
            mean /= n;

            return mean;
        }

        //корреляция с нормированием new
        public static Point normCorrilation(Colors color, Colors colorModel, Bitmap image, Bitmap model, 
            Rectangle rect, ProgressBar progressBar1, Stopwatch sWatch, Label label23)
        {
            function = new double[image.Width - model.Width, image.Height - model.Width];   //массив значений корреляционной функции

            Point point = new Point();

            double F = 0;
            double Fmax = double.MinValue;

            info infoModel;
            info infoImage;

            rectSearch = getRectSearch(image, rect);

            getSearchByteArray(color, image, rect);
            getModelByteArray(colorModel, model, rect);

            setingProgressBar(progressBar1, model);

            sWatch.Start();

            double L = model.Width * model.Height;
            infoModel = meanSko(modelByte, 0, 0, model.Width, model.Height);

            for (int i = 0; i < rectSearch.Width - model.Width; i++)
            {
                for (int j = 0; j < rectSearch.Height - model.Height; j++)
                {
                    infoImage = meanSko(searchByte, i, j, model.Width, model.Height);
                    for (int i2 = 0; i2 < model.Width; i2++)
                    {
                        for (int j2 = 0; j2 < model.Height; j2++)
                        {
                            F += modelByte[i2, j2] * searchByte[i + i2, j + j2];
                        }
                    }
                    F /= L;
                    F -= infoModel.mean * infoImage.mean;
                    F = F / (infoModel.sko * infoImage.sko);
                    function[i, j] = F;     //корреляционная функция
                    if (F > Fmax)
                    {
                        Fmax = F;
                        point.X = rectSearch.X + i;
                        point.Y = rectSearch.Y + j;
                        pointPosition.X = j;        //положение x - максимума в корреляционной функции
                        pointPosition.Y = i;        //положение y - максимума в корреляционной функции
                    }
                    F = 0;
                    infoImage.mean = 0;
                    infoImage.sko = 0;
                }
                ++progressBar1.Value;
            }
            sWatch.Stop();
            progressBar1.Value = 0;
            label23.Text = "max = " + Fmax.ToString();
            return point;
        }

        public static MyPoint getPointSubShiftMax()
        {
            MyPoint myPoint = new MyPoint();

            double a1x = 0.5d * (function[pointPosition.Y, pointPosition.X + 1] - 
                function[pointPosition.Y, pointPosition.X - 1]);
            double a2x = 0.5d * (function[pointPosition.Y, pointPosition.X + 1] +
                function[pointPosition.Y, pointPosition.X - 1]);

            myPoint.x = 0.5d * (a1x / a2x);

            double a1y = 0.5d * (function[pointPosition.Y + 1, pointPosition.X] -
                function[pointPosition.Y - 1, pointPosition.X]);
            double a2y = 0.5d * (function[pointPosition.Y + 1, pointPosition.X] +
                function[pointPosition.Y - 1, pointPosition.X]);

            myPoint.y = 0.5d * (a1y / a2y);

            return myPoint;
        }

        public static MyPoint getPointSubShiftMin()
        {
            MyPoint myPoint = new MyPoint();

            double a1x = 0.5d * (function[pointPosition.Y, pointPosition.X + 1] -
                function[pointPosition.Y, pointPosition.X - 1]);
            double a2x = 0.5d * (function[pointPosition.Y, pointPosition.X + 1] +
                function[pointPosition.Y, pointPosition.X - 1]);

            myPoint.x = - 0.5d * (a1x / a2x);

            double a1y = 0.5d * (function[pointPosition.Y + 1, pointPosition.X] -
                function[pointPosition.Y - 1, pointPosition.X]);
            double a2y = 0.5d * (function[pointPosition.Y + 1, pointPosition.X] +
                function[pointPosition.Y - 1, pointPosition.X]);

            myPoint.y = - 0.5d * (a1y / a2y);

            return myPoint;
        }
    }

}
