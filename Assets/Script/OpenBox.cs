using UnityEngine;
using UnityEngine.UI;

public class BoxInteraction : MonoBehaviour
{
    public GameObject lid;           // Reference to the box lid (polySurface2)
    public GameObject medKit;        // Reference to the medkit inside the box
    public Text interactText;        // UI Text to display "Press E to open"
    private bool playerNearby = false;
    private bool isOpened = false;

    void Start()
    {
        interactText.gameObject.SetActive(false);  // Hide the interact text initially
        medKit.SetActive(false);                   // Disable the medkit trigger initially
    }

    void Update()
    {
        if (playerNearby && !isOpened && Input.GetKeyDown(KeyCode.E))
        {
            OpenBox();
        }
    }

    private void OpenBox()
    {
        isOpened = true;
        lid.SetActive(false);                // Hide the lid (polySurface2)
        medKit.SetActive(true);              // Enable the medkit to be accessible
        interactText.gameObject.SetActive(false);  // Hide the interact text
    }

    private void OnTriggerEnter(Collider other)
    {
        if(isOpened) return;
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            interactText.gameObject.SetActive(true);  // Show "Press E to open" message
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            interactText.gameObject.SetActive(false); // Hide the message when player leaves
        }
    }
}
