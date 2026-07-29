using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class WeaponAudio : MonoBehaviour
{
    [Header("Clips")]
    public AudioClip shootClip;
    public AudioClip reloadClip;

    [Header("Settings")]
    [Range(0f, 1f)] public float volume = 1f;
    public Vector2 randomPitchRange = new Vector2(0.96f, 1.04f);

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public void PlayShoot()
    {
        PlayClip(shootClip);
    }

    public void PlayReload()
    {
        PlayClip(reloadClip);
    }

    void PlayClip(AudioClip clip)
    {
        if (clip == null) return;

        audioSource.pitch = Random.Range(randomPitchRange.x, randomPitchRange.y);
        audioSource.PlayOneShot(clip, volume);
    }
}
