using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UI;

public class Player2 : MonoBehaviour
{

    public Text medNum;
    public int medCount = 1;
    public int maxHealth = 100;
    public int currentHealth;
    public HealthBar healthBar;
    public float damagePerSec = 5;
    public float moveSpeed = 6.0f;  // 移动速度
    public float jumpHeight = 2.0f; // 跳跃高度
    public float mouseSensitivity = 100f;  // 鼠标灵敏度
    public Transform playerCamera;  // 摄像机

    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation = 0f;
    private float gravity = -9.8f;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        currentHealth = maxHealth;
        healthBar.setMaxHealth(maxHealth);
    }

    void Update()
    {
        // 处理鼠标移动
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

        if (Input.GetKey(KeyCode.W)) moveZ = 1f;
        if (Input.GetKey(KeyCode.S)) moveZ = -1f;
        if (Input.GetKey(KeyCode.A)) moveX = -1f;
        if (Input.GetKey(KeyCode.D)) moveX = 1f;

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

        ContinueTakeDamage(damagePerSec * Time.deltaTime);

    }

    private void ContinueTakeDamage(float damage){
        currentHealth = Mathf.Max(currentHealth - (int)damage, 0);
        healthBar.setHealth(currentHealth);

        if (currentHealth <= 0)
        {
            Debug.Log("Player has died.");
        }
    }
    private void OnTriggerEnter(Collider other) {
        if(other.tag == "MedKit"){
            Destroy(other.gameObject);
            medCount++;
            medNum.text = "X " + medCount.ToString();
        }
    }
}
