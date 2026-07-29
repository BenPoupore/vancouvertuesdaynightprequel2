using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace VipExtraction
{
    public sealed class InventoryItemView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text quantityText;
        private InventoryGridController owner;
        private ItemDefinition definition;
        private RectTransform rect;
        private CanvasGroup canvasGroup;
        private Vector2 startPosition;

        public string ItemId => definition.Id;
        public bool Rotated { get; private set; }

        public void Initialize(InventoryGridController grid, ItemDefinition item, int quantity, bool rotated)
        {
            owner = grid;
            definition = item;
            Rotated = rotated;
            rect = (RectTransform)transform;
            canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
            if (icon != null) icon.sprite = item.Icon;
            if (quantityText != null) quantityText.text = quantity > 1 ? quantity.ToString() : string.Empty;
            RefreshSize();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            startPosition = rect.anchoredPosition;
            canvasGroup.blocksRaycasts = false;
            transform.SetAsLastSibling();
        }

        public void OnDrag(PointerEventData eventData) => rect.anchoredPosition += eventData.delta / owner.CanvasScale;

        public void OnEndDrag(PointerEventData eventData)
        {
            canvasGroup.blocksRaycasts = true;
            if (!owner.TryPlaceFromScreen(this, eventData.position)) rect.anchoredPosition = startPosition;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Right || !definition.Rotatable) return;
            Rotated = !Rotated;
            RefreshSize();
            owner.TryKeepOrRestore(this);
        }

        public void SetGridPosition(Vector2 anchoredPosition) => rect.anchoredPosition = anchoredPosition;

        private void RefreshSize()
        {
            int width = Rotated ? definition.GridHeight : definition.GridWidth;
            int height = Rotated ? definition.GridWidth : definition.GridHeight;
            rect.sizeDelta = new Vector2(width * owner.CellSize, height * owner.CellSize);
        }
    }
}
