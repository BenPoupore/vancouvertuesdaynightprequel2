using System;
using System.Collections.Generic;
using UnityEngine;

namespace VipExtraction
{
    [Serializable]
    public sealed class ShopStockEntry
    {
        public ItemDefinition item;

        [Tooltip("-1 means unlimited stock.")]
        public int quantity = -1;
    }

    [CreateAssetMenu(
        menuName = "VIP Extraction/Shop Stock",
        fileName = "ShopStock")]
    public sealed class ShopStock : ScriptableObject
    {
        [SerializeField]
        private List<ShopStockEntry> entries =
            new List<ShopStockEntry>();

        public IReadOnlyList<ShopStockEntry> Entries => entries;
    }
}
