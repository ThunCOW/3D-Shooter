using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject Player;

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