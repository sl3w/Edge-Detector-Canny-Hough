using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Edge_detection
{
    static class ImageProcessing
    {
        //private static int[,] laplas = new int[,] {
          //                       { 0, 1, 0 },
          //                       { 1, -4, 1 },
           //                      { 0, 1, 0 } };

        public static Bitmap ImageToGrey(Bitmap bmp)
        {
            Bitmap resbmp = new Bitmap(bmp.Width, bmp.Height);
            byte brightness;
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    brightness = (byte)(0.299 * bmp.GetPixel(i, j).R + 0.587 * bmp.GetPixel(i, j).G + 0.114 * bmp.GetPixel(i, j).B);
                    resbmp.SetPixel(i, j, Color.FromArgb(brightness, brightness, brightness));
                }
            }
            return resbmp;
        }

        
        public static Bitmap FadeLaplassThreshold(Bitmap bmp, int border)
        {
            Bitmap resbmp = new Bitmap(bmp);
            byte brightness;
            for (int i = 0; i < bmp.Width - 1; i++)
            {
                for (int j = 0; j < bmp.Height - 1; j++)
                {
                    brightness = bmp.GetPixel(i, j).B;
                    if (brightness < border)
                        brightness = 0;
                    else
                        brightness = 255;
                    resbmp.SetPixel(i, j, Color.FromArgb(brightness, brightness, brightness));
                }
            }
            return resbmp;
        }


        public static Bitmap ReverseColor(Bitmap bmp)
        {
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    byte B = bmp.GetPixel(i, j).B;
                    B = (byte)Math.Abs(255 - B);
                    bmp.SetPixel(i, j, Color.FromArgb(B, B, B));
                }
            }
            return bmp;
        }
    }
}
