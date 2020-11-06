using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Edge_detection
{
    public partial class FadeLaplasDetection : Form
    {
        private Image uploadedImage;
        private Thread thread;
        private bool isFirstStart = true;
        private bool isGradient = false;
        private Bitmap afterFade;

        public FadeLaplasDetection(Image img, bool isGrad)
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
            uploadedImage = img;
            isGradient = isGrad;
            if (isGradient)
                Text += " методом на основе градиента";
            else
                Text += " методом на основе лапласиана";
        }

        private void GradientDetection_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            ClearAndStart();
        }

        public void StartProcessing()
        {
            Bitmap afterGrey;
            if (isFirstStart)
            {
                var bmp = new Bitmap(uploadedImage);
                pictureBox1.Invoke(new Action(() => pictureBox1.Image = new Bitmap(bmp)));
                pictureBox8.Invoke(new Action(() => pictureBox8.Image = new Bitmap(bmp)));

                afterGrey = ImageProcessing.ImageToGrey(bmp);
                pictureBox2.Invoke(new Action(() => pictureBox2.Image = new Bitmap(afterGrey)));
                isFirstStart = false;
            }
            else
                afterGrey = new Bitmap(pictureBox2.Image);

            Bitmap afterGauss;
            if (checkBox1.Checked)
            {
                double sigma = 0;
                if (radioButton2.Checked)
                    sigma = (double)numericUpDown4.Value;
                afterGauss = Filters.GaussianFilter(afterGrey, sigma, false);
            }
            else
                afterGauss = afterGrey;

            pictureBox3.Invoke(new Action(() => pictureBox3.Image = new Bitmap(afterGauss)));

            if (isGradient)
                afterFade = Edges.FadeDetection(afterGauss);
            else
                afterFade = Edges.LaplacianDetection(afterGauss);
            pictureBox4.Invoke(new Action(() => pictureBox4.Image = new Bitmap(afterFade)));

            Bitmap afterThreshold = ImageProcessing.SingleThreshold(afterFade, trackBar2.Value);
            pictureBox5.Invoke(new Action(() => pictureBox5.Image = new Bitmap(afterThreshold)));

            ShowSelectedImage();
            button1.Enabled = groupBox2.Enabled = true;
            thread.Abort();
        }

        private void ClearAndStart()
        {
            pictureBox3.Image = null;
            pictureBox4.Image = null;
            pictureBox5.Image = null;
            button1.Enabled = groupBox2.Enabled = false;
            thread = new Thread(StartProcessing);
            thread.Start();
        }

        private void ShowSelectedImage()
        {
            PictureBox pb = Controls["pictureBox" + trackBar1.Value.ToString()] as PictureBox;
            pb.BorderStyle = BorderStyle.Fixed3D;
            pictureBox8.Image = pb.Image;
            for (int i = 1; i < 6; i++)
            {
                if (i != trackBar1.Value)
                {
                    pb = Controls["pictureBox" + i.ToString()] as PictureBox;
                    pb.BorderStyle = BorderStyle.None;
                }
            }
        }

        private void WorkThresholdOnly()
        {
            Bitmap afterThreshold = ImageProcessing.SingleThreshold(afterFade, trackBar2.Value);
            pictureBox5.Invoke(new Action(() => pictureBox5.Image = afterThreshold));

            ShowSelectedImage();
            button1.Enabled = groupBox2.Enabled = true;
            thread.Abort();
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            ShowSelectedImage();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            int x = trackBar1.Value;
            PictureBox pb = sender as PictureBox;
            trackBar1.Value = int.Parse(pb.Name[10].ToString());
            if (trackBar1.Value == x)
                ShowSelectedImage();
        }

        private void GradientDetection_FormClosed(object sender, FormClosedEventArgs e)
        {
            thread.Abort();
        }

        private void trackBar2_ValueChanged(object sender, EventArgs e)
        {
            numericUpDown1.Value = trackBar2.Value;
        }

        private void trackBar2_MouseUp(object sender, MouseEventArgs e)
        {
            pictureBox5.Image = null;
            trackBar2.Enabled = false;
            numericUpDown1.Enabled = false;
            thread = new Thread(WorkThresholdOnly);
            thread.Start();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            trackBar2.Value = (int)numericUpDown1.Value;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SaveResult();
        }

        private void SaveResult()
        {
            LoadAndSaveImage.SaveImage(pictureBox8.Image);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ClearAndStart();
        }

        private void trackBar2_MouseUp_1(object sender, MouseEventArgs e)
        {
            button1.Enabled = groupBox2.Enabled = false;
            thread = new Thread(WorkThresholdOnly);
            thread.Start();
        }

        private void trackBar2_ValueChanged_1(object sender, EventArgs e)
        {
            numericUpDown1.Value = trackBar2.Value;
        }

        private void numericUpDown1_ValueChanged_1(object sender, EventArgs e)
        {
            trackBar2.Value = (int)numericUpDown1.Value;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            pictureBox8.Image = ImageProcessing.ReverseColor(new Bitmap(pictureBox8.Image));
        }

        private void pictureBox11_Click(object sender, EventArgs e)
        {
            pictureBox8.Width = (int)(pictureBox8.Width * 1.1);
            pictureBox8.Height = (int)(pictureBox8.Height * 1.1);
        }

        private void pictureBox12_Click(object sender, EventArgs e)
        {
            pictureBox8.Width = (int)(pictureBox8.Width * 0.9);
            pictureBox8.Height = (int)(pictureBox8.Height * 0.9);
        }

        private void pictureBox13_Click(object sender, EventArgs e)
        {
            pictureBox8.Width = panel1.Width;
            pictureBox8.Height = panel1.Height;
        }
    }
}
