using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public float jumpHeight = 2.0f;

    // Added variables for ground check buffer
    public float groundCheckDistance = 0.2f; // Distance to check for ground
    public LayerMask groundMask; // Layer to check ground collisions
    private bool isGroundedBuffer = false; // Ground buffer
    private float groundBufferTime = 0.2f; // Buffer time for jumping after touching ground
    private float lastGroundedTime = 0f;

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
        // Mouse movement for looking around
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        float x = 0f;
        float z = 0f;

        if (Input.GetKey(KeyCode.W)) z = 1f;
        if (Input.GetKey(KeyCode.S)) z = -1f;
        if (Input.GetKey(KeyCode.A)) x = -1f;
        if (Input.GetKey(KeyCode.D)) x = 1f;

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        // Ground check using raycast
        isGroundedBuffer = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundMask);

        if (isGroundedBuffer)
        {
            lastGroundedTime = Time.time; // Update last grounded time
            velocity.y = -2f;

            // Jump input with buffer time
            if (Input.GetKeyDown(KeyCode.Space) && Time.time - lastGroundedTime <= groundBufferTime)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Check if the player is moving
        isMoving = move.magnitude > 0.1f && isGroundedBuffer;

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
