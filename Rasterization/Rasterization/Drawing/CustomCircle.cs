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

namespace Rasterization.Drawing
{
    public class CustomCircle
    {
        public Point Start { get; set; }
        public int Radius { get; set; } = 1;
        public int Thickness { get; set; } = 1;
        public List<Rectangle> Pixels { get; set; } = new();

        public void DrawCircle(Canvas canvas)
        {
            Pixels.Clear();

            int x0 = (int)Start.X;
            int y0 = (int)Start.Y;

            int x1 = x0 + Radius;
            int y1 = y0;

            int x = 0;
            int y = Radius;
            int d = 1 - Radius;

            DrawSymmetricPoints(canvas, x0, y0, x, y);

            while (x < y)
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

                DrawSymmetricPoints(canvas, x0, y0, x, y);
            }
        }

        public void RedrawCircle(Canvas canvas)
        {
            RemoveCircle(canvas);
            DrawCircle(canvas);
        }

        public void RemoveCircle(Canvas canvas)
        {
            foreach (var pixel in Pixels)
                canvas.Children.Remove(pixel);
            Pixels.Clear();
        }
        private void DrawSymmetricPoints(Canvas canvas, int cx, int cy, int x, int y)
        {
            DrawPixel(canvas, cx + x, cy + y);
            DrawPixel(canvas, cx - x, cy + y);
            DrawPixel(canvas, cx + x, cy - y);
            DrawPixel(canvas, cx - x, cy - y);
            DrawPixel(canvas, cx + y, cy + x);
            DrawPixel(canvas, cx - y, cy + x);
            DrawPixel(canvas, cx + y, cy - x);
            DrawPixel(canvas, cx - y, cy - x);
        }

        private void DrawPixel(Canvas canvas, int x, int y)
        {
            int half = Thickness / 2;

            for (int dx = -half; dx <= half; dx++)
            {
                for (int dy = -half; dy <= half; dy++)
                {
                    Rectangle rect = new Rectangle
                    {
                        Width = 1,
                        Height = 1,
                        Fill = Brushes.Black
                    };
                    Canvas.SetLeft(rect, x + dx);
                    Canvas.SetTop(rect, y + dy);
                    canvas.Children.Add(rect);
                    Pixels.Add(rect);
                }
            }
        }

        public bool IsNearCircle(Point p)
        {
            return Pixels.Any(px =>
            {
                double x = Canvas.GetLeft(px);
                double y = Canvas.GetTop(px);
                return Math.Abs(x - p.X) < 10 && Math.Abs(y - p.Y) < 10;
            });
        }
        public void SetRadiusFromPoint(Point outerPoint)
        {
            Radius = (int)(outerPoint - Start).Length;
        }

    }
}
