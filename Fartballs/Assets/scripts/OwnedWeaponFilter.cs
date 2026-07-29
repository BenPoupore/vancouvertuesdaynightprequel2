using System.Collections.Generic;
using UnityEngine;

namespace VipExtraction
{
    [System.Serializable]
    public sealed class OwnedWeaponMapping
    {
        public string itemId;
        [Min(0)] public int originalWeaponIndex;
    }

    [DefaultExecutionOrder(100)]
    public sealed class OwnedWeaponFilter : MonoBehaviour
    {
        [SerializeField] private GameSession session;
        [SerializeField] private global::PlayerShooter shooter;
        [SerializeField] private OwnedWeaponMapping[] mappings;

        private void Start()
        {
            if (session == null || shooter == null || shooter.weapons == null) return;
            var owned = new List<global::WeaponSlot>();

            AddOwnedWeapon(session.Profile.equippedWeaponId, owned);

            foreach (OwnedWeaponMapping mapping in mappings)
            {
                if (mapping == null || mapping.itemId == session.Profile.equippedWeaponId) continue;
                AddOwnedWeapon(mapping.itemId, owned);
            }

            foreach (global::WeaponSlot slot in shooter.weapons) if (slot.weaponObject != null) slot.weaponObject.SetActive(false);
            shooter.weapons = owned.ToArray();
            shooter.currentWeaponIndex = 0;
            if (shooter.weapons.Length > 0 && shooter.weapons[0].weaponObject != null) shooter.weapons[0].weaponObject.SetActive(true);
            if (shooter.weapons.Length == 0)
                Debug.LogWarning($"Player owns no mapped weapons. Equipped ID is '{session.Profile.equippedWeaponId}'. Check item IDs, ownership, and mapping indices.", this);
        }

        private void AddOwnedWeapon(string itemId, List<global::WeaponSlot> owned)
        {
            if (string.IsNullOrWhiteSpace(itemId) || session.Inventory.GetQuantity(itemId) <= 0) return;
            foreach (OwnedWeaponMapping mapping in mappings)
            {
                if (mapping == null || mapping.itemId != itemId) continue;
                if (mapping.originalWeaponIndex >= 0 && mapping.originalWeaponIndex < shooter.weapons.Length)
                    owned.Add(shooter.weapons[mapping.originalWeaponIndex]);
                return;
            }
        }
    }
}
