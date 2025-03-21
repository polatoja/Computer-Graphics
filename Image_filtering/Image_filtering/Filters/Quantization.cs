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
using System.Windows.Controls.Primitives;


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

            for (int i = 0; i < pixelData.Length; i += 4)
            {
                Color color = Color.FromRgb(pixelData[i + 2], pixelData[i + 1], pixelData[i]);

                if (colorFrequency.ContainsKey(color))
                    colorFrequency[color]++;
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

        private static (List<(byte, byte, byte)>, List<(byte, byte, byte)>) DivideIntoParts(List<(byte, byte, byte)> list)
        {
            byte minR = 255, maxR = 0;
            byte minG = 255, maxG = 0;
            byte minB = 255, maxB = 0;

            foreach (var (r, g, b) in list)
            {
                if (r < minR) minR = r; if (r > maxR) maxR = r;
                if (g < minG) minG = g; if (g > maxG) maxG = g;
                if (b < minB) minB = b; if (b > maxB) maxB = b;
            }

            int rangeR = maxR - minR;
            int rangeG = maxG - minG;
            int rangeB = maxB - minB;

            Func<(byte, byte, byte), byte> selector = rangeR >= rangeG && rangeR >= rangeB
            ? c => c.Item3 : (rangeG >= rangeB ? c => c.Item2 : c => c.Item1);

            var sorted = list.OrderBy(selector).ToList();
            int mid = sorted.Count / 2;

            return (sorted.Take(mid).ToList(), sorted.Skip(mid).ToList());
        }

        private static (byte, byte, byte) GetAverage(List<(byte, byte, byte)> box)
        {
            long sumR = 0, sumG = 0, sumB = 0;
            foreach (var (r, g, b) in box)
            {
                sumR += r;
                sumG += g;
                sumB += b;
            }

            int count = box.Count;
            return ((byte)(sumR / count), (byte)(sumG / count), (byte)(sumB / count));
        }

        private static int DistanceSquared((byte, byte, byte) a, (byte, byte, byte) b)
        {
            int dr = a.Item1 - b.Item1;
            int dg = a.Item2 - b.Item2;
            int db = a.Item3 - b.Item3;
            return dr * dr + dg * dg + db * db;
        }

        public static WriteableBitmap MedianCutQuantization(WriteableBitmap sourceBitmap, int numColors)
        {
            int width = sourceBitmap.PixelWidth;
            int height = sourceBitmap.PixelHeight;
            int stride = width * 4;
            byte[] pixelData = new byte[height * stride];

            sourceBitmap.CopyPixels(pixelData, stride, 0);

            List<(byte, byte, byte)> allColors = new();
            for (int i = 0; i < pixelData.Length; i += 4)
            {
                allColors.Add((pixelData[i], pixelData[i + 1], pixelData[i + 2]));
            }

            List<List<(byte, byte, byte)>> dividedCubes = new() { allColors };


            while (dividedCubes.Count < numColors)
            {
                var boxToSplit = dividedCubes.OrderByDescending(b => b.Count).First();
                dividedCubes.Remove(boxToSplit);

                var (cube1, cube2) = DivideIntoParts(boxToSplit);
                dividedCubes.Add(cube1);
                dividedCubes.Add(cube2);
            }

            var palette = dividedCubes.Select(GetAverage).ToList();

            for (int i = 0; i < pixelData.Length; i += 4)
            {
                var original = (pixelData[i], pixelData[i + 1], pixelData[i + 2]);
                var closest = palette.OrderBy(p => DistanceSquared(original, p)).First();

                pixelData[i] = closest.Item1;
                pixelData[i + 1] = closest.Item2;
                pixelData[i + 2] = closest.Item3;
            }

            WriteableBitmap quantizedBitmap = new WriteableBitmap(sourceBitmap);
            quantizedBitmap.WritePixels(new System.Windows.Int32Rect(0, 0, width, height), pixelData, stride, 0);

            return quantizedBitmap;
        }
    }
}
