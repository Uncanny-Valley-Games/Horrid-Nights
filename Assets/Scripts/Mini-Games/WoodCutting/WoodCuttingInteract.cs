using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Mini_Games.WoodCutting
{
    public class WoodCuttingInteract : MonoBehaviour
    {
        public Transform player;
        public float interactRange = 3f;
        public string interactPrompt = "Press E to chop";
        public GameObject promptUI;
        public TMP_Text promptText;
        [SerializeField] Inventory playerInventory;

        private bool _minigameActive;

        private void Start()
        {
            promptText.text = interactPrompt;
            promptUI.SetActive(false);
        }

        void Update()
        {
            if (!player) return;

            var dist = Vector3.Distance(transform.position, player.position);
            if (promptUI && !_minigameActive && IsHoldingAxe()) promptUI.SetActive(dist <= interactRange);

            if (dist <= interactRange && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame && IsHoldingAxe())
            {
                var mg = WoodCuttingMinigame.Instance;
                promptUI.SetActive(false);
                if (mg)
                {
                    _minigameActive = true;
                    mg.StartMinigame(this);
                }
            }
            if (dist <= interactRange && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame && !IsHoldingAxe())
            {
                Debug.Log("Missing an axe.");
            }
        }

        bool IsHoldingAxe()
        {
            if (playerInventory.GetCurrentItem() != null)
            {
                return playerInventory.GetCurrentItem().itemName == InventoryItem.ItemName.Axe;
            }
            return false;
        }

        public void BreakTree()
        {
            Destroy(gameObject);
        }
    }
}