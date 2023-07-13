using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject Player;

    public GameObject Zombie;

    #region *_________Score_________*

    public delegate void OnUnlockUpgradeList(int level);
    public OnUnlockUpgradeList onUnlockUpgradeList;

    [SerializeField] private int _currentLevel;
    public int CurrentLevel
    {
        get { return _currentLevel; }
        set
        {
            _currentLevel = value;
            if (_currentLevel > _MaxReachedLevel)
                MaxReachedLevel = _currentLevel;
        }
    }
    private int _MaxReachedLevel;
    public int MaxReachedLevel
    {
        get { return _MaxReachedLevel; }
        set
        {
            _MaxReachedLevel = value;
            onUnlockUpgradeList?.Invoke(CurrentLevel);
        }
    }
    #endregion

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
        StartCoroutine(SpawnEnemy());

        CurrentLevel = 10;
    }

    IEnumerator SpawnEnemy()
    {
        yield return new WaitForSeconds(5);

        SpawnManager.Instance.StartSpawnCycle();
        /*yield return new WaitForSeconds(Random.Range(2, 5));

        if (canSpawn)
                Instantiate(Zombie, new Vector3(0, Zombie.transform.position.y, 0), Zombie.transform.rotation);

        StartCoroutine(SpawnEnemy());*/
    }
}
