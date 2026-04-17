namespace ConwayLife.Domain.Rules
{
    /// <summary>
    /// Defines how a cell transitions to its next state.
    /// </summary>
    public interface IGameOfLifeRule
    {
        /// <summary>
        /// Calculates whether a cell is alive in the next generation.
        /// </summary>
        /// <param name="isAlive">Current alive/dead state of the cell.</param>
        /// <param name="neighborCount">Number of live neighbors around the cell.</param>
        /// <returns><c>true</c> if the cell should be alive next; otherwise <c>false</c>.</returns>
        bool GetNextState(bool isAlive, int neighborCount);
    }
}
