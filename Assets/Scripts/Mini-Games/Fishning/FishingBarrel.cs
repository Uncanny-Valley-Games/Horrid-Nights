using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class FishingBarrel : MonoBehaviour
{
    public InputActionAsset playerInput;
    public GameObject promptText;
    public float minDistance = 5f;
    
    private bool _minigameStarted = false;
    private bool _minigameEnded = true;

    private bool _bobFlag = true;
    
    private InputAction _interact;

    private void OnEnable()
    {
        playerInput.FindActionMap("Player").Enable();
    }

    private void OnDisable()
    {
        playerInput.FindActionMap("Player").Disable();
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _interact = InputSystem.actions.FindAction("Interact");
        promptText.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        if (!_minigameStarted)
        {
            if (_minigameEnded && Vector3.SqrMagnitude(transform.position) <  minDistance * minDistance)
            {
                promptText.SetActive(true);

                if (_interact.WasPressedThisFrame())
                {
                    StartMiniGame();
                }
            }
        } 
        else if (_minigameEnded)
        {
            EndMiniGame();
        }
        else // (minigame started and minigame not ended) the logic for the minigame
        {
            
        }
    }

    private void MiniGameLoop()
    {
        
    }

    private void StartMiniGame()
    {
        _minigameStarted = true;
        _minigameEnded = false;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        Time.timeScale = 0;
    }

    private void EndMiniGame()
    {
        // NOTE: No! End Minigame would only run if _minigameEnded is true
        // _minigameEnded = true;
        _minigameStarted = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1;
    }
}
