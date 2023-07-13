using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class EnemyManager : MonoBehaviour
{
    PlayerManager playerManager;

    Animator enemyAnimator;

    Vector3 moveDirection;
    NavMeshAgent navMeshAgent;
    public NavMeshPath pathToPlayer;
    NavMeshObstacle obstacle;

    Transform scuffedChild;

    Rigidbody rigidBody;
    Collider col;
    public Rigidbody RagdollRootRb;
    public Rigidbody RagdollHeadRb;

    public float MovementSpeed;
    public float RotationSpeed;

    public EnemyHealth Health;
    public int ScorePoint;
    
    FadeOutToObjectPool fadeOutToObjectPool;

    [Header("Ragdoll Var")]
    public List<Collider> RagdollColliderList;
    public List<Rigidbody> RagdollRigidBodyList;

    void Awake()
    {
        playerManager = GameManager.Instance.Player.GetComponent<PlayerManager>();

        enemyAnimator = GetComponentInChildren<Animator>();

        rigidBody = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        fadeOutToObjectPool = GetComponent<FadeOutToObjectPool>();

        pathToPlayer = new NavMeshPath();
        navMeshAgent = GetComponentInChildren<NavMeshAgent>();
        obstacle = GetComponent<NavMeshObstacle>();
        scuffedChild = transform.GetChild(0);
    }

    // Start is called before the first frame update
    void Start()
    {
        Health.OnTakeDamage += HandlePain;
        Health.OnDeath += Die;
        RagdollActive(false);
    }

    float countdown = 0;
    bool isDead = false;
    // Update is called once per frame
    void Update()
    {
        if(navMeshAgent.enabled)
            HandlePathNavMesh();
        else
            HandlePath();
    }

    void FixedUpdate()
    {
        if(countdown >= 0)
        {
            HandleMovement();
            Rotation();
        }
    }

    void HandlePath()
    {
        if (!isDead)
        {
            countdown -= Time.deltaTime;
            if (countdown <= 0)
            {
                Vector3 dif = playerManager.transform.position - transform.position;
                //Debug.Log(playerManager.transform.position + " enemy = " + transform.position);
                moveDirection = dif;
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

                countdown = Random.Range(0, 2.6f);
            }
        }
    }

    void HandlePathNavMesh()
    {
        if (!isDead)
        {
            countdown -= Time.deltaTime;
            if (countdown <= 0)
            {
                scuffedChild.localPosition = Vector3.zero;

                RaycastHit[] hit = new RaycastHit[20];

                //LayerMask mask = LayerMask.GetMask("Enemy");
                //Physics.SphereCastNonAlloc(transform.position, 2.5f, transform.forward, hit, 2.5f, mask);

                pathToPlayer.ClearCorners();
                navMeshAgent.CalculatePath(playerManager.transform.position, pathToPlayer);

                Vector3 dif = pathToPlayer.corners[1] - transform.position;
                //Debug.Log(playerManager.transform.position + " enemy = " + transform.position);
                moveDirection = dif;
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

                countdown = Random.Range(0, 2.6f);
            }
        }
    }

    void HandleMovement()
    {
        //Vector3 dif = new Vector3((playerManager.transform.position.x - transform.position.x), 0, (playerManager.transform.position.z - transform.position.z));     // -1 because character has reverse coordinates
        //float sum = Mathf.Abs(moveDirection.x) + Mathf.Abs(moveDirection.z);
        //Vector3 dir = new Vector3(moveDirection.x / sum, 0, moveDirection.z / sum);
        //Debug.Log(moveDirection + " dir = " + dir);
        //moveDirection = dif;
        //moveDirection = AimTarget.transform.forward;
        //moveDirection = moveDirection + AimTarget.transform.right * inputManager.HorizontalInput * -1;
        //moveDirection.Normalize();
        /*
        if((moveDirection.x > 0.25f && moveDirection.x <= 0.5f) || (moveDirection.x > 0.5f && moveDirection.x <= 0.75f))
        {
            moveDirection = new Vector3(0.5f, 0, moveDirection.z);
        }
        else if(moveDirection.x > 0.75f && moveDirection.x <= 1)
        {
            moveDirection = new Vector3(1, 0, moveDirection.z);
        }
        else if(moveDirection.x >= 0 && moveDirection.x < 0.25)
        {
            moveDirection = new Vector3(0, 0, moveDirection.z);
        }*/

        //moveDirection.y = 0;
        //moveDirection = moveDirection * MovementSpeed;
        

        Vector3 movementVelocity = moveDirection * MovementSpeed;
        //Debug.Log(Vector3.Distance(playerManager.transform.position, transform.position));
        if (Vector3.Distance(playerManager.transform.position, transform.position) > 2.65f)
        {
            rigidBody.velocity = movementVelocity;
            enemyAnimator.SetBool("Move", true);
        }
        else
        {
            rigidBody.velocity = Vector3.zero;
            enemyAnimator.SetBool("Move", false);
        }
    }

    Vector3 turnDirection = Vector3.zero;
    private void Rotation()
    {
        if (turnDirection != Vector3.zero)
            transform.forward = turnDirection;
    }


    private void HandlePain(int Damage)
    {
        //Take Damage
        //Debug.Log(gameObject.name + " Taken Damage");
    }

    private void Die(Vector3 position)
    {
        //Debug.Log(gameObject.name + " Has Died");

        isDead = true;

        ScoreManager.Instance.IncreaseScore(ScorePoint);

        RagdollActive(true);

        RagdollHeadRb.detectCollisions = true;
        RagdollHeadRb.isKinematic = false;
        RagdollHeadRb.AddForce(transform.forward * -1 * 50, ForceMode.Impulse);
        //RagdollRootRb.AddTorque(Vector3.left * 100000);

        fadeOutToObjectPool.FadeOut();
    }

    private void RagdollActive(bool active)
    {
        foreach (var col in RagdollColliderList)
            col.enabled = active;
        foreach (var rb in RagdollRigidBodyList)
        {
            rb.detectCollisions = active;
            rb.isKinematic = !active;
        }

        enemyAnimator.enabled = !active;
        rigidBody.detectCollisions = !active;
        rigidBody.isKinematic = active;
        col.enabled = !active;
    }
    private void OnEnable()
    {
        isDead = false;

        RagdollActive(false);

        scuffedChild.localPosition = Vector3.zero;
    }
}
