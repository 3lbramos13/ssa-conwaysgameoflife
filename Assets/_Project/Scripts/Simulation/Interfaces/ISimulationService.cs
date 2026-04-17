namespace ConwayLife.Simulation.Interfaces
{
    /// <summary>
    /// Defines the core Game of Life simulation operations.
    /// </summary>
    public interface ISimulationService
    {
        /// <summary>
        /// Advances the simulation by one generation.
        /// </summary>
        void Step();

        /// <summary>
        /// Gets the current generation index.
        /// </summary>
        int Generation { get; }

        /// <summary>
        /// Gets the number of alive cells in the current generation.
        /// </summary>
        int AliveCount { get; }
    }
}
