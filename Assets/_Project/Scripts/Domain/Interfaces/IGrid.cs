namespace ConwayLife.Domain.Interfaces
{
    /// <summary>
    /// Represents a 2D boolean grid for Game of Life state.
    /// </summary>
    public interface IGrid
    {
        /// <summary>
        /// Gets the number of columns in the grid.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Gets the number of rows in the grid.
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Returns whether the cell at the given position is alive.
        /// </summary>
        /// <param name="x">Zero-based column index.</param>
        /// <param name="y">Zero-based row index.</param>
        /// <returns><c>true</c> if alive; otherwise <c>false</c>.</returns>
        bool GetCell(int x, int y);

        /// <summary>
        /// Sets whether the cell at the given position is alive.
        /// </summary>
        /// <param name="x">Zero-based column index.</param>
        /// <param name="y">Zero-based row index.</param>
        /// <param name="isAlive">New alive/dead state.</param>
        void SetCell(int x, int y, bool isAlive);
    }
}
