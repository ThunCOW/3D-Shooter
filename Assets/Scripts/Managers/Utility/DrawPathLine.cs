using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawPathLine : MonoBehaviour
{
    public bool GetLineRenderer;

    public bool DrawLine;

    LineRenderer lineRenderer;

    //EnemyManager enemyManager;
    EnemyPathfinding enemyPathfinding;
    // Start is called before the first frame update
    void Start()
    {
        enemyPathfinding = GetComponent<EnemyPathfinding>();
    }

    private void Update()
    {
        if (GetLineRenderer)
        {
            GetLineRenderer = false;

            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.startWidth = 0.15f;
            lineRenderer.endWidth = 0.15f;
            lineRenderer.positionCount = 0;
            lineRenderer.sortingOrder = 1;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.material.color = Color.red;
        }

        DrawPath();
    }

    public void DrawPath()
    {
        if (!DrawLine)
            return;

        lineRenderer.positionCount = enemyPathfinding.pathToPlayer.corners.Length;
        
        if (lineRenderer.positionCount < 2)
            return;
        
        lineRenderer.SetPosition(0, transform.position);

        for (int i = 1; i < enemyPathfinding.pathToPlayer.corners.Length; i++)
        {
            Vector3 pos = new Vector3(enemyPathfinding.pathToPlayer.corners[i].x, enemyPathfinding.pathToPlayer.corners[i].y, enemyPathfinding.pathToPlayer.corners[i].z);
            lineRenderer.SetPosition(i, pos);
        }
    }
}
