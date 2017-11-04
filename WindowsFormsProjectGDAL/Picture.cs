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
    public partial class Picture : Form
    {
        private Bitmap image;

        public Picture(Bitmap image)
        {
            InitializeComponent();

            pictureBox1.Image = image;
            this.image = (Bitmap) image.Clone();

            pictureBox1.Size = new Size(image.Width, image.Height);
            pictureBox1.Image = this.image;
        }

        private void pictureBox1_Resize(object sender, EventArgs e)
        {
            
        }

        private void Picture_Resize(object sender, EventArgs e)
        {
            panel1.Size = new Size(this.Width - 15, this.Height - 38);
        }
    }
}
