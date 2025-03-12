using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using static Image_filtering.ConvolutionFilters;

namespace Image_filtering
{
    public partial class KernelWindow : Window
    {
        private int kernelWidth = 3;
        private int kernelHeight = 3;
        private double kernelDivisor = 1;
        private int kernelOffset = 1;
        private TextBox[,] kernelInputs;
        public Kernel SelectedKernel { get; private set; }

        public KernelWindow()
        {
            InitializeComponent();
        }

        private void GenerateKernel_Click(object sender, RoutedEventArgs e)
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
                        Margin = new Thickness(2),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        TextAlignment = TextAlignment.Center
                    };

                    Grid.SetRow(inputBox, row);
                    Grid.SetColumn(inputBox, col);
                    KernelGrid.Children.Add(inputBox);

                    kernelInputs[row, col] = inputBox;
                }
            }
        }

        public void GenerateKernel_Auto(Kernel kernel)
        {
            KernelGrid.Children.Clear();
            KernelGrid.RowDefinitions.Clear();
            KernelGrid.ColumnDefinitions.Clear();
            kernelInputs = new TextBox[kernel.Rows, kernel.Cols];

            for (int row = 0; row < kernelHeight; row++)
            {
                KernelGrid.RowDefinitions.Add(new RowDefinition());
                for (int col = 0; col < kernelWidth; col++)
                {
                    if (row == 0) KernelGrid.ColumnDefinitions.Add(new ColumnDefinition());

                    TextBox inputBox = new TextBox
                    {
                        Text = kernel.KernelValues[row, col].ToString(CultureInfo.InvariantCulture),
                        Width = 50,
                        Height = 30,
                        Margin = new Thickness(2),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        TextAlignment = TextAlignment.Center
                    };

                    Grid.SetRow(inputBox, row);
                    Grid.SetColumn(inputBox, col);
                    KernelGrid.Children.Add(inputBox);

                    kernelInputs[row, col] = inputBox;
                }
            }
        }
        public Kernel GetKernelValues()
        {
            double[,] kernelValues = new double[kernelHeight, kernelWidth];
            double sum = 0;

            if (!int.TryParse(OffsetBox.Text, out kernelOffset))
            {
                MessageBox.Show("Please enter a valid kernel offset.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return default;
            }

            for (int row = 0; row < kernelHeight; row++)
            {
                for (int col = 0; col < kernelWidth; col++)
                {
                    if (!double.TryParse(kernelInputs[row, col].Text, NumberStyles.Float, CultureInfo.InvariantCulture, out kernelValues[row, col]))
                    {
                        MessageBox.Show("Invalid kernel values detected. Please enter numbers only.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return default;
                    }
                    sum += kernelValues[row, col];
                }
            }

            if (sum != 0 && sum != 1)
            {
                kernelDivisor = sum;
                for (int row = 0; row < kernelHeight; row++)
                {
                    for (int col = 0; col < kernelWidth; col++)
                    {
                        kernelValues[row, col] /= kernelDivisor;
                    }
                }

                MessageBox.Show($"Kernel normalized. New divisor: {kernelDivisor}", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            return new Kernel(kernelValues, kernelHeight, kernelWidth, kernelOffset, kernelOffset);
        }



        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            if (kernelInputs == null)
                return;

            Kernel kernel = GetKernelValues();
            if (kernel.KernelValues == null)
                return;

            SelectedKernel = kernel;

            DialogResult = true;
            this.Close();
        }

        /*
        private void Apply_Save_Click(object sender, RoutedEventArgs e)
        {
            if (kernelInputs == null)
                return;

            Kernel kernel = GetKernelValues();
            if (kernel.KernelValues == null)
                return;

            SelectedKernel = kernel;

            SaveKernelToFile(kernel);

            DialogResult = true;
            this.Close();
        }
        */

        private void ApplySave_Click(object sender, RoutedEventArgs e)
        {
            if (kernelInputs == null)
                return;

            Kernel kernel = GetKernelValues();
            if (kernel.KernelValues == null)
                return;

            SelectedKernel = kernel;

            FilenameTextBox.Visibility = Visibility.Visible;
            SaveFilterButton.Visibility = Visibility.Visible;
        }

        private void SaveFilter_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FilenameTextBox.Text))
            {
                MessageBox.Show("Please enter a valid filename.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string filename = FilenameTextBox.Text.Trim();
            SaveKernelToFile(SelectedKernel, filename);
        }



        public void LoadKernelIntoGrid(double[,] kernelValues, int rows, int cols)
        {
            kernelWidth = cols;
            kernelHeight = rows;
            KernelGrid.Children.Clear();
            KernelGrid.RowDefinitions.Clear();
            KernelGrid.ColumnDefinitions.Clear();
            kernelInputs = new TextBox[rows, cols];

            for (int row = 0; row < rows; row++)
            {
                KernelGrid.RowDefinitions.Add(new RowDefinition());
                for (int col = 0; col < cols; col++)
                {
                    if (row == 0) KernelGrid.ColumnDefinitions.Add(new ColumnDefinition());

                    TextBox inputBox = new TextBox
                    {
                        Text = kernelValues[row, col].ToString(CultureInfo.InvariantCulture),
                        Width = 50,
                        Height = 30,
                        Margin = new Thickness(2),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        TextAlignment = TextAlignment.Center
                    };

                    Grid.SetRow(inputBox, row);
                    Grid.SetColumn(inputBox, col);
                    KernelGrid.Children.Add(inputBox);
                    kernelInputs[row, col] = inputBox;
                }
            }
        }

    }
}
