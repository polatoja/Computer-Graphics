using Rasterization.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Rasterization.Tools
{
    public class PacManTool
    {
        private readonly Canvas canvas;
        public List<CustomPacMan> Pacmans { get; set; } = new();

        private Point? centerPoint = null;
        private Point? point1 = null;
        private List<Ellipse> pointMarkers = new();

        private Ellipse previewCircle;


        public PacManTool(Canvas canvas, List<CustomPacMan> Pacmans)
        {
            this.canvas = canvas;
            this.Pacmans = Pacmans;
        }

        public void StartDraw(Point click)
        {
            if (centerPoint == null)
            {
                centerPoint = click;
                DrawPointMarker(click);
            }
            else if (point1 == null)
            {
                point1 = click;
                DrawPointMarker(click);
                DrawPreviewCircle(centerPoint.Value, point1.Value);
            }
            else
            {
                DrawPointMarker(click);
                double radius = Distance(centerPoint.Value, point1.Value);

                Vector dir = click - centerPoint.Value;
                dir.Normalize();

                Point point2OnCircle = centerPoint.Value + dir * radius;

                var pacman = new CustomPacMan
                {
                    Center = centerPoint.Value,
                    Point1 = point1.Value,
                    Point2 = point2OnCircle
                };

                pacman.Draw(canvas);
                Pacmans.Add(pacman);

                centerPoint = null;
                point1 = null;
                canvas.Children.Remove(previewCircle);
                previewCircle = null;

            }
        }

        private double Distance(Point a, Point b)
        {
            return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }

        private void DrawPointMarker(Point point)
        {
            var marker = new Ellipse
            {
                Width = 4,
                Height = 4,
                Fill = Brushes.Red
            };
            Canvas.SetLeft(marker, point.X - 2);
            Canvas.SetTop(marker, point.Y - 2);
            canvas.Children.Add(marker);
            pointMarkers.Add(marker);
        }

        private void DrawPreviewCircle(Point center, Point edge)
        {
            if (previewCircle != null)
                canvas.Children.Remove(previewCircle);

            double radius = Distance(center, edge);

            previewCircle = new Ellipse
            {
                Width = radius * 2,
                Height = radius * 2,
                Stroke = Brushes.Gray,
                StrokeDashArray = new DoubleCollection { 4, 2 },
                StrokeThickness = 1
            };

            Canvas.SetLeft(previewCircle, center.X - radius);
            Canvas.SetTop(previewCircle, center.Y - radius);
            canvas.Children.Add(previewCircle);
        }

    }
}
