using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
//?
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace PS8
{
    static class Filters
    {
        private static Color[,] GetPixelsColor(Bitmap bitmap)
        {
            Color[,] colors = new Color[bitmap.Width, bitmap.Height];
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    colors[x, y] = bitmap.GetPixel(x, y);
                }
            }
            return colors;
        }

        //a. Przekształcenia punktowe

        public static Bitmap ElementaryOperations(Bitmap bitmap, string operate, int redVal = 1, int greenVal = 1, int blueVal = 1)
        {
            int width = bitmap.Width, height = bitmap.Height;
            int red, green, blue;

            Color[,] colors = GetPixelsColor(bitmap);

            Bitmap newBitmap = new Bitmap(width, height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    switch (operate)
                    {
                        case "Add":
                            red = colors[x, y].R + redVal;
                            green = colors[x, y].G + greenVal;
                            blue = colors[x, y].B + blueVal;
                            break;
                        case "Sub":
                            red = colors[x, y].R - redVal;
                            green = colors[x, y].G - greenVal;
                            blue = colors[x, y].B - blueVal;
                            break;
                        case "Div":
                            if (redVal == 0 || greenVal == 0 || blueVal == 0)
                                throw new ArgumentException($"Parametr dzielnika nie może być 0");
                            red = colors[x, y].R / redVal;
                            green = colors[x, y].G / greenVal;
                            blue = colors[x, y].B / blueVal;
                            break;
                        case "Mul":
                            red = colors[x, y].R * redVal;
                            green = colors[x, y].G * greenVal;
                            blue = colors[x, y].B * blueVal;
                            break;
                        default:
                            red = colors[x, y].R;
                            green = colors[x, y].G;
                            blue = colors[x, y].B;
                            break;
                    }

                    red = red > 255 ? 255 : red;
                    red =  red <= 0 ? 0 : red;

                    green = green > 255 ? 255 : green;
                    green = green < 0 ? 0 : green;

                    blue = blue > 255 ? 255 : blue;
                    blue = blue < 0 ? 0 : blue;

                    Color color = Color.FromArgb(red, green, blue);
                    newBitmap.SetPixel(x, y, color);
                }
            }

            return newBitmap;
        }

        public static Bitmap Brightness(Bitmap bitmap, double value)
        {
            int width= bitmap.Width, height= bitmap.Height;
            int red, green, blue;

            Color[,] colors = GetPixelsColor(bitmap);

            Bitmap newBitmap= new Bitmap(width,height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    red = (int)(colors[x, y].R * value);
                    green = (int)(colors[x, y].G * value);
                    blue = (int)(colors[x, y].B * value);

                    red = red > 255 ? 255 : red < 0 ? 0 : red;
                    green = green > 255 ? 255 : green < 0 ? 0 : green;
                    blue = blue > 255 ? 255 : blue < 0 ? 0 : blue;

                    Color color = Color.FromArgb(red, green, blue);
                    newBitmap.SetPixel(x, y, color);
                }
            }

            return newBitmap;
        }

        public static Bitmap GrayScale(Bitmap bitmap, bool luminosity)
        {
            int width = bitmap.Width, height = bitmap.Height;
            double red, green, blue;

            Color[,] colors = GetPixelsColor(bitmap);


            Bitmap newBitmap = new Bitmap(width, height);
            double grayscaleLuminosity;
            int grayscale = 1;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {

                    red = colors[x, y].R;
                    green = colors[x, y].G;
                    blue = colors[x, y].B;

                    if (!luminosity)
                        grayscale = (int) (red + green + blue) / 3;

                    if (luminosity)
                    {
                        grayscaleLuminosity = 0.299 * red + 0.587 * green + 0.114 * blue;
                        grayscale = (int)grayscaleLuminosity;
                    }

                    grayscale = grayscale > 255 ? 255 : grayscale;
                    grayscale = grayscale < 0 ? 0 : grayscale;

                    Color color = Color.FromArgb(grayscale, grayscale, grayscale);
                    newBitmap.SetPixel(x, y, color);
                }
            }

            return newBitmap;
        }

        //b. Metody polepszania jakości obrazów
        //MEAN FILTER
        public static Bitmap AverageFilter(Bitmap bitmap)
        {
            double[,] mask = new double[3, 3];
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    mask[x, y] = 1.0 / 9.0;
                }
            }

            int width = bitmap.Width, height = bitmap.Height;
            int red, green, blue;
            Color[,] colors = GetPixelsColor(bitmap);
            Bitmap newBitmap = new Bitmap(width, height);


            for (int x = 1; x < width-1; x++)
            {
                for (int y = 1; y < height-1; y++)
                {
                    newBitmap.SetPixel(x, y, CalculateColorPixel(x,y,colors,mask));
                }
            }

           return newBitmap;
        }
        public static Bitmap SobelFilter(Bitmap bitmap,string type)
        {
            double[,] gxMask = new double[3, 3];
            double[,] gyMask = new double[3, 3];
            //vertical
            gxMask[0, 0] = -1.0;
            gxMask[0, 1] = 0.0;
            gxMask[0, 2] = 1.0;
            gxMask[1, 0] = -2.0;
            gxMask[1, 1] = 0.0;
            gxMask[1, 2] = 2.0;
            gxMask[2, 0] = -1.0;
            gxMask[2, 1] = 0.0;
            gxMask[2, 2] = 1.0;

            //horizontal
            //w wykladzie wiersz 0 jest - a wiersz 2 +
            gyMask[0, 0] = 1.0;
            gyMask[0, 1] = 2.0;
            gyMask[0, 2] = 1.0;
            gyMask[1, 0] = 0.0;
            gyMask[1, 1] = 0.0;
            gyMask[1, 2] = 0.0;
            gyMask[2, 0] = -1.0;
            gyMask[2, 1] = -2.0;
            gyMask[2, 2] = -1.0;


            int width = bitmap.Width, height = bitmap.Height;
            int red, green, blue;
            Color[,] colors = GetPixelsColor(bitmap);
            Color gx, gy,g;
            Bitmap newBitmapX = new Bitmap(width, height);
            Bitmap newBitmapY = new Bitmap(width, height);


            for (int x = 1; x < width - 1; x++)
            {
                for (int y = 1; y < height - 1; y++)
                {
                    newBitmapX.SetPixel(x, y, CalculateColorPixel(x,y,colors,gxMask));
                }
            }
            for (int x = 1; x < width - 1; x++)
            {
                for (int y = 1; y < height - 1; y++)
                {
                    newBitmapY.SetPixel(x, y, CalculateColorPixel(x, y, colors, gyMask));
                }
            }
            if (type == "XY") return MergeSobelXY(newBitmapX,newBitmapY);            
            if (type == "X") return newBitmapX;
            if (type == "Y") return newBitmapY;
            return null;
        }
        public static Bitmap MedianFilter(Bitmap bitmap)
        {
            int width = bitmap.Width, height = bitmap.Height;
            int red, green, blue;
            Color[,] colors = GetPixelsColor(bitmap);
            Bitmap newBitmap = new Bitmap(width, height);


            for (int x = 1; x < width-1; x++)
            {
                for (int y = 1; y < height-1; y++)
                {
                    newBitmap.SetPixel(x, y, CalculateMedianColorPixel(x,y,colors));
                }
            }

           return newBitmap;
        }
        public static Bitmap HighPassFilter(Bitmap bitmap)
        {
            double[,] mask = new double[3, 3];
            mask[0, 0] = -1.0;
            mask[1, 0] = -1.0;
            mask[2, 0] = -1.0;
            mask[0, 1] = -1.0;
            mask[1, 1] = 9.0;
            mask[2, 1] = -1.0;
            mask[0, 2] = -1.0;
            mask[1, 2] = -1.0;
            mask[2, 2] = -1.0;

            int width = bitmap.Width, height = bitmap.Height;
            int red, green, blue;
            Color[,] colors = GetPixelsColor(bitmap);
            Bitmap newBitmap = new Bitmap(width, height);


            for (int x = 1; x < width - 1; x++)
            {
                for (int y = 1; y < height - 1; y++)
                {
                    newBitmap.SetPixel(x, y, CalculateColorPixel(x, y, colors, mask));
                }
            }

            return newBitmap;
        }
        public static Bitmap GaussianFilter(Bitmap bitmap) 
        {
            double[,] mask = new double[3, 3];
            mask[0, 0] = 1.0 / 16.0;
            mask[1, 0] = 2.0 / 16.0;
            mask[2, 0] = 1.0 / 16.0;

            mask[0, 1] = 2.0 / 16.0;
            mask[1, 1] = 4.0 / 16.0;
            mask[2, 1] = 2.0 / 16.0;

            mask[0, 2] = 1.0 / 16.0;
            mask[1, 2] = 2.0 / 16.0;
            mask[2, 2] = 1.0 / 16.0;

            int width = bitmap.Width, height = bitmap.Height;
            int red, green, blue;
            Color[,] colors = GetPixelsColor(bitmap);
            Bitmap newBitmap = new Bitmap(width, height);

            //skrajne
            for (int x = 0; x < width; x++)
            {
             var pixel = bitmap.GetPixel(x, 0);
                Color color = Color.FromArgb(pixel.R, pixel.G, pixel.B);
                newBitmap.SetPixel(x, 0, color);
                pixel = bitmap.GetPixel(x, height-1);
                color = Color.FromArgb(pixel.R, pixel.G, pixel.B);
                newBitmap.SetPixel(x, height-1, color);
            }

            for (int y = 0; y < height; y++)
            {
                var pixel = bitmap.GetPixel(0,y);
                Color color = Color.FromArgb(pixel.R, pixel.G, pixel.B);
                newBitmap.SetPixel(0, y, color);
                pixel = bitmap.GetPixel(width- 1,0);
                color = Color.FromArgb(pixel.R, pixel.G, pixel.B);
                newBitmap.SetPixel(width - 1, 0, color);
            }

            for (int x = 1; x < width - 1; x++)
            {
                for (int y = 1; y < height - 1; y++)
                {
                    newBitmap.SetPixel(x, y, CalculateColorPixel(x, y, colors, mask));
                }
            }

            return newBitmap;
        }

        private static Color CalculateMedianColorPixel(int x, int y, Color[,] colors)
        {
            int[] reds = new int[9];
            int[] greens= new int[9];
            int[] blues = new int[9];

            int m = 0;
            for (int i = x - 1; i <= x + 1; i++)
            {
                for (int j = y - 1; j <= y + 1; j++)
                {
                    reds[m]= colors[i, j].R;
                    greens[m]= colors[i, j].G;
                    blues[m]= colors[i, j].B;
                    m++;
                }
            }

            Array.Sort(reds);
            Array.Sort(blues);
            Array.Sort(greens);
            return Color.FromArgb(reds[5], greens[5], blues[5]);
        }
        private static Color CalculateColorPixel(int x, int y, Color[,] colors, double[,] mask)
        {
            int red = 0;
            int green = 0;
            int blue = 0;

            var maskI = 0;
            var maskJ = 0;

            for (int i = x - 1; i <= x + 1; i++)
            {
                maskJ = 0;
                for (int j = y - 1; j <= y + 1; j++)
                {
                    red += (int)(colors[i, j].R * mask[maskI, maskJ]);
                    green += (int)(colors[i, j].G * mask[maskI, maskJ]);
                    blue += (int)(colors[i, j].B * mask[maskI, maskJ]);
                    maskJ++;
                }
                maskI++;
            }

            red = red > 255 ? 255 : red < 0 ? 0 : red;
            green = green > 255 ? 255 : green < 0 ? 0 : green;
            blue = blue > 255 ? 255 : blue < 0 ? 0 : blue;

            return Color.FromArgb(red, green, blue);
        }
        private static Bitmap MergeSobelXY(Bitmap bitmapA,Bitmap bitmapB)
        {
            Bitmap newBitmap = new Bitmap(bitmapA.Width, bitmapA.Height);
            Color colorA = new Color();
            Color colorB = new Color();
            Color color = new Color();
            int red, green, blue;

            for (int i=0;i<bitmapA.Width;i++)
            {
                for (int j = 0; j < bitmapA.Height; j++)
                {
                    colorA = bitmapA.GetPixel(i, j);
                    colorB = bitmapB.GetPixel(i, j);
                    red = (int)Math.Sqrt(Math.Pow(colorA.R, 2) + Math.Pow(colorB.R, 2));
                    green = (int)Math.Sqrt(Math.Pow(colorA.G, 2) + Math.Pow(colorB.G, 2));
                    blue = (int)Math.Sqrt(Math.Pow(colorA.B, 2) + Math.Pow(colorB.B, 2));

                    red = red > 255 ? 255 : red < 0 ? 0 : red;
                    green = green > 255 ? 255 : green < 0 ? 0 : green;
                    blue = blue > 255 ? 255 : blue < 0 ? 0 : blue;

                    color = Color.FromArgb(red, green, blue);
                    newBitmap.SetPixel(i, j, color);
                }
            }
                    
            return newBitmap;
        }
    }
}
