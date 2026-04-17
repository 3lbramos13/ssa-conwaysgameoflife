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
        [Tooltip("Configuration asset with grid, speed, and color settings.")]
        [SerializeField] private GameOfLifeConfig _config;
        [Tooltip("View component used to draw the grid.")]
        [SerializeField] private GridView _gridView;

        private IGrid _grid;
        private ISimulationService _simulationService;
        private float _simulationSpeed;
        private float _stepTimer;
        private bool _isPlaying = true;

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

            _grid = new GridModel(_config.GridWidth, _config.GridHeight);
            IGameOfLifeRule rule = new ConwayRule();
            _simulationService = new SimulationService(_grid, rule);
            _simulationSpeed = _config.SimulationSpeed;

            ServiceLocator.Register<IGrid>(_grid);
            ServiceLocator.Register<IGameOfLifeRule>(rule);
            ServiceLocator.Register<ISimulationService>(_simulationService);

            InitializeGridState(_config.RandomFillDensity);
            _gridView.Initialize(_grid.Width, _grid.Height);
            _gridView.SetColors(_config.AliveColor, _config.DeadColor);
            _gridView.Render(_grid);
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

            InitializeGridState(_config.RandomFillDensity);
            _gridView.Render(_grid);
        }

        private void InitializeGridState(float fillDensity)
        {
            for (int x = 0; x < _grid.Width; x++)
            {
                for (int y = 0; y < _grid.Height; y++)
                {
                    bool isAlive = Random.value < fillDensity;
                    _grid.SetCell(x, y, isAlive);
                }
            }
        }
    }
}

