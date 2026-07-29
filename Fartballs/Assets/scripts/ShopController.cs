using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VipExtraction
{
    public sealed class ShopController : MonoBehaviour
    {
        [Header("Shop Data")]
        [SerializeField] private StashService stash;
        [SerializeField] private ShopStock stock;

        [Header("Scrollable Item List")]
        [SerializeField] private RectTransform listContent;
        [SerializeField] private Button itemButtonPrefab;

        [Header("Shop Information")]
        [SerializeField] private TMP_Text moneyText;
        [SerializeField] private TMP_Text messageText;
        [SerializeField] private TMP_Text statsText;

        [Header("Actions")]
        [SerializeField] private Button buyButton;
        [SerializeField] private Button sellButton;

        private readonly List<ItemDefinition> items =
            new List<ItemDefinition>();
        private readonly List<Button> itemButtons =
            new List<Button>();

        private int selectedIndex = -1;
        private bool initialized;

        private void Start()
        {
            if (!ReferencesAreValid())
            {
                enabled = false;
                return;
            }

            BuildItemList();

            buyButton.onClick.AddListener(Buy);
            sellButton.onClick.AddListener(Sell);
            initialized = true;

            if (items.Count > 0)
            {
                SelectItem(0);
            }
            else
            {
                messageText.text = "No stock.";
                RefreshDetails();
            }
        }

        private void OnEnable()
        {
            if (initialized)
            {
                RefreshDetails();
            }
        }

        private void OnDestroy()
        {
            if (buyButton != null)
            {
                buyButton.onClick.RemoveListener(Buy);
            }

            if (sellButton != null)
            {
                sellButton.onClick.RemoveListener(Sell);
            }
        }

        private bool ReferencesAreValid()
        {
            if (stash != null &&
                stash.Profile != null &&
                stock != null &&
                listContent != null &&
                itemButtonPrefab != null &&
                moneyText != null &&
                messageText != null &&
                buyButton != null &&
                sellButton != null)
            {
                return true;
            }

            Debug.LogError(
                "ShopController has missing Inspector references. " +
                "Assign Stash, Stock, List Content, Item Button Prefab, " +
                "Money Text, Message Text, Buy Button and Sell Button.",
                this);
            return false;
        }

        private void BuildItemList()
        {
            items.Clear();

            foreach (Button oldButton in itemButtons)
            {
                if (oldButton != null)
                {
                    Destroy(oldButton.gameObject);
                }
            }
            itemButtons.Clear();

            foreach (ShopStockEntry entry in stock.Entries)
            {
                if (entry == null || entry.item == null)
                {
                    continue;
                }

                int itemIndex = items.Count;
                ItemDefinition item = entry.item;
                items.Add(item);

                Button itemButton =
                    Instantiate(itemButtonPrefab, listContent);
                itemButton.gameObject.SetActive(true);
                itemButton.name = $"ShopItem_{item.Id}";

                TMP_Text buttonText =
                    itemButton.GetComponentInChildren<TMP_Text>(true);
                if (buttonText != null)
                {
                    buttonText.text =
                        $"{item.DisplayName}  |  ${item.BuyPrice}";
                }

                itemButton.onClick.AddListener(
                    () => SelectItem(itemIndex));
                itemButtons.Add(itemButton);
            }
        }

        private void SelectItem(int index)
        {
            if (index < 0 || index >= items.Count)
            {
                return;
            }

            selectedIndex = index;
            messageText.text =
                $"Selected: {items[selectedIndex].DisplayName}";
            RefreshDetails();
        }

        private void Buy()
        {
            if (!TryGetSelectedItem(out ItemDefinition item))
            {
                messageText.text = "Select an item first.";
                return;
            }

            stash.TryBuy(item, out string message);
            messageText.text = message;
            RefreshDetails();
        }

        private void Sell()
        {
            if (!TryGetSelectedItem(out ItemDefinition item))
            {
                messageText.text = "Select an item first.";
                return;
            }

            stash.TrySellFirstStashed(item.Id, out string message);
            messageText.text = message;
            RefreshDetails();
        }

        private bool TryGetSelectedItem(
            out ItemDefinition selectedItem)
        {
            if (selectedIndex >= 0 &&
                selectedIndex < items.Count)
            {
                selectedItem = items[selectedIndex];
                return true;
            }

            selectedItem = null;
            return false;
        }

        private void RefreshDetails()
        {
            if (stash == null ||
                stash.Profile == null ||
                moneyText == null)
            {
                return;
            }

            moneyText.text = $"Money: ${stash.Profile.money}";

            if (statsText == null)
            {
                return;
            }

            if (!TryGetSelectedItem(out ItemDefinition item))
            {
                statsText.text = "AWAITING ITEM SELECTION_";
                return;
            }

            statsText.text =
                $"{item.DisplayName}\n" +
                $"BUY: ${item.BuyPrice}  SELL: ${item.SellPrice}\n" +
                $"{item.StashCategory}  |  " +
                $"{item.GridWidth}x{item.GridHeight}  |  " +
                $"{item.Weight:0.0} KG";
        }
    }
}
