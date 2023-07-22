using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

public class SpawnManager : MonoBehaviour
{
    [Header("Unit Prefabs")]
    public GameObject ZombiePrefab;
    public GameObject FattyPrefab;
    public GameObject RunnerPrefab;
    public GameObject BloatedPrefab;

    [Space]
    [Header("Spawn Phase And Enemies")]
    [SerializeField] private int CurrentSpawnPhase;
    public List<SpawnPhase> SpawnPhase;
    public int WaitUntilSpawn = 5;
    public bool CanSpawn;

    public List<SpawnPhaseRate> SpawnPhaseRate;
    public AnimationCurve SpawnAmount;
    public List<int> SpawnAmountList;

    // Hidden Fields
    public static SpawnManager Instance;


    public ObjectPool<GameObject> ZombiePool;
    public ObjectPool<GameObject> FattyPool;
    public ObjectPool<GameObject> RunnerPool;
    public ObjectPool<GameObject> BloatedPool;

    public ObjectPool<GameObject> RegularZombiePool;
    
    // Private Fields
    private Spawner[] Spawners;
    [SerializeField] private Spawner ChoosenSpawner;

    void OnValidate()
    {
        SpawnAmountList.Clear();
        for (int i = 0; i < 25; i++)
        {
            SpawnAmountList.Add((int)SpawnAmount.Evaluate(i));
        }
    }

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

        ZombiePool = new ObjectPool<GameObject>(() => CreateZombie(ZombiePrefab, ZombiePool));
        FattyPool = new ObjectPool<GameObject>(() => CreateZombie(FattyPrefab, FattyPool));
        RunnerPool = new ObjectPool<GameObject>(() => CreateZombie(RunnerPrefab, RunnerPool));
        BloatedPool = new ObjectPool<GameObject>(() => CreateZombie(BloatedPrefab, BloatedPool));

        RegularZombiePool = new ObjectPool<GameObject>(()=>CreateZombie(ZombiePrefab, RegularZombiePool));

        //StartCoroutine(SpawnRegular());
    }

    public void StartSpawnCycle()
    {
        //StartCoroutine(Spawn());
        StartCoroutine(SpawnSecondOption());
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
        foreach (SpawnSide SpawnInfo in SpawnPhase[CurrentSpawnPhase].Spawns)
        {
            Spawner spawner = ChoosenSpawner == null ? remainingSpawner[Random.Range(0, remainingSpawner.Count)] : ChoosenSpawner;

            spawner.SpawnSide = SpawnInfo.Clone() as SpawnSide;

            spawner.Spawn();

            if (ChoosenSpawner == null) remainingSpawner.Remove(spawner);
        }
        
        yield return new WaitUntil(
            ()=>ZombiePool.CountActive == 0 &&
            FattyPool.CountActive == 0 &&
            RunnerPool.CountActive == 0 &&
            BloatedPool.CountActive == 0
            );

        yield return new WaitForSeconds(WaitUntilSpawn);

        CurrentSpawnPhase++;

        StartCoroutine(Spawn());
    }

    private IEnumerator SpawnSecondOption()
    {
        yield return new WaitUntil(() => CanSpawn);

        SpawnSide spawnSide = new SpawnSide();
        spawnSide.Side = new List<Spawn>();
        
        foreach (SpawnRate spawnRate in SpawnPhaseRate[CurrentSpawnPhase].Spawns)
        {
            Spawn spawn = new Spawn();
            spawn.EnemyType = spawnRate.EnemyType;
            spawn.SpawnAmount = (int)(spawnRate.Rate * SpawnAmountList[CurrentSpawnPhase]);
            spawnSide.Side.Add(spawn);
        }

        List<Spawner> remainingSpawner = Spawners.ToList();

        Spawner spawner = ChoosenSpawner == null ? remainingSpawner[Random.Range(0, remainingSpawner.Count)] : ChoosenSpawner;

        spawner.SpawnSide = spawnSide;

        spawner.Spawn();

        if (ChoosenSpawner == null) remainingSpawner.Remove(spawner);

        yield return new WaitUntil(
            () => ZombiePool.CountActive == 0 &&
            FattyPool.CountActive == 0 &&
            RunnerPool.CountActive == 0 &&
            BloatedPool.CountActive == 0
            );

        yield return new WaitForSeconds(WaitUntilSpawn);

        CurrentSpawnPhase++;

        StartCoroutine(SpawnSecondOption());
    }

    private IEnumerator SpawnRegular()
    {
        yield return new WaitUntil(() => CanSpawn);
        // TODO : Implement Spawn Sides
        // maybe split enemies into sides based on number of enemy limitations ? 
        // one side weak, other side stronger, as enemy amount incresed increase sides etc
        Spawner spawner = ChoosenSpawner == null ? Spawners[Random.Range(0, 3)] : ChoosenSpawner;
        //spawner.Spawns = SpawnPhase[0].Spawns;

        SpawnSide spawnSide = new SpawnSide();
        Spawn randomSpawn = new Spawn();
        randomSpawn.EnemyType = EnemyType.Zombie;
        randomSpawn.SpawnAmount = Random.Range(1, 3);
        spawnSide.Side.Add(randomSpawn);
        
        spawner.SpawnSide = spawnSide;

        //spawner.Spawn(RegularZombiePool);

        yield return new WaitForSeconds(Random.Range(5, 10));

        StartCoroutine(SpawnRegular());
    }

    private GameObject CreateZombie(GameObject UnitPrefab, ObjectPool<GameObject> UnitPool)
    {
        GameObject instance = Instantiate(UnitPrefab);
        instance.GetComponent<FadeOutToObjectPool>().Pool = UnitPool;

        return instance;
    }

    public ObjectPool<GameObject> GetPool(EnemyType Type)
    {
        switch (Type)
        {
            case EnemyType.Zombie:
                return ZombiePool;
            case EnemyType.Fatty:
                return FattyPool;
            case EnemyType.Runner:
                return RunnerPool;
            case EnemyType.Bloated:
                return BloatedPool;
        }
        return null;
    }
}

[System.Serializable]
public class SpawnPhase
{
    public int Phase;
    public List<SpawnSide> Spawns;
}

[System.Serializable]
public class SpawnPhaseRate
{
    public int Phase;
    public List<SpawnRate> Spawns;
}

[System.Serializable]
public class SpawnRate
{
    [Range(0f, 1f)]
    public float Rate;
    public EnemyType EnemyType;
}