using System.Collections;
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
        [Tooltip("CanvasGroup used to fade the control panel on startup.")]
        [SerializeField] private CanvasGroup _controlPanelCanvasGroup;
        [Tooltip("Duration of the startup fade in seconds.")]
        [SerializeField] private float _startupFadeDuration = 0.35f;

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

            if (_controlPanelCanvasGroup != null)
            {
                StartCoroutine(FadeControlPanelAlpha(targetAlpha: 0.15f, _startupFadeDuration));
            }

            RefreshUI();
        }

        private void Update()
        {
            RefreshUI();
        }

        private void OnDestroy()
        {
            if (_speedSlider != null)
            {
                _speedSlider.onValueChanged.RemoveListener(OnSpeedChanged);
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
        }
    }
}
