using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xceed.Wpf.Toolkit.Primitives;
namespace PS8
{

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            HueBar.Source = Parser.BitmapToBitmapImage(Morphology.CreateGradientDiv());
        }

        private void ParseFile(string fileName)
        {
            try
            {
                Bitmap bitmap = Parser.PPMToBitmap(fileName);
                BitmapImage image = Parser.BitmapToBitmapImage(bitmap);
                ImageBox.Source = image;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Slider_ScaleImage(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ImageBox.LayoutTransform = new ScaleTransform(e.NewValue, e.NewValue);
            ImageBoxEdit.LayoutTransform = new ScaleTransform(e.NewValue, e.NewValue);
        }

        private void OpenJpgFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "pliki graficzne PNG |*.png|pliki graficzne JPG |*.jpg|Wszystkie pliki|*.*";


            //MORPH
            InitDefaultMask();
            if (openFileDialog.ShowDialog() == true)
            {
                ImageBox.Source = new BitmapImage(new Uri(openFileDialog.FileName));
                ImageBoxEdit.Source = new BitmapImage(new Uri(openFileDialog.FileName));
            }
            ShowHistogram(ImageBoxEdit.Source as BitmapImage);

        }

        private void GrayScale(object sender, RoutedEventArgs e)
        {
            try
            {
                BitmapImage bitmapImage = ImageBoxEdit.Source as BitmapImage;
                Bitmap bitmap = Parser.BitmapImageToBitmap(bitmapImage);

                BitmapImage image = Parser.BitmapToBitmapImage(Filters.GrayScale(bitmap, false));
                ImageBoxEdit.Source = image;
                ShowHistogram(image);

            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void GrayScaleLuminosity(object sender, RoutedEventArgs e)
        {
            try
            {
                BitmapImage bitmapImage = ImageBoxEdit.Source as BitmapImage;
                Bitmap bitmap = Parser.BitmapImageToBitmap(bitmapImage);

                BitmapImage image = Parser.BitmapToBitmapImage(Filters.GrayScale(bitmap, true));
                ImageBoxEdit.Source = image;
                ShowHistogram(image);

            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
       
        private void Brightness(object sender, RoutedEventArgs e)
        {
            try
            {
                BitmapImage bitmapImage = ImageBoxEdit.Source as BitmapImage;
                Bitmap bitmap = Parser.BitmapImageToBitmap(bitmapImage);

                double value;
                if (!double.TryParse(BrightnessVal.Text, out value))
                {
                    MessageBox.Show("Niepoprawna wartość");
                    BrightnessVal.Text = "100";
                    return;
                }

                BitmapImage image = Parser.BitmapToBitmapImage(Filters.Brightness(bitmap, value/100) );
                ImageBoxEdit.Source = image;
                ShowHistogram(image);

            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ResetPreview(object sender, RoutedEventArgs e)
        {
            ImageBoxEdit.Source = ImageBox.Source;
            ShowHistogram(ImageBoxEdit.Source as BitmapImage);
        }

        private void ElementaryOperations(object sender, RoutedEventArgs e)
        {
            try
            {
                BitmapImage bitmapImage = ImageBoxEdit.Source as BitmapImage;
                Bitmap bitmap = Parser.BitmapImageToBitmap(bitmapImage);

                string opertator =ElementarOpertation_Val();
                BitmapImage image = Parser.BitmapToBitmapImage(Filters.ElementaryOperations(bitmap, opertator,
                                                                                                int.Parse(BlueVal.Text)));
                ImageBoxEdit.Source = image;

                ShowHistogram(image);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AverageFilter(object sender, RoutedEventArgs e)
        {
            try
            {
                BitmapImage bitmapImage = ImageBoxEdit.Source as BitmapImage;
                Bitmap bitmap = Parser.BitmapImageToBitmap(bitmapImage);

                BitmapImage image = Parser.BitmapToBitmapImage(Filters.AverageFilter(bitmap));
                ImageBoxEdit.Source = image;
                ShowHistogram(image);

            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void MedianFilter(object sender, RoutedEventArgs e)
        {
            try
            {
                BitmapImage bitmapImage = ImageBoxEdit.Source as BitmapImage;
                Bitmap bitmap = Parser.BitmapImageToBitmap(bitmapImage);

                BitmapImage image = Parser.BitmapToBitmapImage(Filters.MedianFilter(bitmap));
                ImageBoxEdit.Source = image;
                ShowHistogram(image);

            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void SobelFilter(object sender, RoutedEventArgs e)
        {
            try
            {
                BitmapImage bitmapImage = ImageBoxEdit.Source as BitmapImage;
                Bitmap bitmap = Parser.BitmapImageToBitmap(bitmapImage);

                BitmapImage image = Parser.BitmapToBitmapImage(Filters.SobelFilter(bitmap,"XY"));
                ImageBoxEdit.Source = image;
                ShowHistogram(image);

            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void SobelFilterX(object sender, RoutedEventArgs e)
        {
            try
            {
                BitmapImage bitmapImage = ImageBoxEdit.Source as BitmapImage;
                Bitmap bitmap = Parser.BitmapImageToBitmap(bitmapImage);

                BitmapImage image = Parser.BitmapToBitmapImage(Filters.SobelFilter(bitmap, "X"));
                ImageBoxEdit.Source = image;
                ShowHistogram(image);

            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void SobelFilterY(object sender, RoutedEventArgs e)
        {
            try
            {
                BitmapImage bitmapImage = ImageBoxEdit.Source as BitmapImage;
                Bitmap bitmap = Parser.BitmapImageToBitmap(bitmapImage);

                BitmapImage image = Parser.BitmapToBitmapImage(Filters.SobelFilter(bitmap, "Y"));
                ImageBoxEdit.Source = image;
                ShowHistogram(image);

            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void HighPassFilter(object sender, RoutedEventArgs e)
        {
            try
            {
                BitmapImage bitmapImage = ImageBoxEdit.Source as BitmapImage;
                Bitmap bitmap = Parser.BitmapImageToBitmap(bitmapImage);

                BitmapImage image = Parser.BitmapToBitmapImage(Filters.HighPassFilter(bitmap));
                ImageBoxEdit.Source = image;
                ShowHistogram(image);

            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void GaussianFilter(object sender, RoutedEventArgs e)
        {
            try
            {
                BitmapImage bitmapImage = ImageBoxEdit.Source as BitmapImage;
                Bitmap bitmap = Parser.BitmapImageToBitmap(bitmapImage);

                BitmapImage image = Parser.BitmapToBitmapImage(Filters.GaussianFilter(bitmap));
                ImageBoxEdit.Source = image;
                ShowHistogram(image);

            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        /*  private void ShowHistogram(object sender, RoutedEventArgs e)
          {
              BitmapImage bitmapImage = ImageBoxEdit.Source as BitmapImage;
              Bitmap bitmap = Parser.BitmapImageToBitmap(bitmapImage);
              Bitmap histogram = Histogram.ShowHistogram(bitmap);
              HistogramBox.Source = Parser.BitmapToBitmapImage(histogram);
          }*/
        private void ShowHistogram(BitmapImage bitmapImage)
        {
            Bitmap bitmap = Parser.BitmapImageToBitmap(bitmapImage);
            Bitmap histogram = Histogram.ShowHistogram(bitmap);
            HistogramBox.Source = Parser.BitmapToBitmapImage(histogram);
        }

        private void StretchHistogram(object sender, RoutedEventArgs e)
        {
            try
            {
                BitmapImage bitmapImage = ImageBoxEdit.Source as BitmapImage;
                Bitmap bitmap = Parser.BitmapImageToBitmap(bitmapImage);
                BitmapImage image = Parser.BitmapToBitmapImage(Histogram.StretchHistogram(bitmap));
                ImageBoxEdit.Source = image;
                ShowHistogram(image);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void EqualizationHistogram(object sender, RoutedEventArgs e)
        {
            try
            {
                BitmapImage bitmapImage = ImageBoxEdit.Source as BitmapImage;
                Bitmap bitmap = Parser.BitmapImageToBitmap(bitmapImage);
                BitmapImage image = Parser.BitmapToBitmapImage(Histogram.EqualizationHistogram(bitmap));
                ImageBoxEdit.Source = image;
                ShowHistogram(image);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
       
        private void CustomBinarization(object sender, RoutedEventArgs e)
        {
            try
            {
                BitmapImage bitmapImage = ImageBoxEdit.Source as BitmapImage;
                Bitmap bitmap = Parser.BitmapImageToBitmap(bitmapImage);
                BitmapImage image = Parser.BitmapToBitmapImage(Binarization.CustomBinarization(bitmap,int.Parse(ThresholdVal.Text)));
                ImageBoxEdit.Source = image;
                ShowHistogram(image);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void PercentBlackPixels(object sender, RoutedEventArgs e)
        {
            try
            {
                BitmapImage bitmapImage = ImageBoxEdit.Source as BitmapImage;
                Bitmap bitmap = Parser.BitmapImageToBitmap(bitmapImage);
                BitmapImage image = Parser.BitmapToBitmapImage(Binarization.PercentBlackPixels(bitmap, float.Parse(ThresholdPercentVal.Text)/100));
                ImageBoxEdit.Source = image;
                ShowHistogram(image);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void MeanIterativeSelection(object sender, RoutedEventArgs e)
        {
            try
            {
                BitmapImage bitmapImage = ImageBoxEdit.Source as BitmapImage;
                Bitmap bitmap = Parser.BitmapImageToBitmap(bitmapImage);
                BitmapImage image = Parser.BitmapToBitmapImage(Binarization.MeanIterativeSelection(bitmap));
                ImageBoxEdit.Source = image;
                ShowHistogram(image);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void EntropySelection(object sender, RoutedEventArgs e)
        {
            try
            {
                BitmapImage bitmapImage = ImageBoxEdit.Source as BitmapImage;
                Bitmap bitmap = Parser.BitmapImageToBitmap(bitmapImage);
                BitmapImage image = Parser.BitmapToBitmapImage(Binarization.EntropySelection(bitmap));
                ImageBoxEdit.Source = image;
                ShowHistogram(image);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void MinimumError(object sender, RoutedEventArgs e)
        {
            try
            {
                BitmapImage bitmapImage = ImageBoxEdit.Source as BitmapImage;
                Bitmap bitmap = Parser.BitmapImageToBitmap(bitmapImage);
                BitmapImage image = Parser.BitmapToBitmapImage(Binarization.MinimumError(bitmap));
                ImageBoxEdit.Source = image;
                ShowHistogram(image);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        //Morphology
        private void Dilatation(object sender, RoutedEventArgs e)
        {
            try
            {
                BitmapImage bitmapImage = ImageBoxEdit.Source as BitmapImage;
                Bitmap bitmap = Parser.BitmapImageToBitmap(bitmapImage);
                BitmapImage image = Parser.BitmapToBitmapImage(Morphology.Dilation(bitmap));
                ImageBoxEdit.Source = image;
                ShowHistogram(image);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void Erosion(object sender, RoutedEventArgs e)
        {
            try
            {
                BitmapImage bitmapImage = ImageBoxEdit.Source as BitmapImage;
                Bitmap bitmap = Parser.BitmapImageToBitmap(bitmapImage);
                BitmapImage image = Parser.BitmapToBitmapImage(Morphology.Erosion(bitmap));
                ImageBoxEdit.Source = image;
                ShowHistogram(image);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void Opening(object sender, RoutedEventArgs e)
        {
            try
            {
                BitmapImage bitmapImage = ImageBoxEdit.Source as BitmapImage;
                Bitmap bitmap = Parser.BitmapImageToBitmap(bitmapImage);
                BitmapImage image = Parser.BitmapToBitmapImage(Morphology.Opening(bitmap));
                ImageBoxEdit.Source = image;
                ShowHistogram(image);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void Closing(object sender, RoutedEventArgs e)
        {
            try
            {
                BitmapImage bitmapImage = ImageBoxEdit.Source as BitmapImage;
                Bitmap bitmap = Parser.BitmapImageToBitmap(bitmapImage);
                BitmapImage image = Parser.BitmapToBitmapImage(Morphology.Closing(bitmap));
                ImageBoxEdit.Source = image;
                ShowHistogram(image);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InitDefaultMask()
        {
            int[,] mask =
            {
                  {0,1,0},
                  {1,2,1},
                  {0,1,0}
            };
            MaskVal_0_0.Text = mask[0, 0].ToString();
            MaskVal_0_1.Text = mask[0, 1].ToString();
            MaskVal_0_2.Text = mask[0, 2].ToString();

            MaskVal_1_0.Text = mask[1, 0].ToString();
            MaskVal_1_1.Text = mask[1, 1].ToString();
            MaskVal_1_2.Text = mask[1, 2].ToString();

            MaskVal_2_0.Text = mask[2, 0].ToString();
            MaskVal_2_1.Text = mask[2, 1].ToString();
            MaskVal_2_2.Text = mask[2, 2].ToString();
        }

        private void HitOrMiss(object sender, RoutedEventArgs e)
        {
            //hitAMiss
           /* int[,] mask =
            {
                  {0,1,0},
                  {1,2,1},
                  {0,1,0}
            };*/

            int[,] mask =
            {
                  {int.Parse(MaskVal_0_0.Text),int.Parse(MaskVal_0_1.Text),int.Parse(MaskVal_0_2.Text)},
                  {int.Parse(MaskVal_1_0.Text),int.Parse(MaskVal_1_1.Text),int.Parse(MaskVal_1_2.Text)},
                  {int.Parse(MaskVal_2_0.Text),int.Parse(MaskVal_2_1.Text),int.Parse(MaskVal_2_2.Text)}
            };

            try
            {

                BitmapImage bitmapImage = ImageBoxEdit.Source as BitmapImage;
                Bitmap bitmap = Parser.BitmapImageToBitmap(bitmapImage);
                BitmapImage image = Parser.BitmapToBitmapImage(Morphology.HitOrMiss(bitmap,mask));
                ImageBoxEdit.Source = image;
                ShowHistogram(image);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
       

        private void Thin(object sender, RoutedEventArgs e)
        {
            try
            {
                BitmapImage bitmapImage = ImageBoxEdit.Source as BitmapImage;
                Bitmap bitmap = Parser.BitmapImageToBitmap(bitmapImage);
                BitmapImage image = Parser.BitmapToBitmapImage(Morphology.Thin2(bitmap));
                ImageBoxEdit.Source = image;
                ShowHistogram(image);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void Thick(object sender, RoutedEventArgs e)
        {
            try
            {
                BitmapImage bitmapImage = ImageBoxEdit.Source as BitmapImage;
                Bitmap bitmap = Parser.BitmapImageToBitmap(bitmapImage);
                BitmapImage image = Parser.BitmapToBitmapImage(Morphology.Thick2(bitmap));
                ImageBoxEdit.Source = image;
                ShowHistogram(image);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void GetGreen(object sender, RoutedEventArgs e)
        {
            try
            {
                BitmapImage bitmapImage = ImageBoxEdit.Source as BitmapImage;
                Bitmap bitmap = Parser.BitmapImageToBitmap(bitmapImage);

                //pobranie zielonych kolorów
                //nałożenie maski
                //uzycie filtra otwarcia 
                Bitmap mask = Morphology.Opening(Morphology.GetColorMask(bitmap,70d,170d));
                double amount = Morphology.GetAmount(mask);
                AmountBox.Content = $"{Math.Round(amount/(mask.Width*mask.Height)*100,2)}% ";

                BitmapImage image = Parser.BitmapToBitmapImage(Morphology.SelectMaskDisplay(mask, bitmap));
                ImageBoxEdit.Source = image;
                ShowHistogram(image);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void GetColor(object sender, RoutedEventArgs e)
        {
            try
            {
                BitmapImage bitmapImage = ImageBoxEdit.Source as BitmapImage;
                Bitmap bitmap = Parser.BitmapImageToBitmap(bitmapImage);

                double lower_treshold = Math.Round(HueSlider.LowerValue);
                double higher_treshold = Math.Round(HueSlider.HigherValue);

                //pobranie zielonych kolorów
                //nałożenie maski
                //uzycie filtra otwarcia 
                Bitmap mask = Morphology.Opening(Morphology.GetColorMask(bitmap,lower_treshold,higher_treshold));
                double amount = Morphology.GetAmount(mask);
                AmountBox.Content = $"{Math.Round(amount / (mask.Width * mask.Height) * 100, 2)}% ";

                BitmapImage image = Parser.BitmapToBitmapImage(Morphology.SelectMaskDisplay(mask, bitmap));
                ImageBoxEdit.Source = image;
                ShowHistogram(image);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        //Eleementrary operations
        private void Slider_Conversion(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            inputVal.Text = valueSliderJPG.Value.ToString();
        }

        private void SaveJpgFile(object sender, RoutedEventArgs e)
        {

            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "pliki graficzne jpg|*.jpg";
                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                if (saveFileDialog.ShowDialog() == true)
                {
                    var save = new JpegBitmapEncoder();
                    int jpglvl = (int)valueSliderJPG.Value;

                    save.QualityLevel = (int)jpglvl;
                    save.Frames.Add(BitmapFrame.Create((BitmapSource)ImageBoxEdit.Source));
                    using (var stream = saveFileDialog.OpenFile())
                    {
                        save.Save(stream);
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void SavePngFile(object sender, RoutedEventArgs e)
        {

            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "pliki graficzne png|*.png";
                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                if (saveFileDialog.ShowDialog() == true)
                {
                    var save = new PngBitmapEncoder();
                    save.Frames.Add(BitmapFrame.Create((BitmapSource)ImageBoxEdit.Source));
                    using (var stream = saveFileDialog.OpenFile())
                    {
                        save.Save(stream);
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }


        private void OpenPpmFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "pliki graficzne ppm|*.ppm";

            if (openFileDialog.ShowDialog() == true)
            {
                ParseFile(openFileDialog.FileName);
            }
        }


        private void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private string ElementarOpertation_Val()
        {
            if (Add.IsChecked == true) return Add.Name.ToString();
            if (Sub.IsChecked == true) return Sub.Name.ToString();
            if (Mul.IsChecked == true) return Mul.Name.ToString();
            if (Div.IsChecked == true) return Div.Name.ToString();
            return "none";
        }
    }
}
