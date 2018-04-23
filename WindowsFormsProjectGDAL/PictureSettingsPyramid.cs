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
    public partial class PictureSettingsPyramid : Form
    {
        private PyramidImage pyramidImage;

        public PictureSettingsPyramid()
        {
            InitializeComponent();
        }

        public PictureSettingsPyramid(Bitmap originalImage)
        {
            InitializeComponent();

            pyramidImage = new PyramidImage((Bitmap) originalImage.Clone());

            pictureBox1.Size = new Size(pyramidImage.getOriginal().Width, pyramidImage.getOriginal().Height);
            pictureBox1.Image = pyramidImage.getOriginal();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AffineTransformationForm newForm;

            switch (listBox1.SelectedIndex)
            {
                case 0:
                    pyramidImage.decimationPixels(Convert.ToInt32(textBox1.Text), progressBar1);
                    newForm = new AffineTransformationForm(pyramidImage.getPyramidDecimation());
                    newForm.Show();
                    break;
                case 1:
                    pyramidImage.avaregingPixels(Convert.ToInt32(textBox2.Text), Convert.ToInt32(textBox4.Text), progressBar1);
                    newForm = new AffineTransformationForm(pyramidImage.getPyramidAvareging());
                    newForm.Show();
                    break;
                case 2:
                    pyramidImage.gaussianFilter(Convert.ToInt32(textBox2.Text), trackBar2.Value * 2 + 1, Convert.ToDouble(label17.Text),  progressBar1);
                    newForm = new AffineTransformationForm(pyramidImage.getPyramidGaussian());
                    newForm.Show();
                    break;
                default:
                    MessageBox.Show("Select a method");
                    return;
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            label11.Text = "x" + textBox4.Text;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            switch (trackBar1.Value)
            {
                case 1:
                    label17.Text = "0,1";
                    break;
                case 2:
                    label17.Text = "0,25";
                    break;
                case 3:
                    label17.Text = "0,5";
                    break;
                case 4:
                    label17.Text = "1";
                    break;
                case 5:
                    label17.Text = "1,5";
                    break;
                case 6:
                    label17.Text = "2";
                    break;
                case 7:
                    label17.Text = "5";
                    break;
                case 8:
                    label17.Text = "10";
                    break;
            } 
        }

        private void trackBar2_Scroll_1(object sender, EventArgs e)
        {
            switch (trackBar2.Value)
            {
                case 1:
                    label22.Text = "3x3";
                    break;
                case 2:
                    label22.Text = "5x5";
                    break;
                case 3:
                    label22.Text = "7x7";
                    break;
                case 4:
                    label22.Text = "9x9";
                    break;
                case 5:
                    label22.Text = "11x11";
                    break;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
        }
    }
}
