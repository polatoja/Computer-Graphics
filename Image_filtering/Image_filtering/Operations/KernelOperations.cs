using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using static Image_filtering.Filters.ConvolutionFilters;

namespace Image_filtering.Operations
{
    public static class KernelOperations
    {
        private static string GetFiltersPath()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "ConvFilters");
        }

        public static void ShowKernel(Window owner, WriteableBitmap image)
        {
            KernelWindow kernelWindow = new KernelWindow
            {
                Owner = owner
            };

            if (kernelWindow.ShowDialog() == true)
            {
                Kernel customKernel = kernelWindow.SelectedKernel;

                if (customKernel.KernelValues != null && image is WriteableBitmap writableBitmap)
                {
                    owner.Dispatcher.Invoke(() =>
                    {
                        owner.GetType().GetProperty("ModifiedImage")?.SetValue(owner, ApplyConvolutionFilter(writableBitmap, customKernel));
                    });
                }
            }
        }

        public static ListBox LoadSavedFilters(Grid parentGrid)
        {
            string filtersPath = GetFiltersPath();

            if (!Directory.Exists(filtersPath))
            {
                Directory.CreateDirectory(filtersPath);
            }

            string[] filterFiles = Directory.GetFiles(filtersPath, "*.conv");

            if (filterFiles.Length == 0)
            {
                MessageBox.Show("No saved filters found.", "Filters", MessageBoxButton.OK, MessageBoxImage.Information);
                return null;
            }

            ListBox filtersListBox = new ListBox
            {
                Width = 250,
                Height = 200,
                SelectionMode = SelectionMode.Single,
                Margin = new Thickness(10)
            };

            filtersListBox.MouseDoubleClick += (sender, e) => ApplySelectedFilter(filtersListBox);

            parentGrid.Children.Add(filtersListBox);

            foreach (string file in filterFiles)
            {
                filtersListBox.Items.Add(Path.GetFileName(file));
            }

            return filtersListBox;
        }

        private static void ApplySelectedFilter(ListBox filtersListBox)
        {
            if (filtersListBox.SelectedItem == null)
                return;

            string selectedFilter = filtersListBox.SelectedItem.ToString();
            string filtersPath = Path.Combine(GetFiltersPath(), selectedFilter);

            if (!File.Exists(filtersPath))
            {
                MessageBox.Show("The selected filter file does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                Kernel kernelFilter = LoadKernelFromFile(filtersPath);

                KernelWindow kernelWindow = new KernelWindow();
                kernelWindow.GenerateKernel_Auto(kernelFilter);

                if (kernelWindow.ShowDialog() == true)
                {
                    Kernel customKernel = kernelWindow.SelectedKernel;

                    if (customKernel.KernelValues != null && filtersListBox.Parent is Window window && window.GetType().GetProperty("ModifiedImage")?.GetValue(window) is WriteableBitmap writableBitmap)
                    {
                        window.GetType().GetProperty("ModifiedImage")?.SetValue(window, ApplyConvolutionFilter(writableBitmap, customKernel));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading filter: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static void ChangeAnchor(Window owner, WriteableBitmap image)
        {
            AnchorWindow anchorWindow = new AnchorWindow
            {
                Owner = owner
            };

            if (anchorWindow.ShowDialog() == true)
            {
                Kernel customKernel = anchorWindow.SelectedKernel;

                if (customKernel.KernelValues != null && image is WriteableBitmap writableBitmap)
                {
                    owner.GetType().GetProperty("ModifiedImage")?.SetValue(owner, ApplyConvolutionFilter(writableBitmap, customKernel));
                }
            }
        }
    }
}
