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

        private void button2_Click(object sender, EventArgs e)
        {
            Info info = new Info();
            info.a0 = Convert.ToDouble(textBox1.Text);
            info.a1 = Convert.ToDouble(textBox3.Text);
            info.a2 = Convert.ToDouble(textBox5.Text);
            info.b0 = Convert.ToDouble(textBox2.Text);
            info.b1 = Convert.ToDouble(textBox4.Text);
            info.b2 = Convert.ToDouble(textBox6.Text);

            OriganalImage origanalImage = new OriganalImage(image, new ProgressBar());
            at.transform(Convert.ToDouble(textBox1.Text), Convert.ToDouble(textBox3.Text),
                Convert.ToDouble(textBox5.Text), Convert.ToDouble(textBox2.Text),
                Convert.ToDouble(textBox4.Text), Convert.ToDouble(textBox6.Text),
                radioButton1.Checked);
            OriganalImage origanalImage2 = new OriganalImage(at.getTransformImage(), new ProgressBar());

            AccuracyAlgorithms newForm = new AccuracyAlgorithms(origanalImage, origanalImage2, info);
            newForm.Show();
        }
    }
}






