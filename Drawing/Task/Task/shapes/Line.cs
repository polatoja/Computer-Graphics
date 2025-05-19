using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Text.Json.Serialization;
using System.Windows.Media.Imaging;
using Task;

namespace Task.shapes
{
    public class Line : Shapes
    {
        public Point Start { get; set; }
        public Point End { get; set; }
        public enum Dragging
        {
            Start,
            End,
            All
        }
        public Point? lastClick = null;

        public override void Draw(WriteableBitmap bitmap)
        {
            if (UseAntialiasing)
            {
                AntiAliasing.DrawWuLine(bitmap, Start, End, Color, Thickness);
            }
            else
            {
                DrawLine(bitmap, Start, End);
            }
        }
        public override void DeleteShape(List<Shapes> shapes, Shapes lineToDelete, WriteableBitmap bitmap)
        {
            shapes.Remove(lineToDelete);
            RedrawAll(bitmap, shapes);
        }

        public override bool IsNearShape(Point p, double threshold = 5)
        {
            double dx = End.X - Start.X;
            double dy = End.Y - Start.Y;

            if (dx == 0 && dy == 0)
                return (p - Start).Length <= threshold;

            double t = ((p.X - Start.X) * dx + (p.Y - Start.Y) * dy) / (dx * dx + dy * dy);
            t = Math.Max(0, Math.Min(1, t));
            Point projection = new(Start.X + t * dx, Start.Y + t * dy);
            return (p - projection).Length <= threshold;
        }
    }
}
