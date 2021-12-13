using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PS8
{
    static class Histogram
    {

        public static int[,] GetHistogram(Bitmap bitmap)
        {
            int[,] histogram = new int[256, 4];

            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    var color = bitmap.GetPixel(i, j);

                    histogram[color.R, 0]++;
                    histogram[color.G, 1]++;
                    histogram[color.B, 2]++;
                    histogram[(int)(color.R + color.G + color.B) / 3, 3]++;
                }
            }
            return histogram;
        }
        public static int[] GetOneChannel(int[,] histogram,int channel)
        {
            int[] helper = new int[histogram.GetLength(1)];
            for(int i=0;i<helper.Length;i++)
            {
                helper[i] = histogram[i, channel];
            }
            return helper;
        }

        private static int getMaxColorVal(int[,] value)
        {
            int maxVal = 0;
            for (int i = 0; i <= 3; i++)
            {
                for (int j = 0; j < 256; j++)
                {
                    if (maxVal < value[j, i])
                        maxVal = value[j, i];
                }
            }
            return maxVal;
        }
        public static Bitmap ShowHistogram(Bitmap bitmap)
        {

            int[,] histogram = GetHistogram(bitmap);

            float max = (float)getMaxColorVal(histogram);

            int histHeight = 128;
            Bitmap img = new Bitmap(256, histHeight + 10);

            Pen redPen = new Pen(Color.FromArgb(214, 0, 0));
            Pen greenPen = new Pen(Color.FromArgb(0, 214, 100));
            Pen bluePen = new Pen(Color.FromArgb(0, 146, 214));
            Pen grayPen = new Pen(Color.FromArgb(145, 145, 145));

            using (Graphics g = Graphics.FromImage(img))
            {
                //g.Clear(Color.White);
                g.Clear(Color.FromArgb(230, 230, 230));
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                for (int i = 0; i < 256; i++)
                {

                    float pctR = histogram[i, 0] / max;   // What percentage of the max is this value?
                    g.DrawLine(redPen,
                        new Point(i, img.Height - 5),
                        new Point(i, img.Height - 5 - (int)(pctR * histHeight))  // Use that percentage of the height
                        );
                    float pctG = histogram[i, 1] / max;   // What percentage of the max is this value?
                    g.DrawLine(greenPen,
                        new Point(i, img.Height - 5),
                        new Point(i, img.Height - 5 - (int)(pctG * histHeight))  // Use that percentage of the height
                        );
                    float pctB = histogram[i, 2] / max;   // What percentage of the max is this value?
                    g.DrawLine(bluePen,
                        new Point(i, img.Height - 5),
                        new Point(i, img.Height - 5 - (int)(pctB * histHeight))  // Use that percentage of the height
                        );
                    float pctGray = histogram[i, 3] / max;   // What percentage of the max is this value?
                    g.DrawLine(grayPen,
                        new Point(i, img.Height - 5),
                        new Point(i, img.Height - 5 - (int)(pctGray * histHeight))  // Use that percentage of the height
                        );
                }
            }
            return img;
        }

        private static int[,] calculateWhateverButItsNice(int[,] values)
        {
            int min = 255, max = 0;
            int[,] result = new int[256, 3];

            for (int channel = 0; channel < 3; channel++)
            {
                for (int j = 0; j < 256; j++)
                {
                    if (values[j, channel] < min)
                        min = values[j, channel];

                    if (values[j, channel] > max)
                        max = values[j, channel];
                }

                double a = 255.0 / (max - min);

                for (int i = 0; i < 256; i++)
                {
                    result[i, channel] = (int)(a * (values[i, channel] - min));
                }
            }
            return result;
        }
        private static int[,] calculateLUT(int[,] values)
        {
            int[,] result = new int[256, 3];

            for (int channel = 0; channel < 3; channel++)
            {
                //szukanie najciemniejszego pixela

                int minValue = 0;
                for (int j = 0; j <= 255; j++)
                {
                    if (values[j, channel] != 0)
                    {
                        minValue = j;
                        break;
                    }
                }
                //szukanie najjaśniejszego pixela

                int maxValue = 255;
                for (int j = 255; j >= 0; j--)
                {
                    if (values[j, channel] != 0)
                    {
                        maxValue = j;
                        break;
                    }
                }

                double a = 255.0 / (maxValue - minValue);

                for (int i = 0; i < 256; i++)
                {
                    result[i, channel] = (int)(a * (i - minValue));
                }
            }
            return result;
        }
        public static Bitmap StretchHistogram(Bitmap bitmap)
        {
            int[,] LUThistogram = new int[256, 3];
            int[,] histogram = new int[256, 3];

            histogram = GetHistogram(bitmap);

            LUThistogram = calculateLUT(histogram);

            Bitmap newBitmap = new Bitmap(bitmap.Width, bitmap.Height);

            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    Color pixel = bitmap.GetPixel(x, y);
                    Color newPixel = Color.FromArgb(LUThistogram[pixel.R, 0], LUThistogram[pixel.G, 1], LUThistogram[pixel.B, 2]);
                    newBitmap.SetPixel(x, y, newPixel);
                }
            }
            return newBitmap;
        }

        public static int[,] CalculateDistribution(int[,] histogram)
        {
            int red = 0, green = 0, blue = 0,gray=0;
            int[,] distribution = new int[256,3];
            for (int i = 0; i <= 255; i++)
            {
                red += histogram[i,0];
                green += histogram[i,1];
                blue += histogram[i,2];
                distribution[i,0] = red;
                distribution[i,1] = green;
                distribution[i,2] = blue;
            }
            return distribution;
        }

        public static Bitmap EqualizationHistogram(Bitmap bitmap)
        {
            int[,] histogram = new int[256, 3];
            histogram = GetHistogram(bitmap);

            int[,] distribution = CalculateDistribution(histogram);

            histogram = new int[256, 3];

            for (int channel = 0; channel < 3; channel++)
            {
                int minValue = 0;
                int L = 0;
                for (int j = 0; j <= 255; j++)
                {
                    if (distribution[j, channel] < minValue)
                    {
                        minValue = distribution[j, channel];
                    }
                    if (distribution[j, channel] != 0) 
                        L++;
                }
                for (int i = 0; i <= 255; i++)
                {
                    histogram[i, channel] = (int)Math.Round((((double)distribution[i, channel] - minValue) /(double) (bitmap.Width * bitmap.Height - minValue)) * (double)(L-1));
                }
            }
    
            Bitmap newBitmap = new Bitmap(bitmap.Width, bitmap.Height);

            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    Color pixel = bitmap.GetPixel(x, y);
                    Color newPixel = Color.FromArgb(histogram[pixel.R, 0], histogram[pixel.G, 1], histogram[pixel.B, 2]);
                    newBitmap.SetPixel(x, y, newPixel);
                }
            }
            return newBitmap;
        }
     

    }
}
