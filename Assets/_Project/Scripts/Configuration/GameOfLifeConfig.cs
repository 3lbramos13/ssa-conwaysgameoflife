using UnityEngine;

namespace ConwayLife.Configuration
{
    /// <summary>
    /// Stores editable settings for the Game of Life setup and visuals.
    /// </summary>
    [CreateAssetMenu(fileName = "GameOfLifeConfig", menuName = "Conway Life/Game Of Life Config")]
    public class GameOfLifeConfig : ScriptableObject
    {
        /// <summary>
        /// Number of columns in the simulation grid.
        /// </summary>
        [SerializeField] public int GridWidth;

        /// <summary>
        /// Number of rows in the simulation grid.
        /// </summary>
        [SerializeField] public int GridHeight;

        /// <summary>
        /// Simulation steps per second.
        /// </summary>
        [SerializeField] public float SimulationSpeed;

        /// <summary>
        /// Initial random fill amount from 0 to 1.
        /// </summary>
        [Range(0f, 1f)]
        [SerializeField] public float RandomFillDensity;

        /// <summary>
        /// Color used to render alive cells.
        /// </summary>
        [SerializeField] public Color AliveColor;

        /// <summary>
        /// Color used to render dead cells.
        /// </summary>
        [SerializeField] public Color DeadColor;
    }
}
