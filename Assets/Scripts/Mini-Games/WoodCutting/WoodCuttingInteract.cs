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
        public GameObject promptUI;
        public TMP_Text promptText;
        [SerializeField] Inventory playerInventory;

        private bool _minigameActive;

        private void Start()
        {
            promptText.text = "";
            promptUI.SetActive(false);
        }

        void Update()
        {
            if (!player) return;

            var dist = Vector3.Distance(transform.position, player.position);
            if (promptUI && !_minigameActive && IsHoldingAxe())
            {
                promptUI.SetActive(dist <= interactRange);
                promptText.text = "Press E to chop!";
            }
            else if (promptUI && !_minigameActive && !IsHoldingAxe())
            {
                promptUI.SetActive(dist <= interactRange);
                promptText.text = "Missing an axe";
            }

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
            DGameManager.TreeCuttingMinigameDone = true;
            Destroy(gameObject);
        }
    }
}