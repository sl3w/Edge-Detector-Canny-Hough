using Edge_detection.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Edge_detection
{
    public partial class ImageProcessingForm : Form
    {
        private Image uploadedImage;
        private Thread thread;

        private int pbLocX = 30;
        private int pbLocY = 0;

        private int labLocY = 170;

        public ImageProcessingForm(Image img)
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 4;
            WindowState = FormWindowState.Maximized;
            uploadedImage = img;
        }
        private void ImageProcessingForm_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            groupBox2.Enabled = checkBox3.Checked;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            panel1.Controls.Clear();
            pbLocX = 30;
            thread = new Thread(StartProcessing);
            thread.Start();
        }

        private void AddImageOnPanel(Image image, string text)
        {
            PictureBox pb = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.Zoom,
                Image = image,
                Size = new Size(245, 160),
                Location = new Point(pbLocX, pbLocY)
            };
            pb.Click += pictureBox1_Click;

            Label lab = new Label
            {
                Text = text,
                AutoSize = true
            };
            lab.Location = new Point(pbLocX, labLocY);

            pbLocX += 275;

            panel1.Controls.Add(pb);
            panel1.Controls.Add(lab);
        }

        public void StartProcessing()
        {
            try
            {
                int method = comboBox1.SelectedIndex;
                switch (method)
                {
                    case 4:
                        CannyEdgeDetecting();
                        break;
                    case 5:
                        MarrHildrethDetecting();
                        break;
                }
                thread.Abort();
            }
            catch { }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            var thisPb = (sender as PictureBox);
            pictureBox1.Image = thisPb.Image;
            for (int i = 0; i < panel1.Controls.Count; i++)
            {
                var pb = panel1.Controls[i];
                if (pb is PictureBox)
                    (pb as PictureBox).BorderStyle = BorderStyle.None;
            }
            thisPb.BorderStyle = BorderStyle.Fixed3D;
        }
    
        private void CannyEdgeDetecting()
        {
            Bitmap afterGrey;
            //if (isFirstStart)
            //{
            var bmp = new Bitmap(uploadedImage);
            panel1.Invoke(new Action(() => AddImageOnPanel(new Bitmap(bmp), "Исходное изображение")));

            afterGrey = ImageProcessing.ImageToGrey(bmp);
            panel1.Invoke(new Action(() => AddImageOnPanel(new Bitmap(afterGrey), "Оттенки серого")));
            //pictureBox9.Invoke(new Action(() => pictureBox9.Image = new Bitmap(afterGrey)));
            //isFirstStart = false;
            //}
            //else
            // afterGrey = new Bitmap(pictureBox2.Image);

            //ShowSelectedImage();

            Bitmap afterGauss;
            if (checkBox2.Checked)
            {
                double sigma = 0;
                if (radioButton6.Checked)
                    sigma = (double)numericUpDown4.Value;
                afterGauss = Filters.GaussianFilter(afterGrey, sigma);
                panel1.Invoke(new Action(() => AddImageOnPanel(new Bitmap(afterGauss), "Фильтр Гаусса")));
            }
            else
                afterGauss = afterGrey;

            //ShowSelectedImage();

            Bitmap afterSobel = Edges.SobelConvolve(afterGauss);
            //Bitmap afterSobel2 = Edges.Sobel(afterGauss);
            panel1.Invoke(new Action(() => AddImageOnPanel(new Bitmap(afterSobel), "Фильтр Собеля")));
            //pictureBox14.Invoke(new Action(() => pictureBox14.Image = new Bitmap(afterSobel2)));
            //ShowSelectedImage();

            Bitmap afterSuppression = Edges.NonMaximumSuppression(afterSobel);
            //Bitmap afterSuppression2 = Edges.NonMaximumSuppression(afterSobel2);
            panel1.Invoke(new Action(() => AddImageOnPanel(new Bitmap(afterSuppression), "Подавление не-максимумов")));
            //pictureBox15.Invoke(new Action(() => pictureBox15.Image = new Bitmap(afterSuppression2)));

            //ShowSelectedImage();

            Bitmap afterThreshold = Edges.DoubleThreshold(afterSuppression, trackBar3.Value, trackBar2.Value);
            //Bitmap afterThreshold2 = Edges.DoubleThreshold(afterSuppression2, trackBar3.Value, trackBar2.Value);
            panel1.Invoke(new Action(() => AddImageOnPanel(new Bitmap(afterThreshold), "Двойная проговая фильтрация")));
            //pictureBox16.Invoke(new Action(() => pictureBox16.Image = new Bitmap(afterThreshold2)));
            //ShowSelectedImage();

            Bitmap afterEdgeTrack = Edges.EdgeTracking(afterThreshold);
            //Bitmap afterEdgeTrack2 = Edges.EdgeTracking(afterThreshold2);
            panel1.Invoke(new Action(() => AddImageOnPanel(new Bitmap(afterEdgeTrack), "Трассировка области неоднозначности")));
            //pictureBox17.Invoke(new Action(() => pictureBox17.Image = new Bitmap(afterEdgeTrack2)));
            //ShowSelectedImage();

            //Bitmap afterRestoration = Edges.BorderRestoration(afterEdgeTrack, trackBar4.Value);
            //panel1.Invoke(new Action(() => AddImageOnPanel(new Bitmap(afterRestoration), "Восстановление границ")));

            //ShowSelectedImage();
            //button1.Enabled = button6.Enabled = groupBox1.Enabled = true;
        }

        private void MarrHildrethDetecting()
        {
            Bitmap afterGrey;

            var bmp = new Bitmap(uploadedImage);
            panel1.Invoke(new Action(() => AddImageOnPanel(new Bitmap(bmp), "Исходное изображение")));

            afterGrey = ImageProcessing.ImageToGrey(bmp);
            panel1.Invoke(new Action(() => AddImageOnPanel(new Bitmap(afterGrey), "Оттенки серого")));

            Bitmap afterGauss;
            if (checkBox2.Checked)
            {
                double sigma = 0;
                if (radioButton6.Checked)
                    sigma = (double)numericUpDown4.Value;
                afterGauss = Filters.GaussianFilter(afterGrey, sigma);
                panel1.Invoke(new Action(() => AddImageOnPanel(new Bitmap(afterGauss), "Фильтр Гаусса")));
            }
            else
                afterGauss = afterGrey;

            var afterMarrHildreth = LoG.MarrHildrethEdge(new Bitmap(afterGauss));
            panel1.Invoke(new Action(() => AddImageOnPanel(new Bitmap(afterMarrHildreth), "Метод Марр-Хилдрет")));
        }
    }
}
