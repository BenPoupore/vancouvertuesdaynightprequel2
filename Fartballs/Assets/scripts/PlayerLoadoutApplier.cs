using System.Collections.Generic;
using UnityEngine;

namespace VipExtraction
{
    [System.Serializable]
    public sealed class WeaponLoadoutEntry
    {
        public string itemId;
        [Min(0)] public int playerShooterIndex;
    }

    [DefaultExecutionOrder(100)]
    public sealed class PlayerLoadoutApplier : MonoBehaviour
    {
        [SerializeField] private GameSession session;
        [SerializeField] private global::PlayerShooter playerShooter;
        [SerializeField] private WeaponLoadoutEntry[] weaponMappings;
        [SerializeField] private string suppressorItemId =
            "suppressor_basic";
        [Range(0.01f, 1f), SerializeField]
        private float suppressedAudioMultiplier = 0.25f;

        private void Start()
        {
            if (session == null ||
                session.Profile == null ||
                playerShooter == null ||
                playerShooter.weapons == null ||
                playerShooter.weapons.Length == 0)
            {
                Debug.LogError(
                    "PlayerLoadoutApplier has missing references.",
                    this);
                return;
            }

            ApplyEquipmentSlots();
            ApplySuppressor();
        }

        private void ApplyEquipmentSlots()
        {
            global::WeaponSlot[] originalWeapons =
                playerShooter.weapons;
            var equippedWeapons =
                new List<global::WeaponSlot>();

            AddEquippedWeapon(
                "equip:primary",
                "equip:Primary",
                originalWeapons,
                equippedWeapons);
            AddEquippedWeapon(
                "equip:secondary",
                "equip:Secondary",
                originalWeapons,
                equippedWeapons);
            AddEquippedWeapon(
                "equip:sidearm",
                "equip:Sidearm",
                originalWeapons,
                equippedWeapons);

            foreach (global::WeaponSlot weapon in originalWeapons)
            {
                if (weapon.weaponObject != null)
                {
                    weapon.weaponObject.SetActive(false);
                }
            }

            playerShooter.weapons = equippedWeapons.ToArray();
            playerShooter.currentWeaponIndex = 0;

            if (equippedWeapons.Count == 0)
            {
                Debug.LogWarning(
                    "No firearms are in the Primary, Secondary, " +
                    "or Sidearm equipment slots.",
                    this);
                return;
            }

            if (equippedWeapons[0].weaponObject != null)
            {
                equippedWeapons[0].weaponObject.SetActive(true);
            }
        }

        private void AddEquippedWeapon(
            string currentContainer,
            string legacyContainer,
            global::WeaponSlot[] originalWeapons,
            List<global::WeaponSlot> equippedWeapons)
        {
            if (session.Profile.items == null)
            {
                return;
            }

            ItemInstance equippedInstance =
                session.Profile.items.Find(
                    item =>
                        item != null &&
                        (item.container == currentContainer ||
                         item.container == legacyContainer));

            if (equippedInstance == null)
            {
                return;
            }

            if (weaponMappings == null)
            {
                Debug.LogWarning(
                    "PlayerLoadoutApplier has no weapon mappings.",
                    this);
                return;
            }

            foreach (WeaponLoadoutEntry mapping in weaponMappings)
            {
                if (mapping == null ||
                    mapping.itemId != equippedInstance.itemId ||
                    mapping.playerShooterIndex < 0 ||
                    mapping.playerShooterIndex >=
                    originalWeapons.Length)
                {
                    continue;
                }

                global::WeaponSlot weapon =
                    originalWeapons[mapping.playerShooterIndex];
                if (!equippedWeapons.Contains(weapon))
                {
                    equippedWeapons.Add(weapon);
                }
                return;
            }

            Debug.LogWarning(
                $"No PlayerShooter mapping exists for equipped item " +
                $"'{equippedInstance.itemId}'.",
                this);
        }

        private void ApplySuppressor()
        {
            bool hasWeapon =
                playerShooter.weapons != null &&
                playerShooter.weapons.Length > 0;
            bool suppressorEquipped =
                session.Profile.equippedAttachmentId ==
                suppressorItemId;

            if (hasWeapon &&
                suppressorEquipped &&
                playerShooter.CurrentWeapon.weaponAudio != null)
            {
                playerShooter.CurrentWeapon.weaponAudio.volume *=
                    suppressedAudioMultiplier;
            }
        }
    }
}
