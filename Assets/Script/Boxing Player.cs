using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BoxingPlayer : MonoBehaviour
{
    public float speed = 6.0f;
    public float gravity = -9.8f;
    public float mouseSensitivity = 100f;
    public Transform playerCamera;
    public Image depressionReport;
    public Text PressEtoOpen;

    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation = 0f;

    private bool isNearReport = false; // 标记玩家是否靠近 Depression report

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Mouse input for rotation
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Vertical rotation (up and down)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Horizontal rotation (left and right)
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
        transform.localRotation = Quaternion.Euler(xRotation, transform.localRotation.eulerAngles.y, 0f);

        // Movement input
        float x = 0f;
        float z = 0f;

        if (Input.GetKey(KeyCode.W)) z = 1f;    // Forward
        if (Input.GetKey(KeyCode.S)) z = -1f;   // Backward
        if (Input.GetKey(KeyCode.A)) x = -1f;   // Left
        if (Input.GetKey(KeyCode.D)) x = 1f;    // Right

        // Move based on local forward/right direction
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // 检测玩家是否在 Depression report 附近并按下 E 键
        if (isNearReport && Input.GetKeyDown(KeyCode.E))
        {
            PressEtoOpen.gameObject.SetActive(false); // 隐藏提示
            depressionReport.gameObject.SetActive(true); // 显示报告
        }
        if(Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape)){
            depressionReport.gameObject.SetActive(false);
            Debug.Log("Pressing the left mouse button");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Depression report"))
        {
            isNearReport = true; // 玩家靠近报告
            PressEtoOpen.gameObject.SetActive(true); // 显示提示
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Depression report"))
        {
            isNearReport = false; // 玩家离开报告区域
            PressEtoOpen.gameObject.SetActive(false); // 隐藏提示
        }
    }

}
