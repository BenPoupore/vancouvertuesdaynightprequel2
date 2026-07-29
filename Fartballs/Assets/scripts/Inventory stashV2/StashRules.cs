using System.Collections.Generic;

namespace VipExtraction
{
    public static class StashRules
    {
        public static bool CanPlace(ItemDefinition item, ItemInstance moving, int x, int y, bool rotated, int columns, int rows, IReadOnlyList<ItemInstance> all, ItemCatalog catalog)
        {
            int w = rotated ? item.GridHeight : item.GridWidth;
            int h = rotated ? item.GridWidth : item.GridHeight;
            if (x < 0 || y < 0 || x + w > columns || y + h > rows) return false;
            foreach (ItemInstance other in all)
            {
                if (other == moving || other.container != "stash" || !catalog.TryGet(other.itemId, out ItemDefinition d)) continue;
                int ow = other.rotated ? d.GridHeight : d.GridWidth;
                int oh = other.rotated ? d.GridWidth : d.GridHeight;
                if (x < other.x + ow && x + w > other.x && y < other.y + oh && y + h > other.y) return false;
            }
            return true;
        }

        public static bool TryFindSpace(ItemDefinition item, ItemInstance instance, int columns, int rows, IReadOnlyList<ItemInstance> all, ItemCatalog catalog, out int x, out int y)
        {
            for (y = 0; y < rows; y++)
                for (x = 0; x < columns; x++)
                    if (CanPlace(item, instance, x, y, false, columns, rows, all, catalog)) return true;
            x = y = -1;
            return false;
        }
    }
}
