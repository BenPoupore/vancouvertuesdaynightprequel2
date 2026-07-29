using System.Collections.Generic;
using UnityEngine;

namespace VipExtraction
{
    public sealed class InventoryGridController : MonoBehaviour
    {
        [SerializeField] private GameSession session;
        [SerializeField] private RectTransform gridRoot;
        [SerializeField] private InventoryItemView itemPrefab;
        [Min(1), SerializeField] private int columns = 10;
        [Min(1), SerializeField] private int rows = 20;
        [Min(8f), SerializeField] private float cellSize = 48f;

        private readonly List<InventoryItemView> views = new List<InventoryItemView>();
        private bool initialized;
        public float CellSize => cellSize;
        public float CanvasScale => gridRoot.GetComponentInParent<Canvas>().scaleFactor;

        private void OnEnable()
        {
            if (initialized)
            {
                Rebuild();
            }
        }

        private void Start()
        {
            if (session == null || gridRoot == null || itemPrefab == null)
            {
                Debug.LogError("InventoryGridController has missing references.", this);
                enabled = false;
                return;
            }

            gridRoot.sizeDelta = new Vector2(columns * cellSize, rows * cellSize);
            initialized = true;
            Rebuild();
        }

        public void Rebuild()
        {
            foreach (InventoryItemView view in views) if (view != null) Destroy(view.gameObject);
            views.Clear();
            int fallbackIndex = 0;
            foreach (InventoryEntry entry in session.Profile.inventory)
            {
                if (!session.Catalog.TryGet(entry.itemId, out ItemDefinition item)) continue;
                GridPlacement placement = FindPlacement(item.Id) ?? CreateFallback(item.Id, fallbackIndex++);
                InventoryItemView view = Instantiate(itemPrefab, gridRoot);
                views.Add(view);
                view.Initialize(this, item, entry.quantity, placement.rotated);
                Position(view, placement.x, placement.y);
            }

            session.CommitProfileChanges();
        }

        public bool TryPlaceFromScreen(InventoryItemView view, Vector2 screenPosition)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(gridRoot, screenPosition, null, out Vector2 local);
            int x = Mathf.FloorToInt((local.x + gridRoot.rect.width * gridRoot.pivot.x) / cellSize);
            int y = Mathf.FloorToInt((gridRoot.rect.height * (1f - gridRoot.pivot.y) - local.y) / cellSize);
            return TryPlace(view, x, y);
        }

        public void TryKeepOrRestore(InventoryItemView view)
        {
            GridPlacement placement = FindPlacement(view.ItemId);
            if (placement == null || !TryPlace(view, placement.x, placement.y)) Rebuild();
        }

        private bool TryPlace(InventoryItemView view, int x, int y)
        {
            if (!session.Catalog.TryGet(view.ItemId, out ItemDefinition item)) return false;
            int width = view.Rotated ? item.GridHeight : item.GridWidth;
            int height = view.Rotated ? item.GridWidth : item.GridHeight;
            if (x < 0 || y < 0 || x + width > columns || y + height > rows) return false;
            foreach (InventoryItemView other in views)
            {
                if (other == view || !session.Catalog.TryGet(other.ItemId, out ItemDefinition otherItem)) continue;
                GridPlacement otherPlacement = FindPlacement(other.ItemId);
                if (otherPlacement == null) continue;
                int ow = other.Rotated ? otherItem.GridHeight : otherItem.GridWidth;
                int oh = other.Rotated ? otherItem.GridWidth : otherItem.GridHeight;
                if (x < otherPlacement.x + ow && x + width > otherPlacement.x && y < otherPlacement.y + oh && y + height > otherPlacement.y) return false;
            }

            GridPlacement placement = FindPlacement(view.ItemId) ?? CreateFallback(view.ItemId, 0);
            placement.x = x; placement.y = y; placement.rotated = view.Rotated;
            Position(view, x, y);
            session.CommitProfileChanges();
            return true;
        }

        private void Position(InventoryItemView view, int x, int y) => view.SetGridPosition(new Vector2(x * cellSize, -y * cellSize));
        private GridPlacement FindPlacement(string id) => session.Profile.gridPlacements.Find(p => p.itemId == id);
        private GridPlacement CreateFallback(string id, int index)
        {
            var placement = new GridPlacement { itemId = id, x = index % columns, y = index / columns };
            session.Profile.gridPlacements.Add(placement);
            return placement;
        }
    }
}
