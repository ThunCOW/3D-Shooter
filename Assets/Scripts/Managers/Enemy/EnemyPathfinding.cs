using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class EnemyPathfinding : MonoBehaviour
{
    public bool NavMeshPathfinding;
    [Space]

    public float PathCalculationTimeMax;

    #region Private/Hidden Variables
    [HideInInspector] public NavMeshAgent navMeshAgent;

    private bool isCloseToPlayer;
    
    private Vector3 distanceToTargetVector3;

    private float pathCalculationCountdown;
    
    [HideInInspector] public float distanceToPlayer;
    [HideInInspector] public Vector3 turnDirection;
    [HideInInspector] public Vector3 moveDirection;
    [HideInInspector] public NavMeshPath pathToPlayer;
    #endregion
    
    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        pathToPlayer = new NavMeshPath();
    }

    private void Update()
    {
        distanceToPlayer = Vector3.Distance(GameManager.Instance.Player.transform.position, transform.position);

        if (distanceToPlayer < 10)
            isCloseToPlayer = true;
        else
            isCloseToPlayer = false;

        pathCalculationCountdown -= Time.deltaTime;
    }
    
    public void FindPath()
    {
        if (isCloseToPlayer)
        {
            DirectPath();
        }
        else
        {
            if (NavMeshPathfinding)
            {
                HandlePathNavMesh();
            }
            else
            {
                HandlePath();
            }
        }
    }

    public void HandlePath()
    {
        if (pathCalculationCountdown <= 0)
        {
            distanceToTargetVector3 = GameManager.Instance.Player.transform.position - transform.position;
                
            moveDirection = distanceToTargetVector3;
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

            pathCalculationCountdown = Random.Range(0, PathCalculationTimeMax);
        }
    }

    public void HandlePathNavMesh()
    {
        if (pathCalculationCountdown <= 0)
        {
            pathToPlayer.ClearCorners();
            Vector3 playerPos = GameManager.Instance.Player.transform.position;
            playerPos.y = 0;
            navMeshAgent.CalculatePath(playerPos, pathToPlayer);

            distanceToTargetVector3 = pathToPlayer.corners[1] - transform.position;
            float sum = Mathf.Abs(distanceToTargetVector3.x) + Mathf.Abs(distanceToTargetVector3.z);
            turnDirection = new Vector3(distanceToTargetVector3.x / sum, 0, distanceToTargetVector3.z / sum);

            distanceToTargetVector3.Normalize();
            moveDirection = distanceToTargetVector3;

            pathCalculationCountdown = Random.Range(0, PathCalculationTimeMax);
        }
    }

    public void DirectPath()
    {
        distanceToTargetVector3 = GameManager.Instance.Player.transform.position - transform.position;
        moveDirection = distanceToTargetVector3;
        float sum = Mathf.Abs(moveDirection.x) + Mathf.Abs(moveDirection.z);
        turnDirection = new Vector3(moveDirection.x / sum, 0, moveDirection.z / sum);

        distanceToTargetVector3.Normalize();
        moveDirection = distanceToTargetVector3;
    }

    private void OnEnable()
    {
        if (NavMeshPathfinding)
            navMeshAgent.enabled = true;
    }
}
