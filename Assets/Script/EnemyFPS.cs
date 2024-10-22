using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFPS : MonoBehaviour
{
    public float moveSpeed = 3.0f; // 移速
    public float detectionDistance = 5.0f; // 检测距离
    public float patrolRange = 10.0f;
    public float detectionAngle = 45.0f; // 检测角度
    public Transform player; // 玩家
    public Color normalColor = Color.white; // 默认颜色
    public Color alertColor = Color.red; // 警戒颜色
    public Renderer enemyRenderer; // 敌人材质
    public int EnemyHealth = 6;

    // 新增射击相关参数
    public Transform FirePoint; // 发射点
    public GameObject FirePre;
    public Transform BulletPoint;
    public GameObject bulletPre; // 子弹预制体
    public AudioClip shootClip; // 射击声音
    public float cd = 0.5f; // 射击冷却时间
    private float shootTimer = 0f; // 计时器
    private AudioSource audioSource; // 音频源

    private Vector3 startPosition;
    private bool isPlayerDetected = false;
    private float movedDistance = 0.0f;

    void Start()
    {
        startPosition = transform.position;
        enemyRenderer.material.color = normalColor;

        audioSource = GetComponent<AudioSource>(); // 获取音频组件
    }

    void Update()
    {
        Patrol();
        DetectPlayer();

        // 更新射击冷却时间，确保 shootTimer 每帧都在累加
        shootTimer += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Bullet")
        {
            EnemyHealth--;
            if (EnemyHealth <= 0)
            {
                Destroy(gameObject);
            }
        }
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
                transform.LookAt(player);

                // 添加冷却时间判定
                if (shootTimer >= cd)
                {
                    Shoot();
                    shootTimer = 0f; // 重置射击计时器
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

    void Shoot()
    {
        if (bulletPre != null && FirePre != null)
        {
            // 实例化子弹和火焰
            Instantiate(FirePre, FirePoint.position, FirePoint.rotation);
            Instantiate(bulletPre, BulletPoint.position, BulletPoint.rotation);
            Debug.Log("Bullet and fire instantiated.");

            // 播放射击音效
            if (audioSource != null && shootClip != null)
            {
                audioSource.PlayOneShot(shootClip);
                Debug.Log("Shooting sound played.");
            }
            else
            {
                Debug.LogWarning("AudioSource or shootClip is missing.");
            }
        }
        else
        {
            Debug.LogWarning("Bullet or Fire prefabs are missing.");
        }
    }

    void ChangeColor(Color newColor)
    {
        enemyRenderer.material.color = newColor;
    }
}
