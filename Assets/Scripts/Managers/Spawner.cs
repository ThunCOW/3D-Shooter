using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

public class Spawner : MonoBehaviour
{
    public SpawnSide SpawnSide;

    public void Spawna()
    {
        /*foreach (SpawnSide spawn in SpawnSide) 
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
        }*/
    }

    public void Spawn()
    {
        foreach (Spawn spawn in SpawnSide.Side)
        {
            while (spawn.SpawnAmount > 0)
            {
                Vector3 randomSpawnPos = new Vector3(Random.Range(-10, 11), SpawnManager.Instance.ZombiePrefab.transform.position.y, Random.Range(-10, 11));
                randomSpawnPos = randomSpawnPos + transform.position;

                GameObject Zombie = SpawnManager.Instance.GetPool(spawn.EnemyType).Get();
                Zombie.SetActive(true);

                NavMeshAgent agent = Zombie.GetComponentInChildren<NavMeshAgent>();
                if (agent == null || agent.enabled == false)
                {
                    Zombie.transform.position = randomSpawnPos;

                    spawn.SpawnAmount--;
                }
                else
                {
                    NavMeshHit navMeshHit;
                    NavMesh.SamplePosition(randomSpawnPos, out navMeshHit, 2f, -1);
                    {
                        if(agent.transform == Zombie.transform)
                        {
                            agent.Warp(navMeshHit.position);
                        }
                        else
                        {
                            Transform parent = agent.transform.parent;
                            agent.transform.SetParent(null);

                            agent.Warp(navMeshHit.position);
                            Zombie.transform.position = agent.transform.position;
                            agent.transform.SetParent(parent.transform);
                        }

                        spawn.SpawnAmount--;
                    }
                }
            }
        }
    }

    
}

[System.Serializable]
public class SpawnSide : System.ICloneable
{
    public List<Spawn> Side;

    public object Clone()
    {
        SpawnSide side = new SpawnSide();
        
        side.Side = new List<Spawn>();
        side.Side.AddRange(Side);
        //info.SpawnAmount = SpawnAmount;
        //info.SpawnType = SpawnType;

        return side;
    }
}

[System.Serializable]
public class Spawn
{
    public int SpawnAmount;
    public EnemyType EnemyType;
}