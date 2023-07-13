using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public bool CanSpawn;

    // Hidden Fields
    public static SpawnManager Instance;

    public static int CurrentSpawnPhase;

    public ObjectPool<GameObject> ZombiePool;
    public ObjectPool<GameObject> RegularZombiePool;
    
    // Private Fields
    private Spawner[] Spawners;
    [SerializeField] private Spawner ChoosenSpawner;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        Spawners = GetComponentsInChildren<Spawner>();

        ZombiePool = new ObjectPool<GameObject>(()=>CreateZombie(ZombiePool));
        RegularZombiePool = new ObjectPool<GameObject>(()=>CreateZombie(RegularZombiePool));

        StartCoroutine(SpawnRegular());
    }

    public void StartSpawnCycle()
    {
        StartCoroutine(Spawn());
    }

    private IEnumerator Spawn()
    {
        yield return new WaitUntil(() => CanSpawn);
        // TODO : Implement Spawn Sides
        // maybe split enemies into sides based on number of enemy limitations ? 
        // one side weak, other side stronger, as enemy amount incresed increase sides etc
        List<Spawner> remainingSpawner = Spawners.ToList();

        //foreach (SpawnInfo SpawnInfo in SpawnPhase[CurrentSpawnPhase].Spawns)
            //spawner.Spawns.Add(SpawnInfo.Clone() as SpawnInfo);
        foreach (SpawnInfo SpawnInfo in SpawnPhase[CurrentSpawnPhase].Spawns)
        {
            Spawner spawner = ChoosenSpawner == null ? remainingSpawner[Random.Range(0, remainingSpawner.Count)] : ChoosenSpawner;

            spawner.Spawns.Add(SpawnInfo.Clone() as SpawnInfo);

            spawner.Spawn(ZombiePool);

            if (ChoosenSpawner == null) remainingSpawner.Remove(spawner);
        }

        
        yield return new WaitUntil(()=>ZombiePool.CountActive == 0);

        yield return new WaitForSeconds(WaitUntilSpawn);

        CurrentSpawnPhase++;

        StartCoroutine(Spawn());
    }

    private IEnumerator SpawnRegular()
    {
        yield return new WaitUntil(() => CanSpawn);
        // TODO : Implement Spawn Sides
        // maybe split enemies into sides based on number of enemy limitations ? 
        // one side weak, other side stronger, as enemy amount incresed increase sides etc
        Spawner spawner = ChoosenSpawner == null ? Spawners[Random.Range(0, 3)] : ChoosenSpawner;
        //spawner.Spawns = SpawnPhase[0].Spawns;
        SpawnInfo randomSpawn = new SpawnInfo();
        randomSpawn.SpawnType = SpawnTypes.Zombie;
        randomSpawn.SpawnAmount = Random.Range(1, 3);
        
        spawner.Spawns.Add(randomSpawn);

        spawner.Spawn(RegularZombiePool);

        yield return new WaitForSeconds(Random.Range(5, 10));

        StartCoroutine(SpawnRegular());
    }

    private GameObject CreateZombie(ObjectPool<GameObject> pool)
    {
        GameObject instance = Instantiate(ZombiePrefab);
        instance.GetComponent<FadeOutToObjectPool>().Pool = pool;

        return instance;
    }
}

[System.Serializable]
public class SpawnPhase
{
    public int Phase;
    public List<SpawnInfo> Spawns;
}