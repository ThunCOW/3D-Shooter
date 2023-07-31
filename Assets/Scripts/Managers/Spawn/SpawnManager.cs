using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

public enum EnemyType
{
    Zombie,
    Fatty,
    Runner,
    Bloated,
    Spitter
}

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;
    
    [Header("************** Unit Prefabs ****************")]
    public GameObject ZombiePrefab;
    public GameObject FattyPrefab;
    public GameObject RunnerPrefab;
    public GameObject BloatedPrefab;
    public GameObject SpitterPrefab;
    [Space]

    [Header("************** Spawn Phase And Enemies ***************")]
    public static int CurrentSpawnPhase;
    public List<SpawnPhase> SpawnPhase;
    public int WaitUntilSpawn = 5;
    public bool CanSpawn;

    public List<SpawnPhaseRate> SpawnPhaseRate;
    public AnimationCurve SpawnAmount;
    public List<int> SpawnAmountList;

    // Normal Pools for Phase Spawns
    public ObjectPool<GameObject> ZombiePool;
    public ObjectPool<GameObject> FattyPool;
    public ObjectPool<GameObject> RunnerPool;
    public ObjectPool<GameObject> BloatedPool;
    public ObjectPool<GameObject> SpitterPool;
    // Regular Pools for Regular Spawns
    public ObjectPool<GameObject> RegularZombiePool;
    public ObjectPool<GameObject> RegularFattyPool;
    public ObjectPool<GameObject> RegularRunnerPool;
    public ObjectPool<GameObject> RegularBloatedPool;
    public ObjectPool<GameObject> RegularSpitterPool;

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

        for (int i = 0; i < SpawnAmountList.Count; i++)
        {
            foreach (SpawnRate spawn in SpawnPhaseRate[i].Spawns)
            {
                spawn.EditorViewAmount = (int)(spawn.Rate * SpawnAmountList[i]);
            }
        }
    }

    void Awake()
    {
        if (Instance != null)
            Destroy(Instance);

        Instance = this;

        CurrentSpawnPhase = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        Spawners = GetComponentsInChildren<Spawner>();

        ZombiePool = new ObjectPool<GameObject>(() => CreateZombie(ZombiePrefab, ZombiePool));
        FattyPool = new ObjectPool<GameObject>(() => CreateZombie(FattyPrefab, FattyPool));
        RunnerPool = new ObjectPool<GameObject>(() => CreateZombie(RunnerPrefab, RunnerPool));
        BloatedPool = new ObjectPool<GameObject>(() => CreateZombie(BloatedPrefab, BloatedPool));
        SpitterPool = new ObjectPool<GameObject>(() => CreateZombie(SpitterPrefab, SpitterPool));

        RegularZombiePool = new ObjectPool<GameObject>(() => CreateZombie(ZombiePrefab, RegularBloatedPool));
        RegularFattyPool = new ObjectPool<GameObject>(() => CreateZombie(FattyPrefab, RegularFattyPool));
        RegularRunnerPool = new ObjectPool<GameObject>(() => CreateZombie(RunnerPrefab, RegularRunnerPool));
        RegularBloatedPool = new ObjectPool<GameObject>(() => CreateZombie(BloatedPrefab, RegularBloatedPool));
        RegularSpitterPool = new ObjectPool<GameObject>(() => CreateZombie(SpitterPrefab, RegularSpitterPool));

        StartCoroutine(SpawnRegular());
    }

    public void StartSpawnCycle()
    {
        GameManager.Instance.SpawnPhaseNotification();
        //StartCoroutine(Spawn());
        StartCoroutine(SpawnSecondOption());
    }
    /* Eski Spawn
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
    }*/

    private IEnumerator SpawnSecondOption()
    {
        yield return new WaitUntil(() => CanSpawn);

        SpawnSide spawnSide = new SpawnSide();
        spawnSide.Side = new List<Spawn>();

        int tempSpawnPhase = Mathf.Clamp(CurrentSpawnPhase, 0, SpawnPhaseRate.Count - 1);
        foreach (SpawnRate spawnRate in SpawnPhaseRate[tempSpawnPhase].Spawns)
        {
            Spawn spawn = new Spawn();
            spawn.EnemyType = spawnRate.EnemyType;
            spawn.SpawnAmount = (int)(spawnRate.Rate * SpawnAmountList[tempSpawnPhase]);
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
            BloatedPool.CountActive == 0 &&
            SpitterPool.CountActive == 0
            );

        NextPhase();

        yield return new WaitForSeconds(WaitUntilSpawn);

        StartCoroutine(SpawnSecondOption());
    }

    public void NextPhase()
    {
        CurrentSpawnPhase++;
        GameManager.Instance.SpawnPhaseNotification();
    }

    private IEnumerator SpawnRegular()
    {
        yield return new WaitUntil(() => CanSpawn);
        // TODO : Implement Spawn Sides
        // maybe split enemies into sides based on number of enemy limitations ? 
        // one side weak, other side stronger, as enemy amount incresed increase sides etc
        if (Spawners.Length <= 1)
            yield break;

        List<Spawner> spawnersExceptChoosen = new List<Spawner>();
        spawnersExceptChoosen.AddRange(Spawners);
        spawnersExceptChoosen.Remove(ChoosenSpawner);
        Spawner spawner = spawnersExceptChoosen[Random.Range(0, spawnersExceptChoosen.Count)];

        Spawn randomSpawn = new Spawn();
        int tempSpawnPhase = Mathf.Clamp(CurrentSpawnPhase, 0, SpawnPhaseRate.Count - 1);
        List<SpawnRate> spawns = SpawnPhaseRate[tempSpawnPhase].Spawns;
        int randomEnemyIndex = Random.Range(0, SpawnPhaseRate[tempSpawnPhase].Spawns.Count);
        randomSpawn.EnemyType = spawns[randomEnemyIndex].EnemyType;
        randomSpawn.SpawnAmount = Mathf.Clamp(Random.Range(1, 3), 1, 5);

        SpawnSide spawnSide = new SpawnSide();
        spawnSide.Side = new List<Spawn>();
        spawnSide.Side.Add(randomSpawn);
        
        spawner.SpawnSide = spawnSide;

        spawner.Spawn(true);

        yield return new WaitForSeconds(Random.Range(5, 10));

        StartCoroutine(SpawnRegular());
    }

    private GameObject CreateZombie(GameObject UnitPrefab, ObjectPool<GameObject> UnitPool)
    {
        GameObject instance = Instantiate(UnitPrefab);
        instance.GetComponent<FadeOutToObjectPool>().Pool = UnitPool;

        return instance;
    }

    public ObjectPool<GameObject> GetPool(EnemyType Type, bool isRegular)
    {
        if (!isRegular)
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
                case EnemyType.Spitter:
                    return SpitterPool;
            }
        }
        else
        {
            switch (Type)
            {
                case EnemyType.Zombie:
                    return RegularZombiePool;
                case EnemyType.Fatty:
                    return RegularFattyPool;
                case EnemyType.Runner:
                    return RegularRunnerPool;
                case EnemyType.Bloated:
                    return RegularBloatedPool;
                case EnemyType.Spitter:
                    return RegularSpitterPool;
            }
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
    public List<SpawnRate> Spawns;
}

[System.Serializable]
public class SpawnRate
{
    [Range(0f, 1f)]
    public float Rate;
    public float EditorViewAmount;
    public EnemyType EnemyType;
}