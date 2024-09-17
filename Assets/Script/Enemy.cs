using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float moveSpeed = 3.0f; // 移速
    public float detectionDistance = 2.0f; // 检测距离
    public float patrolRange = 10.0f;
    public float detectionAngle = 45.0f;

    public Transform player;
    public Color normalColor = Color.white;
    public Color alertColor = Color.red;
    public Renderer enemyRenderer;

    private Vector3 startPosition;
    private bool isPlayerDetected = false;
    private float movedDistance = 0.0f;
    private bool movingForward = true;

    void Start()
    {

        startPosition = transform.position;
        enemyRenderer.material.color = normalColor;
    }

    void Update()
    {
        Patrol();
        DetectPlayer();
    }

    void Patrol()
    {

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, detectionDistance))
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
}
