using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace VipExtraction
{
    public sealed class EquipmentSlotDrop :
        MonoBehaviour,
        IDropHandler,
        IPointerEnterHandler,
        IPointerExitHandler
    {
        [SerializeField] private StashService stash;
        [SerializeField] private ItemCatalog catalog;
        [SerializeField] private StashItemWidget itemPrefab;
        [SerializeField] private EquipmentSlotType slotType;
        [SerializeField] private string slotKey = "primary";
        [SerializeField] private Image highlight;

        private readonly Color emptyColor =
            new Color(0.16f, 0.18f, 0.22f, 0.9f);
        private readonly Color hoverColor =
            new Color(0.25f, 0.48f, 0.68f, 0.95f);
        private readonly Color invalidColor =
            new Color(0.65f, 0.18f, 0.18f, 0.95f);

        private StashItemWidget displayedItem;
        private TMP_Text emptyLabel;

        public string SlotKey => slotKey;
        public EquipmentSlotType SlotType => slotType;

        public void Configure(
            StashService stashService,
            ItemCatalog itemCatalog,
            StashItemWidget widgetPrefab,
            EquipmentSlotType acceptedSlot,
            string uniqueSlotKey)
        {
            stash = stashService;
            catalog = itemCatalog;
            itemPrefab = widgetPrefab;
            slotType = acceptedSlot;
            slotKey = uniqueSlotKey;

            if (highlight == null)
            {
                highlight = GetComponent<Image>();
            }

            EnsureEmptyLabel();
            Refresh();
        }

        public void Refresh()
        {
            if (displayedItem != null)
            {
                Destroy(displayedItem.gameObject);
                displayedItem = null;
            }

            if (highlight != null)
            {
                highlight.color = emptyColor;
                highlight.raycastTarget = true;
            }

            if (emptyLabel != null)
            {
                emptyLabel.gameObject.SetActive(true);
            }

            if (stash == null ||
                catalog == null ||
                itemPrefab == null)
            {
                return;
            }

            ItemInstance instance =
                stash.FindEquipped(slotKey, slotType);
            if (instance == null ||
                !catalog.TryGet(
                    instance.itemId,
                    out ItemDefinition definition))
            {
                return;
            }

            displayedItem = Instantiate(
                itemPrefab,
                (RectTransform)transform);
            displayedItem.gameObject.SetActive(true);
            displayedItem.name =
                $"Equipped_{definition.DisplayName}";
            displayedItem.InitializeForEquipmentSlot(
                instance,
                definition);
            displayedItem.transform.SetAsLastSibling();

            if (emptyLabel != null)
            {
                emptyLabel.gameObject.SetActive(false);
            }
        }

        public void OnDrop(PointerEventData eventData)
        {
            StashItemDrag drag =
                eventData.pointerDrag == null
                    ? null
                    : eventData.pointerDrag.GetComponent<StashItemDrag>();

            if (drag == null || drag.Instance == null)
            {
                return;
            }

            bool equipped = stash.TryEquip(
                drag.Instance,
                slotType,
                slotKey,
                out string message);

            if (highlight != null)
            {
                highlight.color =
                    equipped ? Color.green : invalidColor;
            }

            if (!string.IsNullOrEmpty(message))
            {
                Debug.Log(message, this);
            }

            drag.RestoreVisual();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (highlight != null &&
                eventData.dragging)
            {
                highlight.color = hoverColor;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (highlight != null)
            {
                highlight.color = emptyColor;
            }
        }

        private void EnsureEmptyLabel()
        {
            Transform existing =
                transform.Find("EmptySlotLabel");
            if (existing != null)
            {
                emptyLabel =
                    existing.GetComponent<TMP_Text>();
            }

            if (emptyLabel == null)
            {
                var labelObject = new GameObject(
                    "EmptySlotLabel",
                    typeof(RectTransform),
                    typeof(CanvasRenderer),
                    typeof(TextMeshProUGUI));
                labelObject.layer = gameObject.layer;
                labelObject.transform.SetParent(transform, false);
                emptyLabel =
                    labelObject.GetComponent<TextMeshProUGUI>();
            }

            RectTransform labelRect =
                (RectTransform)emptyLabel.transform;
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.pivot = new Vector2(0.5f, 0.5f);
            labelRect.anchoredPosition = Vector2.zero;
            labelRect.sizeDelta = new Vector2(-6f, -6f);

            emptyLabel.text =
                slotKey.Replace("_", " ").ToUpperInvariant();
            emptyLabel.alignment =
                TextAlignmentOptions.Center;
            emptyLabel.fontSize = 14f;
            emptyLabel.color =
                new Color(0.8f, 0.82f, 0.86f, 0.9f);
            emptyLabel.raycastTarget = false;
        }
    }
}
