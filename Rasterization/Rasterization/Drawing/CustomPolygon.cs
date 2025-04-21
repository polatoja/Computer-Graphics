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
    public class CustomPolygon
    {
        public List<Point> Vertices { get; set; } = new();
        public int numVertices { get; set; } = 1;
        public int Thickness { get; set; } = 1;
        public List<Rectangle> Pixels { get; set; } = new();

        public void DrawPolygon(Canvas canvas)
        {
            Pixels.Clear();

            for (int i = 0; i < numVertices; i++)
            {
                if (i + 1 < numVertices)
                    DrawLine(canvas, Vertices[i], Vertices[i + 1]);
                else
                    DrawLine(canvas, Vertices[0], Vertices[i]);
            }
        }
        public void DrawLine(Canvas canvas, Point Start, Point End)
        {
            int x0 = (int)Start.X;
            int y0 = (int)Start.Y;
            int x1 = (int)End.X;
            int y1 = (int)End.Y;

            int dx = x1 - x0;
            int dy = y1 - y0;

            int x = x0;
            int y = y0;

            int xStep = dx >= 0 ? 1 : -1;
            int yStep = dy >= 0 ? 1 : -1;

            dx = Math.Abs(dx);
            dy = Math.Abs(dy);

            if (dx >= dy)
            {
                int d = 2 * dy - dx;
                int incrE = 2 * dy;
                int incrNE = 2 * (dy - dx);

                DrawPixel(canvas, x, y);

                for (int i = 0; i < dx; i++)
                {
                    if (d <= 0)
                    {
                        d += incrE;
                        x += xStep;
                    }
                    else
                    {
                        d += incrNE;
                        x += xStep;
                        y += yStep;
                    }
                    DrawPixel(canvas, x, y);
                }
            }
            else
            {
                int d = 2 * dx - dy;
                int incrN = 2 * dx;
                int incrNE = 2 * (dx - dy);

                DrawPixel(canvas, x, y);

                for (int i = 0; i < dy; i++)
                {
                    if (d <= 0)
                    {
                        d += incrN;
                        y += yStep;
                    }
                    else
                    {
                        d += incrNE;
                        x += xStep;
                        y += yStep;
                    }
                    DrawPixel(canvas, x, y);
                }
            }
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
        public void RedrawPolygon(Canvas canvas)
        {
            RemovePolygon(canvas);
            DrawPolygon(canvas);
        }

        public void RemovePolygon(Canvas canvas)
        {
            foreach (var pixel in Pixels)
                canvas.Children.Remove(pixel);
            Pixels.Clear();
        }

        public bool IsNearPolygon(Point p)
        {
            return Pixels.Any(px =>
            {
                double x = Canvas.GetLeft(px);
                double y = Canvas.GetTop(px);
                return Math.Abs(x - p.X) < 10 && Math.Abs(y - p.Y) < 10;
            });
        }
    }
}
