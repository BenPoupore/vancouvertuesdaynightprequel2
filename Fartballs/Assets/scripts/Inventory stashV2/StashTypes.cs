using System;

namespace VipExtraction
{
    public enum StashCategory { PrimaryWeapon, SecondaryWeapon, Sidearm, Ammunition, AmmoBox, Magazine, Grenade, Armor, Helmet, Backpack, Container, Medical, MissionItem, Miscellaneous, Contraband }
    public enum EquipmentSlotType { None, Primary, Secondary, Sidearm, Armor, Helmet, Backpack, Grenade }

    [Serializable]
    public sealed class ItemInstance
    {
        public string instanceId;
        public string itemId;
        public string container = "stash";
        public int x;
        public int y;
        public bool rotated;
        public int quantity = 1;
        public float condition = 1f;
    }
}
