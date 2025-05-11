using Microsoft.Win32;
using Rasterization.Drawing;
using Rasterization.Tools;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using static System.Runtime.CompilerServices.RuntimeHelpers;


namespace Rasterization
{
    public partial class MainWindow : Window
    {

        private bool isThickening = false;
        private bool isDeleting = false;
        private bool isChangingColors = false;
        private Color selectedColor = Colors.Black;

        // LINE
        private List<CustomLine> lines = new();
        private LineTool lineTool;

        private bool isDrawingLine = false;
        private bool isEditingLine = false;

        // CIRCLE
        private List<CustomCircle> circles = new();
        private CircleTool circleTool;

        private bool isDrawingCircle = false;
        private bool isEditingCircle = false;
        private bool isMovingCircle = false;

        // POLYGON
        private List<CustomPolygon> polygons = new();
        private PolygonTool polygonTool;

        private bool isDrawingPolygon = false;
        private bool isMovingPolygonVertex = false;
        private bool isMovingPolygonEdge = false;
        private bool isMovingPolygon = false;

        // PACMAN
        private List<CustomPacMan> pacmans = new();
        private PacManTool pacmanTool;

        private bool isDrawingPacMan = false;

        // RECTANGLE
        private List<CustomRectangle> rectangles = new();
        private RectangleTool rectangleTool;

        private bool isDrawingRectangle = false;
        private bool isEditingVertexRectangle = false;
        private bool isEditingEdgeRectangle = false;
        private bool isMovingRectangle = false;

        private class Shapes
        {
            public List<CustomLine> Lines { get; set; } = new();
            public List<CustomCircle> Circles { get; set; } = new();
            public List<CustomPolygon> Polygons { get; set; } = new();
            public List<CustomRectangle> Rectangles { get; set; } = new();
        }
        public MainWindow()
        {
            InitializeComponent();
            lineTool = new LineTool(DrawingCanvas, lines);
            circleTool = new CircleTool(DrawingCanvas, circles);
            polygonTool = new PolygonTool(DrawingCanvas, polygons);
            pacmanTool = new PacManTool(DrawingCanvas, pacmans);
            rectangleTool = new RectangleTool(DrawingCanvas, rectangles);

            DrawingCanvas.MouseLeftButtonDown += Canvas_MouseLeftButtonDown;
            DrawingCanvas.MouseRightButtonDown += Canvas_MouseRightButtonDown;
            DrawingCanvas.MouseMove += Canvas_MouseMove;
        }

        private void DrawLine_Click(object sender, RoutedEventArgs e)
        {
            isThickening = false;
            isDeleting = false;
            isChangingColors = false;

            isDrawingLine = true;
            isEditingLine = false;

            isDrawingCircle = false;
            isEditingCircle = false;
            isMovingCircle = false;

            isDrawingPolygon = false;
            isMovingPolygonVertex = false;
            isMovingPolygonEdge = false;
            isMovingPolygon = false;

            isDrawingRectangle = false;
            isEditingVertexRectangle = false;
            isEditingEdgeRectangle = false;
            isMovingRectangle = false;
        }

        private void EditLine_Click(object sender, RoutedEventArgs e)
        {
            isThickening = false;
            isDeleting = false;
            isChangingColors = false;

            isDrawingLine = false;
            isEditingLine = true;

            isDrawingCircle = false;
            isEditingCircle = false;
            isMovingCircle = false;

            isDrawingPolygon = false;
            isMovingPolygonVertex = false;
            isMovingPolygonEdge = false;
            isMovingPolygon = false;

            isDrawingRectangle = false;
            isEditingVertexRectangle = false;
            isEditingEdgeRectangle = false;
            isMovingRectangle = false;
        }

        private void ThickenLine_Click(object sender, RoutedEventArgs e)
        {
            isThickening = false;
            isDeleting = false;
            isChangingColors = false;

            isDrawingLine = false;
            isEditingLine = false;

            isDrawingCircle = false;
            isEditingCircle = false;
            isMovingCircle = false;

            isDrawingPolygon = false;
            isMovingPolygonVertex = false;
            isMovingPolygonEdge = false;
            isMovingPolygon = false;
         
            isDrawingRectangle = false;
            isEditingVertexRectangle = false;
            isEditingEdgeRectangle = false;
            isMovingRectangle = false;
        }

        private void DeleteLine_Click(object sender, RoutedEventArgs e)
        {
            isThickening = false;
            isDeleting = false;
            isChangingColors = false;

            isDrawingLine = false;
            isEditingLine = false;

            isDrawingCircle = false;
            isEditingCircle = false;
            isMovingCircle = false;

            isDrawingPolygon = false;
            isMovingPolygonVertex = false;
            isMovingPolygonEdge = false;
            isMovingPolygon = false;
                
            isDrawingRectangle = false;
            isEditingVertexRectangle = false;
            isMovingRectangle = false;
        }

        private void DrawCircle_Click(object sender, RoutedEventArgs e)
        {
            isThickening = false;
            isDeleting = false;
            isChangingColors = false;

            isDrawingCircle = true;
            isEditingCircle = false;

            isDrawingLine = false;
            isEditingLine = false;
            isMovingCircle = false;

            isDrawingPolygon = false;
            isMovingPolygonVertex = false;
            isMovingPolygonEdge = false;
            isMovingPolygon = false;

            isDrawingRectangle = false;
            isEditingVertexRectangle = false;
            isEditingEdgeRectangle = false;
            isMovingRectangle = false;
        }

        private void EditCircle_Click(object sender, RoutedEventArgs e)
        {
            isThickening = false;
            isDeleting = false;
            isChangingColors = false;

            isDrawingCircle = false;
            isEditingCircle = true;
            isMovingCircle = false;

            isDrawingLine = false;
            isEditingLine = false;

            isDrawingPolygon = false;
            isMovingPolygonVertex = false;
            isMovingPolygonEdge = false;
            isMovingPolygon = false;

            isDrawingRectangle = false;
            isEditingVertexRectangle = false;
            isEditingEdgeRectangle = false;
            isMovingRectangle = false;
        }

        private void MoveCircle_Click(object sender, RoutedEventArgs e)
        {
            isThickening = false;
            isDeleting = false;
            isChangingColors = false;

            isDrawingCircle = false;
            isEditingCircle = false;
            isMovingCircle = true;

            isDrawingLine = false;
            isEditingLine = false;

            isDrawingPolygon = false;
            isMovingPolygonVertex = false;
            isMovingPolygonEdge = false;
            isMovingPolygon = false;

            isDrawingRectangle = false;
            isEditingVertexRectangle = false;
            isEditingEdgeRectangle = false;
            isMovingRectangle = false;
        }

        private void ThickenCircle_Click(object sender, RoutedEventArgs e)
        {
            isThickening = false;
            isDeleting = false;
            isChangingColors = false;

            isDrawingCircle = false;
            isEditingCircle = false;
            isMovingCircle = false;

            isDrawingLine = false;
            isEditingLine = false;

            isDrawingPolygon = false;
            isMovingPolygonVertex = false;
            isMovingPolygonEdge = false;
            isMovingPolygon = false;

            isDrawingRectangle = false;
            isEditingVertexRectangle = false;
            isEditingEdgeRectangle = false;
            isMovingRectangle = false;
        }

        private void DeleteCircle_Click(object sender, RoutedEventArgs e)
        {
            isThickening = false;
            isDeleting = false;
            isChangingColors = false;

            isDrawingCircle = false;
            isEditingCircle = false;
            isMovingCircle = false;

            isDrawingLine = false;
            isEditingLine = false;

            isDrawingPolygon = false;
            isMovingPolygonVertex = false;
            isMovingPolygonEdge = false;
            isMovingPolygon = false;

            isDrawingRectangle = false;
            isEditingVertexRectangle = false;
            isEditingEdgeRectangle = false;
            isMovingRectangle = false;
        }

        private void DrawPolygon_Click(object sender, RoutedEventArgs e)
        {
            isThickening = false;
            isDeleting = false;
            isChangingColors = false;

            isDrawingPolygon = true;
            isMovingPolygonVertex = false;
            isMovingPolygonEdge = false;
            isMovingPolygon = false;

            isDrawingLine = false;
            isEditingLine = false;

            isDrawingCircle = false;
            isEditingCircle = false;
            isMovingCircle = false;

            isDrawingRectangle = false;
            isEditingVertexRectangle = false;
            isEditingEdgeRectangle = false;
            isMovingRectangle = false;
        }

        private void MovePolygonVertex_Click(object sender, RoutedEventArgs e)
        {
            isThickening = false;
            isDeleting = false;
            isChangingColors = false;

            isDrawingPolygon = false;
            isMovingPolygonVertex = true;
            isMovingPolygonEdge = false;
            isMovingPolygon = false;

            isDrawingLine = false;
            isEditingLine = false;

            isDrawingCircle = false;
            isEditingCircle = false;
            isMovingCircle = false;

            isDrawingRectangle = false;
            isEditingVertexRectangle = false;
            isEditingEdgeRectangle = false;
            isMovingRectangle = false;
        }

        private void MovePolygonEdge_Click(object sender, RoutedEventArgs e)
        {
            isThickening = false;
            isDeleting = false;
            isChangingColors = false;

            isDrawingPolygon = false;
            isMovingPolygonVertex = false;
            isMovingPolygonEdge = true;
            isMovingPolygon = false;

            isDrawingLine = false;
            isEditingLine = false;

            isDrawingCircle = false;
            isEditingCircle = false;
            isMovingCircle = false;

            isDrawingRectangle = false;
            isEditingVertexRectangle = false;
            isEditingEdgeRectangle = false;
            isMovingRectangle = false;
        }

        private void MovePolygon_Click(object sender, RoutedEventArgs e)
        {
            isThickening = false;
            isDeleting = false;
            isChangingColors = false;

            isDrawingPolygon = false;
            isMovingPolygonVertex = false;
            isMovingPolygonEdge = false;
            isMovingPolygon = true;

            isDrawingLine = false;
            isEditingLine = false;

            isDrawingCircle = false;
            isEditingCircle = false;
            isMovingCircle = false;

            isDrawingRectangle = false;
            isEditingVertexRectangle = false;
            isEditingEdgeRectangle = false;
            isMovingRectangle = false;
        }

        private void ClearCanvas_Click(object sender, RoutedEventArgs e)
        {
            lineTool.ClearCanvas();
            circleTool.ClearCanvas();
            polygonTool.ClearCanvas();
            rectangleTool.ClearCanvas();
        }

        private void ThickenShape_Click(object sender, RoutedEventArgs e)
        {
            isThickening = true;
            isDeleting = false;
            isChangingColors = false;

            isDrawingPolygon = false;
            isMovingPolygonVertex = false;
            isMovingPolygon = false;

            isDrawingLine = false;
            isEditingLine = false;

            isDrawingCircle = false;
            isEditingCircle = false;
            isMovingCircle = false;

            isDrawingRectangle = false;
            isEditingVertexRectangle = false;
            isEditingEdgeRectangle = false;
            isMovingRectangle = false;
        }

        private void DeleteShape_Click(object sender, RoutedEventArgs e)
        {
            isThickening = false;
            isDeleting = true;
            isChangingColors = false;

            isDrawingPolygon = false;
            isMovingPolygonVertex = false;
            isMovingPolygon = false;

            isDrawingLine = false;
            isEditingLine = false;

            isDrawingCircle = false;
            isEditingCircle = false;
            isMovingCircle = false;

            isDrawingRectangle = false;
            isEditingVertexRectangle = false;
            isEditingEdgeRectangle = false;
            isMovingRectangle = false;
        }

        private void ChangeColors_Click(object sender, RoutedEventArgs e)
        {
            isThickening = false;
            isDeleting = false;
            isChangingColors = true;

            isDrawingPolygon = false;
            isMovingPolygonVertex = false;
            isMovingPolygon = false;

            isDrawingLine = false;
            isEditingLine = false;

            isDrawingCircle = false;
            isEditingCircle = false;
            isMovingCircle = false;

            isDrawingRectangle = false;
            isEditingVertexRectangle = false;
            isEditingEdgeRectangle = false;
            isMovingRectangle = false;
            
            ColorPopup.IsOpen = true;
        }

        private void ApplyPopupColor_Click(object sender, RoutedEventArgs e)
        {
            if (PopupColorPicker.SelectedColor.HasValue)
            {
                selectedColor = PopupColorPicker.SelectedColor.Value;
            }

            ColorPopup.IsOpen = false;
        }
        
        private void AntiAlisaing_Checked(object sender, RoutedEventArgs e)
        {
            lineTool.UseAntialiasing = true;
            circleTool.UseAntialiasing = true;
            polygonTool.UseAntialiasing = true;
            rectangleTool.UseAntialiasing = true;

            foreach (var line in lineTool.Lines)
            {
                line.UseAntialiasing = true;
                line.RedrawLine(DrawingCanvas);
            }
            foreach (var circle in circleTool.Circles)
            {
                circle.UseAntialiasing = true;
                circle.RedrawCircle(DrawingCanvas);
            }
            foreach (var polygon in polygonTool.Polygons)
            {
                polygon.UseAntialiasing = true;
                polygon.RedrawPolygon(DrawingCanvas);
            }
            foreach (var rectangle in rectangleTool.Rectangles)
            {
                rectangle.UseAntialiasing = true;
                rectangle.RedrawRectangle(DrawingCanvas);
            }
        }
        private void AntiAlisaing_Unchecked(object sender, RoutedEventArgs e)
        {
            if(!lineTool.UseAntialiasing || !circleTool.UseAntialiasing || !polygonTool.UseAntialiasing || !rectangleTool.UseAntialiasing)
                return;
            else
            {
                lineTool.UseAntialiasing = false;
                circleTool.UseAntialiasing = false;
                polygonTool.UseAntialiasing = false;

                foreach (var line in lineTool.Lines)
                {
                    line.UseAntialiasing = false;
                    line.RedrawLine(DrawingCanvas);
                }
                foreach (var circle in circleTool.Circles)
                {
                    circle.UseAntialiasing = false;
                    circle.RedrawCircle(DrawingCanvas);
                }
                foreach (var polygon in polygonTool.Polygons)
                {
                    polygon.UseAntialiasing = false;
                    polygon.RedrawPolygon(DrawingCanvas);
                }
                foreach (var rectangle in rectangleTool.Rectangles)
                {
                    rectangle.UseAntialiasing = false;
                    rectangle.RedrawRectangle(DrawingCanvas);
                }
            }
        }

        private void SaveCanva_Click(object sender, RoutedEventArgs e)
        {
            var saveDialog = new SaveFileDialog
            {
                Filter = "JSON Files (*.json)|*.json",
                FileName = "canvas.json"
            };

            if (saveDialog.ShowDialog() == true)
            {
                var canvasData = new Shapes
                {
                    Lines = lines,
                    Circles = circles,
                    Polygons = polygons,
                    Rectangles = rectangles,
                };

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    IncludeFields = true
                };

                string json = JsonSerializer.Serialize(canvasData, options);
                File.WriteAllText(saveDialog.FileName, json);
            }
        }

        private void LoadCanva_Click(object sender, RoutedEventArgs e)
        {
            var openDialog = new OpenFileDialog
            {
                Filter = "JSON Files (*.json)|*.json"
            };

            if (openDialog.ShowDialog() == true)
            {
                var options = new JsonSerializerOptions
                {
                    IncludeFields = true
                };

                string json = File.ReadAllText(openDialog.FileName);
                var data = JsonSerializer.Deserialize<Shapes>(json, options);

                if (data != null)
                {
                    DrawingCanvas.Children.Clear();

                    lines.Clear();
                    circles.Clear();
                    polygons.Clear();
                    rectangles.Clear();

                    lineTool.Lines.Clear();
                    circleTool.Circles.Clear();
                    polygonTool.Polygons.Clear();
                    rectangleTool.Rectangles.Clear();

                    if (data.Lines != null)
                    {
                        lines.AddRange(data.Lines);
                        lineTool.Lines.AddRange(data.Lines);
                        foreach (var line in lineTool.Lines)
                            line.RedrawLine(DrawingCanvas);
                    }

                    if (data.Circles != null)
                    {
                        circles.AddRange(data.Circles);
                        circleTool.Circles.AddRange(data.Circles);
                        foreach (var circle in circleTool.Circles)
                            circle.RedrawCircle(DrawingCanvas);
                    }

                    if (data.Polygons != null)
                    {
                        polygons.AddRange(data.Polygons);
                        polygonTool.Polygons.AddRange(data.Polygons);
                        foreach (var polygon in polygonTool.Polygons)
                            polygon.RedrawPolygon(DrawingCanvas);
                    }

                    if (data.Rectangles != null)
                    {
                        rectangles.AddRange(data.Rectangles);
                        rectangleTool.Rectangles.AddRange(data.Rectangles);
                        foreach (var rectangle in rectangleTool.Rectangles)
                            rectangle.RedrawRectangle(DrawingCanvas);
                    }
                }
            }
        }

        private void DrawPacMan_Click(object sender, RoutedEventArgs e)
        {
            isThickening = false;
            isDeleting = false;
            isChangingColors = false;

            isDrawingLine = false;
            isEditingLine = false;

            isDrawingCircle = false;
            isEditingCircle = false;
            isMovingCircle = false;

            isDrawingPolygon = false;
            isMovingPolygonVertex = false;
            isMovingPolygonEdge = false;
            isMovingPolygon = false;

            isDrawingPacMan = true;

            isDrawingRectangle = false;
            isEditingVertexRectangle = false;
            isEditingEdgeRectangle = false;
            isMovingRectangle = false;
        }

        private void DrawRectangle_Click(object sender, RoutedEventArgs e)
        {
            isThickening = false;
            isDeleting = false;
            isChangingColors = false;

            isDrawingLine = false;
            isEditingLine = false;

            isDrawingCircle = false;
            isEditingCircle = false;
            isMovingCircle = false;

            isDrawingPolygon = false;
            isMovingPolygonVertex = false;
            isMovingPolygonEdge = false;
            isMovingPolygon = false;

            isDrawingPacMan = false;

            isDrawingRectangle = true;
            isEditingVertexRectangle = false;
            isEditingEdgeRectangle = false;
            isMovingRectangle = false;
        }

        private void EditVertexRectangle_Click(object sender, RoutedEventArgs e)
        {
            isThickening = false;
            isDeleting = false;
            isChangingColors = false;

            isDrawingLine = false;
            isEditingLine = false;

            isDrawingCircle = false;
            isEditingCircle = false;
            isMovingCircle = false;

            isDrawingPolygon = false;
            isMovingPolygonVertex = false;
            isMovingPolygonEdge = false;
            isMovingPolygon = false;

            isDrawingPacMan = false;

            isDrawingRectangle = false;
            isEditingVertexRectangle = true;
            isEditingEdgeRectangle = false;
            isMovingRectangle = false;
        }

        private void EditEdgeRectangle_Click(object sender, RoutedEventArgs e)
        {
            isThickening = false;
            isDeleting = false;
            isChangingColors = false;

            isDrawingLine = false;
            isEditingLine = false;

            isDrawingCircle = false;
            isEditingCircle = false;
            isMovingCircle = false;

            isDrawingPolygon = false;
            isMovingPolygonVertex = false;
            isMovingPolygonEdge = false;
            isMovingPolygon = false;

            isDrawingPacMan = false;

            isDrawingRectangle = false;
            isEditingVertexRectangle = false;
            isEditingEdgeRectangle = true;
            isMovingRectangle = false;
        }


        private void MoveRectangle_Click(object sender, RoutedEventArgs e)
        {
            isThickening = false;
            isDeleting = false;
            isChangingColors = false;

            isDrawingLine = false;
            isEditingLine = false;

            isDrawingCircle = false;
            isEditingCircle = false;
            isMovingCircle = false;

            isDrawingPolygon = false;
            isMovingPolygonVertex = false;
            isMovingPolygonEdge = false;
            isMovingPolygon = false;

            isDrawingPacMan = false;

            isDrawingRectangle = false;
            isEditingVertexRectangle = false;
            isEditingEdgeRectangle = false;
            isMovingRectangle = true;
        }
        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point click = e.GetPosition(DrawingCanvas);

            if (isDrawingLine)
                lineTool.StartDraw(click);
            else if (isDrawingCircle)
                circleTool.StartDraw(click);
            else if (isDrawingPolygon)
                polygonTool.StartDraw(click);
            else if (isDrawingPacMan)
                pacmanTool.StartDraw(click);
            else if (isDrawingRectangle)
                rectangleTool.StartDraw(click);

            else if (isEditingLine)
            {
                if (lineTool.IsDragging)
                    lineTool.FinishEdit();
                else
                    lineTool.TryEdit(click);
            }
            else if (isEditingCircle)
            {
                if (circleTool.IsDragging)
                    circleTool.FinishEdit();
                else
                    circleTool.TryEdit(click);
            }
            else if (isEditingVertexRectangle)
            {
                if (rectangleTool.IsDragging)
                    rectangleTool.FinishEdit();
                else
                    rectangleTool.TryEditVertex(click);
            }
            else if (isEditingEdgeRectangle)
            {
                if (rectangleTool.IsDragging)
                    rectangleTool.FinishEdit();
                else
                    rectangleTool.TryEditEdge(click);
            }
            else if (isMovingPolygonVertex)
            {
                if (polygonTool.IsMovingVertex)
                    polygonTool.FinishEdit();
                else
                    polygonTool.TryMoveVertex(click);
            }
            else if (isMovingPolygonEdge)
            {
                if (polygonTool.IsMovingEdge)
                    polygonTool.FinishEdit();
                else
                    polygonTool.TryMoveEdge(click);
            }

            else if (isMovingCircle)
            {
                if (circleTool.IsDragging)
                    circleTool.FinishEdit();
                else
                    circleTool.TryMove(click);
            }
            else if (isMovingPolygon)
            {
                if (polygonTool.IsMoving)
                    polygonTool.FinishEdit();
                else
                    polygonTool.TryMove(click);
            }
            else if(isMovingRectangle)
            {
                if (rectangleTool.IsDragging)
                    rectangleTool.FinishEdit();
                else
                    rectangleTool.TryMove(click);
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isEditingLine)
                lineTool.UpdateDrag(e.GetPosition(DrawingCanvas));
            else if (isEditingCircle || isMovingCircle)
                circleTool.UpdateDrag(e.GetPosition(DrawingCanvas));
            else if (isMovingPolygon)
                polygonTool.UpdateDrag(e.GetPosition(DrawingCanvas));
            else if(isMovingPolygonVertex)
                polygonTool.MoveVertex(e.GetPosition(DrawingCanvas));
            else if (isMovingPolygonEdge)
                polygonTool.MoveEdge(e.GetPosition(DrawingCanvas));
            else if (isEditingVertexRectangle || isEditingEdgeRectangle || isMovingRectangle)
                rectangleTool.UpdateDrag(e.GetPosition(DrawingCanvas));
        }

        private void Canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point click = e.GetPosition(DrawingCanvas);

            if (isDeleting)
            {
                lineTool.TryDelete(click);
                circleTool.TryDelete(click);
                polygonTool.TryDelete(click);
                rectangleTool.TryDelete(click);
            }
            else if (isThickening)
            {
                lineTool.TryIncreaseThickness(click);
                circleTool.TryIncreaseThickness(click);
                polygonTool.TryIncreaseThickness(click);
                rectangleTool.TryIncreaseThickness(click);
            }
            else if (isChangingColors)
            {
                lineTool.TryChangeColor(click, selectedColor);
                circleTool.TryChangeColor(click, selectedColor);
                polygonTool.TryChangeColor(click, selectedColor);
                rectangleTool.TryChangeColor(click, selectedColor);
            }
        }

    }
}
