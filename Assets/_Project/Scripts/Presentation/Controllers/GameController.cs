using ConwayLife.Configuration;
using ConwayLife.Domain.Interfaces;
using ConwayLife.Domain.Models;
using ConwayLife.Domain.Rules;
using ConwayLife.Presentation.Views;
using ConwayLife.Services;
using ConwayLife.Simulation.Interfaces;
using ConwayLife.Simulation.Services;
using UnityEngine;

namespace ConwayLife.Presentation.Controllers
{
    /// <summary>
    /// Composes and drives the Game of Life runtime flow.
    /// </summary>
    public class GameController : MonoBehaviour
    {
        private static readonly Vector2Int[] GliderOffsets =
        {
            new Vector2Int(1, 0),
            new Vector2Int(2, 1),
            new Vector2Int(0, 2),
            new Vector2Int(1, 2),
            new Vector2Int(2, 2)
        };

        private static readonly Vector2Int[] BlinkerOffsets =
        {
            new Vector2Int(0, 0),
            new Vector2Int(1, 0),
            new Vector2Int(2, 0)
        };

        private const int PatternMargin = 2;
        private const int GliderPatternSize = 3;

        [Tooltip("Configuration asset with grid, speed, and color settings.")]
        [SerializeField] private GameOfLifeConfig _config;
        [Tooltip("View component used to draw the grid.")]
        [SerializeField] private GridView _gridView;

        private IGrid _grid;
        private ISimulationService _simulationService;
        private float _simulationSpeed;
        private float _stepTimer;
        private bool _isPlaying = true;
        private float _density;

        /// <summary>
        /// Gets whether simulation stepping is currently active.
        /// </summary>
        public bool IsPlaying => _isPlaying;

        /// <summary>
        /// Gets the current generation value.
        /// </summary>
        public int Generation => _simulationService != null ? _simulationService.Generation : 0;

        /// <summary>
        /// Gets the current alive cell count.
        /// </summary>
        public int AliveCount => _simulationService != null ? _simulationService.AliveCount : 0;

        /// <summary>
        /// Gets the current simulation speed in steps per second.
        /// </summary>
        public float SimulationSpeed => _simulationSpeed;

        /// <summary>
        /// Gets the active config asset.
        /// </summary>
        public GameOfLifeConfig CurrentConfig => _config;

        /// <summary>
        /// Gets the current random fill density used by <see cref="InitializeGridState"/>.
        /// </summary>
        public float CurrentDensity => _density;

        /// <summary>
        /// Builds the app graph and prepares the first frame.
        /// </summary>
        private void Start()
        {
            if (_config == null)
            {
                Debug.LogError("GameController requires a GameOfLifeConfig reference.");
                enabled = false;
                return;
            }

            if (_gridView == null)
            {
                Debug.LogError("GameController requires a GridView reference.");
                enabled = false;
                return;
            }

            _density = _config.RandomFillDensity;
            ApplyConfig(_config);
        }

        /// <summary>
        /// Advances the simulation based on elapsed time.
        /// </summary>
        private void Update()
        {
            if (_simulationService == null)
            {
                return;
            }

            if (!_isPlaying)
            {
                return;
            }

            float speed = Mathf.Max(_simulationSpeed, 0.0001f);
            float stepInterval = 1f / speed;
            _stepTimer += Time.deltaTime;

            while (_stepTimer >= stepInterval)
            {
                _simulationService.Step();
                _gridView.Render(_grid);
                _stepTimer -= stepInterval;
            }
        }

        /// <summary>
        /// Toggles simulation playback.
        /// </summary>
        public void TogglePlayPause()
        {
            _isPlaying = !_isPlaying;
        }

        /// <summary>
        /// Sets simulation speed in steps per second.
        /// </summary>
        /// <param name="speed">Requested speed value.</param>
        public void SetSimulationSpeed(float speed)
        {
            _simulationSpeed = Mathf.Max(speed, 0f);
        }

        /// <summary>
        /// Applies a fresh random state and re-renders the grid.
        /// </summary>
        public void RandomizeGrid()
        {
            if (_grid == null)
            {
                return;
            }

            InitializeGridState(_density);
            _gridView.Render(_grid);
        }

        /// <summary>
        /// Applies a config asset and rebuilds runtime state from it.
        /// </summary>
        /// <param name="selectedConfig">Config asset to apply.</param>
        public void ApplyConfig(GameOfLifeConfig selectedConfig)
        {
            if (selectedConfig == null)
            {
                return;
            }

            _config = selectedConfig;

            _grid = new GridModel(_config.GridWidth, _config.GridHeight);
            IGameOfLifeRule rule = new ConwayRule();
            _simulationService = new SimulationService(_grid, rule);
            _simulationSpeed = _config.SimulationSpeed;
            _stepTimer = 0f;

            ServiceLocator.Register<IGrid>(_grid);
            ServiceLocator.Register<IGameOfLifeRule>(rule);
            ServiceLocator.Register<ISimulationService>(_simulationService);

            InitializeFromConfigMode();
            _gridView.Initialize(_grid.Width, _grid.Height);
            _gridView.SetColors(_config.AliveColor, _config.DeadColor);
            _gridView.Render(_grid);
        }

        /// <summary>
        /// Initializes the grid using the configured startup mode.
        /// </summary>
        private void InitializeFromConfigMode()
        {
            switch (_config.SelectedInitializationMode)
            {
                case InitializationMode.Random:
                    InitializeGridState(_config.RandomFillDensity);
                    break;
                case InitializationMode.Pattern:
                    InitializePattern();
                    break;
                case InitializationMode.Mixed:
                    InitializeGridState(_config.RandomFillDensity);
                    InitializePattern();
                    break;
                default:
                    InitializeGridState(_config.RandomFillDensity);
                    break;
            }
        }

        /// <summary>
        /// Places a small set of predefined starter patterns.
        /// </summary>
        private void InitializePattern()
        {
            SpawnGlider(PatternMargin, PatternMargin);
            SpawnBlinker(_grid.Width / 2, _grid.Height / 2);

            int farX = Mathf.Max(PatternMargin, _grid.Width - (GliderPatternSize + PatternMargin));
            int farY = Mathf.Max(PatternMargin, _grid.Height - (GliderPatternSize + PatternMargin));
            SpawnGlider(farX, farY);
        }

        /// <summary>
        /// Spawns a glider pattern from a top-left anchor.
        /// </summary>
        /// <param name="x">Anchor column.</param>
        /// <param name="y">Anchor row.</param>
        private void SpawnGlider(int x, int y)
        {
            for (int i = 0; i < GliderOffsets.Length; i++)
            {
                Vector2Int offset = GliderOffsets[i];
                SetAliveIfInside(x + offset.x, y + offset.y);
            }
        }

        /// <summary>
        /// Spawns a blinker pattern from a left anchor.
        /// </summary>
        /// <param name="x">Anchor column.</param>
        /// <param name="y">Anchor row.</param>
        private void SpawnBlinker(int x, int y)
        {
            for (int i = 0; i < BlinkerOffsets.Length; i++)
            {
                Vector2Int offset = BlinkerOffsets[i];
                SetAliveIfInside(x + offset.x, y + offset.y);
            }
        }

        /// <summary>
        /// Sets a cell alive only when it is inside grid bounds.
        /// </summary>
        /// <param name="x">Cell column.</param>
        /// <param name="y">Cell row.</param>
        private void SetAliveIfInside(int x, int y)
        {
            if (x < 0 || x >= _grid.Width)
            {
                return;
            }

            if (y < 0 || y >= _grid.Height)
            {
                return;
            }

            _grid.SetCell(x, y, true);
        }

        /// <summary>
        /// Fills the grid using the current <see cref="_density"/> value. The
        /// <paramref name="fillDensity"/> parameter is accepted for call-site
        /// compatibility but is not used.
        /// </summary>
        /// <param name="fillDensity">Unused. Density is read from <see cref="_density"/>.</param>
        private void InitializeGridState(float fillDensity)
        {
            float clampedDensity = Mathf.Clamp01(_density);

            for (int x = 0; x < _grid.Width; x++)
            {
                for (int y = 0; y < _grid.Height; y++)
                {
                    bool isAlive = Random.value < clampedDensity;
                    _grid.SetCell(x, y, isAlive);
                }
            }
        }

        /// <summary>
        /// Sets the random fill density used by <see cref="RandomizeGrid"/>.
        /// </summary>
        /// <param name="value">Density value clamped to [0, 1].</param>
        public void SetDensity(float value)
        {
            _density = Mathf.Clamp01(value);
        }

        /// <summary>
        /// Forwards a new cell size to the view and re-renders the current grid.
        /// </summary>
        /// <param name="size">Cell size in pixels.</param>
        public void SetCellSize(float size)
        {
            _gridView.SetCellSize(size);
            _gridView.Render(_grid);
        }
    }
}

