using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float speed = 6.0f;
    public float gravity = -9.8f;
    public float mouseSensitivity = 100f;
    public Transform playerCamera;
    public bool getKey = false;
    public GameObject door;

    public AudioClip[] footstepSounds;
    private AudioSource audioSource;
    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation = 0f;
    public bool isMoving = false;
    private float stepInterval = 0.5f;
    private float stepTimer = 0f;
    private Animator cameraAnimator;
    public RedLight[] redLights;

    public AudioSource alarmAudioSource;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;

        // Get the AudioSource component
        audioSource = GetComponent<AudioSource>();

        // Ensure there is only one AudioListener in the scene
        AudioListener[] listeners = FindObjectsOfType<AudioListener>();
        if (listeners.Length > 1)
        {
            for (int i = 1; i < listeners.Length; i++)
            {
                Destroy(listeners[i]);
            }
        }
    }

    void Update()
    {
    // 获取鼠标的输入量
    float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
    float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

    // 调整垂直旋转角度（X 轴旋转）
    xRotation -= mouseY;

    // 限制垂直旋转角度，避免旋转过度
    xRotation = Mathf.Clamp(xRotation, -90f, 90f);

    // 将旋转应用到摄像机的局部旋转（垂直方向的上下移动）
    playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

    // 应用水平旋转到玩家的整体（绕 Y 轴旋转，左右移动）
    transform.Rotate(Vector3.up * mouseX);

    // 下面是让摄像头随 Y 轴旋转（附加的 X 轴旋转）
    transform.localRotation = Quaternion.Euler(xRotation, transform.localRotation.eulerAngles.y, 0f);

        float x = 0f;
        float z = 0f;

        if (Input.GetKey(KeyCode.W)) // Move forward
        {
            z = 1f;
        }
        if (Input.GetKey(KeyCode.S)) // Move backward
        {
            z = -1f;
        }
        if (Input.GetKey(KeyCode.A)) // Move left
        {
            x = -1f;
        }
        if (Input.GetKey(KeyCode.D)) // Move right
        {
            x = 1f;
        }

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Check if the player is moving
        isMoving = move.magnitude > 0.1f && controller.isGrounded;

        // Handle footstep sounds
        if (isMoving)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                PlayFootstepSound();
                stepTimer = stepInterval; // Reset step timer
            }
        }
    }

    // Play a random footstep sound
    void PlayFootstepSound()
    {
        if (footstepSounds.Length > 0 && audioSource != null)
        {
            int randomIndex = Random.Range(0, footstepSounds.Length);
            audioSource.PlayOneShot(footstepSounds[randomIndex]);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Key")
        {
            Destroy(other.gameObject);
            getKey = true;

            // Trigger crazy flicker mode for all red lights
            TriggerRedLightFlicker();

            // Play the alarm BGM
            if (alarmAudioSource != null)
            {
                alarmAudioSource.Play();
            }
        }
        if (other.gameObject.tag == "Door")
        {
            openDoor();
        }
    }

    private void openDoor()
    {
        if (getKey == true)
        {
            SceneManager.LoadScene("Graveyard");
        }
    }

    // Method to trigger flickering of red lights
    private void TriggerRedLightFlicker()
    {
        foreach (RedLight redLight in redLights)
        {
            redLight.StartFlickering(); // Start flickering on each red light
        }
    }
}