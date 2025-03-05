using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Image_filtering
{
    public static class ConvolutionFilters
    {
        public struct Kernel
        {
            public double[,] KernelValues { get; }
            public int Rows { get; }
            public int Cols { get; }
            public int OffsetX { get; }
            public int OffsetY { get; }
            public Kernel(double[,] kernel, int rows, int cols, int offsetX, int offsetY)
            {
                KernelValues = kernel;
                Rows = rows;
                Cols = cols;
                OffsetX = offsetX;
                OffsetY = offsetY;
            }
            public Kernel(double[,] kernel, int rows, int cols)
            {
                KernelValues = kernel;
                Rows = rows;
                Cols = cols;
                OffsetX = cols / 2;
                OffsetY = rows / 2;
            }
        }

        public static Kernel LoadKernelFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Kernel file not found: {filePath}");

            string[] lines = File.ReadAllLines(filePath);
            string[] sizeInfo = lines[0].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (sizeInfo.Length != 2 ||
                !int.TryParse(sizeInfo[0], out int rows) ||
                !int.TryParse(sizeInfo[1], out int cols))
            {
                throw new FormatException("Invalid kernel size format. First line should contain two integers: rows and columns.");
            }

            double[,] kernelValues = new double[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                string[] values = lines[i + 1].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (values.Length != cols)
                    throw new FormatException($"Invalid row format at line {i + 2}. Expected {cols} values.");

                for (int j = 0; j < cols; j++)
                {
                    if (!double.TryParse(values[j], NumberStyles.Float, CultureInfo.InvariantCulture, out kernelValues[i, j]))
                    {
                        throw new FormatException($"Invalid number at row {i + 1}, column {j + 1}.");
                    }
                }
            }

            return new Kernel(kernelValues, rows, cols);
        }


        public static WriteableBitmap ApplyConvolutionFilter(WriteableBitmap source, Kernel kernel)
        {
            int height = source.PixelHeight;
            int width = source.PixelWidth;
            int stride = 4 * width;
            byte[] pixelData = new byte[height * stride];
            byte[] resultData = new byte[height * stride];

            source.CopyPixels(pixelData, stride, 0);

            int offset_x = kernel.OffsetX;
            int offset_y = kernel.OffsetY;

            for (int y = offset_y; y < height - offset_y; y++)
            {
                for (int x = offset_x; x < width - offset_x; x++)
                {
                    double[] sum = new double[3]; // R, G, B channels

                    // Apply kernel
                    for (int ky = -offset_y; ky <= offset_y; ky++)
                    {
                        for (int kx = -offset_x; kx <= offset_x; kx++)
                        {
                            int pixelX = x + kx;
                            int pixelY = y + ky;
                            int index = (pixelY * stride) + (pixelX * 4); // 4 bytes per pixel

                            double weight = kernel.KernelValues[ky + offset_y, kx + offset_x];

                            sum[0] += pixelData[index] * weight;      // Blue
                            sum[1] += pixelData[index + 1] * weight;  // Green
                            sum[2] += pixelData[index + 2] * weight;  // Red
                        }
                    }

                    int newIndex = (y * stride) + (x * 4);
                    resultData[newIndex] = (byte)Math.Clamp((int)sum[0], 0, 255);
                    resultData[newIndex + 1] = (byte)Math.Clamp((int)sum[1], 0, 255);
                    resultData[newIndex + 2] = (byte)Math.Clamp((int)sum[2], 0, 255);
                    resultData[newIndex + 3] = pixelData[newIndex + 3]; // alpha
                }
            }

            return new WriteableBitmap(BitmapSource.Create(width, height, source.DpiX, source.DpiY,
                                       PixelFormats.Bgra32, null, resultData, stride));
        }

        public static WriteableBitmap ApplyFromFile(WriteableBitmap source, string filePath)
        {
            Kernel kernel = LoadKernelFromFile(filePath);
            return ApplyConvolutionFilter(source, kernel);
        }

    }
}
