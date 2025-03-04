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

    }
}
