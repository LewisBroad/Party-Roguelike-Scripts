using UnityEngine;

[CreateAssetMenu(menuName = "EnemySpawning/Level Enemy Data")]
public class LevelEnemyDataSO : ScriptableObject
{
    public SpawnManager.LevelType levelType;
    public GameObject[] enemyPrefabs;
}
