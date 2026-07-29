using System.Collections.Generic;
using UnityEngine;

namespace VipExtraction
{
    [System.Serializable]
    public sealed class WeaponInstanceMapping
    {
        public string itemId;
        public int shooterIndex;
    }

    [DefaultExecutionOrder(100)]
    public sealed class LoadoutWeaponBridge : MonoBehaviour
    {
        [SerializeField] private GameSession session;
        [SerializeField] private global::PlayerShooter shooter;
        [SerializeField] private WeaponInstanceMapping[] mappings;

        private void Start()
        {
            if (session == null ||
                session.Profile == null ||
                shooter == null)
            {
                Debug.LogError(
                    "LoadoutWeaponBridge has missing references.",
                    this);
                return;
            }

            global::WeaponSlot[] original = shooter.weapons;
            var allowed = new List<global::WeaponSlot>();

            AddSlot(
                "equip:primary",
                "equip:Primary",
                original,
                allowed);
            AddSlot(
                "equip:secondary",
                "equip:Secondary",
                original,
                allowed);
            AddSlot(
                "equip:sidearm",
                "equip:Sidearm",
                original,
                allowed);

            foreach (global::WeaponSlot slot in original)
            {
                if (slot.weaponObject != null)
                {
                    slot.weaponObject.SetActive(false);
                }
            }

            shooter.weapons = allowed.ToArray();
            shooter.currentWeaponIndex = 0;

            if (allowed.Count > 0 &&
                allowed[0].weaponObject != null)
            {
                allowed[0].weaponObject.SetActive(true);
            }
        }

        private void AddSlot(
            string currentContainer,
            string legacyContainer,
            global::WeaponSlot[] original,
            List<global::WeaponSlot> allowed)
        {
            ItemInstance equipped =
                session.Profile.items.Find(
                    item =>
                        item != null &&
                        (item.container == currentContainer ||
                         item.container == legacyContainer));

            if (equipped == null)
            {
                return;
            }

            foreach (WeaponInstanceMapping mapping in mappings)
            {
                if (mapping.itemId != equipped.itemId ||
                    mapping.shooterIndex < 0 ||
                    mapping.shooterIndex >= original.Length)
                {
                    continue;
                }

                allowed.Add(original[mapping.shooterIndex]);
                return;
            }

            Debug.LogWarning(
                $"No weapon mapping exists for '{equipped.itemId}'.",
                this);
        }
    }
}
