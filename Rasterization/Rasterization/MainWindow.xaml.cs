using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Rasterization
{
    public partial class MainWindow : Window
    {
        private List<CustomLine> lines = new();
        private Point? currentStartPoint = null;
        private CustomLine selectedLine = null;

        // LINE
        private bool isDrawingLine = false;
        private bool isEditingLine = false;
        private bool isDraggingStartLine = false;
        private bool isDraggingEndLine = false;
        private bool isDeletingLine = false;

        // CIRCLE
        private bool isDrawingCircle = false;
        private bool isEditingCircle = false;
        private bool isDraggingStartCircle = false;
        private bool isDraggingEndCircle = false;
        private bool isDeletingCircle = false;

        public MainWindow()
        {
            InitializeComponent();
            DrawingCanvas.MouseLeftButtonDown += Canvas_MouseLeftButtonDown;
            DrawingCanvas.MouseRightButtonDown += Canvas_MouseRightButtonDown;
            DrawingCanvas.MouseMove += Canvas_MouseMove;
            DrawingCanvas.MouseMove += Canvas_MouseMove;
        }

        private void DrawLine_Click(object sender, RoutedEventArgs e)
        {
            isDrawingLine = true;
            isEditingLine = false;
            isDeletingLine = false;
            currentStartPoint = null;
            selectedLine = null;
        }
        private void EditLine_Click(object sender, RoutedEventArgs e)
        {
            isEditingLine = true;
            isDeletingLine = false;
            isDrawingLine = false;
            selectedLine = null;
        }
        private void DeleteLine_Click(object sender, RoutedEventArgs e)
        {
            isDeletingLine = true;
            isDrawingLine = false;
            isEditingLine = false;
            selectedLine = null;
        }

        private void DrawCircle_Click(object sender, RoutedEventArgs e)
        {
            isDrawingCircle = true;
            isEditingCircle = false;
            isDeletingCircle = false;
            currentStartPoint = null;
        }

        private void EditCircle_Click(object sender, RoutedEventArgs e)
        {
            isDrawingCircle = false;
            isEditingCircle = true;
            isDeletingCircle = false;
            currentStartPoint = null;
        }

        private void DeleteCircle_Click(object sender, RoutedEventArgs e)
        {
            isDrawingCircle = false;
            isEditingCircle = false;
            isDeletingCircle = true;
            currentStartPoint = null;
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point click = e.GetPosition(DrawingCanvas);

            if ((isDraggingStartLine || isDraggingEndLine) && selectedLine != null)
            {
                if (!lines.Contains(selectedLine))
                {
                    lines.Add(selectedLine);
                }

                MessageBox.Show("Line edited.");
                isDraggingStartLine = false;
                isDraggingEndLine = false;
                selectedLine = null;
                isEditingLine = false;
            }
            if (isDrawingLine)
            {
                if (currentStartPoint == null)
                {
                    currentStartPoint = click;
                }
                else
                {
                    var newLine = new CustomLine
                    {
                        Start = currentStartPoint.Value,
                        End = click,
                        Thickness = 1
                    };
                    newLine.Draw(DrawingCanvas);
                    lines.Add(newLine);

                    currentStartPoint = null;
                }
                return;
            }
            if (isEditingLine)
            {
                foreach (var line in lines)
                {
                    if (line.IsNearLine(click))
                    {
                        if (line.IsNearPoint(click, line.Start))
                        {
                            selectedLine = line;
                            isDraggingStartLine = true;
                            return;
                        }
                        else if (line.IsNearPoint(click, line.End))
                        {
                            selectedLine = line;
                            isDraggingEndLine = true;
                            return;
                        }
                    }
                }
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (selectedLine != null && (isDraggingStartLine || isDraggingEndLine))
            {
                Point pos = e.GetPosition(DrawingCanvas);
                if (isDraggingStartLine)
                    selectedLine.Start = pos;
                else if (isDraggingEndLine)
                    selectedLine.End = pos;

                selectedLine.Redraw(DrawingCanvas);
            }
        }

        private void Canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point click = e.GetPosition(DrawingCanvas);

            if (isDeletingLine)
            {
                foreach (var line in lines.ToList())
                {
                    if (line.IsNearLine(click))
                    {
                        line.Remove(DrawingCanvas);
                        lines.Remove(line);
                        return;
                    }
                }

                foreach (var line in lines)
                {
                    if (line.IsNearLine(click))
                    {
                        line.Thickness += 1;
                        line.Redraw(DrawingCanvas);
                        return;
                    }
                }
            }
        }
    }

}
