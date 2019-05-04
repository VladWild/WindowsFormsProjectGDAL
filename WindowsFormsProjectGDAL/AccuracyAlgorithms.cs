using System;
using System.Collections;
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
    public partial class AccuracyAlgorithms : Form
    {
        private OriganalImage originalImage, originalImage2;

        private Bitmap image, image2, image4;
        private Bitmap imagePaint, imagePaint2;
        private Bitmap imageOrigin, imageOrigin2;

        private Point downPoint, downPoint2;
        private Point upPoint, upPoint2;

        private Point downSearch;
        private Point upSearch;

        private bool isClicked, isClicked2;

        private Rectangle rectModel;
        private Rectangle rectSearch;
        private Rectangle rectFind;
        private List<Point> points;
        private List<Point> points2;
        private List<Point> points4;
        private Pen penWhite;
        private Pen penGreen;
        private Pen penYellow;
        private Pen penDarkOrange;
        private Graphics gDraw;
        private Graphics gDrawPaint;

        private Stopwatch sWatch;

        private static Bitmap modelImage;
        private static Model modelForm;

        private AccuracyAlgorithmsCorrelation aac;
        private MyPoint k;
        private MyPoint t;

        Rectangle firstRect;

        MyPoint pointAffine;
        MyPoint pointWithOut;

        public AccuracyAlgorithms()
        {
            InitializeComponent();
        }

        public AccuracyAlgorithms(OriganalImage beforeImage, OriganalImage afterImage, Info info)
        {
            InitializeComponent();

            originalImage = beforeImage; originalImage2 = afterImage;

            isClicked = false; isClicked2 = false;

            rectModel = new Rectangle(0, 0, 0, 0);
            rectSearch = new Rectangle(0, 0, 0, 0);
            rectFind = new Rectangle(0, 0, 0, 0);

            penWhite = new Pen(Color.White, 1);
            penGreen = new Pen(Color.Green, 1);
            penYellow = new Pen(Color.Yellow, 1);
            penDarkOrange = new Pen(Color.DarkOrange, 1);
            rectModel = new Rectangle();

            this.image = originalImage.getRedBitmap(); this.image2 = originalImage2.getRedBitmap();
            imageOrigin = (Bitmap) image.Clone(); imageOrigin2 = (Bitmap)image2.Clone();
            imagePaint = new Bitmap(image.Width, image.Height); imagePaint2 = new Bitmap(image2.Width, image2.Height);

            pictureBox1.Image = image; pictureBox2.Image = image2; image4 = (Bitmap) image2.Clone();

            modelForm = new Model();

            aac = new AccuracyAlgorithmsCorrelation(info);

            points = new List<Point>();
            points2 = new List<Point>();
            points4 = new List<Point>();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            e.Graphics.DrawImage(image, 0, 0, pictureBox1.Width, pictureBox1.Height);
        }

        private void pictureBox2_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            e.Graphics.DrawImage(image2, 0, 0, pictureBox2.Width, pictureBox2.Height);
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            rectFind = new Rectangle(0, 0, 0, 0);
            label20.Text = "X1=";
            label19.Text = "Y1=";

            downPoint = new Point(e.X, e.Y);
            label1.Text = "X1=" + downPoint.X.ToString("");
            label2.Text = "Y1=" + downPoint.Y.ToString("");
            label3.Text = "X2=";
            label4.Text = "Y2=";

            this.isClicked = true;
        }

        private void pictureBox2_MouseDown(object sender, MouseEventArgs e)
        {
            rectFind = new Rectangle(0, 0, 0, 0);
            label20.Text = "X1=";
            label19.Text = "Y1=";

            downSearch = new Point(e.X, e.Y);
            label13.Text = "X1=" + downPoint2.X.ToString("");
            label12.Text = "Y1=" + downPoint2.Y.ToString("");
            label11.Text = "X2=";
            label10.Text = "Y2=";

            this.isClicked2 = true;
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            upPoint = new Point(e.X, e.Y);
            this.isClicked = false;

            modelCreate(rectModel);
            modelShow();

            Rectangle rect = coordinateSetting(downPoint, new Point(e.X, e.Y));
            firstRect = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);

            aac.setPointModel(rect);

            MyPoint point = aac.getPointModel();

            label30.Text = "X = " + point.x.ToString();
            label29.Text = "Y = " + point.y.ToString();

            points.Clear();
            points.Add(new Point((int)point.x, (int)point.y));
            drawImage();

            pointAffine = aac.getPointModelTransform();

            label27.Text = "X t=" + pointAffine.x.ToString();
            label22.Text = "Y t=" + pointAffine.y.ToString();
            t = new MyPoint(pointAffine.x, pointAffine.y);

            points2.Clear();
            points2.Add(new Point((int)pointAffine.x, (int)pointAffine.y));
            drawImage2();
        }

        private void pictureBox2_MouseUp(object sender, MouseEventArgs e)
        {
            upPoint2 = new Point(e.X, e.Y);
            this.isClicked2 = false;
        }

        private void info(Rectangle rect)
        {
            textBox4.Text = rect.X.ToString();
            textBox5.Text = rect.X.ToString();
            textBox6.Text = (rect.X + rect.Width).ToString();
            textBox7.Text = (rect.Y + rect.Height).ToString();
            label14.Text = "Ширина=" + rect.Width.ToString();
            label15.Text = "Высота=" + rect.Height.ToString();
        }

        private void info2(Rectangle rect)
        {
            label13.Text = "X1=" + rect.X.ToString();
            label12.Text = "Y1=" + rect.Y.ToString();
            label11.Text = "X2=" + (rect.X + rect.Width).ToString();
            label10.Text = "Y2=" + (rect.Y + rect.Height).ToString();
            label16.Text = "Width=" + rect.Width.ToString();
            label17.Text = "Height=" + rect.Height.ToString();
        }

        private void drawImage()
        {
            gDrawPaint = Graphics.FromImage(imagePaint);
            gDrawPaint.DrawRectangle(penWhite, rectModel);
            foreach (Point point in points){
                gDrawPaint.DrawRectangle(penWhite, new Rectangle(point.X, point.Y, 1, 1));
            }

            gDraw = Graphics.FromImage(pictureBox1.Image);
            gDraw.DrawImage(imagePaint, 0, 0);
            pictureBox1.Invalidate();

            imagePaint = (Bitmap)imageOrigin.Clone();
        }

        private void drawImage2()
        {
            gDrawPaint = Graphics.FromImage(imagePaint2);
            gDrawPaint.DrawRectangle(penYellow, rectSearch);
            gDrawPaint.DrawRectangle(penDarkOrange, rectFind);
            foreach (Point point in points2)
            {
                gDrawPaint.DrawRectangle(penWhite, new Rectangle(point.X, point.Y, 1, 1));
            }
            foreach (Point point in points4)
            {
                gDrawPaint.DrawRectangle(penDarkOrange, new Rectangle(point.X, point.Y, 1, 1));
            }
            

            gDraw = Graphics.FromImage(pictureBox2.Image);
            gDraw.DrawImage(imagePaint2, 0, 0);
            pictureBox2.Invalidate();

            imagePaint2 = (Bitmap)imageOrigin2.Clone();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            image = originalImage.getGreenBitmap();
            image2 = originalImage2.getGreenBitmap();

            imageOrigin = (Bitmap)image.Clone(); imageOrigin2 = (Bitmap)image2.Clone();
            imagePaint = new Bitmap(image.Width, image.Height); imagePaint2 = new Bitmap(image2.Width, image2.Height);

            pictureBox1.Image = image; pictureBox2.Image = image2;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            image = originalImage.getBlueBitmap();
            image2 = originalImage2.getBlueBitmap();

            imageOrigin = (Bitmap)image.Clone(); imageOrigin2 = (Bitmap)image2.Clone();
            imagePaint = new Bitmap(image.Width, image.Height); imagePaint2 = new Bitmap(image2.Width, image2.Height);

            pictureBox1.Image = image; pictureBox2.Image = image2;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            aac.add(k, t);

            label38.Text = "Текущая итерация = " + aac.getCount().ToString();
            label39.Text = "Sx = " + aac.getSx().ToString();
            label40.Text = "Sy = " + aac.getSy().ToString();
            label41.Text = "σ = " + aac.getSko().ToString();

            button2.Enabled = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            aac.clear();

            label38.Text = "Count = 0";
            label39.Text = "Sx = ";
            label40.Text = "Sy = ";
            label41.Text = "σ = ";

            button2.Enabled = true;

            //first model
            downPoint = new Point(firstRect.X, firstRect.Y);
            upPoint = new Point(firstRect.X + firstRect.Width, firstRect.Y + firstRect.Height);
            this.isClicked = false;

            modelCreate(new Rectangle(firstRect.X, firstRect.Y, firstRect.Width, firstRect.Height));
            modelShow();

            Rectangle rect = coordinateSetting(downPoint, upPoint);
            //firstRect = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);

            aac.setPointModel(rect);

            MyPoint point = aac.getPointModel();

            label30.Text = "X t=" + point.x.ToString();
            label29.Text = "Y t=" + point.y.ToString();

            points.Clear();
            points.Add(new Point((int)point.x, (int)point.y));
            drawImage();

            MyPoint pointAffine = aac.getPointModelTransform();

            label27.Text = "X t=" + pointAffine.x.ToString();
            label22.Text = "Y t=" + pointAffine.y.ToString();
            t = new MyPoint(pointAffine.x, pointAffine.y);

            points2.Clear();
            points2.Add(new Point((int)pointAffine.x, (int)pointAffine.y));
            drawImage2();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            int count = Int32.Parse(textBox1.Text);

            int dx = Int32.Parse(textBox2.Text);
            int dy = Int32.Parse(textBox3.Text);

            for (int i = 0; i < count; i++)
            {
                button2_Click(sender, e);

                //down
                rectFind = new Rectangle(0, 0, 0, 0);
                label20.Text = "X1=";
                label19.Text = "Y1=";

                downPoint = new Point(downPoint.X + dx, downPoint.Y + dy);
                label1.Text = "X1=" + downPoint.X.ToString("");
                label2.Text = "Y1=" + downPoint.Y.ToString("");
                label3.Text = "X2=";
                label4.Text = "Y2=";

                this.isClicked = true;

                //move
                if (isClicked) rectModel = coordinateSetting(downPoint, new Point(upPoint.X + dx, upPoint.Y + dy));

                //up
                upPoint = new Point(upPoint.X + dx, upPoint.Y + dy);
                this.isClicked = false;

                modelCreate(rectModel);
                modelShow();

                Rectangle rect = coordinateSetting(downPoint, upPoint);

                aac.setPointModel(rect);

                MyPoint point = aac.getPointModel();

                label30.Text = "X t=" + point.x.ToString();
                label29.Text = "Y t=" + point.y.ToString();

                points.Clear();
                points.Add(new Point((int)point.x, (int)point.y));
                drawImage();

                MyPoint pointAffine = aac.getPointModelTransform();

                label27.Text = "X t=" + pointAffine.x.ToString();
                label22.Text = "Y t=" + pointAffine.y.ToString();
                t = new MyPoint(pointAffine.x, pointAffine.y);

                points2.Clear();
                points2.Add(new Point((int)pointAffine.x, (int)pointAffine.y));
                drawImage2();

                //click
                button1_Click(sender, e);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            image = originalImage.getRedBitmap();
            image2 = originalImage2.getRedBitmap();

            imageOrigin = (Bitmap)image.Clone(); imageOrigin2 = (Bitmap)image2.Clone();
            imagePaint = new Bitmap(image.Width, image.Height); imagePaint2 = new Bitmap(image2.Width, image2.Height);

            pictureBox1.Image = image; pictureBox2.Image = image2;
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

        private Rectangle coordinateSetting2(Point point, Point e)
        {
            Rectangle rect = new Rectangle();

            if (point.X < e.X && point.Y < e.Y)
            {
                rect.X = point.X;
                rect.Y = point.Y;
                rect.Width = e.X - rect.X;
                rect.Height = e.Y - rect.Y;

                drawImage2();
                info2(rect);

            }

            if (point.X < e.X && point.Y > e.Y)
            {
                rect.X = point.X;
                rect.Y = e.Y;
                rect.Width = e.X - rect.X;
                rect.Height = point.Y - e.Y;

                drawImage2();
                info2(rect);

            }

            if (point.X > e.X && point.Y < e.Y)
            {
                rect.X = e.X;
                rect.Y = point.Y;
                rect.Width = point.X - e.X;
                rect.Height = e.Y - rect.Y;

                drawImage2();
                info2(rect);

            }

            if (point.X > e.X && point.Y > e.Y)
            {
                rect.X = e.X;
                rect.Y = e.Y;
                rect.Width = point.X - e.X;
                rect.Height = point.Y - e.Y;

                drawImage2();
                info2(rect);

            }

            return rect;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isClicked) rectModel = coordinateSetting(downPoint, new Point(e.X, e.Y));
        }

        private void pictureBox2_MouseMove(object sender, MouseEventArgs e)
        {
            if (isClicked2) rectSearch = coordinateSetting2(downSearch, new Point(e.X, e.Y));
        }

        
        private void button1_Click(object sender, EventArgs e)
        {
            Point point = new Point();

            int index;
            index = listBox1.SelectedIndex;

            sWatch = new Stopwatch();

            Colors color = new Colors();

            if (radioButton1.Checked) color = Colors.RED;
            if (radioButton2.Checked) color = Colors.GREEN;
            if (radioButton3.Checked) color = Colors.BLUE;

            switch (index)
            {
                case 0:
                    point = Correlator.classicNorm(color, color, image4, Model.modelImage,
                        rectSearch, progressBar1, sWatch, label23);
                    break;
                case 1:
                    point = Correlator.diffAbs(color, color, image4, Model.modelImage,
                        rectSearch, progressBar1, sWatch, label23);
                    break;
                case 2:
                    point = Correlator.diffSqr(color, color, image4, Model.modelImage,
                        rectSearch, progressBar1, sWatch, label23);
                    break;
                case 3:
                    point = Correlator.normCorrilation(color, color, image4, Model.modelImage,
                        rectSearch, progressBar1, sWatch, label23);
                    break;
                default:
                    MessageBox.Show("Выберете алгоритм");
                    break;
            }

            if (index != -1)
            {
                rectFind = new Rectangle(point, Model.modelImage.Size);

                pointWithOut = new MyPoint(((double)(2 * point.X + Model.modelImage.Width)) / 2d,
                    ((double)(2 * point.Y + Model.modelImage.Height)) / 2d);

                points4.Clear();
                points4.Add(new Point((int) Math.Round(pointWithOut.x), (int) Math.Round(pointWithOut.y)));
                label31.Text = "X k0=" + pointWithOut.x.ToString();
                label28.Text = "Y k0=" + pointWithOut.y.ToString();

                MyPoint myPoint = new MyPoint();
                if (index == 0 || index == 3) myPoint = Correlator.getPointSubShiftMax();
                if (index == 1 || index == 2) myPoint = Correlator.getPointSubShiftMin();
                correctedSignMyPointdXdY(myPoint);
                label37.Text = "dx=" + myPoint.x.ToString();
                label36.Text = "dy=" + myPoint.y.ToString();

                MyPoint pointWithSub = new MyPoint(pointWithOut.x + myPoint.x, pointWithOut.y + myPoint.y);
                label34.Text = "X k=" + pointWithSub.x.ToString();
                label33.Text = "Y k=" + pointWithSub.y.ToString();
                k = new MyPoint(pointWithSub.x, pointWithSub.y);        //сохранение k 

                drawImage2();
                label20.Text = "X1=" + point.X;
                label19.Text = "Y1=" + point.Y;
                label24.Text = "time = " + (((double)sWatch.ElapsedMilliseconds) / 1000).ToString() + " sec";
                MessageBox.Show("Прямоугольная область найдена");
            }

            button2.Enabled = true;
        }

        private void modelCreate(Rectangle rect)
        {
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
                    modelImage.SetPixel(j2, i2, Color.FromArgb(255, (int)R, (int)G, (int)B));

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

        private void correctedSignMyPointdXdY(MyPoint myPoint)
        {
            myPoint.x = pointAffine.x > pointWithOut.x ? Math.Abs(myPoint.x) : - Math.Abs(myPoint.x);
            myPoint.y = pointAffine.y > pointWithOut.y ? Math.Abs(myPoint.y) : - Math.Abs(myPoint.y);
        }

    }
}
