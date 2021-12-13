using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace PS8
{
    static class Parser
    {

        public static Bitmap BitmapImageToBitmap(BitmapImage bmpImage)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();

                enc.Frames.Add(BitmapFrame.Create(bmpImage));
                enc.Save(memory);
                Bitmap bitmap = new Bitmap(memory);

                return new Bitmap(bitmap);
            }
        }

        public static BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }

        private static string FindPPMType(FileStream streamFile)
        {
            var data = new byte[2];
            streamFile.Read(data,0,2);
            while ((char)streamFile.ReadByte() != '\n') ;
            string type = $"{(char)data[0]}{(char)data[1]}";
            return type;
        }

        public static Bitmap PPMToBitmap(string path)
        {
            Bitmap image = null;
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
               string type = FindPPMType(stream);
               if (type == "P6") image = ParseP6(stream);
               if (type == "P3") image = ParseP3(stream);
            }

            return image;
        }

        private static Bitmap ParseP3(FileStream streamFile)
        {
            Bitmap image = null;
            double multiplier;
            int width , height; 

            image = GetParams(streamFile, out width,out height,out multiplier);

            int x_pos = 0, y_pos = 0;
            int colorOrder = 0;
            
            var bufferSize = 1024 * 4000;
            var buffer = new byte[bufferSize];
            StringBuilder tempData = new StringBuilder();
            int[] RGB = new int[3];

            while (true)
            {
                var i = streamFile.Read(buffer, 0, bufferSize);
                if (i == 0) break;

                for (int j = 0; j < i;)
                {
                    if (buffer[j] == '#')
                    {
                        while (buffer[j] != '\n') j++;
                    }
                   
                    if (buffer[j] != ' ' && buffer[j] != '\t' && buffer[j] != '\n')
                    {
                        tempData.Append((char)buffer[j]);
                    }
                    else
                    if (tempData.Length > 0)
                    {
                        var val = tempData.ToString();
                        tempData.Clear();
                        int number;

                        if (!int.TryParse(val, out number))
                        {
                            throw new Exception("incorrect value");
                        }
                        
                        RGB[colorOrder] = (int)(number * multiplier);
                        colorOrder = (colorOrder + 1) % 3;
                        if (colorOrder == 0)
                        {
                            Color color = Color.FromArgb(RGB[0], RGB[1], RGB[2]);
                            image.SetPixel(x_pos, y_pos, color);

                            x_pos++;
                            if (x_pos == width)
                            {
                                x_pos = 0;
                                y_pos++;
                                if (y_pos == height) break;
                            }
                        }
                        
                    }
                    j++;
                }
            }

            return image;
        }

        private static Bitmap ParseP6(FileStream streamFile)
        {
            Bitmap image = null;
            double multiplier;
            int width, height;

            image = GetParams(streamFile, out width, out height, out multiplier);

            int x_pos = 0, y_pos = 0;
            int colorOrder = 0;

            var bufferSize = 1024 * 4000;
            var buffer = new byte[bufferSize];
            StringBuilder tempData = new StringBuilder();

            int[] RGB = new int[3];

            while (true)
            {
                var i = streamFile.Read(buffer, 0, bufferSize);
                if (i == 0) break;

                for (int j = 0; j < i; j++)
                {
                     RGB[colorOrder] = (int)(buffer[j] * multiplier);
                     colorOrder = (colorOrder + 1) % 3;

                     if (colorOrder == 0)
                     {
                         Color color = Color.FromArgb(RGB[0], RGB[1], RGB[2]);
                         image.SetPixel(x_pos, y_pos, color);

                         x_pos++;
                         if (x_pos == width)
                         {
                             x_pos = 0;
                             y_pos++;
                            if (y_pos == height) break;
                         }
                     }
                    
                    
                }
            }

            return image;
        }

        private static Bitmap GetParams(FileStream streamFile,out int width,out int height,out double multiplier)
        {
            bool done = false;
            int maxColor = 0;
            width = height = 0;
            StringBuilder tempData = new StringBuilder();
            multiplier = 0.0;
            Bitmap image = null;


            while(!done)
            {
                char tempByte = (char)streamFile.ReadByte();

                if (tempByte == '#')
                {
                    while(tempByte != '\n')
                    tempByte = (char)streamFile.ReadByte();
                }
                else
                if (tempByte != ' ' && tempByte != '\t' && tempByte != '\n')
                {
                    tempData.Append(tempByte);
                }
                else if(tempData.Length >0)
                {
                    var val = tempData.ToString();
                    int number;
                    if (!int.TryParse(val, out number))
                    {
                        throw new Exception("incorrect value");
                    }
                    if(width == 0)
                    {
                        width = number;
                    }

                    else
                    if (height == 0)
                    {
                        height = number;
                    }

                    else
                    if (maxColor == 0)
                    {
                        image = new Bitmap(width, height);
                        maxColor = number;
                        multiplier = (double) 255 / maxColor;
                        done = true;
                    }

                    tempData.Clear();
                }
            }


            return image;
        }
    }
}
