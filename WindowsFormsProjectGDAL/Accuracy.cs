using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsProjectGDAL
{
    public partial class Accuracy : Form
    {
        private Info info;

        private OriganalImage beforeImage;
        private OriganalImage afterImage;

        private Bitmap currentBefore;
        private Bitmap currentAfter;

        //для информации 
        private Point downPoint;
        private Point upPoint;

        //для отрисовки 
        private Bitmap imagePaintBefore;
        private Bitmap imagePaintAfter;

        private Graphics gDraw;
        private Graphics gDrawPaint;

        private Pen penWhite;
        private Pen penGreen;
        private Pen penYellow;
        private Pen penDarkOrange;

        private bool isClicked;

        //прямоугольники
        private Rectangle rectModel;
        private Rectangle rectSearch;
        private Rectangle rectFind;

        public Accuracy(OriganalImage beforeImage, OriganalImage afterImage, Info info)
        {
            InitializeComponent();

            this.beforeImage = beforeImage;
            this.afterImage = afterImage;

            currentBefore = (Bitmap) beforeImage.getRedBitmap().Clone();
            currentAfter = (Bitmap) afterImage.getRedBitmap().Clone();

            pictureBox1.Image = currentBefore;
            pictureBox2.Image = currentAfter;

            imagePaintBefore = new Bitmap(currentBefore.Width, currentBefore.Height);
            imagePaintAfter = new Bitmap(currentAfter.Width, currentAfter.Height);

            penWhite = new Pen(Color.White, 1);
            penGreen = new Pen(Color.Green, 1);
            penYellow = new Pen(Color.Yellow, 1);
            penDarkOrange = new Pen(Color.DarkOrange, 1);

            isClicked = false;

            rectModel = new Rectangle(0, 0, 0, 0);

            this.info = info;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            currentBefore = (Bitmap) beforeImage.getRedBitmap().Clone();
            currentAfter = (Bitmap) afterImage.getRedBitmap().Clone();

            pictureBox1.Image = currentBefore;
            pictureBox2.Image = currentAfter;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            currentBefore = (Bitmap) beforeImage.getGreenBitmap().Clone();
            currentAfter = (Bitmap) afterImage.getGreenBitmap().Clone();

            pictureBox1.Image = currentBefore;
            pictureBox2.Image = currentAfter;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            currentBefore = (Bitmap) beforeImage.getBlueBitmap().Clone();
            currentAfter = (Bitmap) afterImage.getBlueBitmap().Clone();

            pictureBox1.Image = currentBefore;
            pictureBox2.Image = currentAfter;
        }

        //перересовка изображения при выделении эталона 
        private void drawImageBefore()
        {
            gDrawPaint = Graphics.FromImage(imagePaintBefore);
            gDrawPaint.DrawRectangle(penWhite, rectModel);
            gDrawPaint.DrawRectangle(penYellow, rectSearch);
            gDrawPaint.DrawRectangle(penDarkOrange, rectFind);

            gDraw = Graphics.FromImage(pictureBox1.Image);
            gDraw.DrawImage(imagePaintBefore, 0, 0);
            pictureBox1.Invalidate();

            imagePaintBefore = (Bitmap) currentBefore.Clone();
        }

        //настройка координат при выделении эталона 
        private Rectangle coordinateSettingBefore(Point point, Point e)
        {
            Rectangle rect = new Rectangle();

            if (point.X < e.X && point.Y < e.Y)
            {
                rect.X = point.X;
                rect.Y = point.Y;
                rect.Width = e.X - rect.X;
                rect.Height = e.Y - rect.Y;

                drawImageBefore();
                //info(rect);

            }

            if (point.X < e.X && point.Y > e.Y)
            {
                rect.X = point.X;
                rect.Y = e.Y;
                rect.Width = e.X - rect.X;
                rect.Height = point.Y - e.Y;

                drawImageBefore();
                //info(rect);

            }

            if (point.X > e.X && point.Y < e.Y)
            {
                rect.X = e.X;
                rect.Y = point.Y;
                rect.Width = point.X - e.X;
                rect.Height = e.Y - rect.Y;

                drawImageBefore();
                //info(rect);

            }

            if (point.X > e.X && point.Y > e.Y)
            {
                rect.X = e.X;
                rect.Y = e.Y;
                rect.Width = point.X - e.X;
                rect.Height = point.Y - e.Y;

                drawImageBefore();
                //info(rect);

            }

            return rect;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isClicked)
            {
                //pictureBox1.Image = (Bitmap)currentBefore.Clone();
                rectModel = coordinateSettingBefore(downPoint, new Point(e.X, e.Y));
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            rectFind = new Rectangle(0, 0, 0, 0);
            downPoint = new Point(e.X, e.Y);

            //должен быть вывод информации 

            isClicked = true;
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            upPoint = new Point(e.X, e.Y);
            this.isClicked = false;
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            e.Graphics.DrawImage(currentBefore, 0, 0, pictureBox1.Width, pictureBox1.Height);
        }
    }
}


