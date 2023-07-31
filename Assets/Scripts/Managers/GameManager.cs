using System.Collections;
using System.Collections.Generic;
using TMPro;
using Tymski;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // **************** Serialized Field *****************
    [SerializeField] SceneReference LevelScene;
    [Space]
    
    [SerializeField] private GameObject CanvasParent;
    [Space]

    [SerializeField] private GameObject GameOverPrefab;
    [Space]

    [SerializeField] private GameObject PhaseNotificationPrefab;
    [Space]

    // *************** Public Field ****************
    public GameObject Player;
    [Space]

    public bool isGameOver;
    public bool PauseGame;
    
    [Header("************ Prefab Null Ref Bug Workaround *************")]
    public GameObject ZombieSpitProjectilePrefab;
    [Space]

    // ***************** Hidden / Private Field ******************
    [HideInInspector] public int KillCount;

    void Awake()
    {
        if (Instance != null)
            Destroy(Instance);

        Instance = this;

        /*
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(this.gameObject);
        */
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnEnemy());
    }

    void Update()
    {
        RestartGame();
    }

    IEnumerator SpawnEnemy()
    {
        yield return new WaitForSeconds(2.5f);

        SpawnManager.Instance.StartSpawnCycle();
    }

    public void SpawnPhaseNotification()
    {
        Instantiate(PhaseNotificationPrefab, CanvasParent.transform);
    }

    public void GameOver()
    {
        isGameOver = true;

        GameObject gameOverMessage = Instantiate(GameOverPrefab, CanvasParent.transform);
        foreach (TMP_Text t in gameOverMessage.GetComponentsInChildren<TMP_Text>())
        {
            StartCoroutine(TextAppearSlowly(t, 0, 2.5f, 5));
        }
        
    }

    private void RestartGame()
    {
        if (isGameOver)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                isGameOver = false;
                StopAllCoroutines();
                
                GunScriptableObject.ClearProjectilePool();
                EnemyController.ClearBloodPool();
                EnemyLocomotion.ClearZombieSpitProjectilePool();
                SceneManager.LoadScene(LevelScene);
            }
        }
    }

    private IEnumerator TextAppearSlowly(TMP_Text TextSpawn, float startDelay, float appearTime = 1.5f, float speed = 10)
    {
        Color tempColor = TextSpawn.color;

        yield return new WaitForSeconds(startDelay);

        float countdown = appearTime;
        // Start Changing Alpha
        while (countdown > 0)
        {
            countdown -= Time.deltaTime;

            float percentage = 1 - (countdown / appearTime);
            tempColor.a = percentage;

            TextSpawn.color = tempColor;
            TextSpawn.transform.position = new Vector3(TextSpawn.transform.position.x, TextSpawn.transform.position.y - (speed * Time.deltaTime), TextSpawn.transform.position.z);

            yield return new WaitForFixedUpdate();
        }
    }
}