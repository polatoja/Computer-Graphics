using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Image_filtering
{
    public static class ConvolutionFilters
    {
        public static double[,] LoadKernelFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Kernel file not found: {filePath}");

            string[] lines = File.ReadAllLines(filePath);
            double[,] kernel = new double[3, 3];

            for (int i = 0; i < 3; i++)
            {
                string[] values = lines[i].Split(new[] { ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < 3; j++)
                {
                    kernel[i, j] = double.Parse(values[j], CultureInfo.InvariantCulture);
                }
            }

            return kernel;
        }

        public static WriteableBitmap ApplyConvolutionFilter(WriteableBitmap source, double[,] kernel)
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

                    // Apply kernel
                    for (int ky = -offset; ky <= offset; ky++)
                    {
                        for (int kx = -offset; kx <= offset; kx++)
                        {
                            int pixelX = x + kx;
                            int pixelY = y + ky;
                            int index = (pixelY * stride) + (pixelX * 4); // 4 bytes per pixel

                            double weight = kernel[ky + offset, kx + offset];

                            sum[0] += pixelData[index] * weight;      // Blue
                            sum[1] += pixelData[index + 1] * weight;  // Green
                            sum[2] += pixelData[index + 2] * weight;  // Red
                        }
                    }

                    // Set new pixel values
                    int newIndex = (y * stride) + (x * 4);
                    resultData[newIndex] = (byte)Math.Clamp((int)sum[0], 0, 255);
                    resultData[newIndex + 1] = (byte)Math.Clamp((int)sum[1], 0, 255);
                    resultData[newIndex + 2] = (byte)Math.Clamp((int)sum[2], 0, 255);
                    resultData[newIndex + 3] = pixelData[newIndex + 3]; // Keep alpha
                }
            }

            return new WriteableBitmap(BitmapSource.Create(width, height, source.DpiX, source.DpiY,
                                       PixelFormats.Bgra32, null, resultData, stride));
        }



        public static WriteableBitmap ApplyBlurFromFile(WriteableBitmap source, string filePath)
        {
            double[,] kernel = LoadKernelFromFile(filePath);
            return ApplyConvolutionFilter(source, kernel);
        }
    }
}
