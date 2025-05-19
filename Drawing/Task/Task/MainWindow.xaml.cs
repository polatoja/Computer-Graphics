using Microsoft.Win32;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using Task.shapes;

namespace Task
{
    public partial class MainWindow : Window
    {
        public enum EditorMode
        {
            None,

            DrawLine,
            EditLine,

            DrawCircle,
            EditCircle,
            MoveCircle,

            DrawPolygon,
            MovePolygon,
            MovePolygonVertex,
            MovePolygonEdge,

            DrawRectangle,
            MoveRectangleVertex,
            MoveRectangleEdge,
            MoveRectangle,

            DrawPacMan,

            Thicken,
            Delete,
            ChangeColor,
            FillColor,
            FillImage,
            ClipPolygon
        }

        private WriteableBitmap bitmap;
        private List<Shapes> shapes = new();
        private Line? currentLine = null;
        private Line.Dragging? draggingPart = null;
        private Circle? currentCircle = null;
        private Polygon? currentPolygon = null;
        private Rectangle? currentRectangle = null;

        private bool useAntiAliasing = false;
        private Color selectedColor = Colors.Black;
        private EditorMode currentMode = EditorMode.None;

        private List<Point> pointMarkers = new();

        public MainWindow()
        {
            InitializeComponent();

            int width = 900;
            int height = 800;
            bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
            DrawingCanvas.Source = bitmap;
            ClearBitmap(); 
        }

        private void ClearCanvas_Click(object sender, RoutedEventArgs e) => ClearAndDeleteBitmap();
        private void ThickenShape_Click(object sender, RoutedEventArgs e) => currentMode = EditorMode.Thicken;
        private void ChangeColors_Click(object sender, RoutedEventArgs e)
        {
            currentMode = EditorMode.ChangeColor;
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
        private void DeleteShape_Click(object sender, RoutedEventArgs e) => currentMode = EditorMode.Delete;


        private void DrawLine_Click(object sender, RoutedEventArgs e) => currentMode = EditorMode.DrawLine;
        private void EditLine_Click(object sender, RoutedEventArgs e) => currentMode = EditorMode.EditLine;
        private void DrawCircle_Click(object sender, RoutedEventArgs e) => currentMode = EditorMode.DrawCircle;
        private void EditCircle_Click(object sender, RoutedEventArgs e) => currentMode = EditorMode.EditCircle;
        private void MoveCircle_Click(object sender, RoutedEventArgs e) => currentMode = EditorMode.MoveCircle;
        private void DrawPolygon_Click(object sender, RoutedEventArgs e) => currentMode = EditorMode.DrawPolygon;
        private void MovePolygonVertex_Click(object sender, RoutedEventArgs e) => currentMode = EditorMode.MovePolygonVertex;
        private void MovePolygonEdge_Click(object sender, RoutedEventArgs e) => currentMode = EditorMode.MovePolygonEdge;
        private void MovePolygon_Click(object sender, RoutedEventArgs e) => currentMode = EditorMode.MovePolygon;
        private void DrawPacMan_Click(object sender, RoutedEventArgs e) => currentMode = EditorMode.DrawPacMan;
        private void DrawRectangle_Click(object sender, RoutedEventArgs e) => currentMode = EditorMode.DrawRectangle;
        private void MoveRectangleVertex_Click(object sender, RoutedEventArgs e) => currentMode = EditorMode.MoveRectangleVertex;
        private void MoveRectangleEdge_Click(object sender, RoutedEventArgs e) => currentMode = EditorMode.MoveRectangleEdge;
        private void MoveRectangle_Click(object sender, RoutedEventArgs e) => currentMode = EditorMode.MoveRectangle;

        private void SaveCanva_Click(object sender, RoutedEventArgs e)
        {

        }

        private void LoadCanva_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ClearAndDeleteBitmap()
        {
            shapes.Clear();
            currentLine = null;
            currentCircle = null;
            currentPolygon = null;
            currentRectangle = null;
            draggingPart = null;
            pointMarkers.Clear();

            ClearBitmap();
        }

        private void ClearBitmap()
        {
            bitmap.Lock();
            int stride = bitmap.BackBufferStride;
            int size = stride * bitmap.PixelHeight;
            byte[] white = new byte[size];
            for (int i = 0; i < size; i += 4)
            {
                white[i] = 255;     // Blue
                white[i + 1] = 255; // Green
                white[i + 2] = 255; // Red
                white[i + 3] = 255; // Alpha
            }
            bitmap.WritePixels(new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight), white, stride, 0);
            bitmap.Unlock();
        }

        private void DrawPointMarkers()
        {
            foreach (var point in pointMarkers)
            {
                Shapes.DrawPixel(bitmap, (int)point.X, (int)point.Y, Colors.Red, 3);
            }
        }

        private void ErasePointMarkers()
        {
            if (pointMarkers.Count == 0) return;

            foreach (var point in pointMarkers)
            {
                Shapes.DrawPixel(bitmap, (int)point.X, (int)point.Y, Colors.Black, 3);
            }

            pointMarkers.Clear();
        }

        private void AntiAlisaing_Checked(object sender, RoutedEventArgs e)
        {
            ClearBitmap();
            foreach (var shape in shapes)
            {
                shape.UseAntialiasing = true;
                shape.Draw(bitmap);
            }     
        }
        private void AntiAlisaing_Unchecked(object sender, RoutedEventArgs e)
        {
            ClearBitmap();
            foreach (var shape in shapes)
            {
                shape.UseAntialiasing = false;
                shape.Draw(bitmap);
            }
        }

        private void Canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point click = e.GetPosition(DrawingCanvas);

            if (currentMode == EditorMode.Delete)
            {
                foreach(var shape in shapes.ToList())
                {
                    if (shape.IsNearShape(click))
                    {
                        shape.DeleteShape(shapes, shape, bitmap);
                        break;
                    }
                        
                }
            }
            else if (currentMode == EditorMode.Thicken)
            {
                foreach (var shape in shapes.ToList())
                {
                    if (shape.IsNearShape(click))
                    {
                        shape.Thickness += 1;
                        break;
                    }
                }
                ClearBitmap();
                foreach (var shape in shapes)
                    shape.Draw(bitmap);
            }
            else if (currentMode == EditorMode.ChangeColor)
            {
                foreach (var shape in shapes.ToList())
                {
                    if (shape.IsNearShape(click))
                    {
                        shape.Color = selectedColor;
                        break;
                    }
                }
                ClearBitmap();
                foreach (var shape in shapes)
                    shape.Draw(bitmap);
            }
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point click = e.GetPosition(DrawingCanvas);

            if (currentMode == EditorMode.DrawLine)
            {
                if (currentLine == null)
                {
                    currentLine = new Line
                    {
                        Start = click,
                        End = click,
                        Color = Colors.Black,
                        Thickness = 1,
                        UseAntialiasing = useAntiAliasing
                    };
                    pointMarkers.Add(click);
                    DrawPointMarkers();
                }
                else
                {
                    currentLine.End = click;
                    currentLine.Draw(bitmap);
                    shapes.Add(currentLine);
                    currentLine = null;
                    ErasePointMarkers();
                }
            }
            else if (currentMode == EditorMode.EditLine)
            {
                if (currentLine == null)
                {
                    foreach (var shape in shapes)
                    {
                        if (shape is Line line)
                        {
                            if (line.IsNearShape(click))
                            {
                                currentLine = line;
                                draggingPart = Line.Dragging.All;
                                line.lastClick = click;
                                break;
                            }
                            else if (line.IsNearPoint(click, line.Start))
                            {
                                currentLine = line;
                                draggingPart = Line.Dragging.Start;
                                break;
                            }
                            else if (line.IsNearPoint(click, line.End))
                            {
                                currentLine = line;
                                draggingPart = Line.Dragging.End;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    if (draggingPart == Line.Dragging.Start)
                        currentLine.Start = click;
                    else if (draggingPart == Line.Dragging.End)
                        currentLine.End = click;

                    ClearBitmap();
                    foreach (var shape in shapes)
                        shape.Draw(bitmap);

                    currentLine = null;
                    draggingPart = null;
                }
            }
            else if (currentMode == EditorMode.DrawCircle)
            {
                if (currentCircle == null)
                {
                    currentCircle = new Circle
                    {
                        Start = click,
                        Radius = 0,
                        Color = Colors.Black,
                        Thickness = 1,
                        UseAntialiasing = useAntiAliasing
                    };
                    pointMarkers.Add(click);
                    DrawPointMarkers();
                }
                else
                {
                    double dx = click.X - currentCircle.Start.X;
                    double dy = click.Y - currentCircle.Start.Y;
                    currentCircle.Radius = (int)Math.Sqrt(dx * dx + dy * dy);

                    currentCircle.Draw(bitmap);
                    shapes.Add(currentCircle);
                    currentCircle = null;
                    ErasePointMarkers();
                }
            }
            else if (currentMode == EditorMode.MoveCircle)
            {
                if (currentCircle == null)
                {
                    foreach (var shape in shapes)
                    {
                        if (shape is Circle circle)
                        {
                            if(circle.IsNearShape(click))
                            {
                                currentCircle = circle;
                            }
                        }
                    }
                }
                else
                {
                    currentCircle = null;
                }
            }
            else if (currentMode == EditorMode.EditCircle)
            {
                if (currentCircle == null)
                {
                    foreach (var shape in shapes)
                    {
                        if (shape is Circle circle)
                        {
                            if (circle.IsNearShape(click))
                            {
                                currentCircle = circle;
                            }
                        }
                    }
                }
                else
                {
                    currentCircle = null;
                }
            }
            else if (currentMode == EditorMode.DrawPolygon)
            {
                if (currentPolygon == null)
                {
                    currentPolygon = new Polygon
                    {
                        Color = Colors.Black,
                        Thickness = 1,
                        UseAntialiasing = useAntiAliasing
                    };
                    currentPolygon.Vertices.Add(click);
                    pointMarkers.Add(click);
                    DrawPointMarkers();
                    shapes.Add(currentPolygon);
                }
                else
                {
                    if (currentPolygon.Vertices.Count > 2 &&
                        (click - currentPolygon.Vertices[0]).Length < 10)
                    {
                        currentPolygon.Draw(bitmap);
                        shapes.Add(currentPolygon);
                        currentPolygon = null;
                        ErasePointMarkers();
                    }
                    else
                    {
                        currentPolygon.Vertices.Add(click);
                        currentPolygon.numVertices += 1;
                        pointMarkers.Add(click);
                        DrawPointMarkers();
                    }
                }
            }
            else if (currentMode == EditorMode.MovePolygon)
            {
                if (currentPolygon == null)
                {
                    foreach (var shape in shapes)
                    {
                        if (shape is Polygon poly)
                        {
                            if (poly.IsNearShape(click))
                            {
                                currentPolygon = poly;
                            }
                        }
                    }
                }
                else
                {
                    currentPolygon = null;
                }
            }
            else if (currentMode == EditorMode.MovePolygonVertex)
            {
                if (currentPolygon == null)
                {
                    foreach (var shape in shapes)
                    {
                        if (shape is Polygon poly && poly.IsNearShape(click))
                        {
                            currentPolygon = poly;
                            for(int i = 0; i < currentPolygon.numVertices; i++)
                            {
                                if (currentPolygon.IsNearPoint(currentPolygon.Vertices[i], click))
                                {
                                    currentPolygon.selectedVertexIndex = i;
                                    break;
                                }
                            }
                        }
                    }
                    
                }
                else
                {
                    currentPolygon = null;
                }
            }
            else if (currentMode == EditorMode.MovePolygonEdge)
            {
                if (currentPolygon == null)
                {
                    foreach (var shape in shapes)
                    {
                        if (shape is Polygon poly && poly.IsNearShape(click))
                        {
                            currentPolygon = poly;
                            for (int i = 0; i < poly.Vertices.Count; i++)
                            {
                                Point a = poly.Vertices[i];
                                Point b = poly.Vertices[(i + 1) % poly.Vertices.Count];

                                if (currentPolygon.DistanceToSegment(click, a, b) < 10)
                                {
                                    currentPolygon.selectedEdgeIndex = i;
                                    currentPolygon.lastMousePos = click;
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    currentPolygon = null;
                }
            }
            else if (currentMode == EditorMode.DrawRectangle)
            {
                if (currentRectangle == null)
                {
                    currentRectangle = new Rectangle
                    {
                        Color = Colors.Black,
                        Thickness = 1,
                        UseAntialiasing = useAntiAliasing,
                        Vertices = new List<Point> { click }
                    };
                    pointMarkers.Add(click);
                    DrawPointMarkers();
                }
                else
                {
                    Point p1 = currentRectangle.Vertices[0];
                    Point p2 = click;

                    Point topLeft = new Point(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y));
                    Point topRight = new Point(Math.Max(p1.X, p2.X), Math.Min(p1.Y, p2.Y));
                    Point bottomRight = new Point(Math.Max(p1.X, p2.X), Math.Max(p1.Y, p2.Y));
                    Point bottomLeft = new Point(Math.Min(p1.X, p2.X), Math.Max(p1.Y, p2.Y));

                    currentRectangle.Vertices = new List<Point> { topLeft, topRight, bottomRight, bottomLeft };

                    shapes.Add(currentRectangle);
                    currentRectangle.Draw(bitmap);
                    currentRectangle = null;
                    ErasePointMarkers();
                }
            }
            else if (currentMode == EditorMode.MoveRectangle)
            {
                if (currentRectangle == null)
                {
                    foreach (var shape in shapes)
                    {
                        if (shape is Rectangle rect)
                        {
                            if (rect.IsNearShape(click))
                            {
                                currentRectangle = rect;
                            }
                        }
                    }
                }
                else
                {
                    currentRectangle = null;
                }
            }
            else if (currentMode == EditorMode.MoveRectangleVertex)
            {
                if (currentRectangle == null)
                {
                    foreach (var shape in shapes)
                    {
                        if (shape is Rectangle rect && rect.IsNearShape(click))
                        {
                            currentRectangle = rect;
                            for (int i = 0; i < 4; i++)
                            {
                                if (rect.IsNearPoint(rect.Vertices[i], click))
                                {
                                    currentRectangle.selectedVertexIndex = i;
                                    currentRectangle.lastMousePos = click;
                                    break;
                                }
                            }
                        }
                    }

                }
                else
                {
                    currentRectangle = null;
                }
            }
            else if (currentMode == EditorMode.MoveRectangleEdge)
            {
                if (currentRectangle == null)
                {
                    foreach (var shape in shapes)
                    {
                        if (shape is Rectangle rect && rect.IsNearShape(click))
                        {
                            currentRectangle = rect;
                            for (int i = 0; i < 4; i++)
                            {
                                Point a = rect.Vertices[i];
                                Point b = rect.Vertices[(i + 1) % rect.Vertices.Count];

                                if (currentRectangle.DistanceToSegment(click, a, b) < 10)
                                {
                                    currentRectangle.selectedEdgeIndex = i;
                                    currentRectangle.lastMousePos = click;
                                    break;
                                }
                            }
                        }
                    }

                }
                else
                {
                    currentRectangle = null;
                }
            }
        }


        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            Point mousePos = e.GetPosition(DrawingCanvas);

            if (currentMode == EditorMode.EditLine && currentLine != null)
            {
                if (draggingPart == Line.Dragging.All)
                {
                    if (currentLine.lastMousePos != null)
                    {
                        Vector delta = mousePos - currentLine.lastMousePos.Value;
                        currentLine.Start += delta;
                        currentLine.End += delta;
                    }
                    currentLine.lastMousePos = mousePos;
                }
                else if (draggingPart == Line.Dragging.Start)
                    currentLine.Start = mousePos;
                else if (draggingPart == Line.Dragging.End)
                    currentLine.End = mousePos;

                ClearBitmap();
                foreach (var shape in shapes)
                    shape.Draw(bitmap);
            }
            else if (currentMode == EditorMode.MoveCircle && currentCircle != null)
            {
                if (currentCircle.lastMousePos != null)
                {
                    Vector delta = mousePos - currentCircle.lastMousePos.Value;
                    currentCircle.Start += delta;

                    ClearBitmap();
                    foreach (var shape in shapes)
                        shape.Draw(bitmap);
                }

                currentCircle.lastMousePos = mousePos;
            }
            else if (currentMode == EditorMode.EditCircle && currentCircle != null)
            {
                if (currentCircle.lastMousePos != null)
                {
                    double distance = (mousePos - currentCircle.Start).Length;
                    currentCircle.Radius = (int)distance;

                    ClearBitmap();
                    foreach (var shape in shapes)
                        shape.Draw(bitmap);
                }

                currentCircle.lastMousePos = mousePos;
            }
            else if (currentMode == EditorMode.MovePolygon && currentPolygon != null)
            {
                if (currentPolygon.lastMousePos != null)
                {
                    Vector delta = mousePos - currentPolygon.lastMousePos.Value;
                    for(int i = 0; i < currentPolygon.numVertices; i++)
                    {
                        currentPolygon.Vertices[i] += delta;
                    }

                    ClearBitmap();
                    foreach (var shape in shapes)
                        shape.Draw(bitmap);
                }

                currentPolygon.lastMousePos = mousePos;
            }
            else if (currentMode == EditorMode.MovePolygonVertex && currentPolygon != null && currentPolygon.selectedVertexIndex.HasValue)
            {
                currentPolygon.Vertices[currentPolygon.selectedVertexIndex.Value] = mousePos;

                ClearBitmap();
                foreach (var shape in shapes)
                    shape.Draw(bitmap);
            }
            else if (currentMode == EditorMode.MovePolygonEdge && currentPolygon != null && currentPolygon.selectedEdgeIndex.HasValue)
            {
                Vector delta = mousePos - currentPolygon.lastMousePos.Value;

                int i = currentPolygon.selectedEdgeIndex.Value;
                currentPolygon.Vertices[i] += delta;
                currentPolygon.Vertices[(i + 1) % currentPolygon.Vertices.Count] += delta;

                currentPolygon.lastMousePos = mousePos;

                ClearBitmap();
                foreach (var shape in shapes)
                    shape.Draw(bitmap);
            }
            else if (currentMode == EditorMode.MoveRectangle && currentRectangle != null)
            {
                if (currentRectangle.lastMousePos != null)
                {
                    Vector delta = mousePos - currentRectangle.lastMousePos.Value;
                    for (int i = 0; i < 4; i++)
                    {
                        currentRectangle.Vertices[i] += delta;
                    }

                    ClearBitmap();
                    foreach (var shape in shapes)
                        shape.Draw(bitmap);
                }

                currentRectangle.lastMousePos = mousePos;
            }
            else if (currentMode == EditorMode.MoveRectangleVertex && currentRectangle != null && currentRectangle.selectedVertexIndex.HasValue)
            {
                if (currentRectangle.lastMousePos != null)
                {
                    Vector delta = mousePos - currentRectangle.lastMousePos.Value;
                    int id = currentRectangle.selectedVertexIndex.Value;
                    var pts = currentRectangle.Vertices;

                    pts[id] += delta;

                    if (id % 2 == 0)
                    {
                        pts[(id + 1) % 4] = new Point(pts[(id + 1) % 4].X, pts[id].Y);
                        pts[(id + 3) % 4] = new Point(pts[id].X, pts[(id + 3) % 4].Y);
                    }
                    else
                    {
                        pts[(id + 1) % 4] = new Point(pts[id].X, pts[(id + 1) % 4].Y);
                        pts[(id + 3) % 4] = new Point(pts[(id + 3) % 4].X, pts[id].Y);
                    }

                    ClearBitmap();
                    foreach (var shape in shapes)
                        shape.Draw(bitmap);
                }
                currentRectangle.lastMousePos = mousePos;
            }
            else if (currentMode == EditorMode.MoveRectangleEdge && currentRectangle != null && currentRectangle.selectedEdgeIndex.HasValue)
            {
                if (currentRectangle.lastMousePos != null)
                {
                    Vector delta = mousePos - currentRectangle.lastMousePos.Value;
                    int id = currentRectangle.selectedEdgeIndex.Value;
                    var pts = currentRectangle.Vertices;

                    int next = (id + 1) % 4;

                    if (id == 0 || id == 2)
                    {
                        Vector yDelta = new Vector(0, delta.Y);
                        pts[id] += yDelta;
                        pts[next] += yDelta;
                    }
                    else if (id == 1 || id == 3)
                    {
                        Vector xDelta = new Vector(delta.X, 0);
                        pts[id] += xDelta;
                        pts[next] += xDelta;
                    }

                    ClearBitmap();
                    foreach (var shape in shapes)
                        shape.Draw(bitmap);
                }
                currentRectangle.lastMousePos = mousePos;
            }

        }

        
    }
}