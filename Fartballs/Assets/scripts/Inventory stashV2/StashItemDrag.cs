using UnityEngine;
using UnityEngine.EventSystems;

namespace VipExtraction
{
    [RequireComponent(typeof(CanvasGroup))]
    public sealed class StashItemDrag :
        MonoBehaviour,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler
    {
        [SerializeField] private CanvasGroup canvasGroup;

        private RectTransform rect;
        private Canvas canvas;
        private Transform originalParent;
        private Vector2 originalPosition;
        private int originalSiblingIndex;

        public ItemInstance Instance { get; private set; }

        public void Initialize(ItemInstance instance)
        {
            Instance = instance;
            rect = (RectTransform)transform;
            canvasGroup = GetComponent<CanvasGroup>();
            canvas = GetComponentInParent<Canvas>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (Instance == null)
            {
                return;
            }

            originalParent = transform.parent;
            originalPosition = rect.anchoredPosition;
            originalSiblingIndex = transform.GetSiblingIndex();
            canvasGroup.blocksRaycasts = false;

            if (canvas != null)
            {
                transform.SetParent(canvas.transform, true);
            }

            transform.SetAsLastSibling();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (Instance == null || canvas == null)
            {
                return;
            }

            rect.anchoredPosition +=
                eventData.delta / Mathf.Max(0.0001f, canvas.scaleFactor);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            canvasGroup.blocksRaycasts = true;
            RestoreVisual();
        }

        public void RestoreVisual()
        {
            if (originalParent == null)
            {
                return;
            }

            transform.SetParent(originalParent, false);
            transform.SetSiblingIndex(
                Mathf.Clamp(
                    originalSiblingIndex,
                    0,
                    originalParent.childCount - 1));
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = originalPosition;
            rect.localScale = Vector3.one;
        }

        // Kept for EquipmentSlotDrop and any existing scene scripts that
        // still use the original method name.
        public void RefreshAfterDrop()
        {
            RestoreVisual();
        }
    }
}
