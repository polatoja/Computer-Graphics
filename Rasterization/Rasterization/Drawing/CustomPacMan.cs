using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Rasterization.Tools;

namespace Rasterization.Drawing
{
    public class CustomPacMan
    {
        public Point Center { get; set; }
        public Point Point1 { get; set; }
        public Point Point2 { get; set; }
        [JsonIgnore]
        public List<Rectangle> Pixels { get; set; } = new();

        public void Draw(Canvas canvas)
        {
            Pixels.Clear();

            double radius = Distance(Center, Point1);
            double angle1 = NormalizeAngle(Math.Atan2(Point2.Y - Center.Y, Point2.X - Center.X));
            double angle2 = NormalizeAngle(Math.Atan2(Point1.Y - Center.Y, Point1.X - Center.X));

            if (angle1 <= angle2)
                angle1 += 2 * Math.PI;

            DrawCircleArc(canvas, Center, radius, angle1, angle2);

            DrawLine(canvas, Center, Point1);
            DrawLine(canvas, Center, Point2);
        }

        private double Distance(Point a, Point b)
        {
            return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }

        private double NormalizeAngle(double angle)
        {
            while (angle < 0) angle += 2 * Math.PI;
            while (angle >= 2 * Math.PI) angle -= 2 * Math.PI;
            return angle;
        }

        private bool IsAngleBetween(double angle, double start, double end)
        {
            angle = NormalizeAngle(angle);
            start = NormalizeAngle(start);
            end = NormalizeAngle(end);

            if (start < end)
                return angle >= start && angle <= end;
            else
                return angle >= start || angle <= end;
        }

        private void DrawPixel(Canvas canvas, int x, int y)
        {
            Rectangle rect = new Rectangle
            {
                Width = 1,
                Height = 1,
                Fill = System.Windows.Media.Brushes.Black
            };

            Canvas.SetLeft(rect, x);
            Canvas.SetTop(rect, y);
            canvas.Children.Add(rect);
            Pixels.Add(rect);
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

        public void DrawCircleArc(Canvas canvas, Point center, double radius, double angleStart, double angleEnd)
        {
            Pixels.Clear();

            int x0 = (int)center.X;
            int y0 = (int)center.Y;

            int x = 0;
            double y = radius;
            double d = 1 - radius;

            while (x <= y)
            {
                TryDrawSymmetricPoints(canvas, x0, y0, x, y, radius, angleStart, angleEnd);

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
            }
        }

        private void TryDrawSymmetricPoints(Canvas canvas, double x0, double y0, double x, double y, double radius, double aStart, double aEnd)
        {
            Point[] points = new Point[]
            {
                new Point(x0 + x, y0 + y),
                new Point(x0 - x, y0 + y),
                new Point(x0 + x, y0 - y),
                new Point(x0 - x, y0 - y),
                new Point(x0 + y, y0 + x),
                new Point(x0 - y, y0 + x),
                new Point(x0 + y, y0 - x),
                new Point(x0 - y, y0 - x),
            };

            foreach (var p in points)
            {
                double angle = NormalizeAngle(Math.Atan2(p.Y - y0, p.X - x0));

                if (IsAngleBetween(angle, aStart, aEnd))
                {
                    DrawPixel(canvas, (int)p.X, (int)p.Y);
                }

            }
        }

    }
}
