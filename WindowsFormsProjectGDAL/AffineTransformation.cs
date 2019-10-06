using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsProjectGDAL
{
    class AffineTransformation
    {
        struct info{
            public int x0;
            public int y0;

            public double u;
            public double v;

            public double m0;
            public double m1;
            public double m2;
            public double m3;
        }

        private Bitmap imageOriginal;
        private Bitmap imageTransform;

        public AffineTransformation(Bitmap imageOriginal)
        {
            this.imageOriginal = (Bitmap) imageOriginal.Clone();
        }

        //получение x и y по формулам афинного преобразования 
        //---------------------------------------------------------------------------------------

        private double getX(double a0, double a1, double a2, double x_, double y_)
        {
            return a0 + a1 * x_ + a2 * y_;
        }

        private double getY(double b0, double b1, double b2, double x_, double y_)
        {
            return b0 + b1 * x_ + b2 * y_;
        }

        //координаты не вышли за пределы изображения
        //---------------------------------------------------------------------------------------

        private bool isWithInImage(Bitmap image, int x, int y)
        {
            return (x >= image.Width || x < 0 || y >= image.Height || y < 0);
        }

        //1) получение пикселей из исходного изображения отсечением 
        //---------------------------------------------------------------------------------------

        private byte getRed(double x, double y)
        {
            return ((byte)((((UInt32)imageOriginal.GetPixel((int) x, (int) y).ToArgb()) & 0x00FF0000) >> 16));
        }

        private byte getGreen(double x, double y)
        {
            return ((byte)((((UInt32)imageOriginal.GetPixel((int) x, (int) y).ToArgb()) & 0x0000FF00) >> 8));
        }

        private byte getBlue(double x, double y)
        {
            return ((byte)(((UInt32)imageOriginal.GetPixel((int) x, (int) y).ToArgb()) & 0x000000FF));
        }

        //2) получение пикселей из исходного изображения с помощью билинейной интерполяции  
        //---------------------------------------------------------------------------------------

        //проверка условия нахождения всех пикселей на исходном изображении
        private bool isOrgImg(double x, double y)
        {
            return x >= 1 && y >= 1 && x < imageOriginal.Width - 1 && y < imageOriginal.Height - 1;
        }

        //получение структуры
        private info getInfo(double x, double y)
        {
            info a = new info();

            a.x0 = (int)x;
            a.y0 = (int)y;

            a.u = x - a.x0;
            a.v = y - a.y0;

            a.m0 = (1d - a.u) * (1d - a.v);
            a.m1 = a.u * (1 - a.v);
            a.m2 = a.u * a.v;
            a.m3 = a.v * (1 - a.u);

            return a;
        }

        //получение пикселей определенный цветов 
        //-----------------------------------------------------------------

        private byte getRed_BI(double x, double y)
        {
            if (isOrgImg(x, y))
            {
                info a = getInfo(x, y);

                double value = a.m0 * getRed(a.x0, a.y0) + a.m1 * getRed(a.x0 + 1, a.y0) +
                    a.m2 * getRed(a.x0 + 1, a.y0 + 1) + a.m3 * getRed(a.x0, a.y0 + 1);

                return (byte) value;
            }

            return getRed(x, y);
        }

        private byte getGreen_BI(double x, double y)
        {
            if (isOrgImg(x, y))
            {
                info a = getInfo(x, y);

                double value = a.m0 * getGreen(a.x0, a.y0) + a.m1 * getGreen(a.x0 + 1, a.y0) +
                    a.m2 * getGreen(a.x0 + 1, a.y0 + 1) + a.m3 * getGreen(a.x0, a.y0 + 1);

                return (byte) value;
            }

            return getGreen(x, y);
        }

        private byte getBlue_BI(double x, double y)
        {
            if (isOrgImg(x, y))
            {
                info a = getInfo(x, y);

                double value = a.m0 * getBlue(a.x0, a.y0) + a.m1 * getBlue(a.x0 + 1, a.y0) +
                    a.m2 * getBlue(a.x0 + 1, a.y0 + 1) + a.m3 * getBlue(a.x0, a.y0 + 1);

                return (byte)value;
            }

            return getBlue(x, y);
        }

        //афинное преобразование
        //---------------------------------------------------------------------------------------

        public void transform(double a0, double a1, double a2, double b0, double b1, double b2, bool flag)
        {
            imageTransform = new Bitmap(2 * imageOriginal.Width, 2 * imageOriginal.Height);

            double x = 0d;
            double y = 0d;

            byte red;
            byte green;
            byte blue;

            for (int y_ = 0; y_ < imageTransform.Height; y_++)
            {
                for (int x_ = 0; x_ < imageTransform.Width; x_++)
                {
                    //получение x и y расположения нужного пикселя на оригинальном изображении 
                    x = getX(a0, a1, a2, x_, y_);   
                    y = getY(b0, b1, b2, x_, y_);

                    if (isWithInImage(imageOriginal, (int) x, (int) y)) continue;

                    if (flag)
                    {
                        red = getRed_BI(x, y);
                        green = getGreen_BI(x, y);
                        blue = getBlue_BI(x, y);
                    } else
                    {
                        red = getRed(x, y);
                        green = getGreen(x, y);
                        blue = getBlue(x, y);
                    }

                    imageTransform.SetPixel(x_, y_, Color.FromArgb(red, green, blue));
                }
            }
        }

        //получение изображения 
        //----------------------------------------------------------------------------

        public Bitmap getTransformImage()
        {
            return imageTransform;
        }
    }
}


