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
    public partial class Model : Form
    {
        public static Bitmap modelImage;

        public Model()
        {
            InitializeComponent();
        }

        public void setImageInPuctureBox(Bitmap imageModel)
        {
            pictureBox1.Image = imageModel;
            modelImage = (Bitmap) imageModel.Clone();
        }

        public void setSize(Rectangle rect)
        {
            Size = new Size(rect.Width + 16, rect.Height + 40);
            pictureBox1.Size = new Size(rect.Width, rect.Height);
        }

    }
}
