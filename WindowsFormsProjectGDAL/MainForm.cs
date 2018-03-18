﻿using OSGeo.GDAL;
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

            label1.Text = "Library version:\n" + Gdal.VersionInfo("");

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
    }
}
