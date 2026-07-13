using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Attach this to an empty GameObject in the scene.
// The WaveManager controls this spawner through TrySpawnOneEnemy().

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;

    [Header("Spawn Settings")]
    public float spawnInterval = 5f;
    public int maxAliveEnemies = 5;
    public int totalSpawnLimit = 0; // 0 = unlimited
    public bool spawnOnStart = false;

    [Header("Spawn Point Behavior")]
    public bool randomizeSpawnPoint = true;
    public bool avoidRepeatSpawnPoint = true;

    private readonly List<GameObject> aliveEnemies = new List<GameObject>();
    private int totalSpawned;
    private int lastSpawnIndex = -1;
    private int nextCycleIndex;
    private Coroutine spawnRoutine;

    void Start()
    {
        if (spawnOnStart)
            StartSpawning();
    }

    public void StartSpawning()
    {
        if (spawnRoutine == null)
            spawnRoutine = StartCoroutine(SpawnLoop());
    }

    public void StopSpawning()
    {
        if (spawnRoutine == null) return;

        StopCoroutine(spawnRoutine);
        spawnRoutine = null;
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (totalSpawnLimit > 0 && totalSpawned >= totalSpawnLimit)
            {
                StopSpawning();
                yield break;
            }

            TrySpawnOneEnemy();
        }
    }

    // Returns true only when an enemy was successfully spawned.
    public bool TrySpawnOneEnemy()
    {
        RemoveDestroyedEnemies();

        if (enemyPrefab == null)
        {
            Debug.LogWarning("EnemySpawner: Enemy Prefab is not assigned.");
            return false;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("EnemySpawner: assign at least one Spawn Point.");
            return false;
        }

        if (aliveEnemies.Count >= maxAliveEnemies)
            return false;

        if (totalSpawnLimit > 0 && totalSpawned >= totalSpawnLimit)
            return false;

        Transform chosenPoint = GetSpawnPoint();

        if (chosenPoint == null)
        {
            Debug.LogWarning("EnemySpawner: one of the Spawn Points is missing.");
            return false;
        }

        GameObject enemy = Instantiate(
            enemyPrefab,
            chosenPoint.position,
            chosenPoint.rotation
        );

        aliveEnemies.Add(enemy);
        totalSpawned++;
        return true;
    }

    // Kept for compatibility with anything already calling this method.
    public void ForceSpawnNow()
    {
        TrySpawnOneEnemy();
    }

    Transform GetSpawnPoint()
    {
        if (spawnPoints.Length == 1)
            return spawnPoints[0];

        int index;

        if (randomizeSpawnPoint)
        {
            int safety = 0;

            do
            {
                index = Random.Range(0, spawnPoints.Length);
                safety++;
            }
            while (
                avoidRepeatSpawnPoint &&
                index == lastSpawnIndex &&
                safety < 20
            );
        }
        else
        {
            index = nextCycleIndex;
            nextCycleIndex = (nextCycleIndex + 1) % spawnPoints.Length;
        }

        lastSpawnIndex = index;
        return spawnPoints[index];
    }

    void RemoveDestroyedEnemies()
    {
        aliveEnemies.RemoveAll(enemy => enemy == null);
    }

    public int GetAliveCount()
    {
        RemoveDestroyedEnemies();
        return aliveEnemies.Count;
    }

    public int GetTotalSpawnedCount()
    {
        return totalSpawned;
    }

    void OnDrawGizmosSelected()
    {
        if (spawnPoints == null) return;

        Gizmos.color = Color.magenta;

        foreach (Transform point in spawnPoints)
        {
            if (point == null) continue;

            Gizmos.DrawWireSphere(point.position, 0.5f);
            Gizmos.DrawLine(
                point.position,
                point.position + Vector3.up * 2f
            );
        }
    }
}
