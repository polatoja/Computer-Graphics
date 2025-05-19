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
using System.IO;

namespace Task.shapes
{
    public class Polygon : Shapes
    {
        public List<Point> Vertices { get; set; } = new();
        public int numVertices { get; set; } = 1;

        public int? selectedVertexIndex = null;
        public int? selectedEdgeIndex = null;
        public Color? FillColor = null;
        [JsonIgnore]
        public BitmapImage? FillImage { get; set; }
        [JsonIgnore]
        public Rectangle? clippedRectangle = null;

        private string? _fillImageUri;

        public string? FillImageUri
        {
            get => _fillImageUri;
            set
            {
                _fillImageUri = value;
                if (!string.IsNullOrEmpty(value) && File.Exists(value))
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.UriSource = new Uri(value, UriKind.Absolute);
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.EndInit();
                    FillImage = image;
                }
                else
                {
                    FillImage = null;
                }
            }
        }


        private class Edge
        {
            public int YMax;
            public double X;
            public double InvSlope;
        }

        public override void Draw(WriteableBitmap bitmap)
        {
            if (UseAntialiasing)
            {
                
                for (int i = 0; i < numVertices - 1; i++)
                    AntiAliasing.DrawWuLine(bitmap, Vertices[i], Vertices[i + 1], Color, Thickness);
                if (Vertices.Count > 2)
                    AntiAliasing.DrawWuLine(bitmap, Vertices[^1], Vertices[0], Color, Thickness);
                if (FillColor.HasValue || FillImage != null)
                    FillPolygon(bitmap);

                if (clippedRectangle != null)
                {
                    var clipper = new Clipping();
                    clipper.DrawClippedSegments(bitmap, this);
                }
            }
            else
            {
                for(int i = 0; i < numVertices - 1; i++)
                    DrawLine(bitmap, Vertices[i], Vertices[i + 1], Color, Thickness);
                if (Vertices.Count > 2)
                    DrawLine(bitmap, Vertices[^1], Vertices[0], Color, Thickness);

                if (FillColor.HasValue || FillImage != null)
                    FillPolygon(bitmap);

                if (clippedRectangle != null)
                {
                    var clipper = new Clipping();
                    clipper.DrawClippedSegments(bitmap, this);
                }
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

        public void FillPolygon(WriteableBitmap bitmap)
        {
            if (Vertices.Count < 3) return;

            int width = bitmap.PixelWidth;
            int height = bitmap.PixelHeight;

            WriteableBitmap? texture = null;
            byte[]? imagePixels = null;
            int imageStride = 0;

            if (FillImage != null)
            {
                texture = new WriteableBitmap(FillImage);
                imageStride = texture.PixelWidth * 4;
                imagePixels = new byte[texture.PixelHeight * imageStride];
                texture.CopyPixels(imagePixels, imageStride, 0);
            }

            Dictionary<int, List<Edge>> AET = new();
            for (int i = 0; i < Vertices.Count; i++)
            {
                Point a = Vertices[i];
                Point b = Vertices[(i + 1) % Vertices.Count];

                if (a.Y == b.Y) continue;

                Point upper = a.Y < b.Y ? a : b;
                Point lower = a.Y < b.Y ? b : a;

                int yMin = (int)Math.Ceiling(upper.Y);
                int yMax = (int)Math.Ceiling(lower.Y);
                double x = upper.X;
                double invSlope = (lower.X - upper.X) / (lower.Y - upper.Y);

                if (!AET.ContainsKey(yMin))
                    AET[yMin] = new List<Edge>();

                AET[yMin].Add(new Edge { YMax = yMax, X = x, InvSlope = invSlope });
            }

            int scanY = AET.Keys.Min();
            List<Edge> activeEdges = new();

            bitmap.Lock();

            while (AET.Count > 0 || activeEdges.Count > 0)
            {
                if (AET.TryGetValue(scanY, out var newEdges))
                {
                    activeEdges.AddRange(newEdges);
                    AET.Remove(scanY);
                }
                activeEdges.RemoveAll(e => e.YMax <= scanY);
                activeEdges.Sort((a, b) => a.X.CompareTo(b.X));

                for (int i = 0; i < activeEdges.Count - 1; i += 2)
                {
                    int xStart = (int)Math.Round(activeEdges[i].X);
                    int xEnd = (int)Math.Round(activeEdges[i + 1].X);

                    for (int x = xStart; x < xEnd; x++)
                    {
                        if (x < 0 || x >= width || scanY < 0 || scanY >= height)
                            continue;

                        Color fill = FillColor ?? Color;

                        if (texture != null && imagePixels != null)
                        {
                            int fx = x % texture.PixelWidth;
                            int fy = scanY % texture.PixelHeight;
                            int pixelIndex = fy * imageStride + fx * 4;

                            if (pixelIndex + 2 < imagePixels.Length)
                            {
                                fill = Color.FromRgb(imagePixels[pixelIndex + 2], imagePixels[pixelIndex + 1], imagePixels[pixelIndex + 0]);
                            }
                        }

                        unsafe
                        {
                            byte* pPixel = (byte*)bitmap.BackBuffer + scanY * bitmap.BackBufferStride + x * 4;
                            pPixel[0] = fill.B;
                            pPixel[1] = fill.G;
                            pPixel[2] = fill.R;
                            pPixel[3] = 255;
                        }
                    }
                }

                foreach (var edge in activeEdges)
                    edge.X += edge.InvSlope;

                scanY++;
            }

            bitmap.AddDirtyRect(new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight));
            bitmap.Unlock();
        }

    }
}
