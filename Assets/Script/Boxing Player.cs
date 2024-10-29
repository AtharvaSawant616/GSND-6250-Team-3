using System.Collections;
using UnityEngine;

public class BoxingPlayer : MonoBehaviour
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
        transform.Rotate(Vector3.up * mouseX);

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
    }
}
