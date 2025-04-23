using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace Rasterization.Tools
{
    public static class BrushUtils
    {
        public static bool[,] CreateCircularBrush(int thickness)
        {
            int n = thickness;
            bool[,] brush = new bool[n, n];
            int center = n / 2;

            for (int y = 0; y < n; y++)
            {
                for (int x = 0; x < n; x++)
                {
                    int dx = x - center;
                    int dy = y - center;
                    if (dx * dx + dy * dy <= center * center)
                    {
                        brush[x, y] = true;
                    }
                }
            }

            return brush;
        }
    }

}
