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
    public partial class AffineTransformationForm : Form
    {
        private Bitmap image;
        private AffineTransformation at;

        public AffineTransformationForm(Bitmap image)
        {
            InitializeComponent();

            pictureBox1.Image = image;
            this.image = (Bitmap) image.Clone();

            pictureBox1.Size = new Size(image.Width, image.Height);
            pictureBox1.Image = this.image;

            at = new AffineTransformation(image);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            at.transform(Convert.ToDouble(textBox1.Text), Convert.ToDouble(textBox3.Text),
                Convert.ToDouble(textBox5.Text), Convert.ToDouble(textBox2.Text),
                Convert.ToDouble(textBox4.Text), Convert.ToDouble(textBox6.Text),
                radioButton1.Checked);
            Picture newForm = new Picture(at.getTransformImage());
            newForm.Show();
        }
    }
}






