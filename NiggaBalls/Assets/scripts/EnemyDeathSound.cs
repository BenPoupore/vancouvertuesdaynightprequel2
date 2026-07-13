using UnityEngine;

// Put this on your enemy prefab.
// Drag an AudioClip into Death Sound in the Inspector.
// The sound uses PlayClipAtPoint, so it keeps playing even after the enemy is destroyed.

public class EnemyDeathSound : MonoBehaviour
{
    [Header("Death Sound")]
    public AudioClip deathSound;

    [Range(0f, 1f)]
    public float volume = 1f;

    [Header("Pitch Randomization")]
    public bool randomizePitch = true;
    public Vector2 pitchRange = new Vector2(0.95f, 1.05f);

    public void PlayDeathSound()
    {
        if (deathSound == null) return;

        GameObject tempAudio = new GameObject("Enemy Death Sound");
        tempAudio.transform.position = transform.position;

        AudioSource source = tempAudio.AddComponent<AudioSource>();
        source.clip = deathSound;
        source.volume = volume;
        source.spatialBlend = 0f; // 0 = 2D sound. Use 1 if you want positional 3D sound.

        if (randomizePitch)
            source.pitch = Random.Range(pitchRange.x, pitchRange.y);

        source.Play();

        Destroy(tempAudio, deathSound.length + 0.25f);
    }
}
