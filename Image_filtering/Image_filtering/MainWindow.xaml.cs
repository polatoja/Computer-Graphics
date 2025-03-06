using Microsoft.Win32;
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
using static Image_filtering.ConvolutionFilters;

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
                    WriteableBitmap bitmapSource = (WriteableBitmap)ModifiedImage.Source;

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
            
            ModifiedImage.Source = ImageFilters.Invert((WriteableBitmap)ModifiedImage.Source);
        }

        private void Brightness_Click(object sender, RoutedEventArgs e)
        {
            if (ModifiedImage.Source == null)
                return;

            int brightness = 20;
            ModifiedImage.Source = ImageFilters.IncreaseBrightness((WriteableBitmap)ModifiedImage.Source, brightness);
        }

        private void Contrast_Click(object sender, RoutedEventArgs e)
        {
            if (ModifiedImage.Source == null)
                return;

            int contrast = 30;
            ModifiedImage.Source = ImageFilters.EnhanceContrast((WriteableBitmap)ModifiedImage.Source, contrast);
        }

        private void Gamma_Click(object sender, RoutedEventArgs e)
        {
            if (ModifiedImage.Source == null)
                return;

            double gamma = 1.2;
            ModifiedImage.Source = ImageFilters.GammaCorrection((WriteableBitmap)ModifiedImage.Source, gamma);
        }

        private void Blur_Click(object sender, RoutedEventArgs e)
        {
            if (ModifiedImage.Source == null)
                return;

            string filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "ConvFilters", "Blur.conv");
            filePath = System.IO.Path.GetFullPath(filePath);

            if (!File.Exists(filePath))
            {
                MessageBox.Show($"Kernel file not found at: {filePath}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ModifiedImage.Source = ConvolutionFilters.ApplyFromFile((WriteableBitmap)ModifiedImage.Source, filePath);
        }

        private void Sharpen_Click(object sender, RoutedEventArgs e)
        {
            if (ModifiedImage.Source == null)
                return;

            string filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "ConvFilters", "Sharpen.conv");
            filePath = System.IO.Path.GetFullPath(filePath);

            if (!File.Exists(filePath))
            {
                MessageBox.Show($"Kernel file not found at: {filePath}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ModifiedImage.Source = ConvolutionFilters.ApplyFromFile((WriteableBitmap)ModifiedImage.Source, filePath);
        }

        private void Gaussian_Click(object sender, RoutedEventArgs e)
        {
            if (ModifiedImage.Source == null)
                return;

            string filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "ConvFilters", "GaussianBlur.conv");
            filePath = System.IO.Path.GetFullPath(filePath);

            if (!File.Exists(filePath))
            {
                MessageBox.Show($"Kernel file not found at: {filePath}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ModifiedImage.Source = ConvolutionFilters.ApplyFromFile((WriteableBitmap)ModifiedImage.Source, filePath);
        }

        private void Edge_Click(object sender, RoutedEventArgs e)
        {
            if (ModifiedImage.Source == null)
                return;

            string filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "ConvFilters", "EdgeDetection.conv");
            filePath = System.IO.Path.GetFullPath(filePath);

            if (!File.Exists(filePath))
            {
                MessageBox.Show($"Kernel file not found at: {filePath}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ModifiedImage.Source = ConvolutionFilters.ApplyFromFile((WriteableBitmap)ModifiedImage.Source, filePath);
        }

        private void Emboss_Click(object sender, RoutedEventArgs e)
        {
            if (ModifiedImage.Source == null)
                return;

            string filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "ConvFilters", "Emboss.conv");
            filePath = System.IO.Path.GetFullPath(filePath);

            if (!File.Exists(filePath))
            {
                MessageBox.Show($"Kernel file not found at: {filePath}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ModifiedImage.Source = ConvolutionFilters.ApplyFromFile((WriteableBitmap)ModifiedImage.Source, filePath);
        }

        private void ShowKernel_Click(object sender, RoutedEventArgs e)
        {
            KernelWindow kernelWindow = new KernelWindow();
            kernelWindow.Owner = this;

            if (kernelWindow.ShowDialog() == true)
            {
                Kernel customKernel = kernelWindow.SelectedKernel;

                if (customKernel.KernelValues != null && ModifiedImage.Source is WriteableBitmap writableBitmap)
                {
                    ModifiedImage.Source = ConvolutionFilters.ApplyConvolutionFilter(writableBitmap, customKernel);
                }
            }
        }

        private void SavedFilters_Click(object sender, RoutedEventArgs e)
        {
            if (ModifiedImage.Source == null)
                return;

            string filtersPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "ConvFilters");

            if (!Directory.Exists(filtersPath))
            {
                MessageBox.Show("Filter directory not found!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = filtersPath,
                Filter = "Convolution Filters (*.conv)|*.conv*",
                Title = "Select a Filter"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                MessageBox.Show("You are amazing!!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                /*
                try
                {
                    string[] filterData = File.ReadAllLines(filePath);

                    LoadFilter(filterData);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading the filter: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                */
            }
        }

        private void LoadFilter(string[] filterData)
        {
            int rows = filterData.Length;
            int cols = filterData[0].Split(' ').Length;
            double[,] kernelValues = new double[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                var values = filterData[i].Split(' ').Select(double.Parse).ToArray();
                for (int j = 0; j < cols; j++)
                {
                    kernelValues[i, j] = values[j];
                }
            }

            Kernel selectedKernel = new Kernel(kernelValues, rows, cols);
            MessageBox.Show("Filter loaded successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ChangeAnchor_Click(object sender, RoutedEventArgs e)
        {
            AnchorWindow anchorWindow = new AnchorWindow();
            anchorWindow.Owner = this;

            if (anchorWindow.ShowDialog() == true)
            {
                Kernel customKernel = anchorWindow.SelectedKernel;

                if (customKernel.KernelValues != null && ModifiedImage.Source is WriteableBitmap writableBitmap)
                {
                    ModifiedImage.Source = ConvolutionFilters.ApplyConvolutionFilter(writableBitmap, customKernel);
                }
            }
        }
    }
}