using Rasterization.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;

namespace Rasterization
{
    public class Clipping
    {
        public bool LiangBarskyClip(double x0, double y0, double x1, double y1, double xmin, 
            double xmax, double ymin, double ymax, out Point clippedStart, out Point clippedEnd)
        {
            double dx = x1 - x0, dy = y1 - y0;
            double tE = 0.0, tL = 1.0;

            bool Clip(double denom, double numer)
            {
                if (denom == 0)
                {
                    return numer <= 0;
                }

                double t = numer / denom;
                if (denom < 0)
                {
                    if (t > tL) return false;
                    if (t > tE) tE = t;
                }
                else
                {
                    if (t < tE) return false;
                    if (t < tL) tL = t;
                }

                return true;
            }

            if (
                Clip(-dx, x0 - xmin) &&
                Clip(dx, xmax - x0) &&
                Clip(-dy, y0 - ymin) &&
                Clip(dy, ymax - y0))
            {
                clippedStart = new Point(x0 + dx * tE, y0 + dy * tE);
                clippedEnd = new Point(x0 + dx * tL, y0 + dy * tL);
                return true;
            }

            clippedStart = clippedEnd = default;
            return false;
        }

        public void DrawClippedSegments(Canvas canvas, CustomPolygon subject, Rect clipRect, Color clipColor)
        {
            for (int i = 0; i < subject.Vertices.Count; i++)
            {
                var a = subject.Vertices[i];
                var b = subject.Vertices[(i + 1) % subject.Vertices.Count];

                if (LiangBarskyClip(
                    a.X, a.Y, b.X, b.Y,
                    clipRect.Left, clipRect.Right, clipRect.Top, clipRect.Bottom,
                    out Point start, out Point end))
                {
                    var line = new Line
                    {
                        X1 = start.X,
                        Y1 = start.Y,
                        X2 = end.X,
                        Y2 = end.Y,
                        Stroke = new SolidColorBrush(clipColor),
                        StrokeThickness = subject.Thickness
                    };
                    canvas.Children.Add(line);
                }
            }
        }

    }
}
