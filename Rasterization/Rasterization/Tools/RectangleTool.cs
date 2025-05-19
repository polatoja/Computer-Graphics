using Rasterization.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Rasterization.Tools
{
    public class RectangleTool
    {
        private readonly Canvas canvas;
        private readonly List<CustomRectangle> rectangles;
        private CustomRectangle selectedRectangle;
        private Point? firstPoint = null;
        private Point? dragOffset = null;
        private bool moving = false;
        private bool movingVertex = false;
        private bool movingEdge = false;
        private int? selectedVertexIndex = null;
        private (int, int)? selectedEdge = null;

        public bool IsDragging => moving || movingVertex || movingEdge;
        public List<CustomRectangle> Rectangles => rectangles;
        public bool UseAntialiasing = false;

        public RectangleTool(Canvas canvas, List<CustomRectangle> rectangles)
        {
            this.canvas = canvas;
            this.rectangles = rectangles;
        }

        public void StartDraw(Point click)
        {
            if (firstPoint == null)
            {
                firstPoint = click;
            }
            else
            {
                Point Point1 = firstPoint.Value;
                Point Point2 = new Point(click.X, firstPoint.Value.Y);
                Point Point3 = click;
                Point Point4 = new Point(firstPoint.Value.X, click.Y);
                var rect = new CustomRectangle
                {
                    Vertices = new() {Point1,Point2, Point3,Point4 },
                    Thickness = 1,
                    UseAntialiasing = UseAntialiasing
                };
                rect.DrawRectangle(canvas);
                rectangles.Add(rect);
                firstPoint = null;
            }
        }

        public void TryEditVertex(Point click)
        {
            foreach (var rectangle in rectangles)
            {
                for(int i = 0; i < 4; i++)
                {
                    if ((rectangle.Vertices[i] - click).Length <= 10)
                    {
                        selectedRectangle = rectangle;
                        selectedVertexIndex = i;
                        dragOffset = click;
                        movingVertex = true;
                        return;
                    }
                }
            }
        }

        public void TryEditEdge(Point click)
        {
            foreach (var rectangle in rectangles)
            {
                for(int i = 0; i < 4; i++)
                {
                    if (IsNearEdge(rectangle.Vertices[i], rectangle.Vertices[(i + 1) % 4], click))
                    {
                        selectedRectangle = rectangle;
                        selectedEdge = (i, (i + 1) % 4);
                        dragOffset = click;
                        movingEdge = true;
                        return;
                    }
                }
            }
        }

        public void TryMove(Point click)
        {
            foreach (var rectangle in rectangles)
            {
                if (rectangle.IsNearRectangle(click))
                {
                    selectedRectangle = rectangle;
                    dragOffset = click;
                    moving = true;
                    return;
                }
            }
        }

        public void UpdateDrag(Point current)
        {
            if (selectedRectangle == null || !dragOffset.HasValue)
                return;

            Vector delta = current - dragOffset.Value;

            if (moving)
            {
                for(int i = 0; i < 4; i++)
                {
                    selectedRectangle.Vertices[i] += delta;
                }
                dragOffset = current;
                selectedRectangle.RedrawRectangle(canvas);
            }
            else if (movingVertex && selectedVertexIndex.HasValue)
            {
                int id = selectedVertexIndex.Value;
                var pts = selectedRectangle.Vertices;

                pts[id] += delta;

                if(id % 2 == 0)
                {
                    pts[(id + 1) % 4] = new Point(pts[(id + 1) % 4].X, pts[id].Y);
                    pts[(id + 3) % 4] = new Point(pts[id].X, pts[(id + 3) % 4].Y);
                }
                else
                {
                    pts[(id + 1) % 4] = new Point(pts[id].X, pts[(id + 1) % 4].Y);
                    pts[(id + 3) % 4] = new Point(pts[(id + 3) % 4].X, pts[id].Y);
                }


                dragOffset = current;
                selectedRectangle.RedrawRectangle(canvas);
            }

            else if (movingEdge && selectedEdge.HasValue)
            {
                var (id1, id2) = selectedEdge.Value;
                var pts = selectedRectangle.Vertices;

                if (id1 % 2 == 0)
                {
                    Vector yDelta = new Vector(0, delta.Y);
                    pts[id1] += yDelta;
                    pts[id2] += yDelta;
                }
                else
                {
                    Vector xDelta = new Vector(delta.X, 0);
                    pts[id1] += xDelta;
                    pts[id2] += xDelta;
                }

                dragOffset = current;
                selectedRectangle.RedrawRectangle(canvas);
            }

        }

        public void FinishEdit()
        {
            if ((moving || movingVertex || movingEdge) && selectedRectangle != null)
            {
                moving = false;
                movingVertex = false;
                movingEdge = false;
                dragOffset = null;
                selectedVertexIndex = null;
                selectedEdge = null;
                selectedRectangle = null;
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
            foreach (var rectangle in rectangles.ToList())
            {
                if (rectangle.IsNearRectangle(click))
                {
                    rectangle.RemoveRectangle(canvas);
                    rectangles.Remove(rectangle);
                    return;
                }
            }
        }

        public void TryIncreaseThickness(Point click)
        {
            foreach (var rectangle in rectangles)
            {
                if (rectangle.IsNearRectangle(click))
                {
                    rectangle.Thickness++;
                    rectangle.RedrawRectangle(canvas);
                    return;
                }
            }
        }

        public void TryChangeColor(Point click, Color color)
        {
            foreach (var rectangle in rectangles)
            {
                if (rectangle.IsNearRectangle(click))
                {
                    rectangle.myColor = color;
                    rectangle.RedrawRectangle(canvas);
                    return;
                }
            }
        }

        public void ClearCanvas()
        {
            canvas.Children.Clear();
            rectangles.Clear();
            selectedRectangle = null;
            firstPoint = null;
            dragOffset = null;
            moving = false;
            movingVertex = false;
            movingEdge = false;
            selectedVertexIndex = null;
            selectedEdge = null;
        }
    }
}
