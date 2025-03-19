using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Image_filtering.Filters
{
    public static class Quantization
    {
        public static WriteableBitmap UniformColorQuantization(WriteableBitmap sourceBitmap, int levels)
        {
            int width = sourceBitmap.PixelWidth;
            int height = sourceBitmap.PixelHeight;
            int stride = width * 4;
            byte[] pixelData = new byte[height * stride];

            sourceBitmap.CopyPixels(pixelData, stride, 0);

            int step = 256 / levels; // Bin size
            int halfStep = step / 2; // To map values to the center of bins

            for (int i = 0; i < pixelData.Length; i += 4)
            {
                // Extract original colors
                byte b = pixelData[i];     // Blue
                byte g = pixelData[i + 1]; // Green
                byte r = pixelData[i + 2]; // Red

                // Quantize each channel
                byte qR = (byte)((r / step) * step + halfStep);
                byte qG = (byte)((g / step) * step + halfStep);
                byte qB = (byte)((b / step) * step + halfStep);

                // Assign quantized colors
                pixelData[i] = qB;
                pixelData[i + 1] = qG;
                pixelData[i + 2] = qR;
            }

            // Create new bitmap with the quantized colors
            WriteableBitmap quantizedBitmap = new WriteableBitmap(sourceBitmap);
            quantizedBitmap.WritePixels(new System.Windows.Int32Rect(0, 0, width, height), pixelData, stride, 0);

            return quantizedBitmap;
        }

    }
}
