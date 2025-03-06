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
        private int kernelDivisor = 1;
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

        public Kernel GetKernelValues()
        {
            double[,] kernelValues = new double[kernelHeight, kernelWidth];

            if (!int.TryParse(OffsetBox.Text, out kernelOffset) || kernelOffset == 0)
            {
                MessageBox.Show("Please enter valid kernel divisor",
                                "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            if (!int.TryParse(KernelDivisorBox.Text, out kernelDivisor) || kernelDivisor == 0)
            {
                MessageBox.Show("Please enter valid kernel divisor",
                                "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                    kernelValues[row, col] /= kernelDivisor;
                }
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
    }
}
