using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Linq;
using System.Collections.Generic;
using System.Windows;


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

            int step = 256 / levels;
            int halfStep = step / 2;

            for (int i = 0; i < pixelData.Length; i += 4)
            {
                byte blue = pixelData[i];     // Blue
                byte green = pixelData[i + 1]; // Green
                byte red = pixelData[i + 2]; // Red

                byte qR = (byte)((red / step) * step + halfStep);
                byte qG = (byte)((green / step) * step + halfStep);
                byte qB = (byte)((blue / step) * step + halfStep);

                pixelData[i] = qB;
                pixelData[i + 1] = qG;
                pixelData[i + 2] = qR;
            }

            WriteableBitmap quantizedBitmap = new WriteableBitmap(sourceBitmap);
            quantizedBitmap.WritePixels(new System.Windows.Int32Rect(0, 0, width, height), pixelData, stride, 0);

            return quantizedBitmap;
        }

        private static Color FindClosestColor(Color color, List<Color> palette)
        {
            Color closestColor = palette[0];
            double minDistance = double.MaxValue;

            foreach (Color c in palette)
            {
                double distance = Math.Pow(color.R - c.R, 2) + Math.Pow(color.G - c.G, 2) + Math.Pow(color.B - c.B, 2);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestColor = c;
                }
            }

            return closestColor;
        }

        public static WriteableBitmap PopularityQuantization(WriteableBitmap sourceBitmap, int numColors)
        {
            int width = sourceBitmap.PixelWidth;
            int height = sourceBitmap.PixelHeight;
            int stride = width * 4;
            byte[] pixelData = new byte[height * stride];

            sourceBitmap.CopyPixels(pixelData, stride, 0);

            Dictionary<Color, int> colorFrequency = new Dictionary<Color, int>();
            int temp = 0;

            for (int i = 0; i < pixelData.Length; i += 4)
            {
                Color color = Color.FromRgb(pixelData[i + 2], pixelData[i + 1], pixelData[i]);

                if (colorFrequency.ContainsKey(color))
                {
                    colorFrequency[color]++;
                    temp++;
                }
                else
                    colorFrequency[color] = 1;
            }

            List<Color> mostPopularColors = colorFrequency
                .OrderByDescending(c => c.Value)
                .Take(numColors)
                .Select(c => c.Key)
                .ToList();

            for (int i = 0; i < pixelData.Length; i += 4)
            {
                Color originalColor = Color.FromRgb(pixelData[i + 2], pixelData[i + 1], pixelData[i]);
                Color closestColor = FindClosestColor(originalColor, mostPopularColors);

                pixelData[i] = closestColor.B;
                pixelData[i + 1] = closestColor.G;
                pixelData[i + 2] = closestColor.R;
            }

            WriteableBitmap quantizedBitmap = new WriteableBitmap(sourceBitmap);
            quantizedBitmap.WritePixels(new System.Windows.Int32Rect(0, 0, width, height), pixelData, stride, 0);

            return quantizedBitmap;
        }

    }
}
