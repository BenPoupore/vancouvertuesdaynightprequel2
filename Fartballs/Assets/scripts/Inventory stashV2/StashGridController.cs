using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VipExtraction
{
    public sealed class StashGridController : MonoBehaviour
    {
        [SerializeField] private StashService stash;
        [SerializeField] private ItemCatalog catalog;
        [SerializeField] private RectTransform gridRoot;
        [SerializeField] private StashItemWidget itemPrefab;
        [Min(12f), SerializeField] private float maximumCellSize = 64f;
        [Min(1), SerializeField] private int columns = 10;
        [Min(1), SerializeField] private int rows = 5;
        [SerializeField] private Color gridLineColor =
            new Color(0.15f, 0.15f, 0.15f, 0.65f);

        private readonly List<GameObject> spawned =
            new List<GameObject>();
        private float cellSize;

        public float CellSize => cellSize;
        public int Columns => columns;
        public int Rows => rows;

        private void Awake()
        {
            // The previous inventory tutorial used the same GridRoot.
            // It must not rebuild on top of the new instance-based stash.
            InventoryGridController[] legacyControllers =
                FindObjectsByType<InventoryGridController>(
                    FindObjectsInactive.Include);
            foreach (InventoryGridController legacy in legacyControllers)
            {
                legacy.enabled = false;
            }

            if (itemPrefab != null)
            {
                itemPrefab.gameObject.SetActive(false);
            }
        }

        private void OnEnable()
        {
            if (stash == null || catalog == null ||
                gridRoot == null || itemPrefab == null)
            {
                Debug.LogError(
                    "StashGridController has missing Inspector references.",
                    this);
                return;
            }

            stash.Changed += Rebuild;
            Rebuild();
        }

        private void OnDisable()
        {
            if (stash != null)
            {
                stash.Changed -= Rebuild;
            }
        }

        public void Rebuild()
        {
            if (stash == null || stash.Profile == null ||
                catalog == null || gridRoot == null ||
                itemPrefab == null)
            {
                return;
            }

            foreach (GameObject spawnedObject in spawned)
            {
                if (spawnedObject != null)
                {
                    Destroy(spawnedObject);
                }
            }
            spawned.Clear();

            Canvas.ForceUpdateCanvases();
            Vector2 available = gridRoot.rect.size;
            float widthCell = available.x > 1f
                ? available.x / columns
                : maximumCellSize;
            float heightCell = available.y > 1f
                ? available.y / rows
                : maximumCellSize;

            cellSize = Mathf.Max(
                12f,
                Mathf.Min(maximumCellSize, widthCell, heightCell));

            gridRoot.anchorMin = new Vector2(0f, 1f);
            gridRoot.anchorMax = new Vector2(0f, 1f);
            gridRoot.pivot = new Vector2(0f, 1f);
            gridRoot.sizeDelta =
                new Vector2(columns * cellSize, rows * cellSize);

            DrawGridLines();

            foreach (ItemInstance instance in stash.Profile.items)
            {
                if (instance == null ||
                    instance.container != "stash" ||
                    !catalog.TryGet(
                        instance.itemId,
                        out ItemDefinition definition))
                {
                    continue;
                }

                StashItemWidget widget =
                    Instantiate(itemPrefab, gridRoot);
                widget.gameObject.SetActive(true);
                widget.name = $"Item_{definition.DisplayName}";
                spawned.Add(widget.gameObject);
                widget.Initialize(instance, definition, cellSize);

                RectTransform widgetRect =
                    (RectTransform)widget.transform;
                widgetRect.anchoredPosition = new Vector2(
                    instance.x * cellSize + 1f,
                    -instance.y * cellSize - 1f);
                widgetRect.SetAsLastSibling();
            }
        }

        private void DrawGridLines()
        {
            for (int x = 0; x <= columns; x++)
            {
                CreateLine(
                    $"GridLineVertical_{x}",
                    new Vector2(x * cellSize, 0f),
                    new Vector2(1f, rows * cellSize));
            }

            for (int y = 0; y <= rows; y++)
            {
                CreateLine(
                    $"GridLineHorizontal_{y}",
                    new Vector2(0f, -y * cellSize),
                    new Vector2(columns * cellSize, 1f));
            }
        }

        private void CreateLine(
            string lineName,
            Vector2 position,
            Vector2 size)
        {
            var lineObject = new GameObject(
                lineName,
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Image));
            lineObject.layer = gridRoot.gameObject.layer;
            lineObject.transform.SetParent(gridRoot, false);
            spawned.Add(lineObject);

            RectTransform lineRect =
                lineObject.GetComponent<RectTransform>();
            lineRect.anchorMin = new Vector2(0f, 1f);
            lineRect.anchorMax = new Vector2(0f, 1f);
            lineRect.pivot = new Vector2(0f, 1f);
            lineRect.anchoredPosition = position;
            lineRect.sizeDelta = size;
            lineRect.SetAsFirstSibling();

            Image lineImage = lineObject.GetComponent<Image>();
            lineImage.color = gridLineColor;
            lineImage.raycastTarget = false;
        }
    }
}
