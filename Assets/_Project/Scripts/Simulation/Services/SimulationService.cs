using ConwayLife.Domain.Interfaces;
using ConwayLife.Domain.Rules;
using ConwayLife.Simulation.Interfaces;

namespace ConwayLife.Simulation.Services
{
    /// <summary>
    /// Runs Game of Life steps over a grid using a rule strategy.
    /// </summary>
    public class SimulationService : ISimulationService
    {
        private readonly IGrid _grid;
        private readonly IGameOfLifeRule _rule;

        /// <summary>
        /// Creates a simulation service for the given grid and rule.
        /// </summary>
        /// <param name="grid">Grid used as simulation state.</param>
        /// <param name="rule">Rule used for state transitions.</param>
        public SimulationService(IGrid grid, IGameOfLifeRule rule)
        {
            _grid = grid;
            _rule = rule;
            Generation = 0;
            AliveCount = CountAliveCells();
        }

        /// <summary>
        /// Gets the current generation index.
        /// </summary>
        public int Generation { get; private set; }

        /// <summary>
        /// Gets the number of alive cells in the current generation.
        /// </summary>
        public int AliveCount { get; private set; }

        /// <summary>
        /// Advances the simulation by one generation.
        /// </summary>
        public void Step()
        {
            int width = _grid.Width;
            int height = _grid.Height;
            bool[,] nextState = new bool[width, height];
            int nextAliveCount = 0;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    bool isAlive = _grid.GetCell(x, y);
                    int neighborCount = CountAliveNeighbors(x, y);
                    bool willLive = _rule.GetNextState(isAlive, neighborCount);

                    nextState[x, y] = willLive;
                    if (willLive)
                    {
                        nextAliveCount++;
                    }
                }
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    _grid.SetCell(x, y, nextState[x, y]);
                }
            }

            Generation++;
            AliveCount = nextAliveCount;
        }

        private int CountAliveNeighbors(int x, int y)
        {
            int count = 0;

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0)
                    {
                        continue;
                    }

                    int neighborX = x + dx;
                    int neighborY = y + dy;

                    if (neighborX < 0 || neighborX >= _grid.Width)
                    {
                        continue;
                    }

                    if (neighborY < 0 || neighborY >= _grid.Height)
                    {
                        continue;
                    }

                    if (_grid.GetCell(neighborX, neighborY))
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        private int CountAliveCells()
        {
            int count = 0;

            for (int x = 0; x < _grid.Width; x++)
            {
                for (int y = 0; y < _grid.Height; y++)
                {
                    if (_grid.GetCell(x, y))
                    {
                        count++;
                    }
                }
            }

            return count;
        }
    }
}
