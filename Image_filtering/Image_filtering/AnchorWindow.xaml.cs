using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static Image_filtering.ConvolutionFilters;

namespace Image_filtering
{
    /// <summary>
    /// Interaction logic for AnchorWindow.xaml
    /// </summary>
    public partial class AnchorWindow : Window
    {
        private int kernelWidth = 3;
        private int kernelHeight = 3;
        private TextBox[,] kernelInputs;
        public Kernel SelectedKernel { get; private set; }

        public AnchorWindow()
        {
            InitializeComponent();

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
                        Text = (row == kernelHeight / 2 && col == kernelWidth / 2) ? "X" : "",
                        Width = 50,
                        Height = 30,
                        Margin = new Thickness(2),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        TextAlignment = TextAlignment.Center,
                    };

                    Grid.SetRow(inputBox, row);
                    Grid.SetColumn(inputBox, col);
                    KernelGrid.Children.Add(inputBox);

                    kernelInputs[row, col] = inputBox;
                }
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ApplyAnchor_Click(object sender, RoutedEventArgs e)
        {
            
        }

    }
}
