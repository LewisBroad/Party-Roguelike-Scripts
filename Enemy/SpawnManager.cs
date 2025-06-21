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


    [SerializeField] private SpawnType spawnType;
    public List<SpawnVolume> spawnVolumes;

    [Header("Level & Enemy Setting")]
    public LevelType currentLevelType;
    public List<LevelEnemyDataSO> levelEnemyDataList;

    [Header("Spawn Setup")]
    private Transform player;
    public Vector2 mapMinBounds;
    public Vector2 mapMaxBounds;
    public int staticSpawnCount = 10;

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

    public void SpawnEnemy(Vector3 position, EnemyBase.enemyState type) 
    {
        GameObject[] enemies = GetEnemiesForLevel(currentLevelType);
        if(enemies == null || enemies.Length == 0)
        {
            Debug.LogError("No enemies found for the current level type.");
            return;
        }

        GameObject enemyToSpawn = enemies[Random.Range(0, enemies.Length)];
        GameObject enemyGO = Instantiate(enemyToSpawn, position, Quaternion.identity);

        if(enemyGO.TryGetComponent<EnemyBase>(out EnemyBase enemy))
        {
            enemy.playerTarget = player;
            enemy.currentState = type;
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
        for(int i = 0; i < 10; i++)
        {
            SpawnVolume volume = spawnVolumes[Random.Range(0, spawnVolumes.Count)];
            Vector3 pos = volume.GetRandomPointInVolume();
            if (pos != Vector3.zero) return pos; 
/*            float x = Random.Range(mapMinBounds.x, mapMaxBounds.x);
            float z = Random.Range(mapMinBounds.y, mapMaxBounds.y);
            Vector3 randomPoint = new Vector3(x, 100f, z);
            if(NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 150f, NavMesh.AllAreas))
            {
                return hit.position;
            }*/
        }

   
        Debug.LogWarning("Failed to find a valid spawn position after 10 attempts.");
        return Vector3.zero;
    }

}
