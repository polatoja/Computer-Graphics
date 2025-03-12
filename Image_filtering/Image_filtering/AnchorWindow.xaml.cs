using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static Image_filtering.ConvolutionFilters;

namespace Image_filtering
{
    public partial class AnchorWindow : Window
    {
        private int kernelWidth = 3;
        private int kernelHeight = 3;
        private Button[,] kernelButtons;
        private int anchorRow;
        private int anchorCol;
        public Kernel SelectedKernel;

        public AnchorWindow()
        {
            InitializeComponent();
            GenerateGrid();
        }

        private void GenerateGrid()
        {
            if (!int.TryParse(KernelWidthBox.Text, out kernelWidth) || kernelWidth < 1 || kernelWidth > 9 || kernelWidth % 2 == 0 ||
                !int.TryParse(KernelHeightBox.Text, out kernelHeight) || kernelHeight < 1 || kernelHeight > 9 || kernelHeight % 2 == 0)
            {
                MessageBox.Show("Please enter valid kernel dimensions (1-9 ODD values in both directions).",
                                "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            KernelGrid.Children.Clear();
            KernelGrid.RowDefinitions.Clear();
            KernelGrid.ColumnDefinitions.Clear();
            kernelButtons = new Button[kernelHeight, kernelWidth];

            SelectedKernel = new Kernel(new double[kernelHeight, kernelWidth], kernelHeight, kernelWidth, 0, 0);

            anchorRow = kernelHeight / 2;
            anchorCol = kernelWidth / 2;
            SelectedKernel.AnchorRow = 0;
            SelectedKernel.AnchorCol = 0;

            for (int row = 0; row < kernelHeight; row++)
            {
                KernelGrid.RowDefinitions.Add(new RowDefinition());
                for (int col = 0; col < kernelWidth; col++)
                {
                    if (row == 0) KernelGrid.ColumnDefinitions.Add(new ColumnDefinition());

                    int relativeRow = anchorRow - row;
                    int relativeCol = col - anchorCol;

                    Button cellButton = new Button
                    {
                        Content = (row == anchorRow && col == anchorCol) ? "X" : $"({relativeRow}, {relativeCol})",
                        Width = 50,
                        Height = 50,
                        Margin = new Thickness(2),
                        Background = Brushes.White
                    };

                    int r = row, c = col;
                    cellButton.Click += (s, ev) => ChangeAnchor(r, c);

                    Grid.SetRow(cellButton, row);
                    Grid.SetColumn(cellButton, col);
                    KernelGrid.Children.Add(cellButton);

                    kernelButtons[row, col] = cellButton;
                }
            }
        }

        private void ChangeAnchor(int newRow, int newCol)
        {
            kernelButtons[anchorRow, anchorCol].Content = $"({anchorRow - anchorRow}, {anchorCol - anchorCol})";

            anchorRow = newRow;
            anchorCol = newCol;

            kernelButtons[anchorRow, anchorCol].Content = "X";
        }

        private void ApplyAnchor_Click(object sender, RoutedEventArgs e)
        {
            SelectedKernel.AnchorRow = anchorRow - (kernelHeight / 2);
            SelectedKernel.AnchorCol = anchorCol - (kernelWidth / 2);
            MessageBox.Show($"Anchor set at: ({SelectedKernel.AnchorRow}, {SelectedKernel.AnchorCol})", "Anchor Applied", MessageBoxButton.OK, MessageBoxImage.Information);
            this.DialogResult = true;
            this.Close();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
