using OSGeo.GDAL;
using OSGeo.OGR;
using OSGeo.OSR;
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
    public enum Colors { BLUE, GREEN, RED, ALL }

    public partial class MainForm : Form
    {
        private String filename = "C:/Users/Lenovo/Documents/Visual Studio 2015/Projects/WindowsFormsProjectGDAL/WindowsFormsProjectGDAL/resources/images/1.jpg";
        private OriganalImage orgImg;

        private static Colors colorModel; 

        private static Model model;
        private static Bitmap modelImg;


        public MainForm()
        {
            InitializeComponent();
            textBox1.Text = filename;

            model = new Model();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Gdal.AllRegister();     //регистрация драйверов для доступа к растровым данным
            Ogr.RegisterAll();      //регистрирует драйвера доступа в векторным данным

            label1.Text = "Версия библиотеки:\n" + Gdal.VersionInfo("");

            progressBar1.Minimum = 0;

            orgImg = new OriganalImage(filename, progressBar1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            PictureShow newForm = new PictureShow(Colors.BLUE, colorModel, orgImg.getBlueBitmap(), modelImg, model);
            newForm.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            PictureShow newForm = new PictureShow(Colors.GREEN, colorModel, orgImg.getGreenBitmap(), modelImg, model);
            newForm.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            PictureShow newForm = new PictureShow(Colors.RED, colorModel, orgImg.getRedBitmap(), modelImg, model);
            newForm.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    filename = ofd.FileName;
                    textBox1.Text = filename;
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Picture newForm = new Picture(orgImg.getRGBBitmap());
            newForm.Show();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Picture newForm = new Picture(NewImage.getNewImage(orgImg.getBlueBitmap(), orgImg.getGreenBitmap(), orgImg.getRedBitmap()));
            newForm.Show();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            PictureSettingsPyramid newForm = new PictureSettingsPyramid(orgImg.getRGBBitmap());
            newForm.Show();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            AffineTransformationForm newForm = new AffineTransformationForm(orgImg.getRGBBitmap());
            newForm.Show();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            Info info = new Info();
            info.a0 = 0;
            info.a1 = 1.05d;
            info.a2 = 0.05d;
            info.b0 = 0;
            info.b1 = 0.05d;
            info.b2 = 1.05d;

            AffineTransformation at = new AffineTransformation(orgImg.getRGBBitmap());
            at.transform(info.a0, info.a1, info.a2, info.b0, info.b1, info.b2, true);

            OriganalImage origanalImage2 = new OriganalImage(at.getTransformImage(), progressBar1);

            AccuracyAlgorithms newForm = new AccuracyAlgorithms(orgImg, origanalImage2, info);
            newForm.Show();
        }
    }
}
