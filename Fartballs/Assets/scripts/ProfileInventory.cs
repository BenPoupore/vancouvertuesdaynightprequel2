using System;

namespace VipExtraction
{
    public sealed class ProfileInventory
    {
        private readonly PlayerProfile profile;
        private readonly ItemCatalog catalog;

        public ProfileInventory(PlayerProfile profile, ItemCatalog catalog)
        {
            this.profile = profile ?? throw new ArgumentNullException(nameof(profile));
            this.catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
        }

        public int GetQuantity(string itemId)
        {
            InventoryEntry entry = Find(itemId);
            return entry == null ? 0 : entry.quantity;
        }

        public bool TryAdd(string itemId, int quantity, out string reason)
        {
            if (quantity <= 0 || !catalog.TryGet(itemId, out ItemDefinition item))
            {
                reason = "Invalid item or quantity.";
                return false;
            }

            InventoryEntry entry = Find(itemId);
            int current = entry == null ? 0 : entry.quantity;
            if (current + quantity > item.StackLimit)
            {
                reason = $"Stash limit is {item.StackLimit}.";
                return false;
            }

            if (entry == null)
            {
                profile.inventory.Add(new InventoryEntry(itemId, quantity));
            }
            else
            {
                entry.quantity += quantity;
            }

            reason = string.Empty;
            return true;
        }

        public bool TryRemove(string itemId, int quantity, out string reason)
        {
            InventoryEntry entry = Find(itemId);
            if (quantity <= 0 || entry == null || entry.quantity < quantity)
            {
                reason = "Not enough items in the stash.";
                return false;
            }

            entry.quantity -= quantity;
            if (entry.quantity == 0)
            {
                profile.inventory.Remove(entry);
            }

            reason = string.Empty;
            return true;
        }

        private InventoryEntry Find(string itemId)
        {
            return profile.inventory.Find(entry => entry.itemId == itemId);
        }
    }
}

