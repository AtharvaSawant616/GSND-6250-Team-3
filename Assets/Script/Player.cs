using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 6.0f;
    public float gravity = -9.8f;
    public float mouseSensitivity = 100f;
    public Transform playerCamera;

    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked; // 锁定鼠标在屏幕中央
    }

    void Update()
    {
        // 处理鼠标视角旋转
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // 限制玩家上下看角度

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        float x = 0f;
        float z = 0f;

        if (Input.GetKey(KeyCode.W)) // 向前
        {
            z = 1f;
        }
        if (Input.GetKey(KeyCode.S)) // 向后
        {
            z = -1f;
        }
        if (Input.GetKey(KeyCode.A)) // 向左
        {
            x = -1f;
        }
        if (Input.GetKey(KeyCode.D)) // 向右
        {
            x = 1f;
        }

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        // 应用重力
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
