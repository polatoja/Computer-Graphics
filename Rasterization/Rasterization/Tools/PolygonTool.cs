using Rasterization.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Rasterization.Tools
{
    public class PolygonTool
    {
        private readonly Canvas canvas;
        private readonly List<CustomPolygon> polygons;
        private CustomPolygon currentPolygon = null;
        private Point? startPoint = null;
        private List<Ellipse> pointMarkers = new();

        private bool moving = false;
        private bool movingVertex = false;
        private bool movingEdge = false;

        private int? selectedVertexIndex = null;
        private (int, int)? selectedEdgeIndices = null;
        private Point? dragOffset = null;

        public bool IsMoving => moving;
        public bool IsMovingVertex => movingVertex;
        public bool IsMovingEdge => movingEdge;

        public List<CustomPolygon> Polygons => polygons;
        public bool UseAntialiasing = false;

        public PolygonTool(Canvas canvas, List<CustomPolygon> polygons)
        {
            this.canvas = canvas;
            this.polygons = polygons;
        }

        public void StartDraw(Point click)
        {
            if (currentPolygon == null)
            {
                currentPolygon = new CustomPolygon { 
                    UseAntialiasing = UseAntialiasing
                };
                currentPolygon.Vertices.Add(click);
                currentPolygon.numVertices = 1;
                startPoint = click;
                DrawPointMarker(click);
            }
            else
            {
                currentPolygon.UseAntialiasing = UseAntialiasing;
                if (IsNearPoint(startPoint.Value, click))
                {
                    currentPolygon.DrawPolygon(canvas);
                    polygons.Add(currentPolygon);
                    currentPolygon = null;
                    startPoint = null;
                    ClearPointMarkers();
                }
                else
                {
                    currentPolygon.Vertices.Add(click);
                    currentPolygon.numVertices++;
                    DrawPointMarker(click);
                }
            }
        }

        public bool IsNearPoint(Point p1, Point p2, double radius = 20)
        {
            return (p1 - p2).Length <= radius;
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
        private void ClearPointMarkers()
        {
            foreach (var marker in pointMarkers)
            {
                canvas.Children.Remove(marker);
            }
            pointMarkers.Clear();
        }


        public void TryEdit(Point click)
        {
            foreach (var polygon in polygons)
            {
                if (polygon.IsNearPolygon(click))
                {
                    currentPolygon = polygon;
                    moving = true;
                    return;
                }
            }
        }

        public void FinishEdit()
        {
            if ((moving || movingVertex || movingEdge) && currentPolygon != null)
            {
                moving = false;
                movingVertex = false;
                movingEdge = false;
                dragOffset = null;
                currentPolygon = null;
                selectedVertexIndex = null;
                selectedEdgeIndices = null;
            }
        }


        public void TryMove(Point click)
        {
            foreach (var polygon in polygons)
            {
                if (polygon.IsNearPolygon(click))
                {
                    currentPolygon = polygon;
                    dragOffset = click;
                    moving = true;
                    return;
                }
            }
        }
        public void UpdateDrag(Point current)
        {
            if (currentPolygon != null && moving && dragOffset.HasValue)
            {
                Vector delta = current - dragOffset.Value;

                for (int i = 0; i < currentPolygon.Vertices.Count; i++)
                {
                    currentPolygon.Vertices[i] += delta;
                }

                dragOffset = current;
                currentPolygon.RedrawPolygon(canvas);
            }
        }

        public void TryMoveVertex(Point click)
        {
            foreach (var polygon in polygons)
            {
                for (int i = 0; i < polygon.Vertices.Count; i++)
                {
                    if (IsNearPoint(polygon.Vertices[i], click))
                    {
                        currentPolygon = polygon;
                        selectedVertexIndex = i;
                        dragOffset = click;
                        movingVertex = true;
                        return;
                    }
                }
            }
        }

        public void MoveVertex(Point current)
        {
            if (currentPolygon != null && movingVertex && dragOffset.HasValue && selectedVertexIndex.HasValue)
            {
                Vector delta = current - dragOffset.Value;

                currentPolygon.Vertices[selectedVertexIndex.Value] += delta;
                dragOffset = current;
                currentPolygon.RedrawPolygon(canvas);
            }
        }

        public void TryMoveEdge(Point click)
        {
            foreach (var polygon in polygons)
            {
                var vertices = polygon.Vertices;
                int count = vertices.Count;

                for (int i = 0; i < count; i++)
                {
                    Point a = vertices[i];
                    Point b = vertices[(i + 1) % count];

                    if (IsNearEdge(a, b, click))
                    {
                        currentPolygon = polygon;
                        selectedEdgeIndices = (i, (i + 1) % count);
                        dragOffset = click;
                        movingEdge = true;
                        return;
                    }
                }
            }
        }

        public void MoveEdge(Point current)
        {
            if (currentPolygon != null && movingEdge && dragOffset.HasValue && selectedEdgeIndices.HasValue)
            {
                Vector delta = current - dragOffset.Value;

                var (i1, i2) = selectedEdgeIndices.Value;

                currentPolygon.Vertices[i1] += delta;
                currentPolygon.Vertices[i2] += delta;

                dragOffset = current;
                currentPolygon.RedrawPolygon(canvas);
            }
        }

        private bool IsNearEdge(Point a, Point b, Point p, double threshold = 10)
        {
            Vector ab = b - a;
            Vector ap = p - a;

            double t = Vector.Multiply(ap, ab) / ab.LengthSquared;
            t = Math.Max(0, Math.Min(1, t));

            Point projection = a + t * ab;
            return (projection - p).Length <= threshold;
        }

        public void TryDelete(Point click)
        {
            foreach (var polygon in polygons.ToList())
            {
                if (polygon.IsNearPolygon(click))
                {
                    polygon.RemovePolygon(canvas);
                    polygons.Remove(polygon);
                    return;
                }
            }
        }

        public void TryIncreaseThickness(Point click)
        {
            foreach (var polygon in polygons)
            {
                if (polygon.IsNearPolygon(click))
                {
                    polygon.Thickness++;
                    polygon.RedrawPolygon(canvas);
                    return;
                }
            }
        }

        public void ClearCanvas()
        {
            canvas.Children.Clear();
            polygons.Clear();
            pointMarkers.Clear();
            currentPolygon = null;
            startPoint = null;
            dragOffset = null;
            moving = false;
            movingVertex = false;
            selectedVertexIndex = null;
            selectedEdgeIndices = null;
        }

        public void TryChangeColor(Point click, Color color)
        {
            foreach (var polygon in polygons)
            {
                if (polygon.IsNearPolygon(click))
                {
                    polygon.myColor = color;
                    polygon.RedrawPolygon(canvas);
                    return;
                }
            }
        }

        public void TryFillColor(Point click, Color color)
        {
            foreach (var polygon in polygons)
            {
                if (polygon.IsNearPolygon(click))
                {
                    polygon.myColor = color;
                    polygon.FillPolygon(canvas, color);
                    return;
                }
            }
        }
        public void TryFillImage(Point click, ImageBrush imageBrush)
        {
            foreach (var polygon in polygons)
            {
                if (polygon.IsNearPolygon(click))
                {
                    polygon.FillPolygon(canvas, imageBrush);
                    return;
                }
            }
        }
    }
}