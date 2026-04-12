using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    [Header("Enemy Prefabs")]
    public GameObject normalEnemyPrefab;
    public GameObject heavyEnemyPrefab;

    [Header("Heavy Enemy Settings")]
    public int heavyStartWave = 3;
    [Range(0f, 0.49f)]
    public float heavySpawnChance = 0.25f;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("Wave Settings")]
    public int baseEnemiesPerWave = 5;
    public int enemiesAddedPerWave = 2;
    public float timeBetweenWaves = 5f;
    public float timeBetweenSpawns = 0.5f;
    public int totalWaves = 10; // 0 = endless

    [HideInInspector] public int currentWave = 0;
    [HideInInspector] public int killCount = 0;
    [HideInInspector] public int enemiesAlive = 0;

    private bool spawning = false;
    private GameUI gameUI;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        gameUI = FindFirstObjectByType<GameUI>();
        StartCoroutine(StartNextWave());
    }

    IEnumerator StartNextWave()
    {
        yield return new WaitForSeconds(timeBetweenWaves);

        currentWave++;

        gameUI?.ShowWaveBanner(currentWave);

        int count = baseEnemiesPerWave + (currentWave - 1) * enemiesAddedPerWave;

        spawning = true;
        yield return StartCoroutine(SpawnWave(count));
        spawning = false;

        CheckWaveComplete();
    }

    IEnumerator SpawnWave(int count)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }

    void SpawnEnemy()
    {
        if (spawnPoints.Length == 0 || normalEnemyPrefab == null) return;

        Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];

        bool spawnHeavy = heavyEnemyPrefab != null
                          && currentWave >= heavyStartWave
                          && Random.value < heavySpawnChance;

        GameObject prefab = spawnHeavy ? heavyEnemyPrefab : normalEnemyPrefab;
        Instantiate(prefab, point.position, point.rotation);
        enemiesAlive++;
    }

    public void OnEnemyKilled()
    {
        killCount++;
        enemiesAlive = Mathf.Max(0, enemiesAlive - 1);

        if (!spawning)
            CheckWaveComplete();
    }

    void CheckWaveComplete()
    {
        if (enemiesAlive > 0) return;

        if (totalWaves > 0 && currentWave >= totalWaves)
        {
            gameUI?.ShowVictory();
        }
        else
        {
            StartCoroutine(StartNextWave());
        }
    }

    void OnDrawGizmos()
    {
        if (spawnPoints == null) return;
        Gizmos.color = Color.cyan;
        foreach (Transform point in spawnPoints)
        {
            if (point != null)
                Gizmos.DrawWireSphere(point.position, 0.5f);
        }
    }
}
