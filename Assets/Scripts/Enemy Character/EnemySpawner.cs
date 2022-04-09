using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpawnMethod
{
    Random = 0,
    Sequence = 1,
}

public class EnemySpawner : MonoBehaviour
{
    public GameObject playerCharacter;
    public int maximumEnemyEntity = 1;
    public int increaseMaximumEnemyEntityEvery = 1;
    public SpawnMethod spawnEnemyMethod = SpawnMethod.Random;
    public SpawnMethod chooseSpawnPointMethod = SpawnMethod.Random;

    public GameObject[] enemyList;
    public GameObject[] spawnPointList;

    private List<GameObject> enemyEntityInPlay;
    private int currentSpawnEnemyIndex = 0;
    private int currentSpawnPointIndex = 0;
    private int currentSpawnCount = 0;

    private int lastedSpawnPoint = -1;

    private void Start()
    {
        enemyEntityInPlay = new List<GameObject>();
    }

    private void Update()
    {
        Check_EnemyEntity();

        if (enemyEntityInPlay.Count < maximumEnemyEntity)
        {
            Spawn_EnemyEntity();
        }
    }

    void Spawn_EnemyEntity()
    {
        int spawnIndex = currentSpawnEnemyIndex;

        if(spawnEnemyMethod == SpawnMethod.Random)
        {
            spawnIndex = Random.Range(0, enemyList.Length);
        }
        else
        {
            currentSpawnEnemyIndex += 1;

            if(currentSpawnEnemyIndex >= enemyList.Length)
            {
                currentSpawnEnemyIndex = 0;
            }
        }

        int spawnPointIndex = currentSpawnPointIndex;

        if (chooseSpawnPointMethod == SpawnMethod.Random)
        {
            spawnPointIndex = Random.Range(0, spawnPointList.Length);

            while(lastedSpawnPoint == spawnPointIndex)
            {
                spawnPointIndex = Random.Range(0, spawnPointList.Length);
            }
        }
        else
        {
            currentSpawnPointIndex += 1;

            if (currentSpawnPointIndex >= spawnPointList.Length)
            {
                currentSpawnPointIndex = 0;
            }
        }

        lastedSpawnPoint = spawnPointIndex;

        GameObject targetSpawnEnemy = enemyList[spawnIndex];
        GameObject targetSpawnPoint = spawnPointList[spawnPointIndex];

        GameObject enemyInstance = Instantiate(targetSpawnEnemy);
        enemyInstance.transform.position = targetSpawnPoint.transform.position;

        EnemyBehavior_Base enemyBehavior = enemyInstance.GetComponent<EnemyBehavior_Base>();
        enemyBehavior.targetToAttack = playerCharacter;

        enemyEntityInPlay.Add(enemyInstance);

        currentSpawnCount += 1;

        if(currentSpawnCount >= increaseMaximumEnemyEntityEvery)
        {
            maximumEnemyEntity += 1;
            currentSpawnCount = 0;
        }
    }

    void Check_EnemyEntity()
    {
        for(int index = 0; index < enemyEntityInPlay.Count; index++)
        {
            CharacterProperties enemyProperties = enemyEntityInPlay[index].GetComponent<CharacterProperties>();

            if(enemyProperties.Get_CurrentHealth() <= 0)
            {
                enemyEntityInPlay.RemoveAt(index);
                index -= 1;
            }
        }
    }
}
