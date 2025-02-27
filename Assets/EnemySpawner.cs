using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject enemyPrefab; 
    public GameObject mainEnemyPrefab; 
    public Transform mainEnemySpawnPoint; 
    public int numberOfEnemies = 5; 
    public Vector3 spawnAreaSize = new Vector3(50, 10, 50); 
    public Transform player; 

    void Start()
    {
        SpawnMainEnemy();
        SpawnRandomEnemies();
    }

    void SpawnMainEnemy()
    {
        GameObject mainEnemy = Instantiate(mainEnemyPrefab, mainEnemySpawnPoint.position, Quaternion.identity);
        mainEnemy.GetComponent<DragonAIController>().SetAggressionLevel(3); // Max aggression
        mainEnemy.GetComponent<DragonAIController>().player = player;
    }

    void SpawnRandomEnemies()
    {
        for (int i = 0; i < numberOfEnemies; i++)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
                0, // Default ground
                Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2)
            ) + transform.position;

            bool shouldBeFlying = Random.value > 0.5f; // 50% chance to be flying

            if (shouldBeFlying)
            {
                randomPosition.y = Random.Range(5, 15); // Random height for flying enemies
            }

            GameObject enemy = Instantiate(enemyPrefab, randomPosition, Quaternion.identity);
            enemy.GetComponent<DragonAIController>().SetAggressionLevel(Random.Range(1, 3)); // Random aggression
            enemy.GetComponent<DragonAIController>().player = player;
        }
    }
}
