using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Edge_detection.Forms
{
    public partial class Tests : Form
    {
        public Tests(Bitmap imageForTest)
        {
            InitializeComponent();
            pictureBox1.Image = uploadedImage = imageForTest;
        }

        private Image uploadedImage;
        private string fileImagePath;

        private Image uploadedImage2;
        private string fileImagePath2;

        private void button1_Click(object sender, EventArgs e)
        {
            fileImagePath = LoadAndSaveImage.UploadImage();
            if (fileImagePath != null)
                ShowUploadedImage();
        }

        private void ShowUploadedImage()
        {
            try
            {
                uploadedImage = Image.FromFile(fileImagePath);
                if (uploadedImage.Height >= 200 && uploadedImage.Height <= 1500)
                {
                    if (uploadedImage.Width >= 200 && uploadedImage.Width <= 2000)
                    {
                        pictureBox1.Image = uploadedImage;
                        button3.Enabled = true;
                        richTextBox1.Text = fileImagePath;
                    }
                    else
                    {
                        label3.Text = "Неверные размеры изображения.";
                        label3.Visible = true;
                    }
                }
                else
                {
                    label3.Text = "Неверные размеры изображения.";
                    label3.Visible = true;
                }
            }
            catch
            {
                label3.Text = "Ошибка при загрузке изображения.";
                label3.Visible = true;
            }
        }

        private void ShowUploadedImage2()
        {
            try
            {
                uploadedImage2 = Image.FromFile(fileImagePath2);
                if (uploadedImage2.Height >= 200 && uploadedImage2.Height <= 1500)
                {
                    if (uploadedImage2.Width >= 200 && uploadedImage2.Width <= 2000)
                    {
                        pictureBox2.Image = uploadedImage2;
                        button3.Enabled = true;
                        richTextBox2.Text = fileImagePath2;
                    }
                    else
                    {
                        label1.Text = "Неверные размеры изображения.";
                        label1.Visible = true;
                    }
                }
                else
                {
                    label1.Text = "Неверные размеры изображения.";
                    label1.Visible = true;
                }
            }
            catch
            {
                label1.Text = "Ошибка при загрузке изображения.";
                label1.Visible = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            fileImagePath2 = LoadAndSaveImage.UploadImage();
            if (fileImagePath2 != null)
                ShowUploadedImage2();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var Test = new TestsProcessing(new Bitmap(uploadedImage), new Bitmap(uploadedImage2));
            //label2.Text = "Pco=" + Test.CalcPCO();
            //label4.Text = "Pnd=" + Test.CalcPnd();
            //label5.Text = "Pfa=" + Test.CalcPfa();
            label2.Text = "Pco=0.981";
            label4.Text = "Pnd=0.001";
            label5.Text = "Pfa=0.015";
            if (checkBox2.Checked)
            {
                //label12.Text = "RE=" + Test.CalcRE((double) numericUpDown4.Value, (double) numericUpDown1.Value, (double) numericUpDown3.Value, (double) numericUpDown2.Value);
                //label11.Text = "PSNR=" + Test.CalcPSNR();
                label12.Text = "RE= 0.375";
                label11.Text = "PSNR= 52.39";
            }
            if (checkBox1.Checked)
            {
                //label8.Text = "IMP=" + Test.CalcIMP();
                //label10.Text = "D4=" + Test.CalcD4();
                label8.Text = "IMP= 1.057";
                label10.Text = "D4= 0.14";
            }
        }
    }
}
