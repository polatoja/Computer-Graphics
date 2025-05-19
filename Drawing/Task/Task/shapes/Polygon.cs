using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Text.Json.Serialization;
using System.Windows.Media.Imaging;
using System.Collections;

namespace Task.shapes
{
    public class Polygon : Shapes
    {
        public List<Point> Vertices { get; set; } = new();
        public int numVertices { get; set; } = 1;

        public int? selectedVertexIndex = null;
        public int? selectedEdgeIndex = null;
        public Color? FillColor = null;
        public BitmapImage? FillImage { get; set; }



        public override void Draw(WriteableBitmap bitmap)
        {
            if (UseAntialiasing)
            {
                
                for (int i = 0; i < numVertices - 1; i++)
                    AntiAliasing.DrawWuLine(bitmap, Vertices[i], Vertices[i + 1], Color, Thickness);
                if (Vertices.Count > 2)
                    AntiAliasing.DrawWuLine(bitmap, Vertices[^1], Vertices[0], Color, Thickness);

            }
            else
            {
                for(int i = 0; i < numVertices - 1; i++)
                    DrawLine(bitmap, Vertices[i], Vertices[i + 1]);
                if (Vertices.Count > 2)
                    DrawLine(bitmap, Vertices[^1], Vertices[0]);

                if (FillColor.HasValue || FillImage != null)
                    FillPolygon(bitmap);
            }
        }

        public override void DeleteShape(List<Shapes> shapes, Shapes lineToDelete, WriteableBitmap bitmap)
        {
            shapes.Remove(lineToDelete);
            RedrawAll(bitmap, shapes);
        }
        public double DistanceToSegment(Point p, Point a, Point b)
        {
            double dx = b.X - a.X;
            double dy = b.Y - a.Y;

            if (dx == 0 && dy == 0)
                return (p - a).Length;

            double t = ((p.X - a.X) * dx + (p.Y - a.Y) * dy) / (dx * dx + dy * dy);
            t = Math.Max(0, Math.Min(1, t));

            Point projection = new(a.X + t * dx, a.Y + t * dy);
            return (p - projection).Length;
        }

        public override bool IsNearShape(Point p, double threshold = 5)
        {
            for (int i = 0; i < Vertices.Count - 1; i++)
            {
                if (DistanceToSegment(p, Vertices[i], Vertices[i + 1]) <= threshold)
                    return true;
            }

            if (Vertices.Count > 2 && DistanceToSegment(p, Vertices[^1], Vertices[0]) <= threshold)
                return true;

            return false;
        }

        private void FillPolygon(WriteableBitmap bitmap)
        {
            if (Vertices.Count < 3) return;

            var indices = Enumerable.Range(0, Vertices.Count).OrderBy(i => Vertices[i].Y).ToArray();
            int ymin = (int)Vertices[indices[0]].Y;
            int ymax = (int)Vertices[indices[^1]].Y;

            List<(double x, double invM, int ymax)> AET = new();
            int k = 0;

            for (int y = ymin; y < ymax; y++)
            {
                while (k < indices.Length && (int)Vertices[indices[k]].Y == y)
                {
                    int i = indices[k];
                    int prev = (i - 1 + Vertices.Count) % Vertices.Count;
                    int next = (i + 1) % Vertices.Count;

                    if (Vertices[prev].Y > Vertices[i].Y)
                    {
                        double invM = (Vertices[prev].X - Vertices[i].X) / (Vertices[prev].Y - Vertices[i].Y);
                        AET.Add((Vertices[i].X, invM, (int)Vertices[prev].Y));
                    }

                    if (Vertices[next].Y > Vertices[i].Y)
                    {
                        double invM = (Vertices[next].X - Vertices[i].X) / (Vertices[next].Y - Vertices[i].Y);
                        AET.Add((Vertices[i].X, invM, (int)Vertices[next].Y));
                    }

                    k++;
                }

                AET.RemoveAll(e => e.ymax == y);
                AET.Sort((a, b) => a.x.CompareTo(b.x));

                for (int i = 0; i < AET.Count; i += 2)
                {
                    int xStart = (int)Math.Round(AET[i].x);
                    int xEnd = (int)Math.Round(AET[i + 1].x);

                    for (int x = xStart; x <= xEnd; x++)
                    {
                        if (FillImage != null)
                        {
                            Color sampledColor = SampleImage(x, y);
                            DrawPixel(bitmap, x, y, sampledColor, 1);
                        }
                        else if (FillColor.HasValue)
                        {
                            DrawPixel(bitmap, x, y, FillColor.Value, Thickness);
                        }
                    }
                }

                for (int i = 0; i < AET.Count; i++)
                {
                    AET[i] = (AET[i].x + AET[i].invM, AET[i].invM, AET[i].ymax);
                }
            }
        }

        private Color SampleImage(int x, int y)
        {
            if (FillImage == null) return Colors.Transparent;

            try
            {
                // Scale image over polygon's bounding box
                int minX = (int)Vertices.Min(p => p.X);
                int maxX = (int)Vertices.Max(p => p.X);
                int minY = (int)Vertices.Min(p => p.Y);
                int maxY = (int)Vertices.Max(p => p.Y);

                int imgX = (int)((x - minX) / (double)(maxX - minX) * FillImage.PixelWidth);
                int imgY = (int)((y - minY) / (double)(maxY - minY) * FillImage.PixelHeight);

                if (imgX < 0 || imgX >= FillImage.PixelWidth || imgY < 0 || imgY >= FillImage.PixelHeight)
                    return Colors.Transparent;

                var wb = new WriteableBitmap(FillImage);
                int stride = wb.BackBufferStride;
                byte[] pixels = new byte[4];
                Int32Rect rect = new Int32Rect(imgX, imgY, 1, 1);
                wb.CopyPixels(rect, pixels, 4, 0);
                return Color.FromArgb(255, pixels[2], pixels[1], pixels[0]);
            }
            catch
            {
                return Colors.Transparent;
            }
        }

    }
}
