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

namespace Rasterization
{
    public class CustomLine
    {
        public Point Start { get; set; }
        public Point End { get; set; }
        public int Thickness { get; set; } = 1;
        public List<Rectangle> Pixels { get; set; } = new();

        public void Draw(Canvas canvas)
        {
            Pixels.Clear();

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


        public void Redraw(Canvas canvas)
        {
            Remove(canvas);
            Draw(canvas);
        }

        public void Remove(Canvas canvas)
        {
            foreach (var pixel in Pixels)
                canvas.Children.Remove(pixel);
            Pixels.Clear();
        }


        private void DrawPixel(Canvas canvas, int x, int y)
        {
            Rectangle rect = new Rectangle
            {
                Width = Thickness,
                Height = Thickness,
                Fill = Brushes.Black
            };
            Canvas.SetLeft(rect, x - Thickness / 2.0);
            Canvas.SetTop(rect, y - Thickness / 2.0);
            canvas.Children.Add(rect);
            Pixels.Add(rect);
        }


        public bool IsNearPoint(Point p1, Point p2, double radius = 20)
        {
            return (p1 - p2).Length <= radius;
        }

        public bool IsNearLine(Point p)
        {
            return Pixels.Any(px =>
            {
                double x = Canvas.GetLeft(px);
                double y = Canvas.GetTop(px);
                return Math.Abs(x - p.X) < 5 && Math.Abs(y - p.Y) < 5;
            });
        }

    }
}