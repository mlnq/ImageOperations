using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PS8
{
    static class Binarization
    {
        //pull-down menu to add wpf
        public static Bitmap CustomBinarization(Bitmap bitmap,int threshold)
        {

            Bitmap newBitmap = new Bitmap(bitmap.Width, bitmap.Height);
            Color newPixel, pixel;

            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    pixel = bitmap.GetPixel(x, y);

                    //Binaryzacja dwuprogowa:
                    /* if (pixel.R < threshold || pixel.G < threshold || pixel.B < threshold) newPixel = Color.FromArgb(0, 0, 0);
                     else if (pixel.R >= thresholdQ || pixel.G >= thresholdQ || pixel.B >= thresholdQ) newPixel = Color.FromArgb(0, 0, 0);
                     else newPixel = Color.FromArgb(255, 255, 255);*/

                    if (pixel.R < threshold || pixel.G < threshold || pixel.B < threshold) newPixel = Color.FromArgb(0, 0, 0);
                    else newPixel = Color.FromArgb(255, 255, 255);

                    newBitmap.SetPixel(x, y, newPixel);
                }
            }

            return newBitmap;
        }
        public static Bitmap PercentBlackPixels(Bitmap bitmap, float thresholdProcent)
        {

            Bitmap newBitmap = new Bitmap(bitmap.Width, bitmap.Height);
            Color newPixel, pixel;
            var tLvl=0;

            int[,] histogram = Histogram.GetHistogram(bitmap);
            
            float NP = bitmap.Width * bitmap.Height * thresholdProcent;

            int sum=0;
            var threshold = 0;

            for (int i = 0; i < 256; i++)
            {
                sum += histogram[i, 2];
                if (sum>= NP) { threshold = i; break; }
            }

            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    pixel = bitmap.GetPixel(x, y);

                   if (pixel.R < threshold || pixel.G < threshold || pixel.B < threshold) newPixel = Color.FromArgb(0, 0, 0);
                    else newPixel = Color.FromArgb(255, 255, 255);

                    newBitmap.SetPixel(x, y, newPixel);
                }
            }

            return newBitmap;
        }
        public static Bitmap MeanIterativeSelection(Bitmap bitmap)
        {

            Bitmap newBitmap = new Bitmap(bitmap.Width, bitmap.Height);
            Color newPixel, pixel;
            var tLvl = 0;

            int[,] histogram = Histogram.GetHistogram(bitmap);

            int sum =0;
            int threshold = 0;

            for (int i = 0; i < 256; i++)
            {
                sum += i*histogram[i, 3];
            }
            //  srednia wartos histogramu /ilosc pikseli //Global mean

            int Tk = sum/(bitmap.Width*bitmap.Height);

            int Tkm1 = Tk;
            double
                a = 0.0,
                b = 0.0,
                c = 0.0,
                d = 0.0,
                h = 0.0;

            do
            {
                Tkm1 = Tk;
                a = b = c = d = 0.0;

                for (int i = 0; i <= Tkm1; i++)
                {
                    a += i * histogram[i, 3];
                    b += histogram[i, 3];
                }
                b *= 2.0;


                for (int j = Tkm1 + 1; j <= 255; j++)
                {
                    c += j * histogram[j, 3];
                    d += histogram[j, 3];
                }
                d *= 2.0;

                Tk = (int)(a / b + c / d);
            }
            while (Tk != Tkm1);

            threshold = Tk;

            //warunek progowania 
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    pixel = bitmap.GetPixel(x, y);

                    if (pixel.R < threshold || pixel.G < threshold || pixel.B < threshold) newPixel = Color.FromArgb(0, 0, 0);
                    else newPixel = Color.FromArgb(255, 255, 255);

                    newBitmap.SetPixel(x, y, newPixel);
                }
            }

            return newBitmap;
        }
      
        public static Bitmap EntropySelection(Bitmap bitmap)
        {

            Bitmap newBitmap = new Bitmap(bitmap.Width, bitmap.Height);
            Color newPixel, pixel;
            var tLvl = 0;
            int threshold;

            int[,] histogram = Histogram.GetHistogram(bitmap);

            int begining = 0,end = 255;
           /* for(int k=0;k<256;k++)
            {
                if (histogram[k, 3] != 0) { begining = k; break; }
            }
            for (int k = 255; k >=0; k--)
            {
                if (histogram[k, 3] != 0) {end = k; break; }
            }
*/
            int a = 1, b = 1, T = 0;

            double Maximum = 0;
            double H;
            int i;

            for (int t = begining; t <= end; t++)
            {
                int sum1 = 0, sum2 = 0;
                double Hb = 0.0, Hw = 0.0;
                double p1 = 0.0, p2 = 0.0;

                //sigma
                for (int j = 0; j <= t; j++)
                {
                    sum1 += histogram[j, 3];
                }

                //sigma
                for (int j = t + 1; j <= 255; j++)
                {
                    sum2 += histogram[j, 3];
                }
                if (sum1 == 0 || sum2 == 0) continue;
                for (i = 0; i <= t; i++)
                {
                    p1 = (double)histogram[i, 3] / (double)sum1;
                    Hb -=  p1 == 0 ?  0 : p1 * Math.Log2(p1);

                }

                for (i = t + 1; i <= 255; i++)
                {
                    p2 = (double)histogram[i, 3] / (double)sum2;
                    Hw -= p2 == 0 ? 0 : p2 * Math.Log2(p2);
                }

                H = Hb + Hw;

                if (H > Maximum)
                {
                    Maximum = H;

                    T = t;
                }
            }
            threshold = T;

            //warunek progowania 
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    pixel = bitmap.GetPixel(x, y);

                    if (pixel.R < threshold || pixel.G < threshold || pixel.B < threshold) newPixel = Color.FromArgb(0, 0, 0);
                    else newPixel = Color.FromArgb(255, 255, 255);

                    newBitmap.SetPixel(x, y, newPixel);
                }
            }

            return newBitmap;
        }
        public static Bitmap MinimumError(Bitmap bitmap)
        {

            Bitmap newBitmap = new Bitmap(bitmap.Width, bitmap.Height);
            Color newPixel, pixel;
            var tLvl = 0;
            int threshold;

            int[,] histogram = Histogram.GetHistogram(bitmap);
            double[] relativeHistogram = new double[256];

            double[] J = new double[256];

            //criterion function
            for (int i = 0; i <= 255; i++)
            {
                relativeHistogram[i] = (double)histogram[i, 3] / (bitmap.Width*bitmap.Height);
                J[i] = int.MaxValue;
            }


            for (int t = 0; t <= 255; t++)
            {
                double sum1 = 0.0, sum2 = 0.0;
                double sigma1 = 0.0, sigma2 = 0.0;

                double mean1 = 0.0, mean2 = 0.0;

                //sum of pixels
                for (int j = 0; j <= t; j++)
                {
                    sum1 += relativeHistogram[j];
                }

                for (int j = t + 1; j <= 255; j++)
                {
                    sum2 += relativeHistogram[j];
                }

                if(sum1 > 0.0  && sum2 > 0.0)
                {
                    for (int j = 0; j <= t; j++)
                    {
                        mean1 += (relativeHistogram[j] * j) /sum1;
                        sigma1 += Math.Sqrt((relativeHistogram[j] * Math.Pow(j - mean1, 2)) / sum1);
                    }

                    for (int j = t + 1; j <= 255; j++)
                    {
                        mean2 += (relativeHistogram[j] * j) / sum2;
                        sigma2 += Math.Sqrt((relativeHistogram[j] * Math.Pow(j - mean2, 2))/ sum2);
                     }
                }

                if (sigma1 > 0.0 && sigma2 > 0.0)
                {
                    J[t]= (1 + 2 * (sum1 * Math.Log(sigma1,2) + sum2 * Math.Log(sigma2)) 
                                   -2 * (sum1 * Math.Log(sum1,2) + sum2 * Math.Log(sum2)));
                }

            }

            threshold = (int) (J.Min() - 0.5);

            //warunek progowania 
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    pixel = bitmap.GetPixel(x, y);

                    if (pixel.R < threshold || pixel.G < threshold || pixel.B < threshold) newPixel = Color.FromArgb(0, 0, 0);
                    else newPixel = Color.FromArgb(255, 255, 255);

                    newBitmap.SetPixel(x, y, newPixel);
                }
            }

            return newBitmap;
        }

    }
}
