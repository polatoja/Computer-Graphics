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
using System.Collections;
using System.Text.Json.Serialization;

namespace Rasterization.Drawing
{
    public class CustomPolygon
    {
        public List<Point> Vertices { get; set; } = new();
        public int numVertices { get; set; } = 1;
        public int Thickness { get; set; } = 1;
        public Color myColor { get; set; } = Colors.Black;

        [JsonIgnore]
        public Color? FillColor { get; set; }

        [JsonPropertyName("FillColor")]
        public string FillColorString
        {
            get => FillColor?.ToString();
            set => FillColor = string.IsNullOrWhiteSpace(value)
                ? null
                : (Color?)ColorConverter.ConvertFromString(value);
        }



        public bool UseAntialiasing { get; set; } = false;

        public string? FillImageUri { get; set; } = null;

        [JsonIgnore]
        public List<Rectangle> OutlinePixels { get; set; } = new();

        [JsonIgnore]
        public List<Rectangle> FillPixels { get; set; } = new();

        [JsonIgnore]
        public ImageBrush FillBrush { get; set; } = null;

        public void DrawPolygon(Canvas canvas)
        {
            OutlinePixels.Clear();

            for (int i = 0; i < numVertices; i++)
            {
                if (i + 1 < numVertices)
                {
                    if (UseAntialiasing)
                        AntiAliasing.DrawWuLine(canvas, Vertices[i], Vertices[i + 1], myColor, OutlinePixels);
                    else
                        DrawLineAliasing(canvas, Vertices[i], Vertices[i + 1]);
                }
                else
                {
                    if (UseAntialiasing)
                        AntiAliasing.DrawWuLine(canvas, Vertices[0], Vertices[i], myColor, OutlinePixels);
                    else
                        DrawLineAliasing(canvas, Vertices[0], Vertices[i]);
                }
            }
        }

        public void DrawLineAliasing(Canvas canvas, Point Start, Point End)
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
            bool[,] brush = BrushUtils.CreateCircularBrush(Thickness);
            int size = brush.GetLength(0);
            int center = size / 2;

            for (int dy = 0; dy < size; dy++)
            {
                for (int dx = 0; dx < size; dx++)
                {
                    if (!brush[dx, dy]) continue;

                    Brush fillBrush = (Brush)FillBrush ?? new SolidColorBrush(myColor);

                    Rectangle rect = new Rectangle
                    {
                        Width = 1,
                        Height = 1,
                        Fill = fillBrush
                    };

                    Canvas.SetLeft(rect, x + dx - center);
                    Canvas.SetTop(rect, y + dy - center);
                    canvas.Children.Add(rect);
                    OutlinePixels.Add(rect);
                }
            }
        }

        public void RedrawPolygon(Canvas canvas)
        {
            RemovePolygon(canvas);
            DrawPolygon(canvas);

            if (FillBrush != null)
                FillPolygon(canvas, FillBrush);
            else if (FillColor.HasValue)
                FillPolygon(canvas, FillColor.Value);
        }

        public void RemovePolygon(Canvas canvas)
        {
            foreach (var px in OutlinePixels) canvas.Children.Remove(px);
            foreach (var px in FillPixels) canvas.Children.Remove(px);
            OutlinePixels.Clear();
            FillPixels.Clear();
        }

        public bool IsNearPolygon(Point p)
        {
            return OutlinePixels.Any(px =>
            {
                double x = Canvas.GetLeft(px);
                double y = Canvas.GetTop(px);
                return Math.Abs(x - p.X) < 10 && Math.Abs(y - p.Y) < 10;
            });
        }

        public void FillPolygon(Canvas canvas, Color fillColor)
        {
            FillColor = fillColor;
            FillBrush = null;
            FillImageUri = null;
            FillInternal(canvas, new SolidColorBrush(fillColor));
        }

        public void FillPolygon(Canvas canvas, ImageBrush imageBrush)
        {
            FillBrush = imageBrush;
            FillImageUri = imageBrush.ImageSource.ToString();
            FillInternal(canvas, imageBrush);
        }

        private void FillInternal(Canvas canvas, Brush fillBrush)
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
                        Rectangle pixel = new Rectangle
                        {
                            Width = 1,
                            Height = 1,
                            Fill = fillBrush
                        };
                        Canvas.SetLeft(pixel, x);
                        Canvas.SetTop(pixel, y);
                        canvas.Children.Add(pixel);
                        FillPixels.Add(pixel);
                    }
                }

                for (int i = 0; i < AET.Count; i++)
                {
                    AET[i] = (AET[i].x + AET[i].invM, AET[i].invM, AET[i].ymax);
                }
            }
        }

        public void SetFillImage(string imagePath)
        {
            FillImageUri = imagePath;

            // Compute bounding box
            double minX = Vertices.Min(p => p.X);
            double minY = Vertices.Min(p => p.Y);
            double maxX = Vertices.Max(p => p.X);
            double maxY = Vertices.Max(p => p.Y);

            Rect viewBox = new Rect(minX, minY, maxX - minX, maxY - minY);

            FillBrush = new ImageBrush
            {
                ImageSource = new ImageSourceConverter().ConvertFromString(imagePath) as ImageSource,
                Stretch = Stretch.Fill,
                Viewbox = new Rect(0, 0, 1, 1),
                ViewboxUnits = BrushMappingMode.RelativeToBoundingBox,
                Viewport = viewBox,
                ViewportUnits = BrushMappingMode.Absolute,
                AlignmentX = AlignmentX.Left,
                AlignmentY = AlignmentY.Top
            };
        }

        public void DrawImageClippedPolygon(Canvas canvas)
        {
            if (Vertices.Count < 3 || string.IsNullOrEmpty(FillImageUri))
                return;

            var image = new Image
            {
                Source = new ImageSourceConverter().ConvertFromString(FillImageUri) as ImageSource,
                Stretch = Stretch.Fill
            };

            // Compute bounding box
            double minX = Vertices.Min(p => p.X);
            double minY = Vertices.Min(p => p.Y);
            double maxX = Vertices.Max(p => p.X);
            double maxY = Vertices.Max(p => p.Y);

            double width = maxX - minX;
            double height = maxY - minY;

            image.Width = width;
            image.Height = height;

            Canvas.SetLeft(image, minX);
            Canvas.SetTop(image, minY);

            // Create clipping geometry
            var figure = new PathFigure
            {
                StartPoint = new Point(Vertices[0].X - minX, Vertices[0].Y - minY),
                IsClosed = true,
                IsFilled = true
            };

            for (int i = 1; i < Vertices.Count; i++)
            {
                figure.Segments.Add(new LineSegment(new Point(Vertices[i].X - minX, Vertices[i].Y - minY), true));
            }

            var geometry = new PathGeometry();
            geometry.Figures.Add(figure);
            image.Clip = geometry;

            canvas.Children.Add(image);
        }


    }
}
