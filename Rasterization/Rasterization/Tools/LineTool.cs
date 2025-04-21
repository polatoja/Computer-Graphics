using Rasterization.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Rasterization.Tools
{
    public class LineTool
    {
        private readonly Canvas canvas;
        private readonly List<CustomLine> lines;
        private CustomLine selectedLine;
        private Point? startPoint = null;
        private bool draggingStart = false;
        private bool draggingEnd = false;
        public bool IsDragging => draggingStart || draggingEnd;


        public LineTool(Canvas canvas, List<CustomLine> lines)
        {
            this.canvas = canvas;
            this.lines = lines;
        }

        public void StartDraw(Point click)
        {
            if (startPoint == null)
            {
                startPoint = click;
            }
            else
            {
                var line = new CustomLine
                {
                    Start = startPoint.Value,
                    End = click,
                    Thickness = 1
                };
                line.DrawLine(canvas);
                lines.Add(line);
                startPoint = null;
            }
        }

        public void TryEdit(Point click)
        {
            foreach (var line in lines)
            {
                if (line.IsNearPoint(click, line.Start))
                {
                    selectedLine = line;
                    draggingStart = true;
                    draggingEnd = false;
                    return;
                }
                else if (line.IsNearPoint(click, line.End))
                {
                    selectedLine = line;
                    draggingEnd = true;
                    draggingStart = false;
                    return;
                }
            }
        }

        public void UpdateDrag(Point pos)
        {
            if (selectedLine == null) return;

            if (draggingStart)
                selectedLine.Start = pos;
            else if (draggingEnd)
                selectedLine.End = pos;

            selectedLine.RedrawLine(canvas);
        }

        public void FinishEdit()
        {
            if ((draggingStart || draggingEnd) && selectedLine != null)
            {
                if (!lines.Contains(selectedLine))
                {
                    lines.Add(selectedLine);
                }

                MessageBox.Show("Line edited.");
                draggingStart = false;
                draggingEnd = false;
                selectedLine = null;
            }
        }

        public void TryDelete(Point click)
        {
            foreach (var line in lines.ToList())
            {
                if (line.IsNearLine(click))
                {
                    line.RemoveLine(canvas);
                    lines.Remove(line);
                    return;
                }
            }
        }

        public void TryIncreaseThickness(Point click)
        {
            foreach (var line in lines)
            {
                if (line.IsNearLine(click))
                {
                    line.Thickness += 1;
                    line.RedrawLine(canvas);
                    return;
                }
            }
        }
        public void ClearCanvas()
        {
            canvas.Children.Clear();
            lines.Clear();
            selectedLine = null;
            startPoint = null;
            draggingStart = false;
            draggingEnd = false;
        }
    }
}
