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
        [Tooltip("Number of columns in the grid.")]
        [SerializeField] public int GridWidth;

        /// <summary>
        /// Number of rows in the simulation grid.
        /// </summary>
        [Tooltip("Number of rows in the grid.")]
        [SerializeField] public int GridHeight;

        /// <summary>
        /// Simulation steps per second.
        /// </summary>
        [Tooltip("How many simulation steps run per second.")]
        [SerializeField] public float SimulationSpeed;

        /// <summary>
        /// Initial random fill amount from 0 to 1.
        /// </summary>
        [Range(0f, 1f)]
        [Tooltip("Chance for a cell to start alive (0 to 1).")]
        [SerializeField] public float RandomFillDensity;

        /// <summary>
        /// Color used to render alive cells.
        /// </summary>
        [Tooltip("Color used for alive cells.")]
        [SerializeField] public Color AliveColor;

        /// <summary>
        /// Color used to render dead cells.
        /// </summary>
        [Tooltip("Color used for dead cells.")]
        [SerializeField] public Color DeadColor;
    }
}
