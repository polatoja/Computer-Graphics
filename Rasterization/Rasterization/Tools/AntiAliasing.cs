using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;
using Rasterization.Tools;

namespace Rasterization.Tools
{
    public static class AntiAliasing
    {
        private static double Fractional(double x) => x - Math.Floor(x);
        private static double RFractional(double x) => 1 - Fractional(x);

        private static void PlotAA(Canvas canvas, double x, double y, double intensity, Color color, List<Rectangle> store)
        {
            Rectangle pixel = new Rectangle
            {
                Width = 1,
                Height = 1,
                Fill = new SolidColorBrush(Color.FromArgb((byte)(intensity * 255), color.R, color.G, color.B))
            };
            Canvas.SetLeft(pixel, (int)x);
            Canvas.SetTop(pixel, (int)y);
            canvas.Children.Add(pixel);
            store.Add(pixel);
        }

        public static void DrawWuLine(Canvas canvas, Point start, Point end, Color color, List<Rectangle> pixelStore)
        {
            double x0 = start.X;
            double y0 = start.Y;
            double x1 = end.X;
            double y1 = end.Y;

            bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
            if (steep)
            {
                (x0, y0) = (y0, x0);
                (x1, y1) = (y1, x1);
            }

            if (x0 > x1)
            {
                (x0, x1) = (x1, x0);
                (y0, y1) = (y1, y0);
            }

            double dx = x1 - x0;
            double dy = y1 - y0;
            double gradient = dx == 0 ? 1 : dy / dx;

            double xEnd = Math.Round(x0);
            double yEnd = y0 + gradient * (xEnd - x0);
            double xGap = RFractional(x0 + 0.5);
            double xPx1 = xEnd;
            double yPx1 = Math.Floor(yEnd);

            if (steep)
            {
                PlotAA(canvas, yPx1, xPx1, RFractional(yEnd) * xGap, color, pixelStore);
                PlotAA(canvas, yPx1 + 1, xPx1, Fractional(yEnd) * xGap, color, pixelStore);
            }
            else
            {
                PlotAA(canvas, xPx1, yPx1, RFractional(yEnd) * xGap, color, pixelStore);
                PlotAA(canvas, xPx1, yPx1 + 1, Fractional(yEnd) * xGap, color, pixelStore);
            }

            double intery = yEnd + gradient;

            xEnd = Math.Round(x1);
            yEnd = y1 + gradient * (xEnd - x1);
            xGap = Fractional(x1 + 0.5);
            double xPx2 = xEnd;
            double yPx2 = Math.Floor(yEnd);

            if (steep)
            {
                PlotAA(canvas, yPx2, xPx2, RFractional(yEnd) * xGap, color, pixelStore);
                PlotAA(canvas, yPx2 + 1, xPx2, Fractional(yEnd) * xGap, color, pixelStore);
            }
            else
            {
                PlotAA(canvas, xPx2, yPx2, RFractional(yEnd) * xGap, color, pixelStore);
                PlotAA(canvas, xPx2, yPx2 + 1, Fractional(yEnd) * xGap, color, pixelStore);
            }

            if (steep)
            {
                for (int x = (int)(xPx1 + 1); x < xPx2; x++)
                {
                    PlotAA(canvas, Math.Floor(intery), x, RFractional(intery), color, pixelStore);
                    PlotAA(canvas, Math.Floor(intery) + 1, x, Fractional(intery), color, pixelStore);
                    intery += gradient;
                }
            }
            else
            {
                for (int x = (int)(xPx1 + 1); x < xPx2; x++)
                {
                    PlotAA(canvas, x, Math.Floor(intery), RFractional(intery), color, pixelStore);
                    PlotAA(canvas, x, Math.Floor(intery) + 1, Fractional(intery), color, pixelStore);
                    intery += gradient;
                }
            }
        }

        public static void DrawWuCircle(Canvas canvas, Point center, int radius, Color color, List<Rectangle> pixelStore)
        {
            int x0 = (int)center.X;
            int y0 = (int)center.Y;

            void PlotCirclePoints(double cx, double cy, double x, double y, double intensity)
            {
                PlotAA(canvas, cx + x, cy + y, intensity, color, pixelStore);
                PlotAA(canvas, cx - x, cy + y, intensity, color, pixelStore);
                PlotAA(canvas, cx + x, cy - y, intensity, color, pixelStore);
                PlotAA(canvas, cx - x, cy - y, intensity, color, pixelStore);
                PlotAA(canvas, cx + y, cy + x, intensity, color, pixelStore);
                PlotAA(canvas, cx - y, cy + x, intensity, color, pixelStore);
                PlotAA(canvas, cx + y, cy - x, intensity, color, pixelStore);
                PlotAA(canvas, cx - y, cy - x, intensity, color, pixelStore);
            }

            for (double theta = 0; theta < Math.PI / 4; theta += 0.001)
            {
                double x = radius * Math.Cos(theta);
                double y = radius * Math.Sin(theta);

                double xFloor = Math.Floor(x);
                double yFloor = Math.Floor(y);

                double fx = Fractional(x);
                double fy = Fractional(y);

                PlotCirclePoints(x0, y0, xFloor, yFloor, (1 - fx) * (1 - fy));
                PlotCirclePoints(x0, y0, xFloor + 1, yFloor, fx * (1 - fy));
                PlotCirclePoints(x0, y0, xFloor, yFloor + 1, (1 - fx) * fy);
                PlotCirclePoints(x0, y0, xFloor + 1, yFloor + 1, fx * fy);
            }
        }

    }

}
