using Rasterization.Drawing;
using Rasterization.Tools;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace Rasterization
{
    public partial class MainWindow : Window
    {

        private bool isThickening = false;
        private bool isDeleting = false;


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

        public MainWindow()
        {
            InitializeComponent();
            lineTool = new LineTool(DrawingCanvas, lines);
            circleTool = new CircleTool(DrawingCanvas, circles);
            polygonTool = new PolygonTool(DrawingCanvas, polygons);

            DrawingCanvas.MouseLeftButtonDown += Canvas_MouseLeftButtonDown;
            DrawingCanvas.MouseRightButtonDown += Canvas_MouseRightButtonDown;
            DrawingCanvas.MouseMove += Canvas_MouseMove;
        }

        private void DrawLine_Click(object sender, RoutedEventArgs e)
        {
            isThickening = false;
            isDeleting = false;

            isDrawingLine = true;
            isEditingLine = false;

            isDrawingCircle = false;
            isEditingCircle = false;
            isMovingCircle = false;

            isDrawingPolygon = false;
            isMovingPolygonVertex = false;
            isMovingPolygonEdge = false;
            isMovingPolygon = false;
        }

        private void EditLine_Click(object sender, RoutedEventArgs e)
        {
            isThickening = false;
            isDeleting = false;

            isDrawingLine = false;
            isEditingLine = true;

            isDrawingCircle = false;
            isEditingCircle = false;
            isMovingCircle = false;

            isDrawingPolygon = false;
            isMovingPolygonVertex = false;
            isMovingPolygonEdge = false;
            isMovingPolygon = false;
        }

        private void ThickenLine_Click(object sender, RoutedEventArgs e)
        {
            isThickening = false;
            isDeleting = false;

            isDrawingLine = false;
            isEditingLine = false;

            isDrawingCircle = false;
            isEditingCircle = false;
            isMovingCircle = false;

            isDrawingPolygon = false;
            isMovingPolygonVertex = false;
            isMovingPolygonEdge = false;
            isMovingPolygon = false;
        }

        private void DeleteLine_Click(object sender, RoutedEventArgs e)
        {
            isThickening = false;
            isDeleting = false;

            isDrawingLine = false;
            isEditingLine = false;

            isDrawingCircle = false;
            isEditingCircle = false;
            isMovingCircle = false;

            isDrawingPolygon = false;
            isMovingPolygonVertex = false;
            isMovingPolygonEdge = false;
            isMovingPolygon = false;
        }

        private void DrawCircle_Click(object sender, RoutedEventArgs e)
        {
            isThickening = false;
            isDeleting = false;

            isDrawingCircle = true;
            isEditingCircle = false;

            isDrawingLine = false;
            isEditingLine = false;
            isMovingCircle = false;

            isDrawingPolygon = false;
            isMovingPolygonVertex = false;
            isMovingPolygonEdge = false;
            isMovingPolygon = false;
        }

        private void EditCircle_Click(object sender, RoutedEventArgs e)
        {
            isThickening = false;
            isDeleting = false;

            isDrawingCircle = false;
            isEditingCircle = true;
            isMovingCircle = false;

            isDrawingLine = false;
            isEditingLine = false;

            isDrawingPolygon = false;
            isMovingPolygonVertex = false;
            isMovingPolygonEdge = false;
            isMovingPolygon = false;
        }

        private void MoveCircle_Click(object sender, RoutedEventArgs e)
        {
            isThickening = false;
            isDeleting = false;

            isDrawingCircle = false;
            isEditingCircle = false;
            isMovingCircle = true;

            isDrawingLine = false;
            isEditingLine = false;

            isDrawingPolygon = false;
            isMovingPolygonVertex = false;
            isMovingPolygonEdge = false;
            isMovingPolygon = false;
        }

        private void ThickenCircle_Click(object sender, RoutedEventArgs e)
        {
            isThickening = false;
            isDeleting = false;

            isDrawingCircle = false;
            isEditingCircle = false;
            isMovingCircle = false;

            isDrawingLine = false;
            isEditingLine = false;

            isDrawingPolygon = false;
            isMovingPolygonVertex = false;
            isMovingPolygonEdge = false;
            isMovingPolygon = false;
        }

        private void DeleteCircle_Click(object sender, RoutedEventArgs e)
        {
            isThickening = false;
            isDeleting = false;

            isDrawingCircle = false;
            isEditingCircle = false;
            isMovingCircle = false;

            isDrawingLine = false;
            isEditingLine = false;

            isDrawingPolygon = false;
            isMovingPolygonVertex = false;
            isMovingPolygonEdge = false;
            isMovingPolygon = false;
        }

        private void DrawPolygon_Click(object sender, RoutedEventArgs e)
        {
            isThickening = false;
            isDeleting = false;

            isDrawingPolygon = true;
            isMovingPolygonVertex = false;
            isMovingPolygonEdge = false;
            isMovingPolygon = false;

            isDrawingLine = false;
            isEditingLine = false;

            isDrawingCircle = false;
            isEditingCircle = false;
            isMovingCircle = false;
        }

        private void MovePolygonVertex_Click(object sender, RoutedEventArgs e)
        {
            isThickening = false;
            isDeleting = false;

            isDrawingPolygon = false;
            isMovingPolygonVertex = true;
            isMovingPolygonEdge = false;
            isMovingPolygon = false;

            isDrawingLine = false;
            isEditingLine = false;

            isDrawingCircle = false;
            isEditingCircle = false;
            isMovingCircle = false;
        }

        private void MovePolygonEdge_Click(object sender, RoutedEventArgs e)
        {
            isThickening = false;
            isDeleting = false;

            isDrawingPolygon = false;
            isMovingPolygonVertex = false;
            isMovingPolygonEdge = true;
            isMovingPolygon = false;

            isDrawingLine = false;
            isEditingLine = false;

            isDrawingCircle = false;
            isEditingCircle = false;
            isMovingCircle = false;
        }

        private void MovePolygon_Click(object sender, RoutedEventArgs e)
        {
            isThickening = false;
            isDeleting = false;

            isDrawingPolygon = false;
            isMovingPolygonVertex = false;
            isMovingPolygonEdge = false;
            isMovingPolygon = true;

            isDrawingLine = false;
            isEditingLine = false;

            isDrawingCircle = false;
            isEditingCircle = false;
            isMovingCircle = false;
        }

        private void ClearCanvas_Click(object sender, RoutedEventArgs e)
        {
            lineTool.ClearCanvas();
            circleTool.ClearCanvas();
            polygonTool.ClearCanvas();
        }

        private void ThickenShape_Click(object sender, RoutedEventArgs e)
        {
            isThickening = true;
            isDeleting = false;

            isDrawingPolygon = false;
            isMovingPolygonVertex = false;
            isMovingPolygon = false;

            isDrawingLine = false;
            isEditingLine = false;

            isDrawingCircle = false;
            isEditingCircle = false;
            isMovingCircle = false;
        }

        private void DeleteShape_Click(object sender, RoutedEventArgs e)
        {
            isThickening = false;
            isDeleting = true;

            isDrawingPolygon = false;
            isMovingPolygonVertex = false;
            isMovingPolygon = false;

            isDrawingLine = false;
            isEditingLine = false;

            isDrawingCircle = false;
            isEditingCircle = false;
            isMovingCircle = false;
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
        }

        private void Canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point click = e.GetPosition(DrawingCanvas);

            if (isDeleting)
            {
                lineTool.TryDelete(click);
                circleTool.TryDelete(click);
                polygonTool.TryDelete(click);
            }
            else if (isThickening)
            {
                lineTool.TryIncreaseThickness(click);
                circleTool.TryIncreaseThickness(click);
                polygonTool.TryIncreaseThickness(click);
            }
        }
    }
}
