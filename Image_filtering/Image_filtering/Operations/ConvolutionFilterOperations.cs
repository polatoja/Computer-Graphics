using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Image_filtering.Filters;

namespace Image_filtering.Operations
{
    public static class ConvolutionFilterOperations
    {
        private static string GetKernelPath(string kernelFileName)
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "ConvFilters", kernelFileName);
            return Path.GetFullPath(filePath);
        }

        private static WriteableBitmap ApplyConvolution(WriteableBitmap image, string kernelFileName)
        {
            string filePath = GetKernelPath(kernelFileName);

            if (!File.Exists(filePath))
            {
                MessageBox.Show($"Kernel file not found at: {filePath}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return image;
            }

            return ConvolutionFilters.ApplyFromFile(image, filePath);
        }

        public static WriteableBitmap ApplyBlur(WriteableBitmap image) => ApplyConvolution(image, "Blur.conv");
        public static WriteableBitmap ApplySharpen(WriteableBitmap image) => ApplyConvolution(image, "Sharpen.conv");
        public static WriteableBitmap ApplyGaussianBlur(WriteableBitmap image) => ApplyConvolution(image, "GaussianBlur.conv");
        public static WriteableBitmap ApplyEdgeDetection(WriteableBitmap image) => ApplyConvolution(image, "EdgeDetection.conv");
        public static WriteableBitmap ApplyEmboss(WriteableBitmap image) => ApplyConvolution(image, "Emboss.conv");
    }
}
