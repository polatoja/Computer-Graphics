using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Image_filtering
{
    public partial class HSVWindow : Window
    {
        public HSVWindow(WriteableBitmap h, WriteableBitmap s, WriteableBitmap v, WriteableBitmap rgb)
        {
            InitializeComponent();
            HueImage.Source = h;
            SatImage.Source = s;
            ValImage.Source = v;
            RGBImage.Source = rgb;
        }

        public static WriteableBitmap ConvertToHSV(WriteableBitmap sourceBitmap, string channel)
        {
            int width = sourceBitmap.PixelWidth;
            int height = sourceBitmap.PixelHeight;
            int stride = width * 4;
            byte[] pixelData = new byte[height * stride];

            sourceBitmap.CopyPixels(pixelData, stride, 0);

            for (int i = 0; i < pixelData.Length; i += 4)
            {
                byte blue = pixelData[i];
                byte green = pixelData[i + 1];
                byte red = pixelData[i + 2];

                double qR = red / 255.0;
                double qG = green / 255.0;
                double qB = blue / 255.0;

                double max = Math.Max(qR, Math.Max(qG, qB));
                double min = Math.Min(qR, Math.Min(qG, qB));
                double delta = max - min;


                double h = 0;
                if (delta != 0)
                {
                    if (max == qR)
                        h = (60 * ((qG - qB) / delta) + 360) % 360;
                    else if (max == qG)
                        h = (60 * ((qB - qR) / delta) + 120) % 360;
                    else
                        h = (60 * ((qR - qG) / delta) + 240) % 360;

                }
                if (h < 0) h += 360;

                double s = (max == 0) ? 0 : (delta / max);
                double v = max;

                byte outputValue;
                switch (channel.ToUpper())
                {
                    case "H":
                        outputValue = (byte)(h / 360.0 * 255);
                        break;
                    case "S":
                        outputValue = (byte)(s * 255);
                        break;
                    case "V":
                        outputValue = (byte)(v * 255);
                        break;
                    default:
                        outputValue = 0;
                        break;
                }


                pixelData[i] = pixelData[i + 1] = pixelData[i + 2] = outputValue;
                pixelData[i + 3] = 255;
            }

            WriteableBitmap resultBitmap = new WriteableBitmap(sourceBitmap);
            resultBitmap.WritePixels(new System.Windows.Int32Rect(0, 0, width, height), pixelData, stride, 0);

            return resultBitmap;
        }

        public static WriteableBitmap MergeHSVToRGB(WriteableBitmap hueImage, WriteableBitmap satImage, WriteableBitmap valImage)
        {
            int width = hueImage.PixelWidth;
            int height = hueImage.PixelHeight;
            int stride = width * 4;

            byte[] hData = new byte[height * stride];
            byte[] sData = new byte[height * stride];
            byte[] vData = new byte[height * stride];
            byte[] rgbData = new byte[height * stride];

            hueImage.CopyPixels(hData, stride, 0);
            satImage.CopyPixels(sData, stride, 0);
            valImage.CopyPixels(vData, stride, 0);

            for (int i = 0; i < rgbData.Length; i += 4)
            {
                double h = hData[i] / 255.0 * 360;
                double s = sData[i] / 255.0;
                double v = vData[i] / 255.0;

                double c = v * s;
                double x = c * (1 - Math.Abs((h / 60) % 2 - 1));
                double m = v - c;

                double r = 0, g = 0, b = 0;

                if (h >= 0 && h < 60) { r = c; g = x; b = 0; }
                else if (h >= 60 && h < 120) { r = x; g = c; b = 0; }
                else if (h >= 120 && h < 180) { r = 0; g = c; b = x; }
                else if (h >= 180 && h < 240) { r = 0; g = x; b = c; }
                else if (h >= 240 && h < 300) { r = x; g = 0; b = c; }
                else { r = c; g = 0; b = x; }

                rgbData[i] = (byte)((b + m) * 255);
                rgbData[i + 1] = (byte)((g + m) * 255);
                rgbData[i + 2] = (byte)((r + m) * 255);
                rgbData[i + 3] = 255;
            }

            WriteableBitmap resultBitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
            resultBitmap.WritePixels(new System.Windows.Int32Rect(0, 0, width, height), rgbData, stride, 0);

            return resultBitmap;
        }

    }
}
