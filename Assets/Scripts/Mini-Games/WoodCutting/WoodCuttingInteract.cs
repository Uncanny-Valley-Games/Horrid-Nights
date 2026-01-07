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
            if (promptUI && !_minigameActive) promptUI.SetActive(dist <= interactRange);

            if (dist <= interactRange && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
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

        public void BreakTree()
        {
            Destroy(gameObject);
        }
    }
}