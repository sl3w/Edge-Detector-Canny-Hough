using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Edge_detection
{
    public partial class HoughForm : Form
    {
        private Bitmap inputBitmap;
        private Image original;

        public HoughForm(Image afterProc, Image original)
        {
            InitializeComponent();
            inputBitmap = new Bitmap(afterProc);
            this.original = original;

            StartHough();
        }

        private void SwitchEnableElements(bool enable)
        {
            trackBar1.Enabled = enable;
            trackBar2.Enabled = enable;
        }

        private void StartHough()
        {
            SwitchEnableElements(false);

            int r = trackBar2.Value;
            int tr = trackBar1.Value;

            pictureBox1.Image = Hough.TransformCircle(inputBitmap, r);
            pictureBox2.Image = new Bitmap(original);

            //Bitmap img = new Bitmap(original);
            Graphics g = Graphics.FromImage(pictureBox2.Image);
            Pen pen = new Pen(Color.Black, 3);

            Point Size = new Point(inputBitmap.Width, inputBitmap.Height);
            while (true)
            {
                Point pt = Hough.SearchCircle(Size, tr);
                if (pt.X == -1) break;
                g.DrawEllipse(pen, pt.X - r, pt.Y - r, r + r, r + r);
            }
            pictureBox2.Refresh();

            SwitchEnableElements(true);
        }

        private void trackBar2_MouseUp(object sender, MouseEventArgs e)
        {
            StartHough();
        }

        private void trackBar1_MouseUp(object sender, MouseEventArgs e)
        {
            StartHough();
        }
    }
}
