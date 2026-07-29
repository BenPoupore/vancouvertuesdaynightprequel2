using System;
using System.IO;
using UnityEngine;

namespace VipExtraction
{
    public sealed class GameSession : MonoBehaviour
    {
        [SerializeField] private ItemCatalog catalog;
        [Min(0), SerializeField] private int startingMoney = 2500;
        [SerializeField] private bool keepBetweenScenes;

        private const string SaveFileName = "vip_extraction_profile.json";
        private static GameSession instance;

        public event Action Changed;

        public PlayerProfile Profile { get; private set; }
        public ProfileInventory Inventory { get; private set; }
        public ItemCatalog Catalog => catalog;

        private string SavePath => Path.Combine(Application.persistentDataPath, SaveFileName);

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            if (keepBetweenScenes)
            {
                DontDestroyOnLoad(gameObject);
            }

            if (catalog == null)
            {
                Debug.LogError("GameSession requires an ItemCatalog.", this);
                enabled = false;
                return;
            }

            Load();
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }

        public bool TryBuy(ItemDefinition item, out string reason)
        {
            if (item == null)
            {
                reason = "No item selected.";
                return false;
            }

            if (Profile.money < item.BuyPrice)
            {
                reason = "Not enough money.";
                return false;
            }

            if (!Inventory.TryAdd(item.Id, 1, out reason))
            {
                return false;
            }

            Profile.money -= item.BuyPrice;
            SaveAndNotify();
            reason = $"Bought {item.DisplayName}.";
            return true;
        }

        public bool TrySell(ItemDefinition item, out string reason)
        {
            if (item == null)
            {
                reason = "No item selected.";
                return false;
            }

            if (Profile.equippedWeaponId == item.Id || Profile.equippedAttachmentId == item.Id)
            {
                reason = "Unequip this item before selling it.";
                return false;
            }

            if (!Inventory.TryRemove(item.Id, 1, out reason))
            {
                return false;
            }

            Profile.money += item.SellPrice;
            SaveAndNotify();
            reason = $"Sold {item.DisplayName}.";
            return true;
        }

        public bool TryEquip(ItemDefinition item, out string reason)
        {
            if (item == null || Inventory.GetQuantity(item.Id) == 0)
            {
                reason = "You do not own this item.";
                return false;
            }

            if (item.Category == ItemCategory.Weapon)
            {
                Profile.equippedWeaponId = item.Id;
            }
            else if (item.Category == ItemCategory.Attachment)
            {
                Profile.equippedAttachmentId = item.Id;
            }
            else
            {
                reason = "This starter kit only equips weapons and attachments.";
                return false;
            }

            SaveAndNotify();
            reason = $"Equipped {item.DisplayName}.";
            return true;
        }

        public void UnequipSelectedSlots()
        {
            Profile.equippedWeaponId = string.Empty;
            Profile.equippedAttachmentId = string.Empty;
            SaveAndNotify();
        }

        public void AddMoney(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            Profile.money += amount;
            SaveAndNotify();
        }

        public void SelectMission(string missionId)
        {
            Profile.selectedMissionId = missionId ?? string.Empty;
            SaveAndNotify();
        }

        public void RecordMissionResult(string missionName, int baseReward, int extractionBonus)
        {
            Profile.lastMissionName = missionName ?? string.Empty;
            Profile.lastBaseReward = Mathf.Max(0, baseReward);
            Profile.lastExtractionBonus = Mathf.Max(0, extractionBonus);
            Profile.lastTotalReward = Profile.lastBaseReward + Profile.lastExtractionBonus;
            Profile.money += Profile.lastTotalReward;
            Profile.marketCycle++;
            SaveAndNotify();
        }

        public void RequestMissionSelectAtSafehouse()
        {
            Profile.openMissionSelectAtSafehouse = true;
            SaveAndNotify();
        }

        public bool ConsumeMissionSelectRequest()
        {
            bool requested = Profile.openMissionSelectAtSafehouse;
            if (requested)
            {
                Profile.openMissionSelectAtSafehouse = false;
                SaveAndNotify();
            }

            return requested;
        }

        public void CommitProfileChanges() => SaveAndNotify();

        public bool TryBuyAtPrice(ItemDefinition item, int price, out string reason)
        {
            if (item == null || price < 0 || Profile.money < price)
            {
                reason = "Not enough money.";
                return false;
            }

            if (!Inventory.TryAdd(item.Id, 1, out reason)) return false;
            Profile.money -= price;
            SaveAndNotify();
            reason = $"Bought {item.DisplayName}.";
            return true;
        }

        public bool TrySellAtPrice(ItemDefinition item, int price, out string reason)
        {
            if (item == null || price < 0)
            {
                reason = "Invalid item or market price.";
                return false;
            }

            if (!Inventory.TryRemove(item.Id, 1, out reason))
            {
                return false;
            }

            Profile.money += price;
            SaveAndNotify();
            reason = $"Sold {item.DisplayName}.";
            return true;
        }

        private void Load()
        {
            try
            {
                Profile = File.Exists(SavePath)
                    ? JsonUtility.FromJson<PlayerProfile>(File.ReadAllText(SavePath))
                    : CreateNewProfile();
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"Could not load profile. A fresh profile was created. {exception.Message}", this);
                Profile = CreateNewProfile();
            }

            if (Profile == null || Profile.version > PlayerProfile.CurrentVersion)
            {
                Profile = CreateNewProfile();
            }

            Profile.inventory ??= new System.Collections.Generic.List<InventoryEntry>();
            Profile.money = Mathf.Max(0, Profile.money);
            Profile.gridPlacements ??= new System.Collections.Generic.List<GridPlacement>();
            Profile.version = PlayerProfile.CurrentVersion;
            Inventory = new ProfileInventory(Profile, catalog);
            Save();
        }

        private PlayerProfile CreateNewProfile()
        {
            return new PlayerProfile { money = startingMoney };
        }

        private void SaveAndNotify()
        {
            Save();
            Changed?.Invoke();
        }

        private void Save()
        {
            try
            {
                File.WriteAllText(SavePath, JsonUtility.ToJson(Profile, true));
            }
            catch (Exception exception)
            {
                Debug.LogError($"Could not save profile: {exception.Message}", this);
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Delete Save In Editor")]
        private void DeleteSaveInEditor()
        {
            if (File.Exists(SavePath))
            {
                File.Delete(SavePath);
            }

            Profile = CreateNewProfile();
            Inventory = new ProfileInventory(Profile, catalog);
            SaveAndNotify();
        }
#endif
    }
}
