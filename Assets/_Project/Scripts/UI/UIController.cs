using System.Collections;
using System.Collections.Generic;
using ConwayLife.Configuration;
using ConwayLife.Presentation.Controllers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ConwayLife.UI
{
    /// <summary>
    /// Handles UI input and displays runtime simulation data.
    /// </summary>
    public class UIController : MonoBehaviour
    {
        [Tooltip("Game controller used for all simulation commands.")]
        [SerializeField] private GameController _gameController;
        [Tooltip("Optional label for the play/pause button.")]
        [SerializeField] private TMP_Text _playPauseLabel;
        [Tooltip("Slider used to control simulation speed.")]
        [SerializeField] private Slider _speedSlider;
        [Tooltip("Text field that shows current generation.")]
        [SerializeField] private TMP_Text _generationText;
        [Tooltip("Text field that shows current alive cell count.")]
        [SerializeField] private TMP_Text _aliveCountText;
        [Tooltip("Dropdown used to choose an active GameOfLifeConfig.")]
        [SerializeField] private TMP_Dropdown _configDropdown;
        [Tooltip("Available config assets that can be selected at runtime.")]
        [SerializeField] private GameOfLifeConfig[] _configs;
        [Tooltip("Button wired to the Randomize action.")]
        [SerializeField] private Button _randomizeButton;
        [Tooltip("CanvasGroup used to fade the control panel on startup.")]
        [SerializeField] private CanvasGroup _controlPanelCanvasGroup;
        [Tooltip("Duration of the startup fade in seconds.")]
        [SerializeField] private float _startupFadeDuration = 0.35f;
        [Tooltip("Slider used to control the visual cell size.")]
        [SerializeField] private Slider _cellSizeSlider;
        [Tooltip("Slider used to control the random fill density.")]
        [SerializeField] private Slider _densitySlider;

        /// <summary>
        /// Delays UI setup by one frame so GameController finishes Start first.
        /// </summary>
        private IEnumerator Start()
        {
            if (_gameController == null)
            {
                Debug.LogError("UIController requires a GameController reference.");
                enabled = false;
                yield break;
            }

            yield return null;

            if (_speedSlider != null)
            {
                _speedSlider.onValueChanged.RemoveListener(OnSpeedChanged);
                _speedSlider.onValueChanged.AddListener(OnSpeedChanged);

                if (_speedSlider.maxValue < _gameController.SimulationSpeed)
                {
                    _speedSlider.maxValue = _gameController.SimulationSpeed;
                }

                _speedSlider.SetValueWithoutNotify(_gameController.SimulationSpeed);
            }

            if (_cellSizeSlider != null)
            {
                _cellSizeSlider.onValueChanged.RemoveListener(OnCellSizeChanged);
                _cellSizeSlider.onValueChanged.AddListener(OnCellSizeChanged);
                _cellSizeSlider.SetValueWithoutNotify(20f);
            }

            if (_densitySlider != null)
            {
                _densitySlider.onValueChanged.RemoveListener(OnDensityChanged);
                _densitySlider.onValueChanged.AddListener(OnDensityChanged);
                _densitySlider.SetValueWithoutNotify(_gameController.CurrentDensity);
            }

            PopulateConfigDropdown();

            if (_configs != null && _configs.Length > 0)
            {
                ApplySelectedConfig();
            }

            if (_controlPanelCanvasGroup != null)
            {
                StartCoroutine(FadeControlPanelAlpha(targetAlpha: 0.60f, _startupFadeDuration));
            }

            RefreshUI();
        }

        /// <summary>
        /// Refreshes runtime labels every frame.
        /// </summary>
        private void Update()
        {
            RefreshUI();
        }

        /// <summary>
        /// Removes all runtime listener registrations when this object is destroyed.
        /// </summary>
        private void OnDestroy()
        {
            if (_speedSlider != null)
            {
                _speedSlider.onValueChanged.RemoveListener(OnSpeedChanged);
            }

            if (_configDropdown != null)
            {
                _configDropdown.onValueChanged.RemoveListener(OnConfigChanged);
            }

            if (_cellSizeSlider != null)
            {
                _cellSizeSlider.onValueChanged.RemoveListener(OnCellSizeChanged);
            }

            if (_densitySlider != null)
            {
                _densitySlider.onValueChanged.RemoveListener(OnDensityChanged);
            }
        }

        /// <summary>
        /// Toggles simulation playback state.
        /// </summary>
        public void TogglePlayPause()
        {
            if (_gameController == null)
            {
                return;
            }

            _gameController.TogglePlayPause();
            RefreshUI();
        }

        /// <summary>
        /// Applies a speed value from the UI slider.
        /// </summary>
        /// <param name="value">Speed in steps per second.</param>
        public void OnSpeedChanged(float value)
        {
            if (_gameController == null)
            {
                return;
            }

            _gameController.SetSimulationSpeed(value);
        }

        /// <summary>
        /// Applies the currently selected config from the dropdown.
        /// </summary>
        public void ApplySelectedConfig()
        {
            if (_gameController == null || _configDropdown == null || _configs == null || _configs.Length == 0)
            {
                return;
            }

            int selectedIndex = _configDropdown.value;
            if (selectedIndex < 0 || selectedIndex >= _configs.Length)
            {
                return;
            }

            GameOfLifeConfig selectedConfig = _configs[selectedIndex];
            _gameController.ApplyConfig(selectedConfig);

            if (_speedSlider != null)
            {
                if (_speedSlider.maxValue < _gameController.SimulationSpeed)
                {
                    _speedSlider.maxValue = _gameController.SimulationSpeed;
                }

                _speedSlider.SetValueWithoutNotify(_gameController.SimulationSpeed);
            }

            RefreshUI();
        }

        /// <summary>
        /// Handles dropdown selection changes for config switching.
        /// </summary>
        /// <param name="index">Selected dropdown option index.</param>
        public void OnConfigChanged(int index)
        {
            ApplySelectedConfig();
        }

        /// <summary>
        /// Randomizes the grid using current config density.
        /// </summary>
        public void Randomize()
        {
            if (_gameController == null)
            {
                return;
            }

            _gameController.RandomizeGrid();
            RefreshUI();
        }

        /// <summary>
        /// Applies a new visual cell size from the UI slider.
        /// </summary>
        /// <param name="value">Cell size in pixels.</param>
        public void OnCellSizeChanged(float value)
        {
            if (_gameController == null)
            {
                return;
            }

            _gameController.SetCellSize(value);
        }

        /// <summary>
        /// Applies a new random fill density from the UI slider.
        /// </summary>
        /// <param name="value">Density value in [0, 1].</param>
        public void OnDensityChanged(float value)
        {
            if (_gameController == null)
            {
                return;
            }

            _gameController.SetDensity(value);
        }

        /// <summary>
        /// Fades the control panel alpha to the target value.
        /// </summary>
        /// <param name="targetAlpha">Final alpha value.</param>
        /// <param name="duration">Fade duration in seconds.</param>
        private IEnumerator FadeControlPanelAlpha(float targetAlpha, float duration)
        {
            if (_controlPanelCanvasGroup == null)
            {
                yield break;
            }

            if (duration <= 0f)
            {
                _controlPanelCanvasGroup.alpha = targetAlpha;
                yield break;
            }

            float startAlpha = _controlPanelCanvasGroup.alpha;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                _controlPanelCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
                yield return null;
            }

            _controlPanelCanvasGroup.alpha = targetAlpha;
        }

        /// <summary>
        /// Clears and repopulates the config dropdown from the <see cref="_configs"/> array.
        /// </summary>
        private void PopulateConfigDropdown()
        {
            if (_configDropdown == null)
            {
                return;
            }

            _configDropdown.onValueChanged.RemoveListener(OnConfigChanged);
            _configDropdown.ClearOptions();

            if (_configs == null || _configs.Length == 0)
            {
                _configDropdown.onValueChanged.AddListener(OnConfigChanged);
                return;
            }

            var options = new List<TMP_Dropdown.OptionData>(_configs.Length);
            for (int i = 0; i < _configs.Length; i++)
            {
                string optionName = _configs[i] != null ? _configs[i].name : $"Config {i + 1}";
                options.Add(new TMP_Dropdown.OptionData(optionName));
            }

            _configDropdown.AddOptions(options);
            _configDropdown.SetValueWithoutNotify(0);
            _configDropdown.onValueChanged.AddListener(OnConfigChanged);
        }

        private void RefreshUI()
        {
            if (_gameController == null)
            {
                return;
            }

            if (_generationText != null)
            {
                _generationText.text = $"Generation: {_gameController.Generation}";
            }

            if (_aliveCountText != null)
            {
                _aliveCountText.text = $"Alive: {_gameController.AliveCount}";
            }

            if (_playPauseLabel != null)
            {
                _playPauseLabel.text = _gameController.IsPlaying ? "Pause" : "Play";
            }

            if (_randomizeButton != null)
            {
                bool isPatternMode = _gameController.CurrentConfig != null &&
                    _gameController.CurrentConfig.SelectedInitializationMode == InitializationMode.Pattern;
                _randomizeButton.interactable = !isPatternMode;
            }
        }
    }
}
