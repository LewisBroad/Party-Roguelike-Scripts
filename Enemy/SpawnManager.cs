using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpawnManager : MonoBehaviour
{
    public enum SpawnType { Static, Dynamic }
    public enum LevelType { Desert, Lava, Earth, Ice, Underground }
    [System.Serializable]
    public class LevelEnemyData
    {
        public LevelType levelType;
        public GameObject[] enemyPrefabs;
    }

    private List<EnemyBase> activeEnemies = new List<EnemyBase>();


    [SerializeField] private SpawnType spawnType;
    public List<SpawnVolume> spawnVolumes;

    [Header("Level & Enemy Setting")]
    public LevelType currentLevelType;
    public List<LevelEnemyDataSO> levelEnemyDataList;
    private float difficultyTimer;
    private int difficultyLevel;
    public float difficultyIncreaseInterval = 60f;


    [Header("Spawn Setup")]
    private Transform player;
/*    public Vector2 mapMinBounds;
    public Vector2 mapMaxBounds;*/
    public int staticSpawnCount = 10;
    public int baseMaxEnemies = 20;
    private float spawnCooldown = 5f;
    private float spawnTimer;
    [Header("Dynamic Spawn Settings")]
    [SerializeField] private float baseSpawnCooldown = 5f;
    [SerializeField] private float spawnCooldownVariance = 1.5f;
    [SerializeField] private int maxGroupsPerWave = 1; // Starts low, increases with difficulty


    void Start()
    {
        GameObject playerGo = GameObject.FindGameObjectWithTag("Player");
        if(playerGo != null)
        {
            player = playerGo.transform;
        }
        else Debug.LogError("Player not found in the scene. Please ensure the player has the 'Player' tag.");
        if (spawnType == SpawnType.Static)
            for (int i = 0; i < staticSpawnCount; i++)
            {
                Vector3 spawnPos = GetRandomPositionInBounds();
                SpawnEnemy(spawnPos, EnemyBase.enemyState.idle);
            }
    }

    void Update()
    {
        difficultyTimer += Time.deltaTime;
        if (difficultyTimer >= difficultyIncreaseInterval)
        {
            difficultyLevel++;
            difficultyTimer = 0f;
        }

        HandleDynamicSpawning();
    }
    void HandleDynamicSpawning()
    {
        spawnTimer += Time.deltaTime;
        int maxEnemies = baseMaxEnemies + difficultyLevel * 2;
        int currentEnemyCount = activeEnemies.Count;

        if (spawnTimer >= baseSpawnCooldown + Random.Range(-spawnCooldownVariance, spawnCooldownVariance)
                && currentEnemyCount < maxEnemies)
        {
            int availableSpawns = maxEnemies - currentEnemyCount;

            // Increase max groups over time (e.g. 1 more group every 3 difficulty levels)
            int groupsThisWave = Mathf.Clamp(difficultyLevel / 3 + 1, 1, maxGroupsPerWave);
            for (int g = 0; g < groupsThisWave; g++)
            {
                // Randomize group size based on difficulty
                int groupSize = Mathf.Min(Random.Range(1, difficultyLevel + 2), availableSpawns);

                for (int i = 0; i < groupSize; i++)
                {
                    Vector3 groupCenter = GetRandomPositionInBounds(); // Use this for group base
                    Vector3 spawnPos = groupCenter + new Vector3(Random.Range(-2f, 2f), 0f, Random.Range(-2f, 2f));
                    SpawnEnemy(spawnPos, EnemyBase.enemyState.idle);
                    availableSpawns--;

                    if (availableSpawns <= 0)
                        break;
                }

                if (availableSpawns <= 0)
                    break;
            }

            spawnTimer = 0f;
        }
    }
    private Vector3 GetSpawnPositionNearPlayer()
    {
        float minDistance = 10f;
        float maxDistance = 25f;
        for (int i = 0; i < 10; i++)
        {
            SpawnVolume volume = spawnVolumes[Random.Range(0, spawnVolumes.Count)];
            Vector3 point = volume.GetRandomPointInVolume();
            float dist = Vector3.Distance(player.position, point);
            if (dist >= minDistance && dist <= maxDistance)
            {
                return point;
            }
        }
        return player.position + new Vector3(15f, 0f, 0f); // fallback
    }

    public void SpawnEnemy(Vector3 position, EnemyBase.enemyState type) 
    {
        GameObject[] enemies = GetEnemiesForLevel(currentLevelType);
        if(enemies == null || enemies.Length == 0)
        {
            Debug.LogError("No enemies found for the current level type.");
            return;
        }

        GameObject enemyToSpawn = enemies[Random.Range(0, enemies.Length)];
        string enemyTag = enemyToSpawn.name.Replace("(Clone)", "").Trim();
        //GameObject enemyGO = Instantiate(enemyToSpawn, position, Quaternion.identity);
        Vector3 spawnPos = position; // Already grounded in SpawnVolume
        //Vector3 spawnPos = groundedFound ? groundedPosition : position + Vector3.up * 10f;

        if (enemyToSpawn.TryGetComponent<EnemyBase>(out var enemyPrefabBase) && enemyPrefabBase is FlyingEnemy flyingEnemy)
        {
            spawnPos.y += flyingEnemy.idleHoverHeight;
        }

        GameObject enemyGO = ObjectPooler.Instance.SpawnFromPool(enemyTag, spawnPos, Quaternion.identity);

        Debug.DrawRay(spawnPos, Vector3.up * 2f, Color.green, 3f);


        if (enemyGO.TryGetComponent<EnemyBase>(out EnemyBase enemy))
        {
            enemy.playerTarget = player;
            enemy.currentState = type;


            if (enemy is FlyingEnemy FlyingEnemy)
            {
                float hoverHeight = FlyingEnemy.idleHoverHeight;
                spawnPos.y += hoverHeight;
                Debug.Log($"[SpawnEnemy] FlyingEnemy hover adjusted to Y = {spawnPos.y}");
                /*                if (groundedPosition != position)
                                {
                                    spawnPos = new Vector3(groundedPosition.x, groundedPosition.y + hoverHeight, groundedPosition.z);
                                }
                                else
                                {
                                    spawnPos = new Vector3(position.x, position.y + hoverHeight + 10f, position.z);
                                }*/
                /*            Vector3 hoverPos = new Vector3(groundedPosition.x, groundedPosition.y + hoverHeight, groundedPosition.z);
                            enemy.transform.position = hoverPos;*/
            }
                // Grounded enemies also get set just in case the pooler didn't
            enemy.transform.position = spawnPos;


            enemy.BeginSpawning(); // <<< Add this
            Debug.Log($"[SpawnEnemy] Spawning at {spawnPos}");

            activeEnemies.Add(enemy);
            enemy.OnDeath += () => activeEnemies.Remove(enemy);
        }

    }

    private GameObject[] GetEnemiesForLevel(LevelType type)
    {
        foreach (var data in levelEnemyDataList)
        {
            if (data.levelType == type)
                return data.enemyPrefabs;

        }
        return null;

    }

    private Vector3 GetRandomPositionInBounds()
    {
        for (int i = 0; i < 10; i++)
        {
            SpawnVolume volume = spawnVolumes[Random.Range(0, spawnVolumes.Count)];
            Vector3 rawPoint = volume.GetRandomPointInVolume();

            // No second sampling here — already NavMesh-validated inside SpawnVolume
            if (rawPoint != Vector3.zero)
                return rawPoint;
        }

        Debug.LogWarning("Failed to find a valid position from spawn volumes.");
        return Vector3.zero;
    }

    private Vector3 GetGroundedPosition(Vector3 originalPos)
{
    RaycastHit hit;
    Vector3 rayOrigin = originalPos + Vector3.up * 50f;

    if (Physics.Raycast(rayOrigin, Vector3.down, out hit, 100f, ~0, QueryTriggerInteraction.Ignore))
    {
        return new Vector3(originalPos.x, hit.point.y, originalPos.z);
    }

    Debug.LogWarning("Failed to ground enemy spawn position. Using original Y.");
    return originalPos;
}

    private bool TryGetGroundedPosition(Vector3 originalPos, out Vector3 groundedPos)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(originalPos, out hit, 10f, NavMesh.AllAreas))
        {
            groundedPos = hit.position;
            return true;
        }

        groundedPos = Vector3.zero;
        return false;
    }

}
