using UnityEngine;

public class Enemy : MonoBehaviour
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
        // 设置敌人起始位置
        startPosition = transform.position;

        // 初始化敌人的默认颜色
        enemyRenderer.material.color = normalColor;

        // 创建填充的视线扇形Mesh
        CreateViewMesh();
    }

    void Update()
    {
        Patrol();
        DetectPlayer();
        // 每帧更新视线区域
        CreateViewMesh();
    }



void Patrol()
{
    float sphereRadius = 0.1f; // 定义一个半径

    RaycastHit hit;
    if (Physics.SphereCast(transform.position, sphereRadius, transform.forward, out hit, detectionDistance))
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
    // 计算玩家与敌人的距离
    float distanceToPlayer = Vector3.Distance(player.position, transform.position);

    // 如果玩家在检测距离范围内
    if (distanceToPlayer <= detectionDistance)
    {
        // 计算玩家相对于敌人的方向
        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        // 计算玩家与敌人正前方的夹角
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        // 如果玩家在视线角度范围内
        if (angleToPlayer <= detectionAngle / 2)
        {
            // 玩家进入扫描区域，敌人变红色
            isPlayerDetected = true;
            ChangeColor(alertColor);
        }
        else
        {
            // 玩家不在视线角度范围内，恢复正常颜色
            isPlayerDetected = false;
            ChangeColor(normalColor);
        }
    }
    else
    {
        // 玩家不在检测距离范围内，恢复正常颜色
        isPlayerDetected = false;
        ChangeColor(normalColor);
    }
}


    void ChangeColor(Color newColor)
    {
        enemyRenderer.material.color = newColor;
    }

    // 创建用于显示视线区域的Mesh
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

        Vector3[] vertices = new Vector3[segments + 2]; // 中心点 + 每个边缘点
        int[] triangles = new int[segments * 3]; // 每个三角形3个顶点

        vertices[0] = Vector3.zero; // 中心点

        float halfAngle = detectionAngle / 2;
        for (int i = 0; i <= segments; i++)
        {
            // 计算每个边缘点的角度
            float angle = Mathf.Lerp(-halfAngle, halfAngle, (float)i / segments);
            Vector3 direction = Quaternion.Euler(0, angle, 0) * Vector3.forward;
            vertices[i + 1] = direction * detectionDistance;

            if (i < segments)
            {
                triangles[i * 3] = 0; // 中心点
                triangles[i * 3 + 1] = i + 1; // 当前点
                triangles[i * 3 + 2] = i + 2; // 下一个点
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        // 重新计算法线以正确渲染光照
        mesh.RecalculateNormals();
    }

    // 使用Gizmos可视化扇形区域
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
