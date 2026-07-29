using System;
using UnityEngine;

namespace VipExtraction
{
    public sealed class StashService : MonoBehaviour
    {
        [SerializeField] private GameSession session;
        [SerializeField] private ItemCatalog catalog;
        [Min(1), SerializeField] private int columns = 10;
        [Min(1), SerializeField] private int rows = 5;

        public event Action Changed;

        public PlayerProfile Profile =>
            session == null ? null : session.Profile;

        public ItemCatalog Catalog => catalog;
        public int Columns => columns;
        public int Rows => rows;

        private void Start()
        {
            if (Profile != null && Profile.items == null)
            {
                Profile.items =
                    new System.Collections.Generic.List<ItemInstance>();
                SaveAndNotify();
            }

            MigrateLegacyEquipmentKeys();
        }

        public static string EquipmentContainer(string slotKey)
        {
            return "equip:" + NormalizeSlotKey(slotKey);
        }

        public ItemInstance FindEquipped(
            string slotKey,
            EquipmentSlotType legacySlot)
        {
            if (Profile == null || Profile.items == null)
            {
                return null;
            }

            string currentKey = EquipmentContainer(slotKey);
            string legacyKey = "equip:" + legacySlot;

            return Profile.items.Find(
                item =>
                    item != null &&
                    (item.container == currentKey ||
                     item.container == legacyKey));
        }

        public bool TryBuy(
            ItemDefinition item,
            out string message)
        {
            if (item == null)
            {
                message = "No item selected.";
                return false;
            }

            if (Profile == null)
            {
                message = "The player profile is not ready.";
                return false;
            }

            if (Profile.money < item.BuyPrice)
            {
                message = "Not enough money.";
                return false;
            }

            var instance = new ItemInstance
            {
                instanceId = Guid.NewGuid().ToString("N"),
                itemId = item.Id
            };

            if (!StashRules.TryFindSpace(
                    item,
                    instance,
                    columns,
                    rows,
                    Profile.items,
                    catalog,
                    out instance.x,
                    out instance.y))
            {
                message = "Not enough stash space.";
                return false;
            }

            Profile.items.Add(instance);
            Profile.money -= item.BuyPrice;
            SaveAndNotify();
            message = $"Bought {item.DisplayName}.";
            return true;
        }

        public bool TryMove(
            ItemInstance instance,
            string container,
            int x,
            int y,
            bool rotated,
            out string message)
        {
            if (instance == null ||
                catalog == null ||
                !catalog.TryGet(
                    instance.itemId,
                    out ItemDefinition item))
            {
                message = "Unknown item.";
                return false;
            }

            if (container == "stash" &&
                !StashRules.CanPlace(
                    item,
                    instance,
                    x,
                    y,
                    rotated,
                    columns,
                    rows,
                    Profile.items,
                    catalog))
            {
                message = "That stash position is blocked.";
                return false;
            }

            instance.container = container;
            instance.x = x;
            instance.y = y;
            instance.rotated = rotated;
            SaveAndNotify();
            message = string.Empty;
            return true;
        }

        public bool TryMoveToStash(
            ItemInstance instance,
            int requestedX,
            int requestedY,
            bool rotated,
            out string message)
        {
            if (instance == null ||
                catalog == null ||
                !catalog.TryGet(
                    instance.itemId,
                    out ItemDefinition item))
            {
                message = "Unknown item.";
                return false;
            }

            int width = rotated
                ? item.GridHeight
                : item.GridWidth;
            int height = rotated
                ? item.GridWidth
                : item.GridHeight;

            int x = Mathf.Clamp(
                requestedX,
                0,
                Mathf.Max(0, columns - width));
            int y = Mathf.Clamp(
                requestedY,
                0,
                Mathf.Max(0, rows - height));

            return TryMove(
                instance,
                "stash",
                x,
                y,
                rotated,
                out message);
        }

        public bool TryEquip(
            ItemInstance instance,
            EquipmentSlotType slot,
            out string message)
        {
            return TryEquip(
                instance,
                slot,
                slot.ToString(),
                out message);
        }

        public bool TryEquip(
            ItemInstance instance,
            EquipmentSlotType acceptedSlot,
            string slotKey,
            out string message)
        {
            if (instance == null ||
                catalog == null ||
                !catalog.TryGet(
                    instance.itemId,
                    out ItemDefinition item))
            {
                message = "Unknown item.";
                return false;
            }

            if (acceptedSlot == EquipmentSlotType.None ||
                item.EquipmentSlot != acceptedSlot)
            {
                message =
                    $"{item.DisplayName} cannot go in the " +
                    $"{acceptedSlot} slot.";
                return false;
            }

            string container = EquipmentContainer(slotKey);
            ItemInstance occupied = FindEquipped(
                slotKey,
                acceptedSlot);

            if (occupied != null && occupied != instance)
            {
                message = "That equipment slot is occupied.";
                return false;
            }

            instance.container = container;
            instance.x = 0;
            instance.y = 0;
            instance.rotated = false;
            SaveAndNotify();
            message = $"Equipped {item.DisplayName}.";
            return true;
        }

        public bool TrySell(
            ItemInstance instance,
            out string message)
        {
            if (instance == null || instance.container != "stash")
            {
                message =
                    "Move the item into the stash before selling it.";
                return false;
            }

            if (catalog == null ||
                !catalog.TryGet(
                    instance.itemId,
                    out ItemDefinition item))
            {
                message = "Unknown item.";
                return false;
            }

            Profile.items.Remove(instance);
            Profile.money += item.SellPrice;
            SaveAndNotify();
            message = $"Sold {item.DisplayName}.";
            return true;
        }

        public bool TrySellFirstStashed(
            string itemId,
            out string message)
        {
            ItemInstance instance = Profile.items.Find(
                candidate =>
                    candidate.itemId == itemId &&
                    candidate.container == "stash");

            if (instance == null)
            {
                message =
                    "You do not have this item in the stash.";
                return false;
            }

            return TrySell(instance, out message);
        }

        private void MigrateLegacyEquipmentKeys()
        {
            if (Profile == null || Profile.items == null)
            {
                return;
            }

            bool changed = false;
            foreach (ItemInstance item in Profile.items)
            {
                if (item == null)
                {
                    continue;
                }

                string migrated = item.container switch
                {
                    "equip:Primary" => "equip:primary",
                    "equip:Secondary" => "equip:secondary",
                    "equip:Sidearm" => "equip:sidearm",
                    "equip:Armor" => "equip:armor",
                    "equip:Helmet" => "equip:helmet",
                    "equip:Backpack" => "equip:backpack",
                    "equip:Grenade" => "equip:grenade_1",
                    _ => item.container
                };

                if (migrated != item.container)
                {
                    item.container = migrated;
                    changed = true;
                }
            }

            if (changed)
            {
                SaveAndNotify();
            }
        }

        private void SaveAndNotify()
        {
            session.CommitProfileChanges();
            Changed?.Invoke();
        }

        private static string NormalizeSlotKey(string slotKey)
        {
            if (string.IsNullOrWhiteSpace(slotKey))
            {
                return "unnamed";
            }

            return slotKey.Trim().ToLowerInvariant();
        }
    }
}
