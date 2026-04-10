using Downscaled;
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
        [SerializeField] private GameObject treeItself;

        private bool _minigameActive;
        private bool pastPromptUIState;

        private void Start()
        {
            promptText.text = interactPrompt;
            promptUI.SetActive(false);
        }

        void Update()
        {
            if (!player) return;

            var dist = Vector3.Distance(transform.position, player.position);
            if (promptUI && !_minigameActive && IsHoldingAxe()) TogglePromptUI(dist <= interactRange);

            if (dist <= interactRange && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame && IsHoldingAxe())
            {
                var mg = WoodCuttingMinigame.Instance;
                TogglePromptUI(false);
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
            Destroy(treeItself);
            DGameManager.TreeCuttingMinigameDone = true;

            if (DGameManager.FinishedInitialMiniGames())
            {
                UpdatableText.UpdateStatusText("You may head home now!");
            }
            else
            {
                UpdatableText.UpdateStatusText("Enough wood for tonight. Lets go fishing!");
            }
            
            Destroy(gameObject);
        }

        private void TogglePromptUI(bool value)
        {
            if (pastPromptUIState != value)
            {
                pastPromptUIState = value;
                promptUI.SetActive(value);
            }
        }
    }
}