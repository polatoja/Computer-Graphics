﻿using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Image_filtering
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void AddImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif",
                Title = "Select an Image"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                SelectedImage.Source = new WriteableBitmap(new BitmapImage(new Uri(openFileDialog.FileName)));
                ModifiedImage.Source = new WriteableBitmap(new BitmapImage(new Uri(openFileDialog.FileName)));
            }
        }

        private void SaveImage_Click(object sender, RoutedEventArgs e)
        {
            if (ModifiedImage.Source == null)
            {
                MessageBox.Show("No modified image to save!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Open Save File Dialog
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "PNG Image|*.png|JPEG Image|*.jpg|Bitmap Image|*.bmp",
                Title = "Save Image",
                FileName = "ModifiedImage"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    // Convert ImageSource to BitmapSource
                    BitmapSource bitmapSource = (BitmapSource)ModifiedImage.Source;

                    // Encode the image based on file type
                    BitmapEncoder encoder;
                    string extension = System.IO.Path.GetExtension(saveFileDialog.FileName).ToLower();
                    switch (extension)
                    {
                        case ".jpg":
                        case ".jpeg":
                            encoder = new JpegBitmapEncoder();
                            break;
                        case ".bmp":
                            encoder = new BmpBitmapEncoder();
                            break;
                        default:
                            encoder = new PngBitmapEncoder();
                            break;
                    }

                    encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

                    // Save the file
                    using (FileStream fileStream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                    {
                        encoder.Save(fileStream);
                    }

                    MessageBox.Show("Image saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error saving image: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BackToOriginal_Click(object sender, RoutedEventArgs e)
        {
            if (ModifiedImage.Source == null)
                return;

            ModifiedImage.Source = SelectedImage.Source;
        }

        private void Inversion_Click(object sender, RoutedEventArgs e)
        {
            if (ModifiedImage.Source == null)
                return;
            
            ModifiedImage.Source = ImageFilters.Invert((BitmapSource)ModifiedImage.Source);
        }

        private void Brigtness_Click(object sender, RoutedEventArgs e)
        {
            if (ModifiedImage.Source == null)
                return;

            int brightness = 20;
            ModifiedImage.Source = ImageFilters.IncreaseBrightness((BitmapSource)ModifiedImage.Source, brightness);
        }

        private void Contrast_Click(object sender, RoutedEventArgs e)
        {
            if (ModifiedImage.Source == null)
                return;

            int contrast = 30;
            ModifiedImage.Source = ImageFilters.EnhanceContrast((BitmapSource)ModifiedImage.Source, contrast);
        }

        private void Gamma_Click(object sender, RoutedEventArgs e)
        {
            if (ModifiedImage.Source == null)
                return;

            double gamma = 1.2;
            ModifiedImage.Source = ImageFilters.GammaCorrection((BitmapSource)ModifiedImage.Source, gamma);
        }

        private void Blur_Click(object sender, RoutedEventArgs e)
        {
            if (ModifiedImage.Source == null)
                return;

            string filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Blur.conv");
            filePath = System.IO.Path.GetFullPath(filePath);

            if (!File.Exists(filePath))
            {
                MessageBox.Show($"Kernel file not found at: {filePath}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ModifiedImage.Source = ConvolutionFilters.ApplyBlurFromFile((BitmapSource)ModifiedImage.Source, filePath);
        }
    }
}