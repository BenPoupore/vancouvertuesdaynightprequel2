using UnityEngine;

namespace VipExtraction
{
    public enum ItemCategory
    {
        Weapon,
        Attachment,
        Gadget,
        Nonsense
    }

    [CreateAssetMenu(menuName = "VIP Extraction/Item", fileName = "NewItem")]
    public sealed class ItemDefinition : ScriptableObject
    {
        [SerializeField] private string id = "new_item";
        [SerializeField] private string displayName = "New Item";
        [TextArea, SerializeField] private string description;
        [SerializeField] private Sprite icon;
        [SerializeField] private ItemCategory category;
        [SerializeField] private StashCategory stashCategory;
        [SerializeField] private EquipmentSlotType equipmentSlot;
        [Min(1), SerializeField] private int gridWidth = 1;
        [Min(1), SerializeField] private int gridHeight = 1;
        [SerializeField] private bool rotatable = true;
        [SerializeField] private bool stackable;
        [Min(1), SerializeField] private int stackLimit = 1;
        [Min(0), SerializeField] private int buyPrice = 100;
        [Min(0), SerializeField] private int sellPrice = 25;
        [Min(0f), SerializeField] private float weight = 1f;

        public string Id => id;
        public string DisplayName => displayName;
        public string Description => description;
        public Sprite Icon => icon;
        public ItemCategory Category => category;
        public StashCategory StashCategory => stashCategory;
        public EquipmentSlotType EquipmentSlot => equipmentSlot;
        public int GridWidth => gridWidth;
        public int GridHeight => gridHeight;
        public bool Rotatable => rotatable;
        public bool Stackable => stackable;
        public int StackLimit => stackable ? stackLimit : 1;
        public int BuyPrice => buyPrice;
        public int SellPrice => sellPrice;
        public int BaseMarketPrice => buyPrice;
        public float Weight => weight;
    }
}
