using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyController : MonoBehaviour
{
    public int ScorePoint;
    [Space]

    [Space]
    public AnimationCurve NavMeshRadiusCurve;

    [Header("Reaction To Damage Variables")]
    public float DamageReactionTime;
    public AnimationCurve DamageReactionMovementCurve;
    private Coroutine ReactionRoutine;
    [Space]

    public BloodType EnemyBloodType;
    [SerializeField] private GameObject RedBloodPrefab;
    [SerializeField] private GameObject GreenBloodPrefab;
    public static ObjectPool<GameObject> RedBloodPool;
    public static ObjectPool<GameObject> GreenBloodPool;

    [Space]
    [SerializeField] private float ZombieMoanVolume;
    [SerializeField] private List<AudioClip> ZombieMoans;

    #region ************ Private/Hidden Variables ******************

    private EnemyHealth Health;
    private EnemyPathfinding enemyPathfinding;
    private RagdollManager ragdollManager;
    private FadeOutToObjectPool fadeOutToObjectPool;
    private Animator enemyAnimator;
    private CapsuleCollider col;
    private AudioSource audioSource;

    [HideInInspector] public bool isHurt;
    
    #endregion

    void Awake()
    {
        Health = gameObject.GetComponent<EnemyHealth>();
        enemyPathfinding = GetComponent<EnemyPathfinding>();
        ragdollManager = GetComponent<RagdollManager>();
        fadeOutToObjectPool = GetComponent<FadeOutToObjectPool>();
        enemyAnimator = GetComponentInChildren<Animator>();
        col = GetComponent<CapsuleCollider>();
        audioSource = GetComponent<AudioSource>();

        enemyAnimator.Play(0, -1, Random.value);
    }

    // Start is called before the first frame update
    void Start()
    {
        Health.OnTakeDamage += HandlePain;
        Health.OnDeath += Death;

        enemyPathfinding.navMeshAgent.radius = NavMeshRadiusCurve.Evaluate(Random.Range(0f, 1f));

        if (RedBloodPool == null)
        {
            RedBloodPool = new ObjectPool<GameObject>(() => CreateBloodPool(RedBloodPrefab));
        }
        if (GreenBloodPool == null)
        {
            GreenBloodPool = new ObjectPool<GameObject>(() => CreateBloodPool(GreenBloodPrefab));
        }
    }

    private void OnEnable()
    {
        StartCoroutine(ZombieMoansRoutine());

        isHurt = false;

        col.enabled = true;

        // Reset Child Position On Spawn
        transform.GetChild(0).transform.localPosition = Vector3.zero;
    }

    public IEnumerator ZombieMoansRoutine()
    {
        yield return new WaitForSeconds(Random.Range(3, 25));

        audioSource.PlayOneShot(ZombieMoans[Random.Range(0, ZombieMoans.Count)], ZombieMoanVolume);

        StartCoroutine(ZombieMoansRoutine());
    }

    private GameObject CreateBloodPool(GameObject BloodPrefab)
    {
        GameObject Blood = Instantiate(BloodPrefab);

        return Blood;
    }

    public static void ClearBloodPool()
    {
        RedBloodPool.Clear();
        GreenBloodPool.Clear();
    }

    private IEnumerator BloodSplash()
    {
        GameObject Blood = GetBloodType().Get();
        Blood.GetComponent<ParticleSystem>().Play();
        Blood.transform.SetParent(gameObject.transform);
        Blood.transform.localRotation = Quaternion.Euler(-90, -90, 0);
        Blood.transform.SetParent(null);

        Vector3 spawnPos = transform.position;
        spawnPos.y = 1.85f;
        Blood.transform.position = spawnPos;

        yield return new WaitForSeconds(Random.Range(10, 20));

        GetBloodType().Release(Blood);
    }
    #region   ********************* HANDLE PAIN AND DEATH ***********************
    private void Death(Vector3 forceDir)
    {
        GameManager.Instance.StartCoroutine(BloodSplash());

        isHurt = true;

        ScoreManager.Instance.IncreaseScore(ScorePoint);

        enemyPathfinding.navMeshAgent.enabled = false;

        col.enabled = false;

        ragdollManager.Activate(true);
        ragdollManager.RagdollDeath(forceDir);

        fadeOutToObjectPool.FadeOut();

        if (ReactionRoutine != null)
            StopCoroutine(ReactionRoutine);

        GameManager.Instance.KillCount++;
    }

    private void HandlePain(Vector3 forceDir, float pushDistance)
    {
        GameManager.Instance.StartCoroutine(BloodSplash());

        if (ReactionRoutine != null)
            StopCoroutine(ReactionRoutine);

        ReactionRoutine = StartCoroutine(HandlePainRoutine(forceDir, pushDistance));
    }

    private IEnumerator HandlePainRoutine(Vector3 forceDir, float pushDistance)             // Direction is normalized
    {
        isHurt = true;
        enemyAnimator.SetBool("Move", false);

        float time = 0;
        float curveValueDifferenceOld = 0;
        float curveValueDifference = 1;                                                      // Will calculate the value difference between two point in curve to multiply with the pushDistance
        while (time < DamageReactionTime)
        {
            curveValueDifference = DamageReactionMovementCurve.Evaluate(time / DamageReactionTime);
            transform.position += forceDir * pushDistance * (curveValueDifference - curveValueDifferenceOld);
            curveValueDifferenceOld = curveValueDifference;

            time = time + Time.deltaTime > DamageReactionTime ? DamageReactionTime : time + Time.deltaTime;
            yield return null;
        }

        isHurt = false;
    }
    #endregion


    private ObjectPool<GameObject> GetBloodType()
    {
        switch (EnemyBloodType)
        {
            case BloodType.GreenBlood:
                return GreenBloodPool;
            case BloodType.RedBlood:
                return RedBloodPool;
            default:
                Debug.LogWarning("Set Blood Type Here!");
                return RedBloodPool;
        }
    }
    public enum BloodType
    {
        RedBlood,
        GreenBlood
    }
}
