using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsProjectGDAL
{
    class NewImage
    {
        private static byte[,] imageBlueByte;
        private static byte[,] imageGreenByte;
        private static byte[,] imageRedByte;

        public static Point bluePoint;
        public static Point greenPoint;
        public static Point redPoint;

        private static byte[,] getBitmapArray(Colors color, Bitmap bitmap)
        {
            byte[,] imageByte = new byte[bitmap.Width, bitmap.Height];

            for (int i = 0; i < bitmap.Height; i++)
            {
                for (int j = 0; j < bitmap.Width; j++)
                {
                    if (color == Colors.BLUE) imageByte[j, i] = ((byte)(((UInt32)bitmap.GetPixel(j, i).ToArgb()) & 0x000000FF));
                    if (color == Colors.GREEN) imageByte[j, i] = ((byte)((((UInt32)bitmap.GetPixel(j, i).ToArgb()) & 0x0000FF00) >> 8));
                    if (color == Colors.RED) imageByte[j, i] = ((byte)((((UInt32)bitmap.GetPixel(j, i).ToArgb()) & 0x00FF0000) >> 16));
                }
            }

            return imageByte;
        }

        public static Bitmap getNewImage(Bitmap blue, Bitmap green, Bitmap red)
        {
            Bitmap image = new Bitmap(blue.Width, blue.Height);

            Point dG = new Point(bluePoint.X - greenPoint.X, bluePoint.Y - greenPoint.Y);
            Point dR = new Point(bluePoint.X - redPoint.X, bluePoint.Y - redPoint.Y);

            imageBlueByte = getBitmapArray(Colors.BLUE, blue);
            imageGreenByte = getBitmapArray(Colors.GREEN, green);
            imageRedByte = getBitmapArray(Colors.RED, red);

            for (var i = 0; i < image.Height; i++)
            {
                for (var j = 0; j < image.Width; j++)
                {
                    if ( ((j - dR.X) > 0) && ((j - dR.X) < image.Width)   &&
                         ((i - dR.Y) > 0) && ((i - dR.Y) < image.Height)  &&
                         ((j - dG.X) > 0) && ((j - dG.X) < image.Width)   &&
                         ((i - dG.Y) > 0) && ((i - dG.Y) < image.Height))
                    image.SetPixel(j, i, Color.FromArgb(255, imageRedByte[j - dR.X, i - dR.Y], imageGreenByte[j - dG.X, i - dG.Y], imageBlueByte[j, i]));
                }
            }

            return image;
        }


    }
}
