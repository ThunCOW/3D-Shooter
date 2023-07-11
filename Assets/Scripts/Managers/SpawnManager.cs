using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class SpawnManager : MonoBehaviour
{
    [Header("Unit Prefabs")]
    public GameObject ZombiePrefab;

    [Space]
    [Header("Spawn Phase And Enemies")]
    public List<SpawnPhase> SpawnPhase;
    public int WaitUntilSpawn = 5;

    // Hidden Fields
    public static SpawnManager Instance;

    public static int CurrentSpawnPhase;

    public ObjectPool<GameObject> ZombiePool;

    // Private Fields
    private Spawner[] Spawners;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        Spawners = GetComponentsInChildren<Spawner>();

        ZombiePool = new ObjectPool<GameObject>(CreateZombie);
    }

    public void StartSpawnCycle()
    {
        StartCoroutine(Spawn());
    }

    public IEnumerator Spawn()
    {
        // TODO : Implement Spawn Sides
        // maybe split enemies into sides based on number of enemy limitations ? 
        // one side weak, other side stronger, as enemy amount incresed increase sides etc
        Spawner spawner = Spawners[Random.Range(0, 3)];
        spawner.Spawns = SpawnPhase[0].Spawns;
        spawner.Spawn();
        
        yield return new WaitUntil(()=>AreEnemiesDead() == true);

        yield return new WaitForSeconds(WaitUntilSpawn);

        StartCoroutine(Spawn());
    }

    private GameObject CreateZombie()
    {
        GameObject instance = Instantiate(ZombiePrefab);
        instance.GetComponent<FadeOutToObjectPool>().Pool = ZombiePool;

        return instance;
    }

    private bool AreEnemiesDead()
    {
        Debug.Log(ZombiePool.CountActive);

        if (ZombiePool.CountActive == 0)
            return true;
        else
            return false;
    }
}

[System.Serializable]
public class SpawnPhase
{
    public int Phase;
    public List<SpawnInfo> Spawns;
}