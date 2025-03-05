using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Image_filtering
{
    public partial class KernelWindow : Window
    {
        private int kernelWidth = 3;
        private int kernelHeight = 3;
        private TextBox[,] kernelInputs; // Stores references to all textboxes in the grid

        public KernelWindow()
        {
            InitializeComponent();
        }

        private void GenerateKernel_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(KernelWidthBox.Text, out kernelWidth) || kernelWidth < 1 || kernelWidth > 9 ||
                !int.TryParse(KernelHeightBox.Text, out kernelHeight) || kernelHeight < 1 || kernelHeight > 9)
            {
                MessageBox.Show("Please enter valid kernel dimensions (1-9 in both directions).",
                                "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            KernelGrid.Children.Clear();
            KernelGrid.RowDefinitions.Clear();
            KernelGrid.ColumnDefinitions.Clear();
            kernelInputs = new TextBox[kernelHeight, kernelWidth];

            for (int row = 0; row < kernelHeight; row++)
            {
                KernelGrid.RowDefinitions.Add(new RowDefinition());
                for (int col = 0; col < kernelWidth; col++)
                {
                    if (row == 0) KernelGrid.ColumnDefinitions.Add(new ColumnDefinition());

                    TextBox inputBox = new TextBox
                    {
                        Text = "0",
                        Width = 50,
                        Height = 30,
                        Margin = new Thickness(5),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    Grid.SetRow(inputBox, row);
                    Grid.SetColumn(inputBox, col);
                    KernelGrid.Children.Add(inputBox);

                    kernelInputs[row, col] = inputBox;
                }
            }
        }

        public double[,] GetKernelValues()
        {
            double[,] kernel = new double[kernelHeight, kernelWidth];

            for (int row = 0; row < kernelHeight; row++)
            {
                for (int col = 0; col < kernelWidth; col++)
                {
                    if (!double.TryParse(kernelInputs[row, col].Text, NumberStyles.Float, CultureInfo.InvariantCulture, out kernel[row, col]))
                    {
                        MessageBox.Show("Invalid kernel values detected. Please enter numbers only.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return null;
                    }
                }
            }

            return kernel;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            if (kernelInputs == null)
                return;

            double[,] kernel = GetKernelValues();
        }
    }
}
