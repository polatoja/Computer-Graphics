using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using static Image_filtering.Filters.ErrorDiffusion;

namespace Image_filtering.Filters
{
    public static class Ditchering
    {
        public static WriteableBitmap ConvertToGrey(WriteableBitmap source)
        {
            int height = source.PixelHeight;
            int width = source.PixelWidth;
            // stride = number of bytes per row of pixels in the image
            // in 32-bit we have R, G, B, alpha
            int stride = 4 * width;
            byte[] pixelData = new byte[height * stride];

            source.CopyPixels(pixelData, stride, 0);

            // Y=0.299R+0.587G+0.114B
            for (int i = 0; i < pixelData.Length; i += 4)
            {
                byte blue = pixelData[i];       // Blue
                byte green = pixelData[i + 1]; // Green
                byte red = pixelData[i + 2]; // Red

                byte grey = (byte)(0.114 * blue + 0.587 * green + 0.299 * red);

                pixelData[i] = grey;
                pixelData[i + 1] = grey;
                pixelData[i + 2] = grey;
            }

            return new WriteableBitmap(BitmapSource.Create(width, height, source.DpiX, source.DpiY,
                                   PixelFormats.Bgra32, null, pixelData, stride));
        }

        public static WriteableBitmap RandomDitchering(WriteableBitmap source)
        {
            int height = source.PixelHeight;
            int width = source.PixelWidth;
            int stride = 4 * width;
            byte[] pixelData = new byte[height * stride];

            source.CopyPixels(pixelData, stride, 0);

            // Y=Y + (-30,30)
            for (int i = 0; i < pixelData.Length; i += 4)
            {
                Random rand = new Random();
                int n = rand.Next();
                if (n % 2 == 0) n = 1;
                else n = -1;

                byte blue = pixelData[i];       // Blue
                byte green = pixelData[i + 1]; // Green
                byte red = pixelData[i + 2]; // Red

                byte grey = (byte)(0.114 * blue + 0.587 * green + 0.299 * red + n * 30);

                pixelData[i] = grey;
                pixelData[i + 1] = grey;
                pixelData[i + 2] = grey;
            }

            return new WriteableBitmap(BitmapSource.Create(width, height, source.DpiX, source.DpiY,
                                   PixelFormats.Bgra32, null, pixelData, stride));
        }

        public static WriteableBitmap AverageDitchering(WriteableBitmap source)
        {
            int height = source.PixelHeight;
            int width = source.PixelWidth;
            int stride = 4 * width;
            byte[] pixelData = new byte[height * stride];

            source.CopyPixels(pixelData, stride, 0);

            for (int i = 0; i < pixelData.Length; i += 4)
            {
                byte newColor = 0;
                byte grey = (byte)(pixelData[i] + pixelData[i+1] + pixelData[i+2]);
                if (grey > 128) newColor = 255;

                pixelData[i] = newColor;
                pixelData[i + 1] = newColor;
                pixelData[i + 2] = newColor;
            }

            return new WriteableBitmap(BitmapSource.Create(width, height, source.DpiX, source.DpiY,
                                   PixelFormats.Bgra32, null, pixelData, stride));
        }


        public static int[,] GenerateBayerMatrix(int size)
        {
            if (size != 2 && size != 3 && size != 4 && size != 6)
                throw new ArgumentException("Only sizes 2, 3, 4, or 6 are supported.");

            int[,] matrix = new int[size, size];
            GenerateRecursiveMatrix(matrix, size);
            return matrix;
        }
        private static void CopyMatrix(int[,] destination, int[,] source)
        {
            for (int y = 0; y < source.GetLength(0); y++)
            {
                for (int x = 0; x < source.GetLength(1); x++)
                {
                    destination[y, x] = source[y, x];
                }
            }
        }

        private static void GenerateRecursiveMatrix(int[,] matrix, int size)
        {
            if (size == 2)
            {
                int[,] base2x2 = { { 0, 2 }, { 3, 1 } };
                CopyMatrix(matrix, base2x2);
                return;
            }
            else if (size == 3)
            {
                int[,] base3x3 = {
                    {  0,  7,  3 },
                    {  6,  5,  2 },
                    {  4,  1,  8 }
                };
                CopyMatrix(matrix, base3x3);
                return;
            }
            else
            {
                int prevSize = size / 2;
                int[,] smallerMatrix = new int[prevSize, prevSize];
                GenerateRecursiveMatrix(smallerMatrix, prevSize);

                for (int y = 0; y < prevSize; y++)
                {
                    for (int x = 0; x < prevSize; x++)
                    {
                        int value = smallerMatrix[y, x];
                        matrix[y, x] = 4 * (value - 1) + 1;
                        matrix[y, x + prevSize] = 4 * (value - 1) + 3;
                        matrix[y + prevSize, x] = 4 * (value - 1) + 4;
                        matrix[y + prevSize, x + prevSize] = 4 * (value - 1) + 2;
                    }
                }
            }
        }

        public static WriteableBitmap OrderedDithering(WriteableBitmap sourceBitmap, int size)
        {
            int[,] bayerMatrix = GenerateBayerMatrix(size);

            int width = sourceBitmap.PixelWidth;
            int height = sourceBitmap.PixelHeight;
            int stride = width * 4;
            byte[] pixelData = new byte[height * stride];

            sourceBitmap.CopyPixels(pixelData, stride, 0);

            int matrixScale = 256 / (size * size);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = (y * stride) + (x * 4);
                    byte gray = (byte)((pixelData[index] + pixelData[index + 1] + pixelData[index + 2]) / 3);

                    int threshold = bayerMatrix[y % size, x % size] * matrixScale;
                    byte newColor = gray > threshold ? (byte)255 : (byte)0;

                    pixelData[index] = pixelData[index + 1] = pixelData[index + 2] = newColor;
                }
            }

            WriteableBitmap ditheredBitmap = new WriteableBitmap(sourceBitmap);
            ditheredBitmap.WritePixels(new System.Windows.Int32Rect(0, 0, width, height), pixelData, stride, 0);

            return ditheredBitmap;
        }


        public static WriteableBitmap ErrorDiffusionFunc(WriteableBitmap sourceBitmap, int type)
        {
            string filePath = GetFilePath(type);
            ErrorKernel kernel = LoadKernelFromFile(filePath);

            int width = sourceBitmap.PixelWidth;
            int height = sourceBitmap.PixelHeight;
            int stride = width * 4;
            byte[] pixelData = new byte[height * stride];

            sourceBitmap.CopyPixels(pixelData, stride, 0);

            int f_x = kernel.Cols / 2;
            int f_y = kernel.Rows / 2;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = (y * stride) + (x * 4);

                    byte gray = (byte)((pixelData[index] + pixelData[index + 1] + pixelData[index + 2]) / 3);

                    byte newColor = gray > 128 ? (byte)255 : (byte)0;
                    pixelData[index] = pixelData[index + 1] = pixelData[index + 2] = newColor;

                    int error = gray - newColor;

                    // Distribute error using the kernel
                    for (int i = -f_x; i <= f_x; i++)
                    {
                        for (int j = -f_y; j <= f_y; j++)
                        {
                            int newX = x + i;
                            int newY = y + j;

                            // Ensure we're within image bounds
                            if (newX >= 0 && newX < width && newY >= 0 && newY < height)
                            {
                                int newIndex = (newY * stride) + (newX * 4);
                                int diffusedValue = pixelData[newIndex] + (error * kernel.KernelValues[j + f_y, i + f_x]) / kernel.Div;

                                // Clamp to valid byte range [0, 255]
                                pixelData[newIndex] = (byte)Math.Clamp(diffusedValue, 0, 255);
                            }
                        }
                    }
                }
            }

            WriteableBitmap ditheredBitmap = new WriteableBitmap(sourceBitmap);
            ditheredBitmap.WritePixels(new System.Windows.Int32Rect(0, 0, width, height), pixelData, stride, 0);

            return ditheredBitmap;
        }

    }
}
