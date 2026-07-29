using System.Collections.Generic;
using UnityEngine;

namespace VipExtraction
{
    [CreateAssetMenu(menuName = "VIP Extraction/Item Catalog", fileName = "ItemCatalog")]
    public sealed class ItemCatalog : ScriptableObject
    {
        [SerializeField] private List<ItemDefinition> items = new List<ItemDefinition>();

        public IReadOnlyList<ItemDefinition> Items => items;

        public bool TryGet(string itemId, out ItemDefinition definition)
        {
            for (int i = 0; i < items.Count; i++)
            {
                ItemDefinition candidate = items[i];
                if (candidate != null && candidate.Id == itemId)
                {
                    definition = candidate;
                    return true;
                }
            }

            definition = null;
            return false;
        }

        private void OnValidate()
        {
            var seen = new HashSet<string>();
            foreach (ItemDefinition item in items)
            {
                if (item == null)
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(item.Id))
                {
                    Debug.LogError($"Item '{item.name}' has an empty ID.", item);
                }
                else if (!seen.Add(item.Id))
                {
                    Debug.LogError($"Duplicate item ID '{item.Id}' in catalog.", this);
                }
            }
        }
    }
}

