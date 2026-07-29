using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VipExtraction
{
    public sealed class AttachmentEquipController : MonoBehaviour
    {
        [SerializeField] private GameSession session;
        [SerializeField] private TMP_Dropdown weaponDropdown;
        [SerializeField] private TMP_Dropdown attachmentDropdown;
        [SerializeField] private TMP_Text statusText;
        [SerializeField] private Button equipButton;
        private readonly List<ItemDefinition> weapons = new List<ItemDefinition>();
        private readonly List<ItemDefinition> attachments = new List<ItemDefinition>();

        private void Start()
        {
            foreach (ItemDefinition item in session.Catalog.Items)
            {
                if (item == null || session.Inventory.GetQuantity(item.Id) <= 0) continue;
                if (item.Category == ItemCategory.Weapon) weapons.Add(item);
                if (item.Category == ItemCategory.Attachment) attachments.Add(item);
            }
            weaponDropdown.AddOptions(weapons.ConvertAll(x => x.DisplayName));
            attachmentDropdown.AddOptions(attachments.ConvertAll(x => x.DisplayName));
            equipButton.onClick.AddListener(Equip);
        }

        private void Equip()
        {
            if (weapons.Count == 0 || attachments.Count == 0) { statusText.text = "Own a weapon and attachment first."; return; }
            session.TryEquip(weapons[weaponDropdown.value], out _);
            session.TryEquip(attachments[attachmentDropdown.value], out string result);
            statusText.text = result;
        }
    }
}
