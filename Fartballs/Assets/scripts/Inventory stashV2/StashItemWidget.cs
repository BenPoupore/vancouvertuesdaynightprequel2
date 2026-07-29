using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VipExtraction
{
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(StashItemDrag))]
    public sealed class StashItemWidget : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private TMP_Text label;

        public void Initialize(
            ItemInstance instance,
            ItemDefinition definition,
            float cellSize)
        {
            ConfigureContent(instance, definition);

            int width = instance.rotated
                ? definition.GridHeight
                : definition.GridWidth;
            int height = instance.rotated
                ? definition.GridWidth
                : definition.GridHeight;

            RectTransform rect = (RectTransform)transform;
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.localScale = Vector3.one;
            rect.sizeDelta = new Vector2(
                width * cellSize - 2f,
                height * cellSize - 2f);
        }

        public void InitializeForEquipmentSlot(
            ItemInstance instance,
            ItemDefinition definition)
        {
            ConfigureContent(instance, definition);

            RectTransform rect = (RectTransform)transform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(-8f, -8f);
            rect.localScale = Vector3.one;

            if (label != null)
            {
                RectTransform labelRect =
                    (RectTransform)label.transform;
                labelRect.anchorMin = Vector2.zero;
                labelRect.anchorMax = Vector2.one;
                labelRect.pivot = new Vector2(0.5f, 0.5f);
                labelRect.anchoredPosition = Vector2.zero;
                labelRect.sizeDelta = new Vector2(-6f, -6f);
                label.enableAutoSizing = true;
                label.fontSizeMin = 8f;
                label.fontSizeMax = 22f;
            }
        }

        private void ConfigureContent(
            ItemInstance instance,
            ItemDefinition definition)
        {
            image = GetComponent<Image>();
            image.sprite = definition.Icon;
            image.color = definition.Icon == null
                ? CategoryColor(definition.StashCategory)
                : Color.white;
            image.preserveAspect = definition.Icon != null;
            image.raycastTarget = true;

            if (label == null)
            {
                label = GetComponentInChildren<TMP_Text>(true);
            }

            if (label != null)
            {
                label.text =
                    definition.Stackable && instance.quantity > 1
                        ? $"{definition.DisplayName}\nx{instance.quantity}"
                        : definition.DisplayName;
                label.raycastTarget = false;
            }

            GetComponent<StashItemDrag>().Initialize(instance);
        }

        private static Color CategoryColor(StashCategory category)
        {
            if (category == StashCategory.Ammunition)
                return Color.yellow;
            if (category == StashCategory.Grenade)
                return new Color(1f, 0.5f, 0f);
            if (category == StashCategory.Armor)
                return Color.blue;
            if (category == StashCategory.Container ||
                category == StashCategory.Backpack)
                return new Color(0.6f, 0.2f, 0.8f);
            if (category == StashCategory.Medical)
                return Color.green;
            if (category == StashCategory.PrimaryWeapon ||
                category == StashCategory.SecondaryWeapon ||
                category == StashCategory.Sidearm)
                return new Color(0.7f, 0.18f, 0.18f);

            return Color.gray;
        }
    }
}
