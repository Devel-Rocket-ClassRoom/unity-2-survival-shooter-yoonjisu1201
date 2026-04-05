using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class MonsterSpawner : MonoBehaviour
{
    //적 프리팹, 스폰 위치, 시간
    public GameObject[] enemyPrefabs;  //0번 바니, 1번 베어, 2번 코끼리
    public Transform[] spawnPoins;

    public float spawnInterval = 3f;
    private float lastSpawnTime = 0;


    public void Update()
    {
        if (Time.time > lastSpawnTime + spawnInterval)
        {
            int enemyIndex = Random.Range(0, enemyPrefabs.Length);
            int locationIndex = Random.Range(0, spawnPoins.Length);
            SpawnEnemy(enemyIndex, locationIndex);
            lastSpawnTime = Time.time;
        }

    }

    public void SpawnEnemy(int enemyIndex, int locationIndex)
    {
        Transform selectedPoint = spawnPoins[locationIndex];

        Instantiate(enemyPrefabs[enemyIndex], selectedPoint.position, selectedPoint.rotation);
    }

}
