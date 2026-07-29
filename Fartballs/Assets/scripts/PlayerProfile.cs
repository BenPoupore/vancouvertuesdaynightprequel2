using System;
using System.Collections.Generic;

namespace VipExtraction
{
    [Serializable]
    public sealed class InventoryEntry
    {
        public string itemId;
        public int quantity;

        public InventoryEntry(string itemId, int quantity)
        {
            this.itemId = itemId;
            this.quantity = quantity;
        }
    }

    [Serializable]
    public sealed class PlayerProfile
    {
        public const int CurrentVersion = 4;

        public int version = CurrentVersion;
        public int money;
        public string equippedWeaponId = "";
        public string equippedAttachmentId = "";
        public string selectedMissionId = "";
        public string lastMissionName = "";
        public int lastBaseReward;
        public int lastExtractionBonus;
        public int lastTotalReward;
        public bool openMissionSelectAtSafehouse;
        public int marketCycle;
        public List<InventoryEntry> inventory = new List<InventoryEntry>();
        public List<GridPlacement> gridPlacements = new List<GridPlacement>();
        public List<ItemInstance> items = new List<ItemInstance>();
    }
}
