using Rasterization.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Rasterization.Tools
{
    public class CircleTool
    {
        private readonly Canvas canvas;
        private readonly List<CustomCircle> circles;
        private CustomCircle selectedCircle;
        private Point? centerPoint = null;
        private bool dragging = false;
        private bool moving = false;
        private Point? dragOffset = null;
        public bool IsDragging => dragging || moving;

        public List<CustomCircle> Circles => circles;
        public bool UseAntialiasing = false;

        public CircleTool(Canvas canvas, List<CustomCircle> circles)
        {
            this.canvas = canvas;
            this.circles = circles;
        }

        public void StartDraw(Point click)
        {
            if (centerPoint == null)
            {
                centerPoint = click;
            }
            else
            {
                var circle = new CustomCircle
                {
                    Start = centerPoint.Value,
                    Thickness = 1,
                    UseAntialiasing = UseAntialiasing
                };
                circle.SetRadiusFromPoint(click);
                circle.DrawCircle(canvas);
                circles.Add(circle);
                centerPoint = null;
            }
        }

        public void TryEdit(Point click)
        {
            foreach (var circle in circles)
            {
                if (circle.IsNearCircle(click))
                {
                    selectedCircle = circle;
                    dragging = true;
                    return;
                }
            }
        }

        public void TryMove(Point click)
        {
            foreach (var circle in circles)
            {
                if (circle.IsNearCircle(click))
                {
                    selectedCircle = circle;
                    dragOffset = click - (Vector)circle.Start;
                    moving = true;
                    return;
                }
            }
        }

        public void UpdateDrag(Point current)
        {
            if (selectedCircle != null && dragging)
            {
                selectedCircle.SetRadiusFromPoint(current);
                selectedCircle.RedrawCircle(canvas);
            }
            else if (selectedCircle != null && moving && dragOffset.HasValue)
            {
                selectedCircle.Start = (Point)(current - dragOffset.Value);
                selectedCircle.RedrawCircle(canvas);
            }
        }

        public void FinishEdit()
        {
            if ((dragging || moving) && selectedCircle != null)
            {
                dragging = false;
                moving = false;
                dragOffset = null;
                selectedCircle = null;
            }
        }

        public void TryDelete(Point click)
        {
            foreach (var circle in circles.ToList())
            {
                if (circle.IsNearCircle(click))
                {
                    circle.RemoveCircle(canvas);
                    circles.Remove(circle);
                    return;
                }
            }
        }

        public void TryIncreaseThickness(Point click)
        {
            foreach (var circle in circles)
            {
                if (circle.IsNearCircle(click))
                {
                    circle.Thickness++;
                    circle.RedrawCircle(canvas);
                    return;
                }
            }
        }

        public void ClearCanvas()
        {
            canvas.Children.Clear();
            circles.Clear();
            selectedCircle = null;
            centerPoint = null;
            dragOffset = null;
            moving = false;
            dragging = false;
        }

        public void TryChangeColor(Point click, Color color)
        {
            foreach (var circle in circles)
            {
                if (circle.IsNearCircle(click))
                {
                    circle.myColor = color;
                    circle.RedrawCircle(canvas);
                    return;
                }
            }
        }
    }
}