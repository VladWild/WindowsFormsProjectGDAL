using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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

        private static bool isSaveFunction = true;

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
            function = new double[rect.Width - model.Width, rect.Height - model.Height];   //массив значений корреляционной функции

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
                    if (F > Fmax)
                    {
                        Fmax = F;
                        point.X = rectSearch.X + i;
                        point.Y = rectSearch.Y + j;
                        pointPosition.X = i;        //положение x - максимума в корреляционной функции
                        pointPosition.Y = j;        //положение y - максимума в корреляционной функции
                    }
                    F = 0;
                    F2 = 0;
                    F3 = 0;
                    F4 = 0;
                }
                ++progressBar1.Value;
            }
            sWatch.Stop();
            if (isSaveFunction) saveFunctionToFile("1_classic");
            if (isSaveFunction) saveFunctionMinToFile("1_classic_min");
            progressBar1.Value = 0;
            label23.Text = "max = " + Fmax.ToString();
            return point;
        }

        //разностная корреляция с модулем
        public static Point diffAbs(Colors color, Colors colorModel, Bitmap image, Bitmap model, 
            Rectangle rect, ProgressBar progressBar1, Stopwatch sWatch, Label label23)
        {
            function = new double[rect.Width - model.Width, rect.Height - model.Height];   //массив значений корреляционной функции

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
                        pointPosition.X = i;        //положение x - минимума в корреляционной функции
                        pointPosition.Y = j;        //положение y - минимума в корреляционной функции
                    }
                    F = 0;
                }
                ++progressBar1.Value;
            }
            sWatch.Stop();
            if (isSaveFunction) saveFunctionToFile("2_abs_diff");
            progressBar1.Value = 0;
            label23.Text = "min = " + Fmin.ToString();
            return point;
        }

        //разностная корреляция с квадратом
        public static Point diffSqr(Colors color, Colors colorModel, Bitmap image, Bitmap model, 
            Rectangle rect, ProgressBar progressBar1, Stopwatch sWatch, Label label23)
        {
            function = new double[rect.Width - model.Width, rect.Height - model.Height];   //массив значений корреляционной функции

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
                        pointPosition.X = i;        //положение x - минимума в корреляционной функции
                        pointPosition.Y = j;        //положение y - минимума в корреляционной функции
                    }
                    F = 0;
                }
                ++progressBar1.Value;
            }
            sWatch.Stop();
            if (isSaveFunction) saveFunctionToFile("3_sqrt_diff");
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
            function = new double[rect.Width - model.Width, rect.Height - model.Height];   //массив значений корреляционной функции

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
                        pointPosition.X = i;        //положение x - максимума в корреляционной функции
                        pointPosition.Y = j;        //положение y - максимума в корреляционной функции
                    }
                    F = 0;
                    infoImage.mean = 0;
                    infoImage.sko = 0;
                }
                ++progressBar1.Value;
            }
            sWatch.Stop();
            if (isSaveFunction) saveFunctionToFile("4_norm");
            if (isSaveFunction) saveFunctionMinToFile("4_norm_min");
            progressBar1.Value = 0;
            label23.Text = "max = " + Fmax.ToString();
            return point;
        }

        public static MyPoint getPointSubShiftMax()
        {
            double f1x = 1 - function[pointPosition.X - 1, pointPosition.Y];
            double f2x = 1 - function[pointPosition.X, pointPosition.Y];
            double f3x = 1 - function[pointPosition.X + 1, pointPosition.Y];

            double x1 = pointPosition.X - 1;
            double x2 = pointPosition.X;
            double x3 = pointPosition.X + 1;

            double f1y = 1 - function[pointPosition.X, pointPosition.Y - 1];
            double f2y = 1 - function[pointPosition.X, pointPosition.Y];
            double f3y = 1 - function[pointPosition.X, pointPosition.Y + 1];

            double y1 = pointPosition.Y - 1;
            double y2 = pointPosition.Y;
            double y3 = pointPosition.Y + 1;

            MyPoint myPoint = getNewPointByPowellMethod(f1x, f2x, f3x, x1, x2, x3,
                f1y, f2y, f3y, y1, y2, y3);

            return new MyPoint(pointPosition.X - myPoint.x, pointPosition.Y - myPoint.y);
        }

        public static MyPoint getPointSubShiftMin()
        {
            double f1x = function[pointPosition.X - 1, pointPosition.Y];
            double f2x = function[pointPosition.X, pointPosition.Y];
            double f3x = function[pointPosition.X + 1, pointPosition.Y];

            double x1 = pointPosition.X - 1;
            double x2 = pointPosition.X;
            double x3 = pointPosition.X + 1;

            double f1y = function[pointPosition.X, pointPosition.Y - 1];
            double f2y = function[pointPosition.X, pointPosition.Y];
            double f3y = function[pointPosition.X, pointPosition.Y + 1];

            double y1 = pointPosition.Y - 1;
            double y2 = pointPosition.Y;
            double y3 = pointPosition.Y + 1;

            MyPoint myPoint = getNewPointByPowellMethod(f1x, f2x, f3x, x1, x2, x3,
                f1y, f2y, f3y, y1, y2, y3);

            return new MyPoint(pointPosition.X - myPoint.x, pointPosition.Y - myPoint.y);
        }

        private static MyPoint getNewPointByPowellMethod(
            double f1x, double f2x, double f3x, double x1, double x2, double x3,
            double f1y, double f2y, double f3y, double y1, double y2, double y3)
        {
            MyPoint subPixelPoint = new MyPoint();

            subPixelPoint.x = 0.5d * ((x2 * x2 - x3 * x3) * f1x + (x3 * x3 - x1 * x1) * f2x + (x1 * x1 - x2 * x2) * f3x) /
                ((x2 - x3) * f1x + (x3 - x1) * f2x + (x1 - x2) * f3x);

            subPixelPoint.y = 0.5d * ((y2 * y2 - y3 * y3) * f1y + (y3 * y3 - y1 * y1) * f2y + (y1 * y1 - y2 * y2) * f3y) /
                ((y2 - y3) * f1y + (y3 - y1) * f2y + (y1 - y2) * f3y);

            return subPixelPoint;
        }

        //сохранение матрицы значений корреляционной функции в файл
        private static void saveFunctionToFile(string filename)
        {
            string path = @"D:\" + filename + ".txt";

            using (FileStream fstream = new FileStream(path, FileMode.OpenOrCreate))
            {
                int width = function.GetLength(0);
                int heigth = function.GetLength(1);

                string text = "";

                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < heigth; j++)
                    {
                        text = text + function[i, j].ToString().Replace(',', '.') + "; ";
                    }

                    text = text.TrimEnd(text[text.Length - 2]);

                    byte[] array4 = System.Text.Encoding.Default.GetBytes(text);
                    fstream.Write(array4, 0, array4.Length);

                    byte[] newline4 = Encoding.ASCII.GetBytes(Environment.NewLine);
                    fstream.Write(newline4, 0, newline4.Length);

                    text = "";
                }
            }
        }

        //сохранение матрицы значений корреляционной функции в файл
        private static void saveFunctionMinToFile(string filename)
        {
            string path = @"D:\" + filename + ".txt";

            using (FileStream fstream = new FileStream(path, FileMode.OpenOrCreate))
            {
                int width = function.GetLength(0);
                int heigth = function.GetLength(1);

                string text = "";

                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < heigth; j++)
                    {
                        text = text + (1 - function[i, j]).ToString().Replace(',', '.') + "; ";
                    }

                    text = text.TrimEnd(text[text.Length - 2]);

                    byte[] array4 = System.Text.Encoding.Default.GetBytes(text);
                    fstream.Write(array4, 0, array4.Length);

                    byte[] newline4 = Encoding.ASCII.GetBytes(Environment.NewLine);
                    fstream.Write(newline4, 0, newline4.Length);

                    text = "";
                }
            }
        }
    }

}
