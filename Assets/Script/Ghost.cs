using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    public float moveSpeed = 3.0f; // 移速
    public float detectionDistance = 5.0f; // 检测距离
    public float patrolRange = 10.0f;
    public float detectionAngle = 45.0f; // 检测角度
    public Transform player; // 玩家
    public Color normalColor = Color.white; // 默认颜色
    public Color alertColor = Color.red; // 警戒颜色
    public Renderer enemyRenderer; // 敌人材质
    public MeshFilter meshFilter; // 用于显示视线的 MeshFilter
    public int segments = 50; // 扇形的分段数量

    private Vector3 startPosition;
    private bool isPlayerDetected = false;
    private float movedDistance = 0.0f;

    void Start()
    {
        startPosition = transform.position;
        enemyRenderer.material.color = normalColor;


        CreateViewMesh();
    }

    void Update()
    {
        Patrol();
        DetectPlayer();
        CreateViewMesh();
    }



    void Patrol()
    {
        float sphereRadius = 0.1f;
        float rayDistance = 0.8f;

        RaycastHit hit;


        Debug.DrawRay(transform.position, transform.forward * rayDistance, Color.green);


        if (Physics.SphereCast(transform.position, sphereRadius, transform.forward, out hit, rayDistance))
        {
            if (hit.collider != null && hit.collider.CompareTag("Obstacle"))
            {
                TurnAround();
            }
        }


        Vector3 moveDirection = transform.forward * moveSpeed * Time.deltaTime;
        transform.Translate(moveDirection, Space.World);


        movedDistance += moveDirection.magnitude;


        if (movedDistance >= patrolRange)
        {
            TurnAround();
        }
    }

    void TurnAround()
    {
        transform.Rotate(0, 180, 0);
        movedDistance = 0.0f;
    }

    void DetectPlayer()
    {

        float distanceToPlayer = Vector3.Distance(player.position, transform.position);


        if (distanceToPlayer <= detectionDistance)
        {

            Vector3 directionToPlayer = (player.position - transform.position).normalized;


            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);


            if (angleToPlayer <= detectionAngle / 2)
            {

                isPlayerDetected = true;
                ChangeColor(alertColor);
                if (isPlayerDetected == true)
                {
                    Vector3 moveDirection = transform.forward * 0.2f * Time.deltaTime;
                    transform.LookAt(player);
                    transform.Translate(moveDirection, player);
                }
            }
            else
            {

                isPlayerDetected = false;
                ChangeColor(normalColor);
            }
        }
        else
        {

            isPlayerDetected = false;
            ChangeColor(normalColor);
        }
    }


    void ChangeColor(Color newColor)
    {
        enemyRenderer.material.color = newColor;
    }


    void CreateViewMesh()
    {
        if (meshFilter == null)
        {
            Debug.LogError("MeshFilter is not assigned!");
            return;
        }

        Debug.Log("Creating Mesh...");
        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;

        Vector3[] vertices = new Vector3[segments + 2];
        int[] triangles = new int[segments * 3];

        vertices[0] = Vector3.zero;

        float halfAngle = detectionAngle / 2;
        for (int i = 0; i <= segments; i++)
        {

            float angle = Mathf.Lerp(-halfAngle, halfAngle, (float)i / segments);
            Vector3 direction = Quaternion.Euler(0, angle, 0) * Vector3.forward;
            vertices[i + 1] = direction * detectionDistance;

            if (i < segments)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;


        mesh.RecalculateNormals();
    }


    void OnDrawGizmos()
    {
        float halfAngle = detectionAngle / 2;
        Vector3 startPosition = transform.position + Vector3.up * 0.5f;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(startPosition, Quaternion.Euler(0, -halfAngle, 0) * transform.forward * detectionDistance);
        Gizmos.DrawRay(startPosition, Quaternion.Euler(0, halfAngle, 0) * transform.forward * detectionDistance);

        for (int i = 0; i <= segments; i++)
        {
            float angle = Mathf.Lerp(-halfAngle, halfAngle, (float)i / segments);
            Vector3 point = startPosition + Quaternion.Euler(0, angle, 0) * transform.forward * detectionDistance;
            Gizmos.DrawLine(startPosition, point);
        }
    }
}
