using Edge_detection.Properties;
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
    public partial class Form1 : Form
    {
        private int locX = 30;
        private int locY = 30;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            PictureBox pb = new PictureBox();
            pb.SizeMode = PictureBoxSizeMode.Zoom;
            pb.Image = LoG.MarrHildrethEdge(new Bitmap(Resources.no_photo));
            pb.Size = new Size(245, 160);
            pb.Location = new Point(locX, locY);

            locX += 275;

            panel1.Controls.Add(pb);
        }
    }
}
