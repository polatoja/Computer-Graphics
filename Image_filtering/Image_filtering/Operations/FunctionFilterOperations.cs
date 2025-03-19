using System.Windows.Controls;
using System.Windows;
using System.Windows.Media.Imaging;
using Image_filtering.Filters;

namespace Image_filtering.Operations
{
    public static class FunctionFilterOperations
    {
        public static WriteableBitmap ApplyInversion(WriteableBitmap image)
        {
            return ImageFilters.Invert(image);
        }

        public static WriteableBitmap ApplyBrightness(WriteableBitmap image, int brightness)
        {
            return ImageFilters.IncreaseBrightness(image, brightness);
        }

        public static WriteableBitmap ApplyContrast(WriteableBitmap image, int contrast)
        {
            return ImageFilters.EnhanceContrast(image, contrast);
        }

        public static WriteableBitmap ApplyGamma(WriteableBitmap image, double gamma)
        {
            return ImageFilters.GammaCorrection(image, gamma);
        }

        public static WriteableBitmap ApplyMedianFilter(WriteableBitmap image, int filterSize)
        {
            return ImageFilters.MedianFilter(image, filterSize);
        }

        public static int GetFilterSize()
        {
            Window inputDialog = new Window
            {
                Width = 250,
                Height = 150,
                Title = "Filter Size",
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize
            };

            StackPanel panel = new StackPanel { Margin = new Thickness(10) };

            TextBlock message = new TextBlock { Text = "Choose size of filter:", Margin = new Thickness(0, 0, 0, 10) };
            TextBox inputBox = new TextBox { Width = 100 };
            Button confirmButton = new Button { Content = "OK", Width = 50, Margin = new Thickness(10) };

            confirmButton.Click += (s, args) => inputDialog.DialogResult = true;

            panel.Children.Add(message);
            panel.Children.Add(inputBox);
            panel.Children.Add(confirmButton);
            inputDialog.Content = panel;

            if (inputDialog.ShowDialog() == true)
            {
                int filterSize;
                if (!int.TryParse(inputBox.Text, out filterSize) && filterSize < 0 && filterSize > 15 && filterSize % 2 == 0)
                {
                    MessageBox.Show("Invalid filter size. Please enter a positive number.");
                    return -1;
                }
                return filterSize;
            }
            return -1;
        }


    }
}
