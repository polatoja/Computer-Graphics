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

    }
}
