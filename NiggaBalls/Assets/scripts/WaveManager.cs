using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Wave
{
    public string waveName = "Wave";
    [Min(1)] public int enemyCount = 5;
    [Min(0.05f)] public float spawnInterval = 1f;
    [Min(1)] public int maxAliveAtOnce = 3;
}

public class WaveManager : MonoBehaviour
{
    [Header("References")]
    public EnemySpawner enemySpawner;

    [Header("Waves")]
    public Wave[] waves;
    public float firstWaveDelay = 2f;
    public float timeBetweenWaves = 5f;
    public bool startAutomatically = true;

    [Header("Optional HUD")]
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI enemiesText;
    public TextMeshProUGUI messageText;

    [Header("Completion Event")]
    public UnityEvent onAllWavesComplete;

    public int CurrentWaveNumber { get; private set; }
    public bool AllWavesComplete { get; private set; }

    private int spawnedThisWave;
    private int currentWaveEnemyTotal;
    private Coroutine waveRoutine;

    void Awake()
    {
        if (enemySpawner != null)
        {
            enemySpawner.spawnOnStart = false;
            enemySpawner.StopSpawning();
        }
    }

    void Start()
    {
        UpdateHUD();

        if (startAutomatically)
            StartWaves();
    }

    void Update()
    {
        // Keeps the remaining-enemies number accurate as enemies die.
        if (waveRoutine != null)
            UpdateHUD();
    }

    public void StartWaves()
    {
        if (waveRoutine != null || AllWavesComplete)
            return;

        if (enemySpawner == null)
        {
            Debug.LogError("WaveManager: Enemy Spawner is not assigned.");
            return;
        }

        if (waves == null || waves.Length == 0)
        {
            Debug.LogError("WaveManager: No waves have been configured.");
            return;
        }

        waveRoutine = StartCoroutine(RunWaves());
    }

    IEnumerator RunWaves()
    {
        SetMessage("GET READY");
        yield return new WaitForSeconds(firstWaveDelay);

        for (int waveIndex = 0; waveIndex < waves.Length; waveIndex++)
        {
            Wave wave = waves[waveIndex];

            CurrentWaveNumber = waveIndex + 1;
            spawnedThisWave = 0;
            currentWaveEnemyTotal = wave.enemyCount;

            enemySpawner.maxAliveEnemies = wave.maxAliveAtOnce;

            SetMessage(
                string.IsNullOrWhiteSpace(wave.waveName)
                    ? "WAVE " + CurrentWaveNumber
                    : wave.waveName
            );

            UpdateHUD();
            yield return new WaitForSeconds(1f);
            SetMessage("");

            // Spawn every enemy assigned to this wave.
            while (spawnedThisWave < wave.enemyCount)
            {
                if (enemySpawner.TrySpawnOneEnemy())
                    spawnedThisWave++;

                UpdateHUD();
                yield return new WaitForSeconds(wave.spawnInterval);
            }

            // Do not finish the wave until every spawned enemy is dead.
            while (enemySpawner.GetAliveCount() > 0)
            {
                UpdateHUD();
                yield return null;
            }

            SetMessage("WAVE COMPLETE");
            UpdateHUD();

            if (waveIndex < waves.Length - 1)
                yield return new WaitForSeconds(timeBetweenWaves);
        }

        AllWavesComplete = true;
        waveRoutine = null;

        SetMessage("ALL WAVES COMPLETE");
        UpdateHUD();
        onAllWavesComplete?.Invoke();
    }

    void UpdateHUD()
    {
        if (waveText != null)
        {
            int shownWave = Mathf.Max(CurrentWaveNumber, 1);
            int totalWaves = waves == null ? 0 : waves.Length;
            waveText.text = "WAVE " + shownWave + " / " + totalWaves;
        }

        if (enemiesText != null)
        {
            int alive = enemySpawner == null
                ? 0
                : enemySpawner.GetAliveCount();

            int waitingToSpawn = Mathf.Max(
                0,
                currentWaveEnemyTotal - spawnedThisWave
            );

            int remaining = alive + waitingToSpawn;
            enemiesText.text = "ENEMIES: " + remaining;
        }
    }

    void SetMessage(string value)
    {
        if (messageText != null)
            messageText.text = value;
    }
}
