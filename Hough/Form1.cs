using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        Hough alg;
        public Form1()
        {
            InitializeComponent();
            alg = new Hough(); 
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog ();
            dlg.Title = "Open Image";
            dlg.Filter = "Image files (*.bmp , *.jpg , *.png, *.gif )|*.bmp;*.jpg;*.png;*.gif";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = new Bitmap(dlg.FileName);
                pictureBox2.Image = alg.Sobel(new Bitmap(pictureBox1.Image));
            }

            dlg.Dispose();
        }

        // Line
        private void button1_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null) return;

            int tr = Convert.ToInt32(textBox1.Text);

            pictureBox3.Image = alg.TransformLine(new Bitmap(pictureBox2.Image), tr);
            pictureBox4.Image = new Bitmap(pictureBox1.Image);
          
            Bitmap img = new Bitmap(pictureBox4.Image);
            Graphics g = Graphics.FromImage(pictureBox4.Image);
            Pen pen = new Pen(Color.Red,3);

            int dp = (int)Math.Round(Math.Sqrt(Math.Pow(pictureBox1.Image.Width, 2) + Math.Pow(pictureBox1.Image.Height, 2)));
            Point Size = new Point(180, dp);

            while (true)
            {
                Point pt = alg.SearchLine( Size, tr );
                if (pt.X == -1) break;
                if (pt.X > 0)
                {
                    int y1 = (int)((-Math.Cos(pt.X * (Math.PI / 180)) / Math.Sin(pt.X * (Math.PI / 180))) * 0 + (double)pt.Y / Math.Sin(pt.X * (Math.PI / 180)));
                    int y2 = (int)((-Math.Cos(pt.X * (Math.PI / 180)) / Math.Sin(pt.X * (Math.PI / 180))) * img.Width + (double)pt.Y / Math.Sin(pt.X * (Math.PI / 180)));
                    g.DrawLine(pen, 0, y1, img.Width, y2);
                }
            }
            pictureBox4.Refresh();
        }

        // Circle
        private void button2_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null) return;

            int r = Convert.ToInt32(textBox2.Text);
            int tr = Convert.ToInt32(textBox1.Text);

            pictureBox3.Image = alg.TransformCircle(new Bitmap(pictureBox2.Image), tr, r);
            pictureBox4.Image = new Bitmap(pictureBox1.Image);

            Bitmap img = new Bitmap(pictureBox3.Image);
            Graphics g = Graphics.FromImage(pictureBox4.Image);
            Pen pen = new Pen(Color.Red, 3);

            Point Size = new Point(pictureBox1.Image.Width, pictureBox1.Image.Height);
            while (true)
            {
                Point pt = alg.SearchCircle( Size, tr );
                if (pt.X == -1) break;
                g.DrawEllipse(pen, pt.X - r, pt.Y - r, r + r, r + r);
            }
            pictureBox4.Refresh();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (pictureBox3.Image != null)
            {
                pictureBox3.Image.Save("accumulator.bmp");
                pictureBox4.Image.Save("result.bmp");
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


    }
}
