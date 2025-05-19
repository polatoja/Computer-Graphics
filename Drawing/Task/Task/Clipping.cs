using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using Task.shapes;


namespace Task
{
    public class Clipping
    {
        public bool LiangBarskyClip(double x0, double y0, double x1, double y1, double xmin, double xmax, double ymin, double ymax, out Point clippedStart, out Point clippedEnd)
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

        public void DrawClippedSegments(WriteableBitmap bitmap, Polygon polygon)
        {
            if (polygon.clippedRectangle != null)
            {
                var minX = polygon.clippedRectangle.Vertices.Min(p => p.X);
                var maxX = polygon.clippedRectangle.Vertices.Max(p => p.X);
                var minY = polygon.clippedRectangle.Vertices.Min(p => p.Y);
                var maxY = polygon.clippedRectangle.Vertices.Max(p => p.Y);


                for (int i = 0; i < polygon.Vertices.Count; i++)
                {
                    var a = polygon.Vertices[i];
                    var b = polygon.Vertices[(i + 1) % polygon.Vertices.Count];
                    int t = polygon.Thickness + 1;

                    if (LiangBarskyClip(a.X, a.Y, b.X, b.Y, minX, maxX, minY, maxY, out Point start, out Point end))
                    {
                        polygon.DrawLine(bitmap, start, end, Colors.Red, t);
                    }
                }
            }
            else
            {
                return;
            }
        }

    }
}
