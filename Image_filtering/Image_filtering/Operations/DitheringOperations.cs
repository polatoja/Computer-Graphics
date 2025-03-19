using Image_filtering.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using static Image_filtering.Filters.ConvolutionFilters;

namespace Image_filtering.Operations
{
    public static class DitheringOperations
    {
        public static WriteableBitmap ConvertToGrey(WriteableBitmap image)
        {
            return Ditchering.ConvertToGrey(image);
        }

        public static WriteableBitmap ApplyRandomDitchering(WriteableBitmap image)
        {
            return Ditchering.RandomDitchering(image);
        }

        public static WriteableBitmap ApplyAverageDitchering(WriteableBitmap image)
        {
            return Ditchering.AverageDitchering(image);
        }

        public static void ChooseTresholdMapSize(Window owner, WriteableBitmap image)
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
        public static WriteableBitmap ApplyOrderedDitchering(WriteableBitmap image, int size)
        {
            return Ditchering.OrderedDithering(image, size);
        }
    }
}
