using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class WeaponViewmodel : MonoBehaviour
{
    [Header("Sprites")]
    public Image gunImage;
    public Sprite idleSprite;
    public Sprite firingSprite;
    public float firingSpriteTime = 0.05f;

    [Header("Shoot Recoil")]
    public Vector2 recoilOffset = new Vector2(0f, -20f);
    public float recoilRotation = -8f;
    public float recoilOutTime = 0.03f;
    public float recoilReturnTime = 0.12f;

    [Header("Reload Animation")]
    public float reloadDipAmount = 250f;
    public float reloadDownTime = 0.25f;
    public float reloadHoldTime = 0.6f;
    public float reloadUpTime = 0.3f;

    [Header("Idle Sway")]
    public bool enableIdleSway = true;
    public float swayAmount = 6f;
    public float swaySpeed = 1.5f;

    private RectTransform rt;
    private Vector2 restPosition;
    private Quaternion restRotation;
    private Coroutine currentAnim;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        restPosition = rt.anchoredPosition;
        restRotation = rt.localRotation;

        if (gunImage == null)
            gunImage = GetComponent<Image>();

        if (gunImage != null && idleSprite != null)
            gunImage.sprite = idleSprite;
    }

    void Update()
    {
        if (enableIdleSway && currentAnim == null)
        {
            float swayX = Mathf.Sin(Time.time * swaySpeed) * swayAmount;
            float swayY = Mathf.Cos(Time.time * swaySpeed * 2f) * (swayAmount * 0.5f);
            rt.anchoredPosition = restPosition + new Vector2(swayX, swayY);
        }
    }

    public void PlayShootAnimation()
    {
        if (currentAnim != null)
            StopCoroutine(currentAnim);

        currentAnim = StartCoroutine(ShootRoutine());
    }

    public void PlayReloadAnimation()
    {
        if (currentAnim != null)
            StopCoroutine(currentAnim);

        currentAnim = StartCoroutine(ReloadRoutine());
    }

    IEnumerator ShootRoutine()
    {
        if (gunImage != null && firingSprite != null)
            gunImage.sprite = firingSprite;

        Vector2 kickedPos = restPosition + recoilOffset;
        Quaternion kickedRot = restRotation * Quaternion.Euler(0f, 0f, recoilRotation);

        float t = 0f;
        while (t < recoilOutTime)
        {
            t += Time.deltaTime;
            float p = t / recoilOutTime;

            rt.anchoredPosition = Vector2.Lerp(restPosition, kickedPos, p);
            rt.localRotation = Quaternion.Slerp(restRotation, kickedRot, p);

            yield return null;
        }

        yield return new WaitForSeconds(firingSpriteTime);

        if (gunImage != null && idleSprite != null)
            gunImage.sprite = idleSprite;

        t = 0f;
        while (t < recoilReturnTime)
        {
            t += Time.deltaTime;
            float p = t / recoilReturnTime;

            rt.anchoredPosition = Vector2.Lerp(kickedPos, restPosition, p);
            rt.localRotation = Quaternion.Slerp(kickedRot, restRotation, p);

            yield return null;
        }

        rt.anchoredPosition = restPosition;
        rt.localRotation = restRotation;
        currentAnim = null;
    }

    IEnumerator ReloadRoutine()
    {
        if (gunImage != null && idleSprite != null)
            gunImage.sprite = idleSprite;

        Vector2 downPos = restPosition + new Vector2(0f, -reloadDipAmount);

        float t = 0f;
        while (t < reloadDownTime)
        {
            t += Time.deltaTime;
            rt.anchoredPosition = Vector2.Lerp(restPosition, downPos, t / reloadDownTime);
            yield return null;
        }

        yield return new WaitForSeconds(reloadHoldTime);

        t = 0f;
        while (t < reloadUpTime)
        {
            t += Time.deltaTime;
            rt.anchoredPosition = Vector2.Lerp(downPos, restPosition, t / reloadUpTime);
            yield return null;
        }

        rt.anchoredPosition = restPosition;
        rt.localRotation = restRotation;
        currentAnim = null;
    }
}
