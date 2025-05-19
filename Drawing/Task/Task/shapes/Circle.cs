using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Text.Json.Serialization;
using System.Windows.Media.Imaging;

namespace Task.shapes
{
    public class Circle : Shapes
    {
        public Point Start { get; set; }
        public int Radius { get; set; } = 1;

        public override void Draw(WriteableBitmap bitmap)
        {
            if (UseAntialiasing)
            {
                AntiAliasing.DrawWuCircle(bitmap, Start, Radius, Color, Thickness);
            }
            else
            {
                DrawCircle(bitmap);
            }
        }


        private void DrawSymmetricPoints(WriteableBitmap bitmap, int cx, int cy, int x, int y)
        {
            DrawPixel(bitmap, cx + x, cy + y, Color, Thickness);
            DrawPixel(bitmap, cx - x, cy + y, Color, Thickness);
            DrawPixel(bitmap, cx + x, cy - y, Color, Thickness);
            DrawPixel(bitmap, cx - x, cy - y, Color, Thickness);
            DrawPixel(bitmap, cx + y, cy + x, Color, Thickness);
            DrawPixel(bitmap, cx - y, cy + x, Color, Thickness);
            DrawPixel(bitmap, cx + y, cy - x, Color, Thickness);
            DrawPixel(bitmap, cx - y, cy - x, Color, Thickness);
        }

        public void DrawCircle(WriteableBitmap bitmap)
        {
            int x0 = (int)Start.X;
            int y0 = (int)Start.Y;

            int x1 = x0 + Radius;
            int y1 = y0;

            int x = 0;
            int y = Radius;
            int d = 1 - Radius;

            DrawSymmetricPoints(bitmap, x0, y0, x, y);

            while (x <= y)
            {
                x++;
                if (d < 0)
                {
                    d += 2 * x + 1;
                }
                else
                {
                    y--;
                    d += 2 * (x - y) + 1;
                }

                DrawSymmetricPoints(bitmap, x0, y0, x, y);
            }
        }

        public override void DeleteShape(List<Shapes> shapes, Shapes cicleToDelete, WriteableBitmap bitmap)
        {
            shapes.Remove(cicleToDelete);
            RedrawAll(bitmap, shapes);
        }

        public override bool IsNearShape(Point p, double threshold = 5)
        {
            double distToCenter = (p - Start).Length;
            return Math.Abs(distToCenter - Radius) <= threshold;
        }
    }
}
