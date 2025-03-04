using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;

namespace Image_filtering
{
    public static class ImageFilters
    {

        public static BitmapSource Invert(BitmapSource source)
        {
            int height = source.PixelHeight;
            int width = source.PixelWidth;
            // stride = number of bytes per row of pixels in the image
            // in 32-bit we have R, G, B, alpha
            int stride = 4 * width;
            byte[] pixelData = new byte[height * stride];

            source.CopyPixels(pixelData, stride, 0);

            for (int i = 0; i < pixelData.Length; i += 4)
            {
                pixelData[i] = (byte)(255 - pixelData[i]);       // Blue
                pixelData[i + 1] = (byte)(255 - pixelData[i + 1]); // Green
                pixelData[i + 2] = (byte)(255 - pixelData[i + 2]); // Red
            }

            return BitmapSource.Create(width, height, source.DpiX, source.DpiY,
                                   PixelFormats.Bgra32, null, pixelData, stride);
        }

        public static BitmapSource IncreaseBrightness(BitmapSource source, int brightness)
        {
            int height = source.PixelHeight;
            int width = source.PixelWidth;
            int stride = 4 * width;
            byte[] pixelData = new byte[height * stride];

            source.CopyPixels(pixelData, stride, 0);

            for (int i = 0; i < pixelData.Length; i += 4)
            {
                pixelData[i] = (byte)Math.Max(0, Math.Min(255, pixelData[i] + brightness));       // Blue
                pixelData[i + 1] = (byte)Math.Max(0, Math.Min(255, pixelData[i + 1] + brightness)); // Green
                pixelData[i + 2] = (byte)Math.Max(0, Math.Min(255, pixelData[i + 2] + brightness)); // Red
            }

            return BitmapSource.Create(width, height, source.DpiX, source.DpiY,
                                   PixelFormats.Bgra32, null, pixelData, stride);
        }

        public static BitmapSource EnhanceContrast(BitmapSource source, int contrast)
        {
            int height = source.PixelHeight;
            int width = source.PixelWidth;
            int stride = 4 * width;
            byte[] pixelData = new byte[height * stride];

            source.CopyPixels(pixelData, stride, 0);

            double factor = (double) 255 / (255 - 2 * contrast);

            for (int i = 0; i < pixelData.Length; i += 4)
            {
                pixelData[i] = (byte)Math.Max(0, Math.Min(255, factor * (pixelData[i] - 128) + 128));       // Blue
                pixelData[i + 1] = (byte)Math.Max(0, Math.Min(255, factor * (pixelData[i + 1] - 128) + 128)); // Green
                pixelData[i + 2] = (byte)Math.Max(0, Math.Min(255, factor * (pixelData[i + 2] - 128) + 128)); // Red
            }

            return BitmapSource.Create(width, height, source.DpiX, source.DpiY,
                                   PixelFormats.Bgra32, null, pixelData, stride);
        }

        public static BitmapSource GammaCorrection(BitmapSource source, double gamma)
        {
            int height = source.PixelHeight;
            int width = source.PixelWidth;
            int stride = 4 * width;
            byte[] pixelData = new byte[height * stride];

            source.CopyPixels(pixelData, stride, 0);

            for (int i = 0; i < pixelData.Length; i += 4)
            {
                pixelData[i] = (byte)Math.Max(0, Math.Min(255, (int)(255 * Math.Pow(pixelData[i] / 255.0, gamma))));       // Blue
                pixelData[i + 1] = (byte)Math.Max(0, Math.Min(255, (int)(255 * Math.Pow(pixelData[i + 1] / 255.0, gamma)))); // Green
                pixelData[i + 2] = (byte)Math.Max(0, Math.Min(255, (int)(255 * Math.Pow(pixelData[i + 2] / 255.0, gamma)))); // Red
            }

            return BitmapSource.Create(width, height, source.DpiX, source.DpiY,
                                   PixelFormats.Bgra32, null, pixelData, stride);
        }

        public static double[,] LoadKernelFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Kernel file not found: {filePath}");

            string[] lines = File.ReadAllLines(filePath);
            double[,] kernel = new double[3, 3];

            for (int i = 0; i < 3; i++)
            {
                string[] values = lines[i].Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < 3; j++)
                {
                    kernel[i, j] = double.Parse(values[j], CultureInfo.InvariantCulture);
                }
            }

            return kernel;
        }


        public static BitmapSource ApplyBlurFromFile(BitmapSource source, string filePath)
        {
            double[,] kernel = LoadKernelFromFile(filePath);
            return ApplyConvolutionFilter(source, kernel);
        }


        public static BitmapSource ApplyConvolutionFilter(BitmapSource source, double[,] kernel)
        {
            int height = source.PixelHeight;
            int width = source.PixelWidth;
            int stride = 4 * width;
            byte[] pixelData = new byte[height * stride];
            byte[] resultData = new byte[height * stride];

            source.CopyPixels(pixelData, stride, 0);

            int kernelSize = 3;
            int offset = kernelSize / 2;

            for (int y = offset; y < height - offset; y++)
            {
                for (int x = offset; x < width - offset; x++)
                {
                    double[] sum = new double[3]; // R, G, B channels

                    for (int ky = -offset; ky <= offset; ky++)
                    {
                        for (int kx = -offset; kx <= offset; kx++)
                        {
                            int pixelIndex = ((y + ky) * stride) + ((x + kx) * 4);
                            double kernelValue = kernel[ky + offset, kx + offset];

                            sum[0] += pixelData[pixelIndex] * kernelValue;       // Blue
                            sum[1] += pixelData[pixelIndex + 1] * kernelValue;   // Green
                            sum[2] += pixelData[pixelIndex + 2] * kernelValue;   // Red
                        }
                    }

                    int resultIndex = (y * stride) + (x * 4);
                    resultData[resultIndex] = (byte)Math.Max(0, Math.Min(255, sum[0])); // Blue
                    resultData[resultIndex + 1] = (byte)Math.Max(0, Math.Min(255, sum[1])); // Green
                    resultData[resultIndex + 2] = (byte)Math.Max(0, Math.Min(255, sum[2])); // Red
                    resultData[resultIndex + 3] = pixelData[resultIndex + 3]; // Alpha
                }
            }

            return BitmapSource.Create(width, height, source.DpiX, source.DpiY,
                                       PixelFormats.Bgra32, null, resultData, stride);
        }

    }
}
