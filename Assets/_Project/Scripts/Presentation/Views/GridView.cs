using ConwayLife.Domain.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace ConwayLife.Presentation.Views
{
    /// <summary>
    /// Renders a Game of Life grid using UI Image cells.
    /// </summary>
    public class GridView : MonoBehaviour
    {
        [Tooltip("UI prefab for a single cell (must include an Image).")]
        [SerializeField] private GameObject _cellPrefab;
        [Tooltip("RectTransform parent that will contain instantiated cells.")]
        [SerializeField] private RectTransform _cellRoot;
        [Tooltip("Horizontal and vertical spacing between cell visuals.")]
        [SerializeField] private Vector2 _cellSpacing = new Vector2(20f, 20f);

        private Color _aliveColor = Color.white;
        private Color _deadColor = Color.black;
        private Image[,] _cellImages;

        /// <summary>
        /// Creates the visual grid for the given size.
        /// </summary>
        /// <param name="width">Number of columns.</param>
        /// <param name="height">Number of rows.</param>
        public void Initialize(int width, int height)
        {
            if (_cellPrefab == null)
            {
                Debug.LogError("GridView requires a cell prefab.");
                return;
            }

            if (_cellRoot == null)
            {
                Debug.LogError("GridView requires a cell root RectTransform.");
                return;
            }

            ClearExistingCells();

            _cellImages = new Image[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    GameObject cellObject = Instantiate(_cellPrefab, _cellRoot);

                    RectTransform cellRect = cellObject.GetComponent<RectTransform>();
                    if (cellRect != null)
                    {
                        cellRect.anchorMin = new Vector2(0f, 1f);
                        cellRect.anchorMax = new Vector2(0f, 1f);
                        cellRect.pivot = new Vector2(0f, 1f);
                        cellRect.anchoredPosition = new Vector2(x * _cellSpacing.x, -y * _cellSpacing.y);
                    }

                    Image image = cellObject.GetComponent<Image>();
                    if (image == null)
                    {
                        image = cellObject.GetComponentInChildren<Image>();
                    }

                    if (image != null)
                    {
                        image.color = _deadColor;
                    }

                    _cellImages[x, y] = image;
                }
            }
        }

        /// <summary>
        /// Sets the colors used to render alive and dead cells.
        /// </summary>
        /// <param name="aliveColor">Color used for alive cells.</param>
        /// <param name="deadColor">Color used for dead cells.</param>
        public void SetColors(Color aliveColor, Color deadColor)
        {
            _aliveColor = aliveColor;
            _deadColor = deadColor;
        }

        /// <summary>
        /// Updates cell visuals from the current grid state.
        /// </summary>
        /// <param name="grid">Grid state to render.</param>
        public void Render(IGrid grid)
        {
            if (_cellImages == null)
            {
                return;
            }

            int width = grid.Width;
            int height = grid.Height;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Image image = _cellImages[x, y];
                    if (image == null)
                    {
                        continue;
                    }

                    image.color = grid.GetCell(x, y) ? _aliveColor : _deadColor;
                }
            }
        }

        private void ClearExistingCells()
        {
            if (_cellRoot == null)
            {
                return;
            }

            for (int i = _cellRoot.childCount - 1; i >= 0; i--)
            {
                Destroy(_cellRoot.GetChild(i).gameObject);
            }
        }
    }
}
