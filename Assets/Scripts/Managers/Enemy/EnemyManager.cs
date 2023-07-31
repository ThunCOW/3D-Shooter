using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class EnemyManager : MonoBehaviour
{
    #region Private/Hidden Variables

    Transform scuffedChild;
    
    private PlayerManager playerManager;
    private EnemyHealth Health;
    private RagdollManager ragdollManager;
    private FadeOutToObjectPool fadeOutToObjectPool;
    private Animator enemyAnimator;
    private CapsuleCollider col;

    private Vector3 distanceToTargetVector;
    private float distanceToPlayer;
    private Vector3 moveDirection;
    Vector3 turnDirection = Vector3.zero;
    private NavMeshAgent navMeshAgent;
    [HideInInspector] public NavMeshPath pathToPlayer;

    private float countdownForPath = 0;
    private bool canMove = true;
    #endregion

    
    public float PathCalculationTimeMax;
    private bool isFarToPlayer;                                 // if a unit is far its path will be calculated through navmesh
    
    public bool SnapMovementAndRotation;
    public float MovementSpeed;
    public float RotateTime;

    public int ScorePoint;

    [Space]
    public AnimationCurve RagdollRadiusCurve;

    [Header("Reaction To Damage Variables")]
    public float DamageReactionTime;
    public AnimationCurve DamageReactionMovementCurve;
    private Coroutine ReactionRoutine;

    void Awake()
    {
        playerManager = GameManager.Instance.Player.GetComponent<PlayerManager>();
        Health = gameObject.GetComponent<EnemyHealth>();
        ragdollManager = GetComponent<RagdollManager>();
        fadeOutToObjectPool = GetComponent<FadeOutToObjectPool>();
        enemyAnimator = GetComponentInChildren<Animator>();
        col = GetComponent<CapsuleCollider>();

        pathToPlayer = new NavMeshPath();
        navMeshAgent = GetComponentInChildren<NavMeshAgent>();
        
        scuffedChild = transform.GetChild(0);

        enemyAnimator.Play(0, -1, Random.value);
    }

    // Start is called before the first frame update
    void Start()
    {
        Health.OnTakeDamage += HandlePain;
        Health.OnDeath += Die;

        navMeshAgent.radius = RagdollRadiusCurve.Evaluate(Random.Range(0f, 1f));
    }

    // Update is called once per frame
    void Update()
    {
        if (navMeshAgent.enabled)
        {
            //HandlePathNavMesh();
            //Rotation();
        }
        else
        {
            //HandlePath();
            //Rotation();
        }
    }

    void FixedUpdate()
    {
        if(canMove)
        {
            //HandleMovement();
        }
    }

    void HandlePath()
    {
        if (canMove)
        {
            countdownForPath -= Time.deltaTime;
            if (countdownForPath <= 0)
            {
                distanceToTargetVector = playerManager.transform.position - transform.position;
                //Debug.Log(playerManager.transform.position + " enemy = " + transform.position);
                moveDirection = distanceToTargetVector;
                float sum = Mathf.Abs(moveDirection.x) + Mathf.Abs(moveDirection.z);
                Vector3 dir = new Vector3(moveDirection.x / sum, 0, moveDirection.z / sum);
                moveDirection = dir;

                #region snapp movement vertically
                if ((moveDirection.x > 0.25f && moveDirection.x <= 0.5f) || (moveDirection.x > 0.5f && moveDirection.x <= 0.75f))
                {
                    moveDirection = new Vector3(0.5f, 0, moveDirection.z);
                }
                else if (moveDirection.x > 0.75f && moveDirection.x <= 1)
                {
                    moveDirection = new Vector3(1, 0, moveDirection.z);
                }
                else if (moveDirection.x >= 0 && moveDirection.x < 0.25)
                {
                    moveDirection = new Vector3(0, 0, moveDirection.z);
                }
                else if ((moveDirection.x < -0.25f && moveDirection.x >= -0.5f) || (moveDirection.x < -0.5f && moveDirection.x >= -0.75f))
                {
                    moveDirection = new Vector3(-0.5f, 0, moveDirection.z);
                }
                else if (moveDirection.x < -0.75f && moveDirection.x >= -1)
                {
                    moveDirection = new Vector3(-1, 0, moveDirection.z);
                }
                else if (moveDirection.x < 0 && moveDirection.x >= -0.25)
                {
                    moveDirection = new Vector3(0, 0, moveDirection.z);
                }
                #endregion
                #region snap movement orizontally
                if ((moveDirection.z > 0.25f && moveDirection.z <= 0.5f) || (moveDirection.z > 0.5f && moveDirection.z <= 0.75f))
                {
                    moveDirection = new Vector3(moveDirection.x, 0, 0.5f);
                }
                else if (moveDirection.z > 0.75f && moveDirection.z <= 1)
                {
                    moveDirection = new Vector3(moveDirection.x, 0, 1);
                }
                else if (moveDirection.z >= 0 && moveDirection.z < 0.25)
                {
                    moveDirection = new Vector3(moveDirection.x, 0, 0);
                }
                else if ((moveDirection.z < -0.25f && moveDirection.z >= -0.5f) || (moveDirection.z < -0.5f && moveDirection.z >= -0.75f))
                {
                    moveDirection = new Vector3(moveDirection.x, 0, -0.5f);
                }
                else if (moveDirection.z < -0.75f && moveDirection.z >= -1)
                {
                    moveDirection = new Vector3(moveDirection.x, 0, -1);
                }
                else if (moveDirection.z < 0 && moveDirection.z >= -0.25)
                {
                    moveDirection = new Vector3(moveDirection.x, 0, 0);
                }
                #endregion

                turnDirection = moveDirection;

                //Debug.Log(moveDirection + " dir = " + dir);

                countdownForPath = Random.Range(0, PathCalculationTimeMax);
            }
        }
    }

    void HandlePathNavMesh()
    {
        if (canMove)
        {
            countdownForPath -= Time.deltaTime;
            if (isFarToPlayer && countdownForPath <= 0)
            {
                //scuffedChild.localPosition = Vector3.zero;

                RaycastHit[] hit = new RaycastHit[20];

                //LayerMask mask = LayerMask.GetMask("Enemy");
                //Physics.SphereCastNonAlloc(transform.position, 2.5f, transform.forward, hit, 2.5f, mask);

                pathToPlayer.ClearCorners();
                Vector3 playerPos = playerManager.transform.position;
                playerPos.y = 0;
                navMeshAgent.CalculatePath(playerPos, pathToPlayer);

                distanceToTargetVector = pathToPlayer.corners[1] - transform.position;
                float sum = Mathf.Abs(distanceToTargetVector.x) + Mathf.Abs(distanceToTargetVector.z);
                turnDirection = new Vector3(distanceToTargetVector.x / sum, 0, distanceToTargetVector.z / sum);
                Rotation();

                distanceToTargetVector.Normalize();
                moveDirection = distanceToTargetVector;

                Vector3 movementVelocity = moveDirection * MovementSpeed;
                //Debug.Log(Vector3.Distance(playerManager.transform.position, transform.position));
                if (Vector3.Distance(playerManager.transform.position, transform.position) > 2.65f)
                {
                    //rigidBody.velocity = movementVelocity;
                    enemyAnimator.SetBool("Move", true);
                }

                //Debug.Log(moveDirection + " dir = " + dir);

                countdownForPath = Random.Range(0, PathCalculationTimeMax);

            }
        }
    }

    bool isMoving;
    void HandleMovement()
    {
        //Vector3 movementVelocity = moveDirection * MovementSpeed;
        //Debug.Log(Vector3.Distance(playerManager.transform.position, transform.position));
        distanceToPlayer = Vector3.Distance(playerManager.transform.position, transform.position);
        if (distanceToPlayer > 2.65f)
        {
            //rigidBody.velocity = movementVelocity;

            if (!isMoving)
            {
                isMoving = true;
                enemyAnimator.SetBool("Move", true);
            }

            if (distanceToPlayer <= 10)
            {
                isFarToPlayer = false;

                distanceToTargetVector = playerManager.transform.position - transform.position;
                moveDirection = distanceToTargetVector;
                float sum = Mathf.Abs(moveDirection.x) + Mathf.Abs(moveDirection.z);
                turnDirection = new Vector3(moveDirection.x / sum, 0, moveDirection.z / sum);
                Rotation();
                
                distanceToTargetVector.Normalize();
                moveDirection = distanceToTargetVector;

                transform.position += moveDirection * MovementSpeed * Time.deltaTime;
            }
            else
            {
                isFarToPlayer = true;
                transform.position += moveDirection * MovementSpeed * Time.deltaTime;
            }
            /*if (distanceToPlayer < 5)
            {
                if (!col.enabled)
                    col.isTrigger = false;
                    col.enabled = true;
            }
            else
            {
                if (col.enabled)
                    col.isTrigger = true;
                    col.enabled = false;
            }*/

        }
        else
        {
            //rigidBody.velocity = Vector3.zero;

            if (isMoving)
            {
                isMoving = false;
                enemyAnimator.SetBool("Move", false);
                Attack();
            }
        }
    }

    private void Rotation()
    {
        /*if (SnapMovementAndRotation)
        {
            #region snapp rotation vertically
            if ((turnDirection.x > 0.25f && turnDirection.x <= 0.5f) || (turnDirection.x > 0.5f && turnDirection.x <= 0.75f))
            {
                turnDirection = new Vector3(0.5f, 0, turnDirection.z);
            }
            else if (turnDirection.x > 0.75f && turnDirection.x <= 1)
            {
                turnDirection = new Vector3(1, 0, turnDirection.z);
            }
            else if (turnDirection.x >= 0 && turnDirection.x < 0.25)
            {
                turnDirection = new Vector3(0, 0, turnDirection.z);
            }
            else if ((turnDirection.x < -0.25f && turnDirection.x >= -0.5f) || (turnDirection.x < -0.5f && turnDirection.x >= -0.75f))
            {
                turnDirection = new Vector3(-0.5f, 0, turnDirection.z);
            }
            else if (turnDirection.x < -0.75f && turnDirection.x >= -1)
            {
                turnDirection = new Vector3(-1, 0, turnDirection.z);
            }
            else if (turnDirection.x < 0 && turnDirection.x >= -0.25)
            {
                turnDirection = new Vector3(0, 0, turnDirection.z);
            }
            #endregion
            #region snap rotation horizontally
            if ((turnDirection.z > 0.25f && turnDirection.z <= 0.5f) || (turnDirection.z > 0.5f && turnDirection.z <= 0.75f))
            {
                turnDirection = new Vector3(turnDirection.x, 0, 0.5f);
            }
            else if (turnDirection.z > 0.75f && turnDirection.z <= 1)
            {
                turnDirection = new Vector3(turnDirection.x, 0, 1);
            }
            else if (turnDirection.z >= 0 && turnDirection.z < 0.25)
            {
                turnDirection = new Vector3(turnDirection.x, 0, 0);
            }
            else if ((turnDirection.z < -0.25f && turnDirection.z >= -0.5f) || (turnDirection.z < -0.5f && turnDirection.z >= -0.75f))
            {
                turnDirection = new Vector3(turnDirection.x, 0, -0.5f);
            }
            else if (turnDirection.z < -0.75f && turnDirection.z >= -1)
            {
                turnDirection = new Vector3(turnDirection.x, 0, -1);
            }
            else if (turnDirection.z < 0 && turnDirection.z >= -0.25)
            {
                turnDirection = new Vector3(turnDirection.x, 0, 0);
            }
            #endregion
        }*/

        transform.forward = turnDirection;
    }

    private IEnumerator RotateSlowly(float rotateTime)
    {
        while ( rotateTime > 0)
        {
            rotateTime -= Time.deltaTime;
            
            if (turnDirection != Vector3.zero)
                transform.forward = turnDirection * Time.deltaTime / rotateTime;

            yield return new WaitForFixedUpdate();
        }
        transform.forward = turnDirection;
    }
    private void Die(Vector3 forceDir)
    {
        //Debug.Log(gameObject.name + " Has Died");

        canMove = false;

        ScoreManager.Instance.IncreaseScore(ScorePoint);

        navMeshAgent.enabled = false;

        col.enabled = false;

        ragdollManager.Activate(true);
        ragdollManager.RagdollDeath(forceDir);

        fadeOutToObjectPool.FadeOut();

        StopAllCoroutines();
    }
    
    private void HandlePain(Vector3 forceDir, float pushDistance)
    {
        if (ReactionRoutine != null)
            StopCoroutine(ReactionRoutine);
        StartCoroutine(HandlePainRoutine(forceDir, pushDistance));
    }

    private IEnumerator HandlePainRoutine(Vector3 forceDir, float pushDistance)             // Direction is normalized
    {
        canMove = false;
        isMoving = false;
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

        canMove = true;
    }


    private void Attack()
    {
        enemyAnimator.SetTrigger("Attack");
        Vector3 forceDir = transform.position - GameManager.Instance.Player.transform.position;
        forceDir.y = 0;
        forceDir.Normalize();
        GameManager.Instance.Player.GetComponent<EnemyHealth>().TakeDamage(25, forceDir * -1, 2.5f);
    }

    private void OnEnable()
    {
        canMove = true;

        col.enabled = true;

        navMeshAgent.enabled = true;

        scuffedChild.localPosition = Vector3.zero;
    }
}
