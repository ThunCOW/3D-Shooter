using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;

public class EnemyLocomotion : MonoBehaviour
{
    EnemyController enemyController;
    EnemyPathfinding enemyPathfinding;
    EnemyAnimationEvents enemyAnimationEvents;
    Animator enemyAnimator;
    AudioSource audioSource;

    public float MovementSpeed;
    [Space]

    [SerializeField] private float AttackDistance;
    [Space]

    public static ObjectPool<GameObject> ZombieSpitProjectilePool;
    [Space]

    [Header("************* Audio Variables ***********")]
    [SerializeField] private float AttackVolume;
    [SerializeField] private  List<AudioClip> AttackUseAudio;
    [SerializeField] private List<AudioClip> AttackHitAudio;
    [Space]

    #region *********** Action Variables ***********

    private bool isMoving;
    private bool isAttacking;
    private float animationParameterMovementFloat;

    #endregion

    void Start()
    {
        enemyController = GetComponent<EnemyController>();
        enemyPathfinding = GetComponent<EnemyPathfinding>();
        enemyAnimationEvents = GetComponentInChildren<EnemyAnimationEvents>();
        enemyAnimator = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (ZombieSpitProjectilePool == null)
            ZombieSpitProjectilePool = new ObjectPool<GameObject>(() => CreateZombieSpitProjectile(ZombieSpitProjectilePool));

        enemyAnimationEvents.Damage += Damage;
    }

    void OnEnable()
    {
        isMoving = false;
        isAttacking = false;
    }

    void Update()
    {
        HandleAllLogic();
    }

    void FixedUpdate()
    {
        HandleAllMovement();
    }

    void HandleAllLogic()
    {
        if (!enemyController.isHurt)
        {
            if (isMoving)
            {
                if (!GameManager.Instance.isGameOver)
                    enemyPathfinding.FindPath();
            }
        }
        HandleActionsLogic();
        HandleAnimationState();
    }
    void HandleAllMovement()
    {
        if (!enemyController.isHurt)
        {
            if (isMoving)
            {
                HandleMovement();
                Rotation();
            }
            else if (isAttacking)
            {
                Rotation();
            }
        }
    }

    void HandleMovement()
    {
        transform.position += enemyPathfinding.moveDirection * MovementSpeed * Time.deltaTime;
        
        //Vector3 movementVelocity = moveDirection * MovementSpeed;
        //rigidBody.velocity = movementVelocity;
    }

    private void Rotation()
    {
        if (enemyPathfinding.turnDirection == Vector3.zero)
            return;

        //transform.forward = enemyPathfinding.turnDirection;

        Vector3 turnDirection = enemyPathfinding.turnDirection;

        // Rotate the forward vector towards the target direction by one step
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, turnDirection, 5 * Time.deltaTime, 0.0f);

        // Calculate a rotation a step closer to the target and applies rotation to this object
        transform.rotation = Quaternion.LookRotation(newDirection);
    }


    // ******************* Animation **********************

    void HandleAnimationState()
    {
        enemyAnimator.SetBool("Move", isMoving);
    }





    // ********************* Actions ********************
    void HandleActionsLogic()
    {
        // if player is dead, stop all actions

        // if not attacking
        if (!isAttacking)
        {
            // if far enought from player to attack
            if (enemyPathfinding.distanceToPlayer > AttackDistance)
            {
                if (!isMoving)
                {
                    isMoving = true;
                    HandleMovementAction();
                }
            }
            // if close enough to attack
            else
            {
                isMoving = false;
                HandleAttackAction();
            }
        }
    }

    #region ******************* HANDLE ATTACK *****************
    private void HandleAttackAction()
    {
        StopAllCoroutines();
        StartCoroutine(StartAttack());
        StartCoroutine(RotateTowardsPlayerBeforeAttack(0.25f));
    }

    private IEnumerator StartAttack()
    {
        isAttacking = true;
        Attack();

        // Wait for animation to finish
        yield return new WaitUntil(() => enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Zombie_Attack_1"));
        yield return new WaitUntil(() => !enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Zombie_Attack_1"));

        StartCoroutine(RotateTowardsPlayerBeforeAttack(0.5f));
        yield return new WaitForSeconds(.5f);
        isAttacking = false;
    }
    private IEnumerator RotateTowardsPlayerBeforeAttack(float rotateForSeconds)
    {
        if (enemyPathfinding.distanceToPlayer < AttackDistance)
        {
            float countDown = rotateForSeconds;
            while (countDown > 0 && transform.forward != enemyPathfinding.turnDirection)
            {
                enemyPathfinding.DirectPath();
                countDown -= Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }
        }
    }

    private void Attack()
    {
        enemyAnimator.SetTrigger("Attack");
    }
    private void Damage(AttackType AttackType)
    {
        if (AttackType is AttackType.Melee)
            MeleeAttack();
        else
            ProjectileAttack();
    }
    private void MeleeAttack()
    {
        audioSource.PlayOneShot(AttackUseAudio[Random.Range(0, AttackUseAudio.Count)], AttackVolume);
        // Miss
        if (Mathf.Abs(enemyPathfinding.distanceToPlayer) > AttackDistance)
        {
            return;
        }

        audioSource.PlayOneShot(AttackHitAudio[Random.Range(0, AttackHitAudio.Count)], AttackVolume);

        Vector3 forceDir = GameManager.Instance.Player.transform.position - transform.position;
        forceDir.y = 0;
        forceDir.Normalize();
        GameManager.Instance.Player.GetComponent<EnemyHealth>().TakeDamage(25, forceDir, 2.5f);
    }

    // ********************** Projectile **************************
    private void ProjectileAttack()
    {
        audioSource.PlayOneShot(AttackUseAudio[Random.Range(0, AttackUseAudio.Count)], AttackVolume);

        // Spawn Projectile
        GameObject instance = ZombieSpitProjectilePool.Get();
        instance.transform.SetParent(transform);
        instance.transform.localPosition = GameManager.Instance.ZombieSpitProjectilePrefab.transform.position;
        instance.transform.localRotation = GameManager.Instance.ZombieSpitProjectilePrefab.transform.rotation;
        instance.transform.SetParent(null);
        Projectile projectile = instance.GetComponentInChildren<Projectile>();
        projectile.OnCollision += HandleProjectileHit;
        projectile.StartPoint = instance.transform.position;
        //Vector3 endPoint = GameManager.Instance.Player.transform.position - transform.position;
        //endPoint.y = 0;
        //endPoint.Normalize();
        Vector3 endPoint = transform.forward;
        projectile.EndPoint = endPoint * 100;
        instance.transform.GetChild(0).gameObject.SetActive(true);
        //instance forward
        instance.SetActive(true);
    }
    private GameObject CreateZombieSpitProjectile(ObjectPool<GameObject> Pool)
    {
        GameObject instance = Instantiate(GameManager.Instance.ZombieSpitProjectilePrefab, transform);

        Projectile projectile = instance.GetComponentInChildren<Projectile>();
        projectile.pool = Pool;

        return instance;
    }
    public static void ClearZombieSpitProjectilePool()
    {
        ZombieSpitProjectilePool.Clear();
    }
    private void HandleProjectileHit(Projectile projectile, Collider other)
    {
        Vector3 forceDir = GameManager.Instance.Player.transform.position - projectile.transform.position;
        forceDir.y = 0;
        forceDir.Normalize();
        GameManager.Instance.Player.GetComponent<EnemyHealth>().TakeDamage(25, forceDir, 2.5f);
    }

    #endregion

    #region ************ HANDLE MOVEMENT ****************

    private void HandleMovementAction()
    {
        animationParameterMovementFloat = Random.Range(0, 3);
        enemyAnimator.SetFloat("Movement", animationParameterMovementFloat);
        StartCoroutine(StartMovement());
    }
    private IEnumerator StartMovement()
    {
        while (true)
        {
            float waitTillNextAnimation = enemyAnimator.GetCurrentAnimatorStateInfo(0).length * Random.Range(1, 6);
        
            yield return new WaitForSeconds(waitTillNextAnimation);

            float nextMovementParameter = Random.Range(0, 3);
            float dif = nextMovementParameter - animationParameterMovementFloat;

            float blendingTime = 1f;
            float time = 0;
            while (time < blendingTime)
            {
                time += Time.deltaTime;

                float animationParamater = animationParameterMovementFloat + (dif * time / blendingTime);
                enemyAnimator.SetFloat("Movement", animationParamater);
                
                yield return null;
            }
            animationParameterMovementFloat = nextMovementParameter;
            enemyAnimator.SetFloat("Movement", animationParameterMovementFloat);
        }
    }

    #endregion
}
