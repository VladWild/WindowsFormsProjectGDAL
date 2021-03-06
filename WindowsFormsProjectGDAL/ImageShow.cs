﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsProjectGDAL
{
    public partial class PictureShow : Form
    {
        private Colors colorImage;

        private Bitmap image;
        private Bitmap imagePaint;
        private Bitmap imageOrigin;

        private Point downPoint;
        private Point upPoint;

        private Point downSearch;
        private Point upSearch;

        private bool isClicked;

        private Rectangle rectModel;
        private Rectangle rectSearch;
        private Rectangle rectFind;
        private Pen penWhite;
        private Pen penGreen;
        private Pen penYellow;
        private Pen penDarkOrange;
        private Graphics gDraw;
        private Graphics gDrawPaint;

        private Stopwatch sWatch;

        private static Colors colorModel;

        private static Bitmap modelImage;
        private static Model modelForm;

        public PictureShow()
        {
            InitializeComponent();
        }

        public PictureShow(Colors colorImage, Colors colorMdl, Bitmap image, Bitmap modelImg, Model model)
        {
            InitializeComponent();

            this.colorImage = colorImage;
            Text = colorImage.ToString();

            isClicked = false;

            rectModel = new Rectangle(0, 0, 0, 0);
            rectSearch = new Rectangle(0, 0, 0, 0);
            rectFind = new Rectangle(0, 0, 0, 0);

            penWhite = new Pen(Color.White, 1);
            penGreen = new Pen(Color.Green, 1);
            penYellow = new Pen(Color.Yellow, 1);
            penDarkOrange = new Pen(Color.DarkOrange, 1);
            rectModel = new Rectangle();

            this.image = image;
            imageOrigin = (Bitmap) image.Clone();
            imagePaint = new Bitmap(image.Width, image.Height);

            panel2.Size = new Size(this.Width, panel2.Size.Height + 20);                 //изменение размеров panel2
            panel2.Location = new Point(0, this.Height - panel2.Size.Height - 20);       //изменение локации panel2
            panel1.Size = new Size(this.Width - 15, this.Height - panel2.Height -20);   //изменение размеров panel1
            panel1.Location = new Point(0, 0);                                      //изменение локации panel1

            pictureBox1.Image = image;

            modelForm = model;
            modelImage = modelImg;

            colorModel = colorMdl;

        }

        //событие изменения размеров формы
        private void Form2BlueImage_Resize(object sender, EventArgs e)
        {
            panel1.Size = new Size(this.Width - 15, this.Height - panel2.Size.Height);
            panel2.Size = new Size(this.Width - 15, panel2.Size.Height);
            panel2.Location = new Point(0, this.Height - panel2.Size.Height);
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            e.Graphics.DrawImage(image, 0, 0, pictureBox1.Width, pictureBox1.Height);
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            rectFind = new Rectangle(0, 0, 0, 0);
            label20.Text = "X1=";
            label19.Text = "Y1=";
            if (radioButton4.Checked)
            {
                downPoint = new Point(e.X, e.Y);
                label1.Text = "X1=" + downPoint.X.ToString("");
                label2.Text = "Y1=" + downPoint.Y.ToString("");
                label3.Text = "X2=";
                label4.Text = "Y2=";
            }
            if (radioButton3.Checked)
            {
                downSearch = new Point(e.X, e.Y);
                label13.Text = "X1=" + downPoint.X.ToString("");
                label12.Text = "Y1=" + downPoint.Y.ToString("");
                label11.Text = "X2=";
                label10.Text = "Y2=";
            }
            this.isClicked = true;
        }

        private void modelCreate(Rectangle rect)
        {
            colorModel = colorImage;

            modelImage = new Bitmap(rect.Width, rect.Height);

            int i2 = 0;
            int j2 = 0;

            for (var i = rect.Y; i < rect.Y + rect.Height; i++, i2++)
            {
                for (var j = rect.X; j < rect.X + rect.Width; j++, j2++)
                {   
                    UInt32 pixel = (UInt32)(imageOrigin.GetPixel(j, i).ToArgb());   // получаем (j, i) пиксель
                    float R = (float)((pixel & 0x00FF0000) >> 16);                  // красный
                    float G = (float)((pixel & 0x0000FF00) >> 8);                   // зеленый
                    float B = (float)(pixel & 0x000000FF);                          // синий
                    if (radioButton2.Checked) R = G = B = R + G + B;                // делаем цвет одинаковым во всех трех каналах
                    modelImage.SetPixel(j2, i2, Color.FromArgb(255, (int) R, (int) G, (int) B));

                }
                j2 = 0;
            }

        }

        private void modelShow()
        {
            modelForm.setImageInPuctureBox(modelImage);
            modelForm.setSize(rectModel);
            modelForm.Show();
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (radioButton4.Checked)
            {
                upPoint = new Point(e.X, e.Y);
                this.isClicked = false;

                modelCreate(rectModel);
                modelShow();
            }
            if (radioButton3.Checked)
            {
                this.isClicked = false;
            }

        }

        private void drawImage()
        {
            gDrawPaint = Graphics.FromImage(imagePaint);
            gDrawPaint.DrawRectangle(penWhite, rectModel);
            gDrawPaint.DrawRectangle(penYellow, rectSearch);
            gDrawPaint.DrawRectangle(penDarkOrange, rectFind);

            gDraw = Graphics.FromImage(pictureBox1.Image);
            gDraw.DrawImage(imagePaint, 0, 0);
            pictureBox1.Invalidate();

            imagePaint = (Bitmap)imageOrigin.Clone();
        }

        private void info(Rectangle rect)
        {
            if (radioButton4.Checked)
            {
                label1.Text = "X1=" + rect.X.ToString();
                label2.Text = "Y1=" + rect.Y.ToString();
                label3.Text = "X2=" + (rect.X + rect.Width).ToString();
                label4.Text = "Y2=" + (rect.Y + rect.Height).ToString();
                label14.Text = "Ширина=" + rect.Width.ToString();
                label15.Text = "Высота=" + rect.Height.ToString();
            }
            if (radioButton3.Checked)
            {
                label13.Text = "X1=" + rect.X.ToString();
                label12.Text = "Y1=" + rect.Y.ToString();
                label11.Text = "X2=" + (rect.X + rect.Width).ToString();
                label10.Text = "Y2=" + (rect.Y + rect.Height).ToString();
                label16.Text = "Ширина=" + rect.Width.ToString();
                label17.Text = "Высота=" + rect.Height.ToString();
            }


        }

        private Rectangle coordinateSetting(Point point, Point e)
        {
            Rectangle rect = new Rectangle();

            if (point.X < e.X && point.Y < e.Y)
            {
                rect.X = point.X;
                rect.Y = point.Y;
                rect.Width = e.X - rect.X;
                rect.Height = e.Y - rect.Y;

                drawImage();
                info(rect);

            }

            if (point.X < e.X && point.Y > e.Y)
            {
                rect.X = point.X;
                rect.Y = e.Y;
                rect.Width = e.X - rect.X;
                rect.Height = point.Y - e.Y;

                drawImage();
                info(rect);

            }

            if (point.X > e.X && point.Y < e.Y)
            {
                rect.X = e.X;
                rect.Y = point.Y;
                rect.Width = point.X - e.X;
                rect.Height = e.Y - rect.Y;

                drawImage();
                info(rect);

            }

            if (point.X > e.X && point.Y > e.Y)
            {
                rect.X = e.X;
                rect.Y = e.Y;
                rect.Width = point.X - e.X;
                rect.Height = point.Y - e.Y;

                drawImage();
                info(rect);

            }

            return rect;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isClicked)
            {
                if (radioButton4.Checked)
                {
                    rectModel = coordinateSetting(downPoint, new Point(e.X, e.Y));
                }
                if (radioButton3.Checked)
                {
                    rectSearch = coordinateSetting(downSearch, new Point(e.X, e.Y));
                }
            }

        }     

        private void button1_Click(object sender, EventArgs e)
        {
            Point point = new Point();

            int index;
            index = listBox1.SelectedIndex;

            sWatch = new Stopwatch();

            switch (index)
            {
                case 0:
                    point = Correlator.classicNorm(colorImage, colorModel, imageOrigin, Model.modelImage,
                        rectSearch, progressBar1, sWatch, label23);
                    break;
                case 1:
                    point = Correlator.diffAbs(colorImage, colorModel, imageOrigin, Model.modelImage,
                        rectSearch, progressBar1, sWatch, label23);
                    break;
                case 2:
                    point = Correlator.diffSqr(colorImage, colorModel, imageOrigin, Model.modelImage,
                        rectSearch, progressBar1, sWatch, label23);
                    break;
                case 3:
                    point = Correlator.normCorrilation(colorImage, colorModel, imageOrigin, Model.modelImage,
                        rectSearch, progressBar1, sWatch, label23);
                    break;
                default:
                    MessageBox.Show("Выберете корреляционный алгоритм для поиска");
                    break;
            }

            if (index != -1)
            {
                rectFind = new Rectangle(point, Model.modelImage.Size);
                drawImage();
                label20.Text = "X1=" + point.X;
                label19.Text = "Y1=" + point.Y;
                label24.Text = "время = " + (((double)sWatch.ElapsedMilliseconds) / 1000).ToString() + " сек";
                MessageBox.Show("Фрагмент найден");
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (colorImage == Colors.BLUE) NewImage.bluePoint = new Point(rectModel.X, rectModel.Y);
            if (colorImage == Colors.GREEN) NewImage.greenPoint = new Point(rectModel.X, rectModel.Y);
            if (colorImage == Colors.RED) NewImage.redPoint = new Point(rectModel.X, rectModel.Y);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (colorImage == Colors.BLUE) NewImage.bluePoint = new Point(rectFind.X, rectFind.Y);
            if (colorImage == Colors.GREEN) NewImage.greenPoint = new Point(rectFind.X, rectFind.Y);
            if (colorImage == Colors.RED) NewImage.redPoint = new Point(rectFind.X, rectFind.Y);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Picture.saveImage(this.pictureBox1);
        }
    }
}
