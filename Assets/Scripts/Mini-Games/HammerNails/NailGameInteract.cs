using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class NailGameInteract : MonoBehaviour
{
    [SerializeField] InputActionAsset inputs;
    InputAction interact;

    [SerializeField] NailMinigame minigame;
    [SerializeField] TextMeshProUGUI promptText;
    [SerializeField] GameObject nailGameWidget;
    [SerializeField] Inventory playerInventory;

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
        promptText.text = "";

        interact = InputSystem.actions.FindAction("Interact");
    }

    void Update()
    {
        if (playerIsNear)
        {
            // The minigame will only start when the interact key is pressed if the player is next to the window,
            // it's damaged, and they're holding a hammer
            if (interact.WasPressedThisFrame() && GetComponentInParent<WindowBarricade>().GetIsDamaged() && IsHoldingHammer())
            {
                promptText.text = "";
                nailGameWidget.SetActive(true);
                minigame.BeginMinigame(gameObject);
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
                Time.timeScale = 0;
            }
            else if (interact.WasPressedThisFrame() && !IsHoldingHammer())
            {
                promptText.text = "Missing a hammer.";
            }
            else if (interact.WasPressedThisFrame() && !GetComponentInParent<WindowBarricade>().GetIsDamaged())
            {
                promptText.text = "This window doesn't need to be repaired.";
            }
        }
    }
    bool IsHoldingHammer()
    {
        if (playerInventory.GetCurrentItem() != null)
        {
            return playerInventory.GetCurrentItem().itemName == InventoryItem.ItemName.Hammer;
        }
        return false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerIsNear = true;
            promptText.text = "Press 'E' to board up the window";
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerIsNear = false;
            promptText.text = "";
        }
    }
}
