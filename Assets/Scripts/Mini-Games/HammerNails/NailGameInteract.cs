using UnityEngine;
using UnityEngine.InputSystem;

public class NailGameInteract : MonoBehaviour
{
    [SerializeField] InputActionAsset inputs;
    InputAction interact;

    [SerializeField] GameObject promptText;
    [SerializeField] GameObject nailGameWidget;

    bool playerIsNear;

    void OnEnable()
    {
        inputs.FindActionMap("Player").Enable();
    }

    void OnDisable()
    {
        inputs.FindActionMap("Player").Disable();
    }

    void Start()
    {
        promptText.SetActive(false);

        interact = InputSystem.actions.FindAction("Interact");
    }

    void Update()
    {
        if (playerIsNear)
        {
            // The minigame will only start when the interact key is pressed if the player is next to the window
            if (interact.WasPressedThisFrame())
            {
                promptText.SetActive(false);
                nailGameWidget.SetActive(true);
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerIsNear = true;
            promptText.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerIsNear = false;
            promptText.SetActive(false);
        }
    }
}
