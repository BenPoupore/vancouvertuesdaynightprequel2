using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    [Header("References")]
    public Health playerHealth;
    public PlayerShooter playerShooter;

    [Header("UI Text Elements")]
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI ammoText;

    [Header("Display Format")]
    public string healthFormat = "HP: {0} / {1}";
    public string ammoFormat = "{0} / {1}";
    public string infiniteAmmoText = "∞";

    [Header("Low Health Warning")]
    public bool changeColorOnLowHealth = true;
    public float lowHealthThreshold = 0.3f;
    public Color normalHealthColor = Color.white;
    public Color lowHealthColor = Color.red;

    [Header("Low Ammo Warning")]
    public bool changeColorOnLowAmmo = true;
    public float lowAmmoThreshold = 0.25f;
    public Color normalAmmoColor = Color.white;
    public Color lowAmmoColor = Color.red;

    void Update()
    {
        UpdateHealthDisplay();
        UpdateAmmoDisplay();
    }

    void UpdateHealthDisplay()
    {
        if (playerHealth == null || healthText == null) return;

        healthText.text = string.Format(healthFormat, playerHealth.currentHealth, playerHealth.maxHealth);

        if (changeColorOnLowHealth)
        {
            float ratio = (float)playerHealth.currentHealth / playerHealth.maxHealth;
            healthText.color = (ratio <= lowHealthThreshold) ? lowHealthColor : normalHealthColor;
        }
    }

    void UpdateAmmoDisplay()
    {
        if (playerShooter == null || ammoText == null) return;

        if (playerShooter.MagazineSize <= 0)
        {
            ammoText.text = infiniteAmmoText;
            ammoText.color = normalAmmoColor;
            return;
        }

        ammoText.text = string.Format(ammoFormat, playerShooter.CurrentAmmo, playerShooter.MagazineSize);

        if (changeColorOnLowAmmo)
        {
            float ratio = (float)playerShooter.CurrentAmmo / playerShooter.MagazineSize;
            ammoText.color = (ratio <= lowAmmoThreshold) ? lowAmmoColor : normalAmmoColor;
        }
    }
}
