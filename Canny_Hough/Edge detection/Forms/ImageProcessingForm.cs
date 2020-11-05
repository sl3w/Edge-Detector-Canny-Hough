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
        
        private bool gaussChanged = true;
        private bool sobelChanged = true;
        private Bitmap gaussed;
        private Bitmap sobeled;
        private Bitmap grayscaled;

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

        // private void checkBox3_CheckedChanged(object sender, EventArgs e)
        // {
        //     switch (comboBox1.SelectedIndex)
        //     {
        //         case 4:
        //         case 5:
        //             button2.Enabled = false;
        //             break;
        //         default:
        //             button2.Enabled = true;
        //             break;
        //     }
        // }

        private void button1_ClickTest(object sender, EventArgs e)
        {
            Bitmap afterGrey;

            var bmp = new Bitmap(uploadedImage);
            panel1.Invoke(new Action(() => AddImageOnPanel(new Bitmap(bmp), "Исходное изображение")));

            afterGrey = ImageProcessing.ImageToGrey(bmp);
            panel1.Invoke(new Action(() => AddImageOnPanel(new Bitmap(afterGrey), "Оттенки серого")));

            double sigma = 0;
            Bitmap afterGauss = Filters.GaussianFilter(afterGrey, sigma, !checkBox3.Checked);
            panel1.Invoke(new Action(() => AddImageOnPanel(new Bitmap(afterGauss), "Фильтр Гаусса")));

            Bitmap afterColor = Filters.GaussianFilterColor(bmp, 0);
            panel1.Invoke(new Action(() => AddImageOnPanel(new Bitmap(afterColor), "Цветное")));

            //
            //
            // Bitmap afterMedian = Filters.MedianFilter(afterGrey, 5);
            // panel1.Invoke(new Action(() => AddImageOnPanel(new Bitmap(afterMedian), "Медианный фильтр")));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (thread != null && thread.IsAlive)
                thread.Abort();
            panel1.Controls.Clear();
            panel1.AutoScroll = false;
            panel1.AutoScroll = true;
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
            button2.Enabled = false;
            pictureBox1.Image = null;
            try
            {
                int method = comboBox1.SelectedIndex;
                switch (method)
                {
                    case 0:
                        FadeLaplacianDetecting(true);
                        break;
                    case 1:
                        FadeLaplacianDetecting(false);
                        break;
                    case 2:
                        SobelDetecting();
                        break;
                    case 4:
                        CannyEdgeDetecting();
                        break;
                    case 5:
                        MarrHildrethDetecting();
                        break;
                }

                button2.Enabled = true;
                var lastPb = (PictureBox) panel1.Controls[panel1.Controls.Count - 2];
                pictureBox1.Image = lastPb.Image;
                thread.Abort();
            }
            catch
            {
                button2.Enabled = true;
            }
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

        private void SobelDetecting()
        {
            var bmp = new Bitmap(uploadedImage);
            panel1.Invoke(new Action(() => AddImageOnPanel(new Bitmap(bmp), "Исходное изображение")));

            Bitmap afterGrey;
            if (checkBox3.Checked)
            {
                afterGrey = ImageProcessing.ImageToGrey(bmp);
                panel1.Invoke(new Action(() => AddImageOnPanel(new Bitmap(afterGrey), "Оттенки серого")));
            }
            else
                afterGrey = bmp;

            Bitmap afterGauss;
            if (checkBox2.Checked)
            {
                double sigma = 0;
                if (radioButton6.Checked)
                    sigma = (double) numericUpDown4.Value;
                afterGauss = Filters.GaussianFilter(afterGrey, sigma, !checkBox3.Checked);
                panel1.Invoke(new Action(() => AddImageOnPanel(new Bitmap(afterGauss), "Фильтр Гаусса")));
            }
            else
                afterGauss = afterGrey;


            var afterSobel =
                checkBox3.Checked ? Edges.SobelConvolve(afterGauss) : Edges.SobelConvolveColor(afterGauss);
            panel1.Invoke(new Action(() => AddImageOnPanel(new Bitmap(afterSobel), "Фильтр Собеля")));

            if (checkBox4.Checked)
            {
                var afterThreshold = checkBox3.Checked
                    ? ImageProcessing.FadeLaplassThreshold(afterSobel, trackBar5.Value)
                    : ImageProcessing.FadeLaplassThresholdColor(afterSobel, trackBar5.Value);
                panel1.Invoke(new Action(() => AddImageOnPanel(new Bitmap(afterThreshold), "Пороговая фильтрация")));
            }
        }

        private void CannyEdgeDetecting()
        {
            //if (isFirstStart)
            //{
            var bmp = new Bitmap(uploadedImage);
            panel1.Invoke(new Action(() => AddImageOnPanel(new Bitmap(bmp), "Исходное изображение")));

            Bitmap afterGrey;
            if (checkBox3.Checked)
            {
                if (grayscaled == null)
                {
                    afterGrey = ImageProcessing.ImageToGrey(bmp);
                    grayscaled = afterGrey;
                }
                else
                    afterGrey = grayscaled;
                panel1.Invoke(new Action(() => AddImageOnPanel(new Bitmap(afterGrey), "Оттенки серого")));
            }
            else
                afterGrey = bmp;
            
            Bitmap afterGauss;
            if (checkBox2.Checked)
            {
                if (gaussChanged)
                {
                    double sigma = 0;
                    if (radioButton6.Checked)
                        sigma = (double) numericUpDown4.Value;
                    afterGauss = Filters.GaussianFilter(afterGrey, sigma, !checkBox3.Checked);
                    gaussChanged = false;
                    gaussed = afterGauss;
                }
                else
                    afterGauss = gaussed;
                panel1.Invoke(new Action(() => AddImageOnPanel(new Bitmap(afterGauss), "Фильтр Гаусса")));
            }
            else
                afterGauss = afterGrey;

            
            if (checkBox3.Checked)
            {
                Bitmap afterSobel;
                if (sobelChanged)
                {
                    afterSobel = Edges.SobelConvolve(afterGauss);
                    sobeled = afterSobel;
                    sobelChanged = false;
                }
                else
                    afterSobel = sobeled;

                panel1.Invoke(new Action(() => AddImageOnPanel(new Bitmap(afterSobel), "Фильтр Собеля")));

                Bitmap afterSuppression = Edges.NonMaximumSuppression(afterSobel);
                panel1.Invoke(
                    new Action(() => AddImageOnPanel(new Bitmap(afterSuppression), "Подавление не-максимумов")));
                
                Bitmap afterThreshold = Edges.DoubleThreshold(afterSuppression, trackBar3.Value, trackBar2.Value);
                panel1.Invoke(new Action(() =>
                    AddImageOnPanel(new Bitmap(afterThreshold), "Двойная проговая фильтрация")));

                Bitmap afterEdgeTrack = Edges.EdgeTracking(afterThreshold);
                panel1.Invoke(new Action(() =>
                    AddImageOnPanel(new Bitmap(afterEdgeTrack), "Трассировка области неоднозначности")));

                //Bitmap afterRestoration = Edges.BorderRestoration(afterEdgeTrack, trackBar4.Value);
                //panel1.Invoke(new Action(() => AddImageOnPanel(new Bitmap(afterRestoration), "Восстановление границ")));
            }
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
                    sigma = (double) numericUpDown4.Value;
                afterGauss = Filters.GaussianFilter(afterGrey, sigma, !checkBox3.Checked);
                panel1.Invoke(new Action(() => AddImageOnPanel(new Bitmap(afterGauss), "Фильтр Гаусса")));
            }
            else
                afterGauss = afterGrey;

            var afterMarrHildreth = LoG.MarrHildrethEdge(new Bitmap(afterGauss));
            panel1.Invoke(new Action(() => AddImageOnPanel(new Bitmap(afterMarrHildreth), "Метод Марр-Хилдрет")));

            Bitmap afterThreshold = ImageProcessing.FadeLaplassThreshold(afterMarrHildreth, trackBar5.Value);
            panel1.Invoke(new Action(() => AddImageOnPanel(new Bitmap(afterThreshold), "Пороговая фильтрация")));
        }

        public void FadeLaplacianDetecting(bool isFade)
        {
            Bitmap afterGrey;

            var bmp = new Bitmap(uploadedImage);
            panel1.Invoke(new Action(() => AddImageOnPanel(new Bitmap(bmp), "Исходное изображение")));

            if (checkBox3.Checked)
            {
                afterGrey = ImageProcessing.ImageToGrey(bmp);
                panel1.Invoke(new Action(() => AddImageOnPanel(new Bitmap(afterGrey), "Оттенки серого")));
            }
            else
                afterGrey = bmp;

            Bitmap afterGauss;
            if (checkBox2.Checked)
            {
                double sigma = 0;
                if (radioButton6.Checked)
                    sigma = (double) numericUpDown4.Value;
                afterGauss = Filters.GaussianFilter(afterGrey, sigma, !checkBox3.Checked);
                panel1.Invoke(new Action(() => AddImageOnPanel(new Bitmap(afterGauss), "Фильтр Гаусса")));
            }
            else
                afterGauss = afterGrey;

            
            //if (checkBox3.Checked)
            
                var afterFade = isFade
                    ? (checkBox3.Checked
                        ? Edges.FadeDetection(new Bitmap(afterGauss))
                        : Edges.FadeDetectionColor(new Bitmap(afterGauss)))
                    : (checkBox3.Checked
                        ? Edges.LaplacianDetection(new Bitmap(afterGauss))
                        : Edges.LaplacianDetectionColor(new Bitmap(afterGauss)));
                var text = isFade ? "Метод на основе градиента" : "Метод на основе лапласиана";
                panel1.Invoke(new Action(() => AddImageOnPanel(new Bitmap(afterFade), text)));


                if (checkBox4.Checked)
                {
                    var afterThreshold = checkBox3.Checked
                        ? ImageProcessing.FadeLaplassThreshold(afterFade, trackBar5.Value)
                        : ImageProcessing.FadeLaplassThresholdColor(afterFade, trackBar5.Value);
                    panel1.Invoke(new Action(() =>
                        AddImageOnPanel(new Bitmap(afterThreshold), "Пороговая фильтрация")));
                }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            groupBox3.Enabled = checkBox2.Checked;
            radioButton5.Enabled = checkBox2.Checked;
            radioButton1.Enabled = checkBox2.Checked;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked && !checkBox3.Checked)
            {
                switch (comboBox1.SelectedIndex)
                {
                    case 0:
                    case 1:
                        button2.Enabled = true;
                        // groupBox7.Enabled = true;
                        radioButton2.Enabled = true;
                        checkBox4.Enabled = true;
                        checkBox3.Enabled = true;
                        radioButton2.Checked = true;
                        //groupBox6.Enabled = true;
                        break;
                    case 4:
                        radioButton2.Enabled = false;
                        checkBox4.Enabled = false;
                        checkBox4.Checked = true;
                        checkBox3.Enabled = false;
                        checkBox3.Checked = true;
                        radioButton3.Checked = true;
                        
                        button2.Enabled = false;
                        break;
                    case 5:
                        groupBox7.Enabled = true;
                        radioButton2.Checked = true;
                        button2.Enabled = false;
                        break;
                    default:
                        radioButton2.Enabled = true;
                        radioButton2.Checked = true;
                        checkBox4.Enabled = true;
                        checkBox3.Enabled = true;
                        button2.Enabled = true;
                        //groupBox6.Enabled = false;
                        break;
                }
            }
            else
            {
                button2.Enabled = true;
                switch (comboBox1.SelectedIndex)
                {
                    case 0:
                    case 1:
                        button2.Enabled = true;
                        radioButton2.Checked = true;
                        radioButton2.Enabled = true;
                        checkBox4.Enabled = true;
                        checkBox3.Enabled = true;
                        //groupBox6.Enabled = true;
                        break;
                    case 4:
                        //groupBox6.Enabled = true;
                        radioButton2.Enabled = false;
                        checkBox4.Enabled = false;
                        checkBox4.Checked = true;
                        checkBox3.Enabled = false;
                        checkBox3.Checked = true;
                        radioButton3.Checked = true;
                        break;
                    default:
                        //groupBox6.Enabled = false;
                        radioButton2.Checked = true;
                        radioButton2.Enabled = true;
                        checkBox4.Enabled = true;
                        checkBox3.Enabled = true;
                        break;
                }
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            comboBox1.Enabled = checkBox4.Checked = groupBox7.Enabled = checkBox1.Checked;
            if (!checkBox1.Checked) checkBox3.Enabled = true;
            else
            {
                if (comboBox1.SelectedIndex == 4)
                    checkBox3.Enabled = false;
            }
            if (checkBox1.Checked && !checkBox3.Checked)
            {
                switch (comboBox1.SelectedIndex)
                {
                    case 4:
                    case 5:
                        button2.Enabled = false;
                        break;
                    default:
                        button2.Enabled = true;
                        break;
                }
            }
            else
            {
                button2.Enabled = true;
            }
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            radioButton6.Visible = radioButton7.Visible =
                pictureBox10.Visible = numericUpDown4.Visible = radioButton5.Checked;
            trackBar1.Visible = label2.Visible = numericUpDown5.Visible = !radioButton5.Checked;
            groupBox3.Text = radioButton5.Checked ? "Настройки фильтра Гаусса" : "Настройки медианного фильтра";
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            groupBox6.Enabled = checkBox4.Checked;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDown1.Visible = label3.Visible = trackBar2.Visible = radioButton3.Checked;
            numericUpDown2.Visible = label4.Visible = trackBar3.Visible = radioButton3.Checked;
            numericUpDown6.Visible = label5.Visible = trackBar5.Visible = !radioButton3.Checked;
        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            gaussChanged = true;
            sobelChanged = true;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            LoadAndSaveImage.SaveImage(pictureBox1.Image);
        }

        private void pictureBox11_Click(object sender, EventArgs e)
        {
            pictureBox1.Width = (int)(pictureBox1.Width * 1.1);
            pictureBox1.Height = (int)(pictureBox1.Height * 1.1);
        }

        private void pictureBox12_Click(object sender, EventArgs e)
        {
            pictureBox1.Width = (int)(pictureBox1.Width * 0.9);
            pictureBox1.Height = (int)(pictureBox1.Height * 0.9);
        }

        private void pictureBox13_Click(object sender, EventArgs e)
        {
            pictureBox1.Width = panel2.Width;
            pictureBox1.Height = panel2.Height;
        }
    }
}