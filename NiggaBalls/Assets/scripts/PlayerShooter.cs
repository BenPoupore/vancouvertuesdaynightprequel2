using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class WeaponSlot
{
    [Header("Weapon Info")]
    public string weaponName = "Weapon";

    [Header("View")]
    public GameObject weaponObject;
    public WeaponViewmodel viewmodel;
    public WeaponAudio weaponAudio;
    public Transform firePoint;
    public GameObject muzzleFlashPrefab;

    [Header("Fire Settings")]
    public float roundsPerMinute = 600f;
    public int damage = 20;
    public float bulletSpeed = 60f;
    public float range = 100f;
    public bool useHitscan = true;
    public bool automatic = true;
    public GameObject bulletPrefab;

    [Header("Hitscan Pellet Settings")]
    [Min(1)] public int pelletCount = 1;
    [Min(0f)] public float spreadDegrees = 0f;

    [Header("Ammo")]
    public int magazineSize = 30;
    public float reloadTime = 1.5f;

    [HideInInspector] public int currentAmmo;
}

public class PlayerShooter : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;

    [Header("Weapons")]
       public WeaponSlot[] weapons;
    public int currentWeaponIndex = 0;

    private bool isReloading;
    private float fireCooldown;

    public WeaponSlot CurrentWeapon => weapons[currentWeaponIndex];
    public int CurrentAmmo => CurrentWeapon.currentAmmo;
    public int MagazineSize => CurrentWeapon.magazineSize;
    public string WeaponName => CurrentWeapon.weaponName;

    void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        if (weapons == null || weapons.Length == 0)
        {
            Debug.LogWarning("No weapons assigned to PlayerShooter.");
            return;
        }

        currentWeaponIndex = Mathf.Clamp(currentWeaponIndex, 0, weapons.Length - 1);

        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].currentAmmo = weapons[i].magazineSize;

            if (weapons[i].weaponObject != null)
                weapons[i].weaponObject.SetActive(i == currentWeaponIndex);
        }
    }

    void Update()
    {
        if (weapons == null || weapons.Length == 0)
            return;

        fireCooldown -= Time.deltaTime;

        HandleWeaponSwap();
        HandleReload();
        HandleShooting();
    }

    void HandleWeaponSwap()
    {
        if (isReloading || Keyboard.current == null)
            return;

        if (Keyboard.current.digit1Key.wasPressedThisFrame) SwitchWeapon(0);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) SwitchWeapon(1);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) SwitchWeapon(2);
        if (Keyboard.current.digit4Key.wasPressedThisFrame) SwitchWeapon(3);
        if (Keyboard.current.digit5Key.wasPressedThisFrame) SwitchWeapon(4);
    }

    void SwitchWeapon(int newIndex)
    {
        if (newIndex < 0 || newIndex >= weapons.Length)
            return;

        if (newIndex == currentWeaponIndex)
            return;

        if (weapons[currentWeaponIndex].weaponObject != null)
            weapons[currentWeaponIndex].weaponObject.SetActive(false);

        currentWeaponIndex = newIndex;
        fireCooldown = 0f;

        if (weapons[currentWeaponIndex].weaponObject != null)
            weapons[currentWeaponIndex].weaponObject.SetActive(true);
    }

    void HandleReload()
    {
        if (Keyboard.current == null)
            return;

        WeaponSlot weapon = CurrentWeapon;

        if (Keyboard.current.rKey.wasPressedThisFrame &&
            !isReloading &&
            weapon.magazineSize > 0 &&
            weapon.currentAmmo < weapon.magazineSize)
        {
            StartCoroutine(Reload());
        }
    }

    void HandleShooting()
    {
        if (Mouse.current == null)
            return;

        WeaponSlot weapon = CurrentWeapon;

        bool wantsToFire = weapon.automatic
            ? Mouse.current.leftButton.isPressed
            : Mouse.current.leftButton.wasPressedThisFrame;

        if (!wantsToFire || fireCooldown > 0f || isReloading)
            return;

        if (weapon.magazineSize > 0 && weapon.currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        Shoot();
    }

    void Shoot()
    {
        WeaponSlot weapon = CurrentWeapon;

        float safeRpm = Mathf.Max(1f, weapon.roundsPerMinute);
        fireCooldown = 60f / safeRpm;

        if (weapon.magazineSize > 0)
            weapon.currentAmmo--;

        if (weapon.viewmodel != null)
            weapon.viewmodel.PlayShootAnimation();

        if (weapon.weaponAudio != null)
            weapon.weaponAudio.PlayShoot();

        if (weapon.muzzleFlashPrefab != null && weapon.firePoint != null)
        {
            GameObject flash = Instantiate(
                weapon.muzzleFlashPrefab,
                weapon.firePoint.position,
                weapon.firePoint.rotation
            );

            Destroy(flash, 0.15f);
        }

        if (weapon.useHitscan)
            ShootHitscan(weapon);
        else
            ShootProjectile(weapon);
    }

    void ShootHitscan(WeaponSlot weapon)
    {
        int pellets = Mathf.Max(1, weapon.pelletCount);

        for (int i = 0; i < pellets; i++)
        {
            float horizontalSpread = Random.Range(
                -weapon.spreadDegrees,
                weapon.spreadDegrees
            );

            float verticalSpread = Random.Range(
                -weapon.spreadDegrees,
                weapon.spreadDegrees
            );

            Vector3 direction =
                Quaternion.AngleAxis(horizontalSpread, playerCamera.transform.up) *
                Quaternion.AngleAxis(verticalSpread, playerCamera.transform.right) *
                playerCamera.transform.forward;

            Ray ray = new Ray(playerCamera.transform.position, direction);

            Debug.DrawRay(
                ray.origin,
                ray.direction * weapon.range,
                Color.green,
                0.2f
            );

            if (!Physics.Raycast(ray, out RaycastHit hit, weapon.range))
                continue;

            IDamageable damageable =
                hit.collider.GetComponent<IDamageable>();

            if (damageable == null)
                damageable = hit.collider.GetComponentInParent<IDamageable>();

            if (damageable != null)
                damageable.TakeDamage(weapon.damage);
        }
    }

    void ShootProjectile(WeaponSlot weapon)
    {
        if (weapon.bulletPrefab == null || weapon.firePoint == null)
        {
            Debug.LogWarning(
                "Assign a bulletPrefab and firePoint to use projectile mode."
            );
            return;
        }

        GameObject bullet = Instantiate(
            weapon.bulletPrefab,
            weapon.firePoint.position,
            playerCamera.transform.rotation
        );

        BulletProjectile projectile =
            bullet.GetComponent<BulletProjectile>();

        if (projectile != null)
        {
            projectile.damage = weapon.damage;
            projectile.speed = weapon.bulletSpeed;
        }

        Collider playerCollider = GetComponentInChildren<Collider>();
        Collider bulletCollider = bullet.GetComponent<Collider>();

        if (playerCollider != null && bulletCollider != null)
            Physics.IgnoreCollision(playerCollider, bulletCollider);
    }

    IEnumerator Reload()
    {
        WeaponSlot weapon = CurrentWeapon;
        isReloading = true;

        if (weapon.viewmodel != null)
            weapon.viewmodel.PlayReloadAnimation();

        if (weapon.weaponAudio != null)
            weapon.weaponAudio.PlayReload();

        yield return new WaitForSeconds(weapon.reloadTime);

        weapon.currentAmmo = weapon.magazineSize;
        isReloading = false;
    }
}
