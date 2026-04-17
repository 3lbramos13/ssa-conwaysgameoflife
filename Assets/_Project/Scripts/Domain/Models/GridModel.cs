using ConwayLife.Domain.Interfaces;

namespace ConwayLife.Domain.Models
{
    /// <summary>
    /// Stores the alive/dead state for a fixed 2D grid.
    /// </summary>
    public class GridModel : IGrid
    {
        private readonly bool[,] _cells;

        /// <summary>
        /// Creates a grid with the given size.
        /// </summary>
        /// <param name="width">Number of columns.</param>
        /// <param name="height">Number of rows.</param>
        public GridModel(int width, int height)
        {
            Width = width;
            Height = height;
            _cells = new bool[width, height];
        }

        /// <summary>
        /// Gets the number of columns.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Gets the number of rows.
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// Gets the state of a cell.
        /// </summary>
        /// <param name="x">Zero-based column index.</param>
        /// <param name="y">Zero-based row index.</param>
        /// <returns><c>true</c> if alive; otherwise <c>false</c>.</returns>
        public bool GetCell(int x, int y)
        {
            return _cells[x, y];
        }

        /// <summary>
        /// Sets the state of a cell.
        /// </summary>
        /// <param name="x">Zero-based column index.</param>
        /// <param name="y">Zero-based row index.</param>
        /// <param name="isAlive">Cell state to store.</param>
        public void SetCell(int x, int y, bool isAlive)
        {
            _cells[x, y] = isAlive;
        }
    }
}
