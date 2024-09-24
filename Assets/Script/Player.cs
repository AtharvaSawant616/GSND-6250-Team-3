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

    public AudioClip[] footstepSounds; // Array to hold footstep sounds
    private AudioSource audioSource;
    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation = 0f;
    public bool isMoving = false; // To track if the player is moving
    private float stepInterval = 0.5f; // Time interval between steps
    private float stepTimer = 0f; // Timer for footsteps
    private Animator cameraAnimator; // Reference to the camera animator

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;

        // Get the AudioSource component
        audioSource = GetComponent<AudioSource>();

        // Get the Animator component from the camera
        cameraAnimator = playerCamera.GetComponent<Animator>();

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
        // Handle mouse rotation
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Limit player's up and down view angles

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

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

            // Trigger camera shake animation when player is moving
            if (cameraAnimator != null)
            {
                cameraAnimator.SetBool("isMoving", true); // Assuming you have a boolean "isMoving" parameter in the animation
            }
        }
        else
        {
            if (cameraAnimator != null)
            {
                cameraAnimator.SetBool("isMoving", false); // Stop camera shake when player stops moving
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
}
