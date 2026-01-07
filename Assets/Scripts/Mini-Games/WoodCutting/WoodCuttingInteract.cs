using UnityEngine;
using UnityEngine.InputSystem;

public class WoodCuttingInteract : MonoBehaviour
{
    public Transform player;
    public float interactRange = 3f;
    public string interactPrompt = "Press E to chop";
    public GameObject promptUI;

    void Update()
    {
        if (!player) return;

        float dist = Vector3.Distance(transform.position, player.position);
        if (promptUI) promptUI.SetActive(dist <= interactRange);

        if (dist <= interactRange && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            var mg = WoodCuttingMinigame.Instance;
            if (mg)
            {
                mg.StartMinigame(this);
            }
        }
    }

    public void BreakTree()
    {
        Destroy(gameObject);
    }
}