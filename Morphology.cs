using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Documents;

namespace PS8
{
    class Morphology
    {
        public static Bitmap Dilation(Bitmap bitmap)
        {
            Color color;
            Bitmap newBitmap = new Bitmap(bitmap);

            for (int x = 1; x < bitmap.Width - 1; x++)
            {
                for (int y = 1; y < bitmap.Height - 1; y++)
                {
                    bool black = false;
                    for (int i = x - 1; i <= x + 1; i++)
                        for (int j = y - 1; j <= y + 1; j++)
                        {
                            color = bitmap.GetPixel(i, j);
                            if (i == x && j == y) continue;
                            if (color.R == 255)
                            {
                                black = true;
                                break;
                            }

                        }
                    if (black) newBitmap.SetPixel(x, y, Color.FromArgb(255, 255, 255));
                    else newBitmap.SetPixel(x, y, Color.FromArgb(0, 0, 0));
                }
            }

            return newBitmap;
        }

        public static Bitmap Erosion(Bitmap bitmap)
        {
            Color color;
            Bitmap newBitmap = new Bitmap(bitmap);

            for (int x = 1; x < bitmap.Width - 1; x++)
            {
                for (int y = 1; y < bitmap.Height - 1; y++)
                {
                    bool white = false;
                    for (int i = x - 1; i <= x + 1; i++)
                        for (int j = y - 1; j <= y + 1; j++)
                        {
                            color = bitmap.GetPixel(i, j);
                            if (i == x && j == y) continue;
                            if (color.R == 0)
                            {
                                white = true;
                                break;
                            }

                        }
                    if (!white) newBitmap.SetPixel(x, y, Color.FromArgb(255, 255, 255));
                    else newBitmap.SetPixel(x, y, Color.FromArgb(0, 0, 0));
                }
            }

            return newBitmap;
        }

        public static Bitmap Opening(Bitmap bitmap)
        {
            return Erosion(Dilation(bitmap));
        }

        public static Bitmap Closing(Bitmap bitmap)
        {
            
            return Dilation(Erosion(bitmap)); ;
        }

        public static Bitmap HitOrMiss(Bitmap bitmap,int [,]mask)
        {
            Color color;
            Bitmap newBitmap = new Bitmap(bitmap);

            int accouracy = mask.GetLength(0) * mask.GetLength(1);

            for (int x = 1; x < bitmap.Width - 1; x++)
            {
                for (int y = 1; y < bitmap.Height - 1; y++)
                {
                    bool hit = true;
                    int maskX = 0;
                    for (int i = x - 1; i <= x + 1; i++)
                    {
                        int maskY = 0;
                        for (int j = y - 1; j <= y + 1; j++)
                        {

                            color = bitmap.GetPixel(i, j);
                            int colorVal = color.R == 255 ? 1 : 0;

                            if (mask[maskX, maskY] == 2)
                            {
                                maskY++;
                                continue;
                            }

                            if (mask[maskX, maskY] !=  colorVal)
                            {
                                hit = false;
                                break;
                            }

                            maskY++;
                        }
                        maskX++;
                    }

                    if (hit) newBitmap.SetPixel(x, y, Color.FromArgb(255, 255, 255));
                    else newBitmap.SetPixel(x, y, Color.FromArgb(0, 0, 0));
                }
            }

            return newBitmap;
        }
        [DllImport("msvcrt.dll")]
        private static extern int memcmp(IntPtr b1, IntPtr b2, long count);
        public static bool CompareMemCmp(Bitmap b1, Bitmap b2)
        {
            if ((b1 == null) != (b2 == null)) return false;
            if (b1.Size != b2.Size) return false;

            var bd1 = b1.LockBits(new Rectangle(new Point(0, 0), b1.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var bd2 = b2.LockBits(new Rectangle(new Point(0, 0), b2.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            try
            {
                IntPtr bd1scan0 = bd1.Scan0;
                IntPtr bd2scan0 = bd2.Scan0;

                int stride = bd1.Stride;
                int len = stride * b1.Height;

                return memcmp(bd1scan0, bd2scan0, len) == 0;
            }
            finally
            {
                b1.UnlockBits(bd1);
                b2.UnlockBits(bd2);
            }
        }

        //2 - obojetny
        //1 - bialy
        //0 - czarny
        //tutorial -1 czarny 1 bialy
        public static Bitmap Thin(Bitmap bitmap)
        {
            List<int[,]> maskA = new List<int[,]>();
            maskA.Add(
                    new int[,]{
                      {0,0,0},
                      {2,1,2},
                      {1,1,1}
                    }
                );
            maskA.Add(
                    new int[,]{
                      {1,2,0},
                      {1,1,0},
                      {1,2,0}
                    }
                );
            maskA.Add(
                    new int[,]{
                      {1,1,1},
                      {2,1,2},
                      {0,0,0}
                    }
                );
            maskA.Add(
                    new int[,]{
                      {0,2,1},
                      {0,1,1},
                      {0,2,1}
                    }
                );
            List<int[,]> maskB = new List<int[,]>();
            maskB.Add(
                    new int [,]{
                      { 2,0,0},
                      { 1,1,0},
                      { 2,1,2}
                    }
                );
            maskB.Add(
                    new int[,]{
                      {2,1,2},
                      {1,1,0},
                      {2,0,0}
                    }
                );
            maskB.Add(
                    new int[,]{
                      {2,1,2},
                      {0,1,1},
                      {0,0,2}
                    }
                );
            maskB.Add(
                    new int[,]{
                      {0,0,2},
                      {0,1,1},
                      {2,1,2}
                    }
                );
            Bitmap sourceBitmap = new Bitmap(bitmap);
            Bitmap 
                bitmapMaskA = null, 
                bitmapMaskB = null,
                newBitmap=null;
            bool identical =false;
            while (true)
            {
                for (int i = 0; i < 4; i++)
                {
                    //odejmij od pierwotnego obrazu Maske A
                    bitmapMaskA = HitOrMiss(sourceBitmap, maskA[i]);
                    sourceBitmap = SubstactBitmaps(new Bitmap(sourceBitmap), bitmapMaskA);

                    //odejmij od pierwotnego obrazu Maske B
                    bitmapMaskB = HitOrMiss(sourceBitmap, maskB[i]);
                    sourceBitmap = SubstactBitmaps(new Bitmap(sourceBitmap), bitmapMaskB);


                    identical = CompareMemCmp(sourceBitmap, newBitmap);
                    if (identical) break;

                    newBitmap = sourceBitmap;
                }
                if (identical) break;
            }
            return sourceBitmap;
        }

        public static Bitmap SubstactBitmaps(Bitmap bitmapA, Bitmap bitmapB)
        {
            for (int x = 1; x < bitmapA.Width-1 ; x++)
            {
                for (int y = 1; y < bitmapA.Height-1; y++)
                {
                    Color colorA = bitmapA.GetPixel(x,y);
                    Color colorB = bitmapB.GetPixel(x, y);
                    Color subAB = Color.FromArgb(colorA.R- colorB.R, colorA.G - colorB.G, colorA.B - colorB.B);
                    bitmapA.SetPixel(x, y, subAB);
                }
            }
            return bitmapA;
        }
        public static Bitmap SumBitmaps(Bitmap bitmapA, Bitmap bitmapB)
        {
            for (int x = 1; x < bitmapA.Width - 1; x++)
            {
                for (int y = 1; y < bitmapA.Height - 1; y++)
                {
                    Color colorA = bitmapA.GetPixel(x, y);
                    Color colorB = bitmapB.GetPixel(x, y);
                    Color subAB = Color.FromArgb(colorA.R + colorB.R > 255? 255: colorA.R + colorB.R, 
                                                 colorA.G + colorB.G > 255 ? 255 : colorA.G + colorB.G, 
                                                 colorA.B + colorB.B > 255 ? 255 : colorA.B + colorB.B);
                    bitmapA.SetPixel(x, y, subAB);
                }
            }
            return bitmapA;
        }


        //2 - obojetny
        //1 - bialy
        //0 - czarny
        //tutorial -1 czarny 1 bialy
        public static Bitmap Thick(Bitmap bitmap)
        {
            Color color;

            //przeleciec wsio maski
            /* int[,] maskA =
             {
                   {1,1,2},
                   {1,0,2},
                   {1,2,0}
             };
             int[,] maskB =
             {
                   {2,1,1},
                   {2,0,1},
                   {0,2,1}
             };*/

            List<int[,]> maskA = new List<int[,]>();
            maskA.Add(
                    new int[,]{
                      {1,1,2},
                      {1,0,2},
                      {1,2,0}
                    }
                );
            maskA.Add(
                    new int[,]{
                      {1,1,1},
                      {2,0,1},
                      {0,2,2}
                    }
                );
            maskA.Add(
                    new int[,]{
                      {0,2,1},
                      {2,0,1},
                      {2,1,1}
                    }
                );
            maskA.Add(
                    new int[,]{
                      {2,2,0},
                      {1,0,2},
                      {1,1,1}
                    }
                );
            List<int[,]> maskB = new List<int[,]>();
            maskB.Add(
                    new int[,]{
                      {2,1,1},
                      {2,0,1},
                      {0,2,1}
                    }
                );
            maskB.Add(
                    new int[,]{
                      {0,2,2},
                      {2,0,1},
                      {1,1,1}
                    }
                );
            maskB.Add(
                    new int[,]{
                      {1,2,0},
                      {1,0,2},
                      {1,1,2}
                    }
                );
            maskB.Add(
                    new int[,]{
                      {1,1,1},
                      {1,0,2},
                      {2,2,0}
                    }
                );
            Bitmap sourceBitmap = new Bitmap(bitmap);
            Bitmap
                bitmapMaskA = null,
                bitmapMaskB = null,
                newBitmap = null;
            bool identical = false;
            while (true)
            {
                for (int i = 0; i < 4; i++)
                {
                    //odejmij od pierwotnego obrazu Maske A
                    bitmapMaskA = HitOrMiss(sourceBitmap, maskA[i]);
                    sourceBitmap = SumBitmaps(new Bitmap(sourceBitmap), bitmapMaskA);

                    //odejmij od pierwotnego obrazu Maske B
                    bitmapMaskB = HitOrMiss(sourceBitmap, maskB[i]);
                    sourceBitmap = SumBitmaps(new Bitmap(sourceBitmap), bitmapMaskB);


                    identical = CompareMemCmp(sourceBitmap, newBitmap);
                    if (identical) break;

                    newBitmap = sourceBitmap;
                }
                if (identical) break;
            }
            return sourceBitmap;
        }

        public static Bitmap Thick2(Bitmap bitmap)
        {
            List<int[,]> maskA = new List<int[,]>();
            maskA.Add(
                    new int[,]{
                      {1,1,2},
                      {1,0,2},
                      {1,2,0}
                    }
                );
            maskA.Add(
                    new int[,]{
                      {1,1,1},
                      {2,0,1},
                      {0,2,2}
                    }
                );
            maskA.Add(
                    new int[,]{
                      {0,2,1},
                      {2,0,1},
                      {2,1,1}
                    }
                );
            maskA.Add(
                    new int[,]{
                      {2,2,0},
                      {1,0,2},
                      {1,1,1}
                    }
                );
            List<int[,]> maskB = new List<int[,]>();
            maskB.Add(
                    new int[,]{
                      {2,1,1},
                      {2,0,1},
                      {0,2,1}
                    }
                );
            maskB.Add(
                    new int[,]{
                      {0,2,2},
                      {2,0,1},
                      {1,1,1}
                    }
                );
            maskB.Add(
                    new int[,]{
                      {1,2,0},
                      {1,0,2},
                      {1,1,2}
                    }
                );
            maskB.Add(
                    new int[,]{
                      {1,1,1},
                      {1,0,2},
                      {2,2,0}
                    }
                );
            int Width = bitmap.Width, Height = bitmap.Height;
            Bitmap sourceBitmap = new Bitmap(bitmap);
            /* Bitmap
                 bitmapMaskA = null,
                 bitmapMaskB = null,
                 newBitmap = null;*/
            Color[,] sourceColors = new Color[Width, Height];
            Color[,] newBitmapColors = new Color[Width, Height];

            for (int x = 0; x < sourceColors.GetLength(0); x++)
            {
                for (int y = 0; y < sourceColors.GetLength(1); y++)
                {
                    Color color = sourceBitmap.GetPixel(x, y);
                    sourceColors[x, y] = Color.FromArgb(color.R,color.G, color.B);
                    newBitmapColors[x,y]= Color.FromArgb(1,1,1);
                }
            }
            bool identical = false;
            int[,] mask;
            while (true)
            {
                for (int i = 0; i < 4; i++)
                {
                    //odejmij od pierwotnego obrazu Maske A
                    //bitmapMaskA = HitOrMiss(sourceBitmap, maskA[i]);
                    //sourceBitmap = SumBitmaps(new Bitmap(sourceBitmap), bitmapMaskA);
                    mask = maskA[i];
                    for (int x = 1; x < Width - 1; x++)
                    {
                        for (int y = 1; y < Height - 1; y++)
                        {
                            bool hit = true;
                            int maskX = 0;
                            for (int i_pos = x - 1; i_pos <= x + 1; i_pos++)
                            {
                                int maskY = 0;
                                for (int j_pos = y - 1; j_pos <= y + 1; j_pos++)
                                {

                                    Color color = sourceColors[i_pos, j_pos];
                                    int colorVal = color.R == 255 ? 1 : 0;

                                    if (mask[maskX, maskY] == 2)
                                    {
                                        maskY++;
                                        continue;
                                    }

                                    if (mask[maskX, maskY] != colorVal)
                                    {
                                        hit = false;
                                        break;
                                    }

                                    maskY++;
                                }
                                maskX++;
                            }

                            Color colorA, colorB;
                            if (hit) colorB = Color.FromArgb(255, 255, 255);
                            else colorB = Color.FromArgb(0, 0, 0);

                            colorA = sourceColors[x, y];

                            Color subAB = Color.FromArgb(colorA.R + colorB.R > 255 ? 255 : colorA.R + colorB.R,
                                                         colorA.G + colorB.G > 255 ? 255 : colorA.G + colorB.G,
                                                         colorA.B + colorB.B > 255 ? 255 : colorA.B + colorB.B);
                            sourceColors[x, y] = subAB;
                            //sourceBitmap.SetPixel(x, y, subAB);
                        }
                    }

                    //odejmij od pierwotnego obrazu Maske B
                    /* bitmapMaskB = HitOrMiss(sourceBitmap, maskB[i]);
                     sourceBitmap = SumBitmaps(new Bitmap(sourceBitmap), bitmapMaskB);*/
                     mask = maskB[i];
                    for (int x = 1; x < Width - 1; x++)
                    {
                        for (int y = 1; y < Height - 1; y++)
                        {
                            bool hit = true;
                            int maskX = 0;
                            for (int i_pos = x - 1; i_pos <= x + 1; i_pos++)
                            {
                                int maskY = 0;
                                for (int j_pos = y - 1; j_pos <= y + 1; j_pos++)
                                {

                                    Color color = sourceColors[i_pos, j_pos];
                                    int colorVal = color.R == 255 ? 1 : 0;

                                    if (mask[maskX, maskY] == 2)
                                    {
                                        maskY++;
                                        continue;
                                    }

                                    if (mask[maskX, maskY] != colorVal)
                                    {
                                        hit = false;
                                        break;
                                    }

                                    maskY++;
                                }
                                maskX++;
                            }

                            Color colorA, colorB;

                            if (hit) colorB = Color.FromArgb(255, 255, 255);
                            else colorB = Color.FromArgb(0, 0, 0);

                            colorA = sourceColors[x, y];

                            Color subAB = Color.FromArgb(colorA.R + colorB.R > 255 ? 255 : colorA.R + colorB.R,
                                                         colorA.G + colorB.G > 255 ? 255 : colorA.G + colorB.G,
                                                         colorA.B + colorB.B > 255 ? 255 : colorA.B + colorB.B);
                            sourceColors[x, y] = subAB;
                            //sourceBitmap.SetPixel(x, y, subAB);
                        }
                    }

                    identical = CompareColorsArrays(sourceColors, newBitmapColors);

                    
                    //identical = CompareMemCmp(sourceBitmap, newBitmap);
                    if (identical) break;

                    newBitmapColors = (Color[,]) sourceColors.Clone(); 


 
                    /*  
                        sourceColors.CopyTo(newBitmapColors, 1);
                    //kopiowanie maski do newBitmapColors

                    /*
                    newBitmapColors = new Color[Width,Height];
                      
                    for (int x = 0; x < sourceColors.GetLength(0); x++)
                    {
                        for (int y = 0; y < sourceColors.GetLength(1); y++)
                        {
                            Color color = sourceColors[x, y];
                            newBitmapColors[x, y] = Color.FromArgb(color.R, color.G, color.B);
                        }
                    }*/
                }
                if (identical) break;
            }

            //Przepisanie kolorów na piksele
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Color color = sourceColors[x, y];
                    sourceBitmap.SetPixel(x,y ,Color.FromArgb(color.R, color.G, color.B));
                }
            }
            /* 
             * int[,] mask = maskA[0];
              for (int x = 1; x < bitmap.Width - 1; x++)
              {
                  for (int y = 1; y < bitmap.Height - 1; y++)
                  {
                      bool hit = true;
                      int maskX = 0;
                      for (int i_pos = x - 1; i_pos <= x + 1; i_pos++)
                      {
                          int maskY = 0;
                          for (int j_pos = y - 1; j_pos <= y + 1; j_pos++)
                          {

                              Color color = bitmap.GetPixel(i_pos, j_pos);
                              int colorVal = color.R == 255 ? 1 : 0;

                              if (mask[maskX, maskY] == 2)
                              {
                                  maskY++;
                                  continue;
                              }

                              if (mask[maskX, maskY] != colorVal)
                              {
                                  hit = false;
                                  break;
                              }

                              maskY++;
                          }
                          maskX++;
                      }

                      Color colorA, colorB;
                      if (hit) colorB = Color.FromArgb(255, 255, 255);
                      else colorB = Color.FromArgb(0, 0, 0);

                      colorA = sourceBitmap.GetPixel(x, y);

                      Color subAB = Color.FromArgb(colorA.R + colorB.R > 255 ? 255 : colorA.R + colorB.R,
                                                   colorA.G + colorB.G > 255 ? 255 : colorA.G + colorB.G,
                                                   colorA.B + colorB.B > 255 ? 255 : colorA.B + colorB.B);
                      sourceBitmap.SetPixel(x, y, subAB);
                  }
              }
              mask = maskB[0];
              for (int x = 1; x < bitmap.Width - 1; x++)
              {
                  for (int y = 1; y < bitmap.Height - 1; y++)
                  {
                      bool hit = true;
                      int maskX = 0;
                      for (int i_pos = x - 1; i_pos <= x + 1; i_pos++)
                      {
                          int maskY = 0;
                          for (int j_pos = y - 1; j_pos <= y + 1; j_pos++)
                          {

                              Color color = bitmap.GetPixel(i_pos, j_pos);
                              int colorVal = color.R == 255 ? 1 : 0;

                              if (mask[maskX, maskY] == 2)
                              {
                                  maskY++;
                                  continue;
                              }

                              if (mask[maskX, maskY] != colorVal)
                              {
                                  hit = false;
                                  break;
                              }

                              maskY++;
                          }
                          maskX++;
                      }

                      Color colorA, colorB;
                      if (hit) colorB = Color.FromArgb(255, 255, 255);
                      else colorB = Color.FromArgb(0, 0, 0);

                      colorA = sourceBitmap.GetPixel(x, y);

                      Color subAB = Color.FromArgb(colorA.R + colorB.R > 255 ? 255 : colorA.R + colorB.R,
                                                   colorA.G + colorB.G > 255 ? 255 : colorA.G + colorB.G,
                                                   colorA.B + colorB.B > 255 ? 255 : colorA.B + colorB.B);
                      sourceBitmap.SetPixel(x, y, subAB);
                  }
              }*/

            return sourceBitmap;
        }
        public static Bitmap Thin2(Bitmap bitmap)
        {
            List<int[,]> maskA = new List<int[,]>();
            maskA.Add(
                    new int[,]{
                      {0,0,0},
                      {2,1,2},
                      {1,1,1}
                    }
                );
            maskA.Add(
                    new int[,]{
                      {1,2,0},
                      {1,1,0},
                      {1,2,0}
                    }
                );
            maskA.Add(
                    new int[,]{
                      {1,1,1},
                      {2,1,2},
                      {0,0,0}
                    }
                );
            maskA.Add(
                    new int[,]{
                      {0,2,1},
                      {0,1,1},
                      {0,2,1}
                    }
                );
            List<int[,]> maskB = new List<int[,]>();
            maskB.Add(
                    new int[,]{
                      { 2,0,0},
                      { 1,1,0},
                      { 2,1,2}
                    }
                );
            maskB.Add(
                    new int[,]{
                      {2,1,2},
                      {1,1,0},
                      {2,0,0}
                    }
                );
            maskB.Add(
                    new int[,]{
                      {2,1,2},
                      {0,1,1},
                      {0,0,2}
                    }
                );
            maskB.Add(
                    new int[,]{
                      {0,0,2},
                      {0,1,1},
                      {2,1,2}
                    }
                );
            int Width = bitmap.Width, Height = bitmap.Height;
            Bitmap sourceBitmap = new Bitmap(bitmap);
            /* Bitmap
                 bitmapMaskA = null,
                 bitmapMaskB = null,
                 newBitmap = null;*/
            Color[,] sourceColors = new Color[Width, Height];
            Color[,] newBitmapColors = new Color[Width, Height];

            for (int x = 0; x < sourceColors.GetLength(0); x++)
            {
                for (int y = 0; y < sourceColors.GetLength(1); y++)
                {
                    Color color = sourceBitmap.GetPixel(x, y);
                    sourceColors[x, y] = Color.FromArgb(color.R, color.G, color.B);
                    newBitmapColors[x, y] = Color.FromArgb(1, 1, 1);
                }
            }
            bool identical = false;
            int[,] mask;
            while (true)
            {
                for (int i = 0; i < 4; i++)
                {
                    //odejmij od pierwotnego obrazu Maske A
                    //bitmapMaskA = HitOrMiss(sourceBitmap, maskA[i]);
                    //sourceBitmap = SumBitmaps(new Bitmap(sourceBitmap), bitmapMaskA);
                    mask = maskA[i];
                    for (int x = 1; x < Width - 1; x++)
                    {
                        for (int y = 1; y < Height - 1; y++)
                        {
                            bool hit = true;
                            int maskX = 0;
                            for (int i_pos = x - 1; i_pos <= x + 1; i_pos++)
                            {
                                int maskY = 0;
                                for (int j_pos = y - 1; j_pos <= y + 1; j_pos++)
                                {

                                    Color color = sourceColors[i_pos, j_pos];
                                    int colorVal = color.R == 255 ? 1 : 0;

                                    if (mask[maskX, maskY] == 2)
                                    {
                                        maskY++;
                                        continue;
                                    }

                                    if (mask[maskX, maskY] != colorVal)
                                    {
                                        hit = false;
                                        break;
                                    }

                                    maskY++;
                                }
                                maskX++;
                            }

                            Color colorA, colorB;
                            if (hit) colorB = Color.FromArgb(255, 255, 255);
                            else colorB = Color.FromArgb(0, 0, 0);

                            colorA = sourceColors[x, y];

                            Color subAB = Color.FromArgb(colorA.R - colorB.R > 255 ? 255 : colorA.R - colorB.R,
                                                         colorA.G - colorB.G > 255 ? 255 : colorA.G - colorB.G,
                                                         colorA.B - colorB.B > 255 ? 255 : colorA.B - colorB.B);
                            sourceColors[x, y] = subAB;
                            //sourceBitmap.SetPixel(x, y, subAB);
                        }
                    }

                    //odejmij od pierwotnego obrazu Maske B
                    /* bitmapMaskB = HitOrMiss(sourceBitmap, maskB[i]);
                     sourceBitmap = SumBitmaps(new Bitmap(sourceBitmap), bitmapMaskB);*/
                    mask = maskB[i];
                    for (int x = 1; x < Width - 1; x++)
                    {
                        for (int y = 1; y < Height - 1; y++)
                        {
                            bool hit = true;
                            int maskX = 0;
                            for (int i_pos = x - 1; i_pos <= x + 1; i_pos++)
                            {
                                int maskY = 0;
                                for (int j_pos = y - 1; j_pos <= y + 1; j_pos++)
                                {

                                    Color color = sourceColors[i_pos, j_pos];
                                    int colorVal = color.R == 255 ? 1 : 0;

                                    if (mask[maskX, maskY] == 2)
                                    {
                                        maskY++;
                                        continue;
                                    }

                                    if (mask[maskX, maskY] != colorVal)
                                    {
                                        hit = false;
                                        break;
                                    }

                                    maskY++;
                                }
                                maskX++;
                            }

                            Color colorA, colorB;

                            if (hit) colorB = Color.FromArgb(255, 255, 255);
                            else colorB = Color.FromArgb(0, 0, 0);

                            colorA = sourceColors[x, y];

                            Color subAB = Color.FromArgb(colorA.R - colorB.R > 255 ? 255 : colorA.R - colorB.R,
                                                         colorA.G - colorB.G > 255 ? 255 : colorA.G - colorB.G,
                                                         colorA.B - colorB.B > 255 ? 255 : colorA.B - colorB.B);
                            sourceColors[x, y] = subAB;
                            //sourceBitmap.SetPixel(x, y, subAB);
                        }
                    }

                    identical = CompareColorsArrays(sourceColors, newBitmapColors);


                    //identical = CompareMemCmp(sourceBitmap, newBitmap);
                    if (identical) break;

                    newBitmapColors = (Color[,])sourceColors.Clone();



                    /*  
                        sourceColors.CopyTo(newBitmapColors, 1);
                    //kopiowanie maski do newBitmapColors

                    /*
                    newBitmapColors = new Color[Width,Height];
                      
                    for (int x = 0; x < sourceColors.GetLength(0); x++)
                    {
                        for (int y = 0; y < sourceColors.GetLength(1); y++)
                        {
                            Color color = sourceColors[x, y];
                            newBitmapColors[x, y] = Color.FromArgb(color.R, color.G, color.B);
                        }
                    }*/
                }
                if (identical) break;
            }

            //Przepisanie kolorów na piksele
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Color color = sourceColors[x, y];
                    sourceBitmap.SetPixel(x, y, Color.FromArgb(color.R, color.G, color.B));
                }
            }
            /* 
             * int[,] mask = maskA[0];
              for (int x = 1; x < bitmap.Width - 1; x++)
              {
                  for (int y = 1; y < bitmap.Height - 1; y++)
                  {
                      bool hit = true;
                      int maskX = 0;
                      for (int i_pos = x - 1; i_pos <= x + 1; i_pos++)
                      {
                          int maskY = 0;
                          for (int j_pos = y - 1; j_pos <= y + 1; j_pos++)
                          {

                              Color color = bitmap.GetPixel(i_pos, j_pos);
                              int colorVal = color.R == 255 ? 1 : 0;

                              if (mask[maskX, maskY] == 2)
                              {
                                  maskY++;
                                  continue;
                              }

                              if (mask[maskX, maskY] != colorVal)
                              {
                                  hit = false;
                                  break;
                              }

                              maskY++;
                          }
                          maskX++;
                      }

                      Color colorA, colorB;
                      if (hit) colorB = Color.FromArgb(255, 255, 255);
                      else colorB = Color.FromArgb(0, 0, 0);

                      colorA = sourceBitmap.GetPixel(x, y);

                      Color subAB = Color.FromArgb(colorA.R + colorB.R > 255 ? 255 : colorA.R + colorB.R,
                                                   colorA.G + colorB.G > 255 ? 255 : colorA.G + colorB.G,
                                                   colorA.B + colorB.B > 255 ? 255 : colorA.B + colorB.B);
                      sourceBitmap.SetPixel(x, y, subAB);
                  }
              }
              mask = maskB[0];
              for (int x = 1; x < bitmap.Width - 1; x++)
              {
                  for (int y = 1; y < bitmap.Height - 1; y++)
                  {
                      bool hit = true;
                      int maskX = 0;
                      for (int i_pos = x - 1; i_pos <= x + 1; i_pos++)
                      {
                          int maskY = 0;
                          for (int j_pos = y - 1; j_pos <= y + 1; j_pos++)
                          {

                              Color color = bitmap.GetPixel(i_pos, j_pos);
                              int colorVal = color.R == 255 ? 1 : 0;

                              if (mask[maskX, maskY] == 2)
                              {
                                  maskY++;
                                  continue;
                              }

                              if (mask[maskX, maskY] != colorVal)
                              {
                                  hit = false;
                                  break;
                              }

                              maskY++;
                          }
                          maskX++;
                      }

                      Color colorA, colorB;
                      if (hit) colorB = Color.FromArgb(255, 255, 255);
                      else colorB = Color.FromArgb(0, 0, 0);

                      colorA = sourceBitmap.GetPixel(x, y);

                      Color subAB = Color.FromArgb(colorA.R + colorB.R > 255 ? 255 : colorA.R + colorB.R,
                                                   colorA.G + colorB.G > 255 ? 255 : colorA.G + colorB.G,
                                                   colorA.B + colorB.B > 255 ? 255 : colorA.B + colorB.B);
                      sourceBitmap.SetPixel(x, y, subAB);
                  }
              }*/

            return sourceBitmap;
        }
        //zamiast na bitmapie to wykonywać operacje na 2 tablicach pikseli dopiero pod koniec zrobic powrtonie bitmape
        private static Color[,] GetColors(Bitmap bitmap)
        {
            Color[,] colors = new Color[bitmap.Width,bitmap.Height];
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    colors[x, y] = bitmap.GetPixel(x, y);
                }
            }

            return colors;
        }

        private static Boolean CompareColorsArrays(Color[,] A , Color[,] B)
        {
            for (int x = 0; x < A.GetLength(0); x++)
                {
                    for (int y = 0; y < A.GetLength(1); y++)
                    {
                        
                    if (A[x, y].R != B[x, y].R) return false;
                    }
                }
                return true;
        }

        public static Color HsvToColor(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - saturation));
            int q = Convert.ToInt32(value * (1 - f * saturation));
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return Color.FromArgb(v, t, p);
            else if (hi == 1)
                return Color.FromArgb(q, v, p);
            else if (hi == 2)
                return Color.FromArgb(p, v, t);
            else if (hi == 3)
                return Color.FromArgb(p, q, v);
            else if (hi == 4)
                return Color.FromArgb(t, p, v);
            else
                return Color.FromArgb(v, p, q);
        }
        private static void ColorToHSV(Color color, out double hue, out double saturation, out double value)
        {
            int max = Math.Max(color.R, Math.Max(color.G, color.B));
            int min = Math.Min(color.R, Math.Min(color.G, color.B));

            hue = color.GetHue();
            saturation = (max == 0) ? 0 : 1d - (1d * min / max);
            value = max / 255d;
        }

        public static Bitmap GetColorMask(Bitmap bitmap, double hueMin,double hueMax)
        {
            //pobranie zielonych kolorów
            //nałożenie maski
            //uzycie filtra otwarcia / domkniecia
           
            Bitmap sourceBitmap = new Bitmap(bitmap);

            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    Color rgbColor = bitmap.GetPixel(x, y);
                    double hue, saturation, value;
                    ColorToHSV(rgbColor,out hue,out saturation,out value);

                    //hue > 90d && hue < 150d
                    //(hue > 70d && hue < 170d
                    if (hue > hueMin && hue < hueMax &&
                       saturation > 0.1d && saturation < 1d &&
                       value > 0.1d && value < 1d
                       )
                        sourceBitmap.SetPixel(x, y, Color.FromArgb(255, 255, 255));
                    else
                        sourceBitmap.SetPixel(x, y, Color.FromArgb(0, 0, 0));
                }
            }
          return sourceBitmap;
        }


        public static double GetAmount(Bitmap mask)
        {
            double allPixels = mask.Width * mask.Height;
            double whites = 0;
            for (int x = 0; x < mask.Width; x++)
            {
                for (int y = 0; y < mask.Height; y++)
                {
                    Color colorMask = mask.GetPixel(x, y);
                    if (colorMask.R == 255)
                    {
                        whites++;
                    }
                }
            }

            return Math.Round(whites);
        }

        public static Bitmap SelectMaskDisplay(Bitmap mask, Bitmap bitmap)
        {
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    Color colorMask = mask.GetPixel(x, y);
                    Color colorBitmap = bitmap.GetPixel(x, y);
                    if (colorMask.R==255)
                    {
                        double hue, saturation, value;

                        ColorToHSV(colorBitmap, out hue,out saturation,out value);
                        colorBitmap = HsvToColor(10d,saturation + 0.2d >=1d ? saturation: saturation + 0.2d, value);

                        bitmap.SetPixel(x, y, Color.FromArgb(colorBitmap.R, colorBitmap.G, colorBitmap.B));
                    }
                    else bitmap.SetPixel(x, y, Color.FromArgb(colorBitmap.R, colorBitmap.G, colorBitmap.B));
                }
            }
            return bitmap;
        }

        public static Bitmap CreateGradientDiv()
        {
            Bitmap gradientDiv = new Bitmap(360,20);

            for (int x = 0; x < 359; x++)
            {
                //Color color = HsvToColor((double)x,1d,1d);
                for (int y = 0; y < 20; y++)
                {
                    Color color = HsvToColor((double)x,1d-((double)y*0.05), 1d);
                    gradientDiv.SetPixel(x,y,color);
                }
            }

                return gradientDiv;
        }

        private static int[,] Rotate90Clockwise(int[,] arry)
        {
            int m = arry.GetLength(0);
            int n = arry.GetLength(1);
            int j = 0;
            int p = 0;
            int q = 0;
            int i = m - 1;
            int[,] rotatedMask = new int[m, n];

            for (int k = 0; k < m; k++)
            {
                while (i >= 0)
                {
                    rotatedMask[p, q] = arry[i, j];
                    q++;
                    i--;
                }
                j++;
                i = m - 1;
                q = 0;
                p++;
            }
            return rotatedMask;
        }
        //inwersja
        /* public static Bitmap Erosion(Bitmap bitmap)
         {

             Bitmap newBitmap = new Bitmap(bitmap.Width, bitmap.Height);
             Color newPixel, pixel, borderPixel;


             int[,] mask = new int[3, 3];
             //na bialo
             for (int x = 0; x < bitmap.Width; x++)
             {
                 for (int y = 0; y < bitmap.Height; y++)
                 {
                     pixel = bitmap.GetPixel(x, y);
                     newBitmap.SetPixel(x, y, Color.FromArgb(0, 0, 0));
                 }
             }

             int quant = mask.GetLength(0) * mask.GetLength(1) - 1;
             bool allMask;

             for (int x = 1; x < bitmap.Width - 1; x++)
             {
                 for (int y = 1; y < bitmap.Height - 1; y++)
                 {
                     int blacks = 0;
                     allMask = false;
                     for (int i = (x - 1); i <= (x + 1); i++)
                     {
                         for (int j = (y - 1); j <= (y + 1); j++)
                         {
                             borderPixel = bitmap.GetPixel(i, j);

                             if (i == x && j == y) ;
                             else
                             //sprawdzenie koloru czarnego
                             if (borderPixel.R == 0 || borderPixel.G == 0 || borderPixel.B == 0) blacks++;
                             //jezeliwszystkie są czarne to 
                         }
                     }

                     if (blacks == quant) allMask = true;

                     if (allMask)
                     {
                         *//*newBitmap.SetPixel(x, y, Color.FromArgb(0, 0, 0));*//*
                         for (int i = (x - 1); i <= (x + 1); i++)
                         {
                             for (int j = (y - 1); j <= (y + 1); j++)
                             {
                                 if (i == x && j == y) ;
                                 else
                                     newBitmap.SetPixel(i, j, Color.FromArgb(255, 255, 255));
                             }
                         }
                     }
                 }
             }

             return newBitmap;
         }*/
        //wszywanki
        /* public static Bitmap Erosion(Bitmap bitmap)
         {

             Bitmap newBitmap = new Bitmap(bitmap.Width, bitmap.Height);
             Color newPixel, pixel, borderPixel;


             int[,] mask = new int[3, 3];
             newBitmap = bitmap;
             *//* for (int i = 0; i < mask.GetLength(0); i++)
              {
                  for (int j = 0; j < mask.GetLength(1); j++)
                  {
                      mask[i, j] = 255;
                  }
              }
  */
        /*for (int x = 0; x < bitmap.Width; x++)
        {
            for (int y = 0; y < bitmap.Height; y++)
            {
                pixel = bitmap.GetPixel(x, y);
                newBitmap.SetPixel(x, y, Color.FromArgb(255, 255, 255));
            }
        }*//*

        int quant = mask.GetLength(0) * mask.GetLength(1) - 1;
        bool allMask;

        for (int x = 1; x < bitmap.Width - 1; x++)
        {
            for (int y = 1; y < bitmap.Height - 1; y++)
            {
                int blacks = 0;
                allMask = false;
                for (int i = (x - 1); i <= (x + 1); i++)
                {
                    for (int j = (y - 1); j <= (y + 1); j++)
                    {
                        borderPixel = bitmap.GetPixel(i, j);

                        if (i == x && j == y) ;
                        else
                        //sprawdzenie koloru czarnego
                        if (borderPixel.R == 0 || borderPixel.G == 0 || borderPixel.B == 0) blacks++;
                        //jezeliwszystkie są czarne to 
                    }
                }
                if (blacks == quant)
                    allMask = true;

                *//*                    pixel = bitmap.GetPixel(x, y);
                *//*
                if (allMask)
                {
                    for (int i = (x - 1); i <= (x + 1); i++)
                    {
                        for (int j = (y - 1); j <= (y + 1); j++)
                        {
                            newBitmap.SetPixel(i, j, Color.FromArgb(255, 255, 255));
                        }
                    }
                    newBitmap.SetPixel(x, y, Color.FromArgb(0, 0, 0));
                }
                *//* else
                 {
                     if (pixel.R == 255 || pixel.G == 255 || pixel.B == 255)
                         newBitmap.SetPixel(x, y, Color.FromArgb(255, 255, 255));
                     else
                         newBitmap.SetPixel(x, y, Color.FromArgb(0, 0, 0));
                 }*//*
            }
        }

        return newBitmap;
    }*/



        //OLD
        /* Bitmap newBitmap = new Bitmap(bitmap.Width, bitmap.Height);
         Color newPixel, pixel;

       for (int x = 0; x < bitmap.Width ; x++)
         {
             for (int y = 1; y < bitmap.Height; y++)
             {
                 pixel = bitmap.GetPixel(x, y);
                 newBitmap.SetPixel(x, y, Color.FromArgb(0, 0, 0));
             }
         }

         for (int x = 1; x < bitmap.Width - 1; x++)
         {
             for (int y = 1; y < bitmap.Height - 1; y++)
             {
                 pixel = bitmap.GetPixel(x, y);

                 if (pixel.R == 255 || pixel.G == 255 || pixel.B == 255)
                 {
                     for (int i = (x - 1); i <= (x + 1); i++)
                     {
                         for (int j = (y - 1); j <= (y + 1); j++)
                         {
                             newBitmap.SetPixel(i, j, Color.FromArgb(255, 255, 255));
                         }
                     }
                 }

             }
         }*/


        //przed czyszczeniem
       /* public static Bitmap Thin(Bitmap bitmap)
        {
            Color color;

            //przeleciec wsio maski

            *//*int[,,] maskA =
            { 
                {
                  {0,0,0},
                  {2,1,2},
                  {1,1,1}
                },
                {
                  {1,2,0},
                  {1,1,0},
                  {1,2,0}
                },
                {
                  {1,1,1},
                  {2,1,2},
                  {0,0,0}
                },
                {
                  {0,2,1},
                  {0,1,1},
                  {0,2,1}
                }
            };*//*
            List<int[,]> maskA = new List<int[,]>();
            maskA.Add(
                    new int[,]{
                      {0,0,0},
                      {2,1,2},
                      {1,1,1}
                    }
                );
            maskA.Add(
                    new int[,]{
                      {1,2,0},
                      {1,1,0},
                      {1,2,0}
                    }
                );
            maskA.Add(
                    new int[,]{
                      {1,1,1},
                      {2,1,2},
                      {0,0,0}
                    }
                );
            maskA.Add(
                    new int[,]{
                      {0,2,1},
                      {0,1,1},
                      {0,2,1}
                    }
                );
            List<int[,]> maskB = new List<int[,]>();
            maskB.Add(
                    new int[,]{
                      { 2,0,0},
                      { 1,1,0},
                      { 2,1,2}
                    }
                );
            maskB.Add(
                    new int[,]{
                      {2,1,2},
                      {1,1,0},
                      {2,0,0}
                    }
                );
            maskB.Add(
                    new int[,]{
                      {2,1,2},
                      {0,1,1},
                      {0,0,2}
                    }
                );
            maskB.Add(
                    new int[,]{
                      {0,0,2},
                      {0,1,1},
                      {2,1,2}
                    }
                );
            *//* int[,,] maskB ={
                 {
                   {2,0,0},
                   {1,1,0},
                   {2,1,2}
                 },
                 {
                   {2,1,2},
                   {1,1,0},
                   {2,0,0}
                 },
                 {
                   {2,1,2},
                   {0,1,1},
                   {0,0,2}
                 },
                 {
                   {0,0,2},
                   {0,1,1},
                   {2,1,2}
                 }
             };
            *//*
            Bitmap sourceBitmap = new Bitmap(bitmap);

            Bitmap
                bitmapMaskA = null,
                bitmapMaskB = null,
                newBitmap = null;



            bool identical = false;
            while (true)
            {
                for (int i = 0; i < 4; i++)
                {
                    //odejmij od pierwotnego obrazu Maske A
                    bitmapMaskA = HitOrMiss(sourceBitmap, maskA[i]);
                    sourceBitmap = SubstactBitmaps(new Bitmap(sourceBitmap), bitmapMaskA);

                    //odejmij od pierwotnego obrazu Maske B
                    bitmapMaskB = HitOrMiss(sourceBitmap, maskB[i]);
                    sourceBitmap = SubstactBitmaps(new Bitmap(sourceBitmap), bitmapMaskB);


                    identical = CompareMemCmp(sourceBitmap, newBitmap);
                    if (identical) break;

                    newBitmap = sourceBitmap;
                }
                if (identical) break;
            }
            *//* int[,] mA =
                 {
                   {0,0,0},
                   {2,1,2},
                   {1,1,1}
                 };
             int[,] mB =
                  {
                    {2,0,0},
                    {1,1,0},
                    {2,1,2}
                  };
             bitmapMaskA = HitOrMiss(sourceBitmap, mA);
             sourceBitmap = SubstactBitmaps(sourceBitmap, bitmapMaskA);

             //odejmij od pierwotnego obrazu Maske B
             bitmapMaskB = HitOrMiss(sourceBitmap, mB);
             sourceBitmap = SubstactBitmaps(sourceBitmap, bitmapMaskB);*//*

            return sourceBitmap;
        }*/


        
    }

}
