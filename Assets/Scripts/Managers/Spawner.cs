using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public List<SpawnInfo> Spawns;

    public void Spawn()
    {
        foreach (SpawnInfo spawn in Spawns) 
        {
            while (spawn.SpawnAmount > 0)
            {
                Vector3 randomSpawnPos = new Vector3(Random.Range(-10, 11), SpawnManager.Instance.ZombiePrefab.transform.position.y, Random.Range(-10, 11));
                randomSpawnPos = randomSpawnPos + transform.position;

                GameObject Zombie = SpawnManager.Instance.ZombiePool.Get();
                Zombie.SetActive(true);
                Zombie.transform.position = randomSpawnPos;

                spawn.SpawnAmount--;
            }
        }
    }
}

[System.Serializable]
public class SpawnInfo
{
    public int SpawnAmount;
    public SpawnTypes SpawnType;
}

public enum SpawnTypes
{
    Zombie,
}