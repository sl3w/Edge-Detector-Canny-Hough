using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Edge_detection
{
    public partial class CannyDetection : Form
    {
        private Image uploadedImage;
        private Thread thread;
        private Bitmap afterSuppression;
        private Bitmap afterEdgeTrack;
        private bool isFirstStart = true;

        public CannyDetection(Image img)
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
            uploadedImage = img;
        }

        private void CannyDetection_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            ClearAndStart();
        }

        private void ClearAndStart()
        {
            pictureBox3.Image = null;
            pictureBox4.Image = null;
            pictureBox5.Image = null;
            pictureBox6.Image = null;
            pictureBox7.Image = null;
            pictureBox8.Image = null;
            pictureBox9.Image = null;
            button1.Enabled = button6.Enabled = groupBox1.Enabled = false;
            thread = new Thread(StartProcessing);
            thread.Start();
        }

        public void StartProcessing()
        {
            Bitmap afterGrey;
            if (isFirstStart)
            {
                var bmp = new Bitmap(uploadedImage);
                pictureBox1.Invoke(new Action(() => pictureBox1.Image = new Bitmap(bmp)));

                afterGrey = ImageProcessing.ImageToGrey(bmp);
                pictureBox2.Invoke(new Action(() => pictureBox2.Image = new Bitmap(afterGrey)));
                pictureBox9.Invoke(new Action(() => pictureBox9.Image = new Bitmap(afterGrey)));
                isFirstStart = false;
            }
            else
                afterGrey = new Bitmap(pictureBox2.Image);

            ShowSelectedImage();

            Bitmap afterGauss;
            if (checkBox1.Checked)
            {
                double sigma = 0;
                if (radioButton2.Checked)
                    sigma = (double)numericUpDown4.Value;
                afterGauss = Filters.GaussianFilter(afterGrey, sigma);
            }
            else
                afterGauss = afterGrey;

            ShowSelectedImage();

            pictureBox3.Invoke(new Action(() => pictureBox3.Image = new Bitmap(afterGauss)));

            ShowSelectedImage();

            Bitmap afterSobel = Edges.SobelConvolve(afterGauss);
            Bitmap afterSobel2 = Edges.Sobel(afterGauss);
            pictureBox4.Invoke(new Action(() => pictureBox4.Image = new Bitmap(afterSobel)));
            pictureBox14.Invoke(new Action(() => pictureBox14.Image = new Bitmap(afterSobel2)));
            ShowSelectedImage();

            afterSuppression = Edges.NonMaximumSuppression(afterSobel);
            Bitmap afterSuppression2 = Edges.NonMaximumSuppression(afterSobel2);
            pictureBox5.Invoke(new Action(() => pictureBox5.Image = new Bitmap(afterSuppression)));
            pictureBox15.Invoke(new Action(() => pictureBox15.Image = new Bitmap(afterSuppression2)));

            ShowSelectedImage();

            Bitmap afterThreshold = Edges.DoubleThreshold(afterSuppression, trackBar3.Value, trackBar2.Value);
            Bitmap afterThreshold2 = Edges.DoubleThreshold(afterSuppression2, trackBar3.Value, trackBar2.Value);
            pictureBox6.Invoke(new Action(() => pictureBox6.Image = new Bitmap(afterThreshold)));
            pictureBox16.Invoke(new Action(() => pictureBox16.Image = new Bitmap(afterThreshold2)));
            ShowSelectedImage();

            afterEdgeTrack = Edges.EdgeTracking(afterThreshold);
            Bitmap afterEdgeTrack2 = Edges.EdgeTracking(afterThreshold2);
            pictureBox7.Invoke(new Action(() => pictureBox7.Image = new Bitmap(afterEdgeTrack)));
            pictureBox17.Invoke(new Action(() => pictureBox17.Image = new Bitmap(afterEdgeTrack2)));
            ShowSelectedImage();

            Bitmap afterRestoration = Edges.BorderRestoration(afterEdgeTrack, trackBar4.Value);
            pictureBox8.Invoke(new Action(() => pictureBox8.Image = new Bitmap(afterRestoration)));

            ShowSelectedImage();
            button1.Enabled = button6.Enabled = groupBox1.Enabled = true;
            thread.Abort();
        }

        private void WorkFromThreshold()
        {
            Bitmap afterThreshold = Edges.DoubleThreshold(afterSuppression, trackBar3.Value, trackBar2.Value);
            pictureBox6.Invoke(new Action(() => pictureBox6.Image = new Bitmap(afterThreshold)));

            afterEdgeTrack = Edges.EdgeTracking(afterThreshold);
            pictureBox7.Invoke(new Action(() => pictureBox7.Image = new Bitmap(afterEdgeTrack)));

            Bitmap afterRestoration = Edges.BorderRestoration(afterEdgeTrack, trackBar4.Value);
            pictureBox8.Invoke(new Action(() => pictureBox8.Image = new Bitmap(afterRestoration)));

            ShowSelectedImage();
            button1.Enabled = button6.Enabled = groupBox1.Enabled = true;
            thread.Abort();
        }

        private void WorkBorderRestorOnly()
        { 
            Bitmap afterRestoration = Edges.BorderRestoration(afterEdgeTrack, trackBar4.Value);
            pictureBox8.Invoke(new Action(() => pictureBox8.Image = new Bitmap(afterRestoration)));

            ShowSelectedImage();
            button1.Enabled = button6.Enabled = groupBox1.Enabled = true;
            thread.Abort();
        }

        private void ShowSelectedImage()
        {
            PictureBox pb = Controls["pictureBox" + trackBar1.Value.ToString()] as PictureBox;
            pb.BorderStyle = BorderStyle.Fixed3D;
            pictureBox9.Image = pb.Image;
            for (int i = 1; i < 9; i++)
            {
                if (i != trackBar1.Value)
                {
                    pb = Controls["pictureBox" + i.ToString()] as PictureBox;
                    pb.BorderStyle = BorderStyle.None;
                }
            }
        }

        private void trackBar_MouseUp(object sender, MouseEventArgs e)
        {
            pictureBox6.Image = null;
            pictureBox7.Image = null;
            pictureBox8.Image = null;
            button1.Enabled = button6.Enabled = groupBox1.Enabled = false;
            thread = new Thread(WorkFromThreshold);
            thread.Start();
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            ShowSelectedImage();
        }

        private void WorkForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            thread.Abort();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ClearAndStart();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SaveResult();
        }

        private void SaveResult()
        {
            LoadAndSaveImage.SaveImage(pictureBox9.Image);
        }

        private void trackBar3_ValueChanged(object sender, EventArgs e)
        {
            if (trackBar3.Value <= trackBar2.Value + 1)
                trackBar3.Value = trackBar2.Value + 2;
            numericUpDown2.Value = trackBar3.Value;
        }

        private void trackBar2_ValueChanged(object sender, EventArgs e)
        {
            if (trackBar2.Value >= trackBar3.Value - 1)
                trackBar2.Value = trackBar3.Value - 2;
            numericUpDown1.Value = trackBar2.Value;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            trackBar2.Value = (int)numericUpDown1.Value;
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            trackBar3.Value = (int)numericUpDown2.Value;
        }

        private void pictureBox_Click(object sender, EventArgs e)
        {
            int x = trackBar1.Value;
            PictureBox pb = sender as PictureBox;
            //test
            //trackBar1.Value = int.Parse(pb.Name.Substring(10));
            pictureBox9.Image = pb.Image;
            //if (trackBar1.Value == x)
              //  ShowSelectedImage();
        }

        private void trackBar4_ValueChanged(object sender, EventArgs e)
        {
            numericUpDown3.Value = trackBar4.Value;
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            trackBar4.Value = (int)numericUpDown3.Value;
        }

        private void trackBar4_MouseUp(object sender, MouseEventArgs e)
        {
            pictureBox8.Image = null;
            button1.Enabled = button6.Enabled = groupBox1.Enabled = false;
            thread = new Thread(WorkBorderRestorOnly);
            thread.Start();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            pictureBox1.BorderStyle = BorderStyle.Fixed3D;
            pictureBox9.Image = pictureBox1.Image;
            for (int i = 2; i < 9; i++)
            {
                (Controls["pictureBox" + i.ToString()] as PictureBox).BorderStyle = BorderStyle.None;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            pictureBox9.Image = ImageProcessing.ReverseColor(new Bitmap(pictureBox9.Image));
        }

        private Bitmap bitmapSravn;

        private void button4_Click(object sender, EventArgs e)
        {           
            Bitmap mineImg = new Bitmap(pictureBox9.Image);

            int count = 0;
            for (int i = 0; i < bitmapSravn.Width; i++)
            {
                for (int j = 0; j < bitmapSravn.Height; j++)
                {
                    if (bitmapSravn.GetPixel(i, j).B == mineImg.GetPixel(i, j).B)
                        count++;
                }
            }
            label13.Text = count + "/" + (bitmapSravn.Width * bitmapSravn.Height) + Environment.NewLine +
                (float)count / (bitmapSravn.Width * bitmapSravn.Height);
            label13.Visible = true;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            bitmapSravn = new Bitmap(Image.FromFile(LoadAndSaveImage.UploadImage()));
        }

        private void pictureBox11_Click(object sender, EventArgs e)
        {
            pictureBox9.Width = (int)(pictureBox9.Width * 1.1);
            pictureBox9.Height = (int)(pictureBox9.Height * 1.1);
        }

        private void pictureBox12_Click(object sender, EventArgs e)
        {
            pictureBox9.Width = (int)(pictureBox9.Width * 0.9);
            pictureBox9.Height = (int)(pictureBox9.Height * 0.9);
        }

        private void pictureBox13_Click(object sender, EventArgs e)
        {
            pictureBox9.Width = panel1.Width;
            pictureBox9.Height = panel1.Height;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            new HoughForm(pictureBox9.Image, pictureBox1.Image, "lines").Show();
        }
    }
}
