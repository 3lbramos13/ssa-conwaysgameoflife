namespace ConwayLife.Domain.Rules
{
    /// <summary>
    /// Applies the standard Conway's Game of Life transition rules.
    /// </summary>
    public class ConwayRule : IGameOfLifeRule
    {
        /// <summary>
        /// Returns whether a cell should be alive in the next generation.
        /// </summary>
        /// <param name="isAlive">Current alive/dead state.</param>
        /// <param name="neighborCount">Count of live neighboring cells.</param>
        /// <returns><c>true</c> if the next state is alive; otherwise <c>false</c>.</returns>
        public bool ShouldLive(bool isAlive, int neighborCount)
        {
            if (isAlive)
            {
                if (neighborCount < 2)
                {
                    return false;
                }

                if (neighborCount == 2 || neighborCount == 3)
                {
                    return true;
                }

                return false;
            }

            return neighborCount == 3;
        }

        /// <summary>
        /// Computes the next state using the configured Conway rules.
        /// </summary>
        /// <param name="isAlive">Current alive/dead state.</param>
        /// <param name="neighborCount">Count of live neighboring cells.</param>
        /// <returns><c>true</c> if the next state is alive; otherwise <c>false</c>.</returns>
        public bool GetNextState(bool isAlive, int neighborCount)
        {
            return ShouldLive(isAlive, neighborCount);
        }
    }
}
