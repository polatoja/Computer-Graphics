using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Controls;
using Image_filtering.Operations;
using Image_filtering.Filters;
using static System.Net.Mime.MediaTypeNames;

namespace Image_filtering
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private ListBox filtersListBox;
        private int selectedDitherSize;
        private int errorDiffusionType;
        private void AddImage_Click(object sender, RoutedEventArgs e)
        {
            SelectedImage.Source = ImageOperations.LoadImage();
            ModifiedImage.Source = SelectedImage.Source;
        }

        private void SaveImage_Click(object sender, RoutedEventArgs e)
        {
            ImageOperations.SaveImage((WriteableBitmap)ModifiedImage.Source);
        }

        private void BackToOriginal_Click(object sender, RoutedEventArgs e)
        {
            ModifiedImage.Source = SelectedImage.Source;
        }

        private void Inversion_Click(object sender, RoutedEventArgs e)
        {
            ModifiedImage.Source = FunctionFilterOperations.ApplyInversion((WriteableBitmap)ModifiedImage.Source);
        }

        private void Brightness_Click(object sender, RoutedEventArgs e)
        {
            ModifiedImage.Source = FunctionFilterOperations.ApplyBrightness((WriteableBitmap)ModifiedImage.Source, 20);
        }

        private void Contrast_Click(object sender, RoutedEventArgs e)
        {
            ModifiedImage.Source = FunctionFilterOperations.ApplyContrast((WriteableBitmap)ModifiedImage.Source, 30);
        }

        private void Gamma_Click(object sender, RoutedEventArgs e)
        {
            ModifiedImage.Source = FunctionFilterOperations.ApplyGamma((WriteableBitmap)ModifiedImage.Source, 1.2);
        }

        private void Median_Click(object sender, RoutedEventArgs e)
        {
            int filterSize = FunctionFilterOperations.GetFilterSize();
            ModifiedImage.Source = FunctionFilterOperations.ApplyMedianFilter((WriteableBitmap)ModifiedImage.Source, filterSize);
        }

        private void Blur_Click(object sender, RoutedEventArgs e)
        {
            ModifiedImage.Source = ConvolutionFilterOperations.ApplyBlur((WriteableBitmap)ModifiedImage.Source);
        }

        private void Sharpen_Click(object sender, RoutedEventArgs e)
        {
            ModifiedImage.Source = ConvolutionFilterOperations.ApplySharpen((WriteableBitmap)ModifiedImage.Source);
        }

        private void Gaussian_Click(object sender, RoutedEventArgs e)
        {
            ModifiedImage.Source = ConvolutionFilterOperations.ApplyGaussianBlur((WriteableBitmap)ModifiedImage.Source);
        }

        private void Edge_Click(object sender, RoutedEventArgs e)
        {
            ModifiedImage.Source = ConvolutionFilterOperations.ApplyEdgeDetection((WriteableBitmap)ModifiedImage.Source);
        }

        private void Emboss_Click(object sender, RoutedEventArgs e)
        {
            ModifiedImage.Source = ConvolutionFilterOperations.ApplyEmboss((WriteableBitmap)ModifiedImage.Source);
        }

        private void ShowKernel_Click(object sender, RoutedEventArgs e)
        {
            KernelOperations.ShowKernel(this, (WriteableBitmap)ModifiedImage.Source);
        }

        private void SavedFilters_Click(object sender, RoutedEventArgs e)
        {
            if (filtersListBox == null)
            {
                filtersListBox = KernelOperations.LoadSavedFilters(YourFilterGrid);
            }
        }

        private void ChangeAnchor_Click(object sender, RoutedEventArgs e)
        {
            KernelOperations.ChangeAnchor(this, (WriteableBitmap)ModifiedImage.Source);
        }

        private void GreyScale_Click(object sender, RoutedEventArgs e)
        {
            ModifiedImage.Source = Ditchering.ConvertToGrey((WriteableBitmap)ModifiedImage.Source);
        }

        private void RandomDitchering_Click(object sender, RoutedEventArgs e)
        {
            ModifiedImage.Source = Ditchering.RandomDitchering((WriteableBitmap)ModifiedImage.Source);
        }

        private void AverageDitchering_Click(object sender, RoutedEventArgs e)
        {
            ModifiedImage.Source = Ditchering.AverageDitchering((WriteableBitmap)ModifiedImage.Source);
        }

        private void OrderedDitchering_Click(object sender, RoutedEventArgs e)
        {
            if (ModifiedImage.Source == null)
                return;

            if(OrderedDitcheringSize.Visibility == Visibility.Visible)
                OrderedDitcheringSize.Visibility = Visibility.Collapsed;
            else
                OrderedDitcheringSize.Visibility = Visibility.Visible;
        }

        private void DitherSize_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && int.TryParse(button.Tag.ToString(), out int size))
            {
                selectedDitherSize = size;
                MessageBox.Show($"Dithering size set to: {selectedDitherSize}x{selectedDitherSize}", "Dithering", MessageBoxButton.OK, MessageBoxImage.Information);

                ModifiedImage.Source = Ditchering.OrderedDithering((WriteableBitmap)ModifiedImage.Source, selectedDitherSize);
            }
            OrderedDitcheringSize.Visibility = Visibility.Collapsed;
        }

        private void ErrorDiffusion_Click(object sender, RoutedEventArgs e)
        {
            if (ModifiedImage.Source == null)
                return;

            if (ErrorDiffusionType.Visibility == Visibility.Visible)
                ErrorDiffusionType.Visibility = Visibility.Collapsed;
            else
                ErrorDiffusionType.Visibility = Visibility.Visible; 
        }

        private void ErrDiffType_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && int.TryParse(button.Tag.ToString(), out int type))
            {
                errorDiffusionType = type;
                MessageBox.Show($"Error diffusion type: {errorDiffusionType}", "Dithering", MessageBoxButton.OK, MessageBoxImage.Information);
                
                ModifiedImage.Source = Ditchering.ErrorDiffusionFunc((WriteableBitmap)ModifiedImage.Source, errorDiffusionType);
            }
            ErrorDiffusionType.Visibility = Visibility.Collapsed;
        }

        private void UniformQuantization_Click(object sender, RoutedEventArgs e)
        {
            UniformQuantizationLevel.Visibility = Visibility.Visible;
        }

        private void ApplyUniformQuantization_Click(object sender, RoutedEventArgs e)
        {
            if (ModifiedImage.Source == null)
                return;

            if (!int.TryParse(UniformQuantizationLevelTextBox.Text, out int levels) || levels < 2 || levels > 256)
            {
                MessageBox.Show("Please enter a valid number between 2 and 256.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            UniformQuantizationLevel.Visibility = Visibility.Collapsed;
            ModifiedImage.Source = Quantization.UniformColorQuantization((WriteableBitmap)ModifiedImage.Source, levels);
        }

        private void PopularityQuantization_Click(object sender, RoutedEventArgs e)
        {
            if (ModifiedImage.Source == null)
                return;
            PopularityQuantizationNum.Visibility = Visibility.Visible;
        }

        private void ApplyPopularityQuantization_Click(object sender, RoutedEventArgs e)
        {
            if (ModifiedImage.Source == null)
                return;

            if (!int.TryParse(UniformQuantizationLevelTextBox.Text, out int numColors) || numColors < 2 || numColors > 256)
            {
                MessageBox.Show("Please enter a valid number between 2 and 256.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            PopularityQuantizationNum.Visibility = Visibility.Collapsed;
            ModifiedImage.Source = Quantization.PopularityQuantization((WriteableBitmap)ModifiedImage.Source, numColors);
        }
    }
}
