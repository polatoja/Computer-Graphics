using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Task.shapes
{
    public abstract class Shapes
    {
        public int Thickness { get; set; } = 1;
        public Color Color { get; set; } = Colors.Black;
        public bool UseAntialiasing { get; set; } = false;

        public Point? lastMousePos = null;

        public abstract void Draw(WriteableBitmap bitmap);
        public abstract void DeleteShape(List<Shapes> shapes, Shapes shapeToDelete, WriteableBitmap bitmap);
        public abstract bool IsNearShape(Point p, double threshold = 5);
        public static bool[,] CreateCircularBrush(int thickness)
        {
            int n = thickness;
            bool[,] brush = new bool[n, n];
            int center = n / 2;

            for (int y = 0; y < n; y++)
            {
                for (int x = 0; x < n; x++)
                {
                    int dx = x - center;
                    int dy = y - center;
                    if (dx * dx + dy * dy <= center * center)
                    {
                        brush[x, y] = true;
                    }
                }
            }

            return brush;
        }
        public static void DrawPixel(WriteableBitmap bmp, int x, int y, Color color, int thickness = 1)
        {
            if (x < 0 || x >= bmp.PixelWidth || y < 0 || y >= bmp.PixelHeight) return;

            bool[,] brush = CreateCircularBrush(thickness);
            int size = brush.GetLength(0);
            int offset = size / 2;

            bmp.Lock();
            unsafe
            {
                IntPtr buffer = bmp.BackBuffer;
                int stride = bmp.BackBufferStride;

                for (int dy = 0; dy < size; dy++)
                {
                    for (int dx = 0; dx < size; dx++)
                    {
                        if (!brush[dx, dy]) continue;
                        int px = x + dx - offset;
                        int py = y + dy - offset;

                        if (px >= 0 && px < bmp.PixelWidth && py >= 0 && py < bmp.PixelHeight)
                        {
                            IntPtr pixel = buffer + py * stride + px * 4;
                            int colorData = color.A << 24 | color.R << 16 | color.G << 8 | color.B;
                            *((int*)pixel) = colorData;
                        }
                    }
                }
            }
            bmp.AddDirtyRect(new Int32Rect(x - offset, y - offset, thickness, thickness));
            bmp.Unlock();
        }

        public void DrawLine(WriteableBitmap bitmap, Point Start, Point End)
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

                DrawPixel(bitmap, x, y, Color, Thickness);

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
                    DrawPixel(bitmap, x, y, Color, Thickness);
                }
            }
            else
            {
                int d = 2 * dx - dy;
                int incrN = 2 * dx;
                int incrNE = 2 * (dx - dy);

                DrawPixel(bitmap, x, y, Color, Thickness);

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
                    DrawPixel(bitmap, x, y, Color, Thickness);
                }
            }
        }

        public static void RedrawAll(WriteableBitmap bitmap, List<Shapes> shapes)
        {
            bitmap.Lock();
            int stride = bitmap.BackBufferStride;
            int size = stride * bitmap.PixelHeight;
            byte[] white = new byte[size];
            for (int i = 0; i < size; i += 4)
            {
                white[i] = 255; white[i + 1] = 255; white[i + 2] = 255; white[i + 3] = 255;
            }
            bitmap.WritePixels(new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight), white, stride, 0);
            bitmap.Unlock();

            foreach (var shape in shapes)
                shape.Draw(bitmap);
        }

        public bool IsNearPoint(Point p1, Point p2, double radius = 20)
        {
            return (p1 - p2).Length <= radius;
        }

    }
}
