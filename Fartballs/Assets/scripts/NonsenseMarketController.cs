using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VipExtraction
{
    public sealed class NonsenseMarketController : MonoBehaviour
    {
        [SerializeField] private GameSession session;
        [SerializeField] private TMP_Dropdown dropdown;
        [SerializeField] private TMP_Text priceText;
        [SerializeField] private TMP_Text balanceText;
        [SerializeField] private Button buyButton;
        [SerializeField] private Button sellButton;
        private readonly List<ItemDefinition> items = new List<ItemDefinition>();

        private void Start()
        {
            foreach (ItemDefinition item in session.Catalog.Items) if (item != null && item.Category == ItemCategory.Nonsense) items.Add(item);
            var names = new List<string>(); foreach (ItemDefinition item in items) names.Add(item.DisplayName);
            dropdown.ClearOptions(); dropdown.AddOptions(names);
            dropdown.onValueChanged.AddListener(_ => Refresh());
            buyButton.onClick.AddListener(Buy); sellButton.onClick.AddListener(Sell); Refresh();
        }

        private int Price(ItemDefinition item)
        {
            int hash = Mathf.Abs((item.Id + session.Profile.marketCycle).GetHashCode());
            float multiplier = 0.65f + (hash % 91) / 100f;
            return Mathf.Max(1, Mathf.RoundToInt(item.BaseMarketPrice * multiplier));
        }

        private ItemDefinition Selected => dropdown.value >= 0 && dropdown.value < items.Count ? items[dropdown.value] : null;
        private void Buy() { if (Selected != null) session.TryBuyAtPrice(Selected, Price(Selected), out _); Refresh(); }
        private void Sell() { if (Selected != null) session.TrySellAtPrice(Selected, Price(Selected), out _); Refresh(); }
        private void Refresh()
        {
            ItemDefinition item = Selected;
            priceText.text = item == null ? "No nonsense listed" : $"Market price: ${Price(item)}  |  Owned: {session.Inventory.GetQuantity(item.Id)}";
            balanceText.text = $"Balance: ${session.Profile.money}";
        }
    }
}
