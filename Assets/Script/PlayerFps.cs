using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFps : MonoBehaviour
{
    public float moveSpeed = 6.0f;  // 移动速度
    public float jumpHeight = 2.0f; // 跳跃高度
    public float mouseSensitivity = 100f;  // 鼠标灵敏度
    public Transform playerCamera;  // 摄像机

    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation = 0f;
    private float gravity = -9.8f;
    private bool isGrounded;
    private AudioSource footStep;
    private bool isMoving; // 是否在移动
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        footStep = GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // 限制垂直视角范围

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
        transform.localRotation = Quaternion.Euler(xRotation, transform.localRotation.eulerAngles.y, 0f);

        // 检测是否在地面
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // 保持玩家紧贴地面
        }

        // 获取WASD输入
        float moveX = 0f;
        float moveZ = 0f;
        isMoving = false;

        if (Input.GetKey(KeyCode.W))
        {
            moveZ = 1f;
            isMoving = true;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveZ = -1f;
            isMoving = true;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveX = -1f;
            isMoving = true;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveX = 1f;
            isMoving = true;
        }

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        controller.Move(move * moveSpeed * Time.deltaTime);

        // 跳跃
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // 应用重力
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // 播放脚步声
        if (isMoving && isGrounded && !footStep.isPlaying)
        {
            footStep.Play();
        }
        else if (!isMoving || !isGrounded)
        {
            footStep.Stop();
        }
    }
}
