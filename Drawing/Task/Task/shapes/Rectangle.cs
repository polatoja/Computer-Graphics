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
    public class Rectangle : Shapes 
    {
        public List<Point> Vertices { get; set; } = new(4);
        public int? selectedVertexIndex = null;
        public int? selectedEdgeIndex = null;

        public override void Draw(WriteableBitmap bitmap)
        {
            if (UseAntialiasing)
            {
                if (Vertices.Count != 4) return;

                AntiAliasing.DrawWuLine(bitmap, Vertices[0], Vertices[1], Color, Thickness); // top
                AntiAliasing.DrawWuLine(bitmap, Vertices[1], Vertices[2], Color, Thickness); // right
                AntiAliasing.DrawWuLine(bitmap, Vertices[2], Vertices[3], Color, Thickness); // bottom
                AntiAliasing.DrawWuLine(bitmap, Vertices[3], Vertices[0], Color, Thickness); // left
            }
            else
            {
                if (Vertices.Count != 4) return;

                DrawLine(bitmap, Vertices[0], Vertices[1], Color, Thickness); // top
                DrawLine(bitmap, Vertices[1], Vertices[2], Color, Thickness); // right
                DrawLine(bitmap, Vertices[2], Vertices[3], Color, Thickness); // bottom
                DrawLine(bitmap, Vertices[3], Vertices[0], Color, Thickness); // left
            }
        }

        public override void DeleteShape(List<Shapes> shapes, Shapes rectangleToDelete, WriteableBitmap bitmap)
        {
            shapes.Remove(rectangleToDelete);
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
    }
}
