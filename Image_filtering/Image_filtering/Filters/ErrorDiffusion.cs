using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_filtering.Filters
{
    public static class ErrorDiffusion
    {
        public struct ErrorKernel
        {
            public int[,] KernelValues { get; }
            public int Rows { get; }
            public int Cols { get; }
            public int Div { get; }

            public ErrorKernel(int[,] _kernelValues, int _rows, int _cols, int _div)
            {
                KernelValues = _kernelValues;
                Rows = _rows;
                Cols = _cols;
                Div = _div;
            }
        }

        public enum ErrorDiffusionType
        {
            FloydSteinberg = 1,
            Burkers = 2,
            Stucky = 3,
            Sierra = 4,
            Atkinson = 5
        }

        public static string GetFilePath(int type)
        {
            if (!Enum.IsDefined(typeof(ErrorDiffusionType), type))
                throw new ArgumentException("Invalid error diffusion type.");

            ErrorDiffusionType selectedType = (ErrorDiffusionType)type;
            string fileName = $"{selectedType}.errdiff";

            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "ErrDiffusion", fileName);
            return Path.GetFullPath(filePath);
        }

        public static ErrorKernel LoadKernelFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Kernel file not found: {filePath}");

            string[] lines = File.ReadAllLines(filePath);
            string[] sizeInfo = lines[0].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (sizeInfo.Length != 2 ||
                !int.TryParse(sizeInfo[0], out int rows) ||
                !int.TryParse(sizeInfo[1], out int cols))
            {
                throw new FormatException("Invalid kernel size format. First line should contain two integers: rows and columns.");
            }

            if (!int.TryParse(lines[1], out int div))
            {
                throw new FormatException("Invalid divisor format. Second line should contain one integer.");
            }

            int[,] kernelValues = new int[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                string[] values = lines[i + 2].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (values.Length != cols)
                    throw new FormatException($"Invalid row format at line {i + 2}. Expected {cols} values.");

                for (int j = 0; j < cols; j++)
                {
                    if (!int.TryParse(values[j], NumberStyles.Float, CultureInfo.InvariantCulture, out kernelValues[i, j]))
                    {
                        throw new FormatException($"Invalid number at row {i + 1}, column {j + 1}.");
                    }
                }
            }

            return new ErrorKernel(kernelValues, rows, cols, div);
        }
    }
}
