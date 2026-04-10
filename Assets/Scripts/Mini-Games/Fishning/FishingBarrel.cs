using System.Collections;
using Downscaled;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class FishingBarrel : MonoBehaviour
{
    public InputActionAsset playerInput;
    public GameObject promptText;
    public float minDistance = 5f;
    public GameObject playerGameObject;
    public GameObject minigameObject;
    public RectTransform bait;
    public float baitDefaultYPosition;
    public float baitCaughtYPosition;
    public float baitNotCaughtYPosition;
    public float baitBobDelay;
    
    private bool _minigameStarted;
    private bool _minigameEnded = true;

    private bool _canBob = true;
    private bool _bobFlag = true;
    
    private InputAction _interact;
    
    private Inventory playerInventory;

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
        minigameObject.SetActive(false);
        playerInventory = playerGameObject.GetComponent<Inventory>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!_minigameStarted)
        {
            if (_minigameEnded && IsHoldingRod() && Vector3.Distance(transform.position, playerGameObject.transform.position) <  minDistance)
            {
                promptText.SetActive(true);

                if (_interact.WasPressedThisFrame())
                {
                    StartMiniGame();
                }
            }
            else
            {
                promptText.SetActive(false);
            }
        } 
        else if (_minigameEnded)
        {
            EndMiniGame();
        }
        else // (minigame started and minigame not ended) the logic for the minigame
        {
            MiniGameLoop();
        }
    }

    private bool IsHoldingRod()
    {
        if (playerInventory.GetCurrentItem() != null)
        {
            return playerInventory.GetCurrentItem().itemName == InventoryItem.ItemName.FishingRod;
        }
        return false;
    }

    private void MiniGameLoop()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame
                        || Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (Mathf.Approximately(bait.anchoredPosition.y, baitCaughtYPosition))
            {
                Debug.Log("BaitCaught"); // todo: change this to affect a global variable for the day

                if (DGameManager.FinishedInitialMiniGames())
                {
                    UpdatableText.UpdateStatusText("You may head home now!");
                }
                else
                {
                    UpdatableText.UpdateStatusText("Fish caught! Go cut some wood now.");
                }
                
                DGameManager.FishingMinigameDone = true;
            }
            else
            {
                Debug.Log("BaitNotCaught"); // todo: add some kind of feedback
                UpdatableText.UpdateStatusText("Fish missed!");
            }

            _minigameEnded = true;
        }

        if (_canBob)
        {
            if (_bobFlag)
            {
                bait.anchoredPosition = new (0.0f, baitDefaultYPosition);
            }
            else
            {
                if (Random.Range(0f, 100f) <= 50)
                {
                    bait.anchoredPosition = new (0.0f, baitCaughtYPosition);
                }
                else
                {
                    bait.anchoredPosition = new (0.0f, baitNotCaughtYPosition);
                }
            }
            
            _bobFlag = !_bobFlag;
            
            StartCoroutine(WaitForBob());
        }
    }

    private IEnumerator WaitForBob()
    {
        _canBob = false;
        yield return new WaitForSecondsRealtime(baitBobDelay);
        _canBob = true;
    }

    private void StartMiniGame()
    {
        _minigameStarted = true;
        _minigameEnded = false;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        Time.timeScale = 0;
        minigameObject.SetActive(true);
    }

    private void EndMiniGame()
    {
        // NOTE: No! End Minigame would only run if _minigameEnded is true
        // _minigameEnded = true;
        _minigameStarted = false;
        _canBob = true;
        _bobFlag = true;
        bait.anchoredPosition = new (0.0f, baitDefaultYPosition);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1;
        minigameObject.SetActive(false);
    }
}
