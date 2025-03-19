using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Controls;
using Image_filtering.Operations;
using Image_filtering.Filters;

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
            ModifiedImage.Source = DitheringOperations.ConvertToGrey((WriteableBitmap)ModifiedImage.Source);
        }

        private void RandomDitchering_Click(object sender, RoutedEventArgs e)
        {
            ModifiedImage.Source = DitheringOperations.ApplyRandomDitchering((WriteableBitmap)ModifiedImage.Source);
        }

        private void AverageDitchering_Click(object sender, RoutedEventArgs e)
        {
            ModifiedImage.Source = DitheringOperations.ApplyAverageDitchering((WriteableBitmap)ModifiedImage.Source);
        }

        private void OrderedDitchering_Click(object sender, RoutedEventArgs e)
        {
            if (ModifiedImage.Source == null)
                return;

            OrderedDitcheringSize.Visibility = Visibility.Visible;
        }

        private void DitherSize_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && int.TryParse(button.Tag.ToString(), out int size))
            {
                selectedDitherSize = size;
                MessageBox.Show($"Dithering size set to: {selectedDitherSize}x{selectedDitherSize}", "Dithering", MessageBoxButton.OK, MessageBoxImage.Information);

                ModifiedImage.Source = DitheringOperations.ApplyOrderedDitchering((WriteableBitmap)ModifiedImage.Source, selectedDitherSize);
            }
            OrderedDitcheringSize.Visibility = Visibility.Collapsed;
        }

    }
}
