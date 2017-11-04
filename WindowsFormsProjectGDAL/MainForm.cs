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
        private String filename = "C:/Users/Lenovo/Documents/Visual Studio 2015/Projects/WindowsFormsProjectGDAL/WindowsFormsProjectGDAL/resources/images/nature.jpg";
        private OriganalImage orgImg;

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

            label1.Text = "Library version:\n" + Gdal.VersionInfo("");

            progressBar1.Minimum = 0;

            orgImg = new OriganalImage(filename, progressBar1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            PictureShow newForm = new PictureShow(Colors.BLUE, orgImg.getBlueBitmap(), orgImg.getBlueGrayBitmap(), modelImg, model);
            newForm.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            PictureShow newForm = new PictureShow(Colors.GREEN, orgImg.getGreenBitmap(), orgImg.getGreenGrayBitmap(), modelImg, model);
            newForm.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            PictureShow newForm = new PictureShow(Colors.RED, orgImg.getRedBitmap(), orgImg.getRedGrayBitmap(), modelImg, model);
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
    }
}
