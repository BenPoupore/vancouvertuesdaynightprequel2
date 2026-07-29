using UnityEngine;

namespace VipExtraction
{
    public sealed class EquipmentPanelController : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private StashService stash;
        [SerializeField] private ItemCatalog catalog;
        [SerializeField] private StashItemWidget itemPrefab;
        [SerializeField] private bool autoLayout = true;

        [Header("Weapon Slots")]
        [SerializeField] private RectTransform primarySlot;
        [SerializeField] private RectTransform secondarySlot;
        [SerializeField] private RectTransform sidearmSlot;

        [Header("Grenade Slots")]
        [SerializeField] private RectTransform grenadeSlot1;
        [SerializeField] private RectTransform grenadeSlot2;

        private EquipmentSlotDrop[] configuredSlots;
        private bool initialized;

        private void Start()
        {
            Initialize();
        }

        private void OnEnable()
        {
            if (initialized)
            {
                Subscribe();
                Refresh();
            }
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        private void Initialize()
        {
            if (stash == null ||
                catalog == null ||
                itemPrefab == null ||
                primarySlot == null ||
                secondarySlot == null ||
                sidearmSlot == null ||
                grenadeSlot1 == null ||
                grenadeSlot2 == null)
            {
                Debug.LogError(
                    "EquipmentPanelController has missing " +
                    "Inspector references.",
                    this);
                enabled = false;
                return;
            }

            configuredSlots = new[]
            {
                ConfigureSlot(
                    primarySlot,
                    EquipmentSlotType.Primary,
                    "primary"),
                ConfigureSlot(
                    secondarySlot,
                    EquipmentSlotType.Secondary,
                    "secondary"),
                ConfigureSlot(
                    sidearmSlot,
                    EquipmentSlotType.Sidearm,
                    "sidearm"),
                ConfigureSlot(
                    grenadeSlot1,
                    EquipmentSlotType.Grenade,
                    "grenade_1"),
                ConfigureSlot(
                    grenadeSlot2,
                    EquipmentSlotType.Grenade,
                    "grenade_2")
            };

            if (autoLayout)
            {
                ApplyCompactLayout();
            }

            initialized = true;
            Subscribe();
            Refresh();
        }

        private EquipmentSlotDrop ConfigureSlot(
            RectTransform slotRoot,
            EquipmentSlotType slotType,
            string slotKey)
        {
            EquipmentSlotDrop drop =
                slotRoot.GetComponent<EquipmentSlotDrop>();
            if (drop == null)
            {
                drop =
                    slotRoot.gameObject.AddComponent<
                        EquipmentSlotDrop>();
            }

            drop.Configure(
                stash,
                catalog,
                itemPrefab,
                slotType,
                slotKey);
            return drop;
        }

        private void Subscribe()
        {
            stash.Changed -= Refresh;
            stash.Changed += Refresh;
        }

        private void Unsubscribe()
        {
            if (stash != null)
            {
                stash.Changed -= Refresh;
            }
        }

        private void Refresh()
        {
            if (configuredSlots == null)
            {
                return;
            }

            foreach (EquipmentSlotDrop slot in configuredSlots)
            {
                if (slot != null)
                {
                    slot.Refresh();
                }
            }
        }

        private void ApplyCompactLayout()
        {
            RectTransform panel = (RectTransform)transform;
            panel.anchorMin = new Vector2(1f, 0.5f);
            panel.anchorMax = new Vector2(1f, 0.5f);
            panel.pivot = new Vector2(1f, 0.5f);
            panel.anchoredPosition = new Vector2(-12f, 0f);
            panel.sizeDelta = new Vector2(190f, 300f);

            SetSlotRect(
                primarySlot,
                new Vector2(10f, -26f),
                new Vector2(170f, 52f));
            SetSlotRect(
                secondarySlot,
                new Vector2(10f, -86f),
                new Vector2(170f, 52f));
            SetSlotRect(
                sidearmSlot,
                new Vector2(10f, -146f),
                new Vector2(105f, 52f));
            SetSlotRect(
                grenadeSlot1,
                new Vector2(10f, -206f),
                new Vector2(78f, 60f));
            SetSlotRect(
                grenadeSlot2,
                new Vector2(98f, -206f),
                new Vector2(78f, 60f));
        }

        private static void SetSlotRect(
            RectTransform slot,
            Vector2 anchoredPosition,
            Vector2 size)
        {
            slot.anchorMin = new Vector2(0f, 1f);
            slot.anchorMax = new Vector2(0f, 1f);
            slot.pivot = new Vector2(0f, 1f);
            slot.anchoredPosition = anchoredPosition;
            slot.sizeDelta = size;
            slot.localScale = Vector3.one;
        }
    }
}
