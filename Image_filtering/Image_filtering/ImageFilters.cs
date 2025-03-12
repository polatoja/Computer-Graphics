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

        public static WriteableBitmap Invert(WriteableBitmap source)
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

            return new WriteableBitmap(BitmapSource.Create(width, height, source.DpiX, source.DpiY,
                                   PixelFormats.Bgra32, null, pixelData, stride));
        }

        public static WriteableBitmap IncreaseBrightness(WriteableBitmap source, int brightness)
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

            return new WriteableBitmap(BitmapSource.Create(width, height, source.DpiX, source.DpiY,
                                   PixelFormats.Bgra32, null, pixelData, stride));
        }

        public static WriteableBitmap EnhanceContrast(WriteableBitmap source, int contrast)
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

            return new WriteableBitmap(BitmapSource.Create(width, height, source.DpiX, source.DpiY,
                                   PixelFormats.Bgra32, null, pixelData, stride));
        }

        public static WriteableBitmap GammaCorrection(WriteableBitmap source, double gamma)
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

            return new WriteableBitmap(BitmapSource.Create(width, height, source.DpiX, source.DpiY,
                                   PixelFormats.Bgra32, null, pixelData, stride));
        }


        public static WriteableBitmap MedianFilter(WriteableBitmap source, int filterSize)
        {
            int height = source.PixelHeight;
            int width = source.PixelWidth;
            int stride = 4 * width;
            byte[] pixelData = new byte[height * stride];
            byte[] resultData = new byte[height * stride];

            source.CopyPixels(pixelData, stride, 0);
            int halfSize = filterSize / 2;

            for (int y = halfSize; y < height - halfSize; y++)
            {
                for (int x = halfSize; x < width - halfSize; x++)
                {
                    int pixelIndex = (y * stride) + (x * 4);

                    byte[] blueChannel = new byte[filterSize * filterSize];
                    byte[] greenChannel = new byte[filterSize * filterSize];
                    byte[] redChannel = new byte[filterSize * filterSize];

                    int index = 0;
                    for (int fy = -halfSize; fy <= halfSize; fy++)
                    {
                        for (int fx = -halfSize; fx <= halfSize; fx++)
                        {
                            int neighborIndex = ((y + fy) * stride) + ((x + fx) * 4);
                            blueChannel[index] = pixelData[neighborIndex];
                            greenChannel[index] = pixelData[neighborIndex + 1];
                            redChannel[index] = pixelData[neighborIndex + 2];
                            index++;
                        }
                    }

                    Array.Sort(blueChannel);
                    Array.Sort(greenChannel);
                    Array.Sort(redChannel);

                    int medianIndex = blueChannel.Length / 2;
                    resultData[pixelIndex] = blueChannel[medianIndex];
                    resultData[pixelIndex + 1] = greenChannel[medianIndex];
                    resultData[pixelIndex + 2] = redChannel[medianIndex];
                    resultData[pixelIndex + 3] = pixelData[pixelIndex + 3];
                    resultData[pixelIndex + 3] = pixelData[pixelIndex + 3];
                }
            }

            return new WriteableBitmap(BitmapSource.Create(width, height, source.DpiX, source.DpiY,
                                       PixelFormats.Bgra32, null, resultData, stride));
        }

    }
}
