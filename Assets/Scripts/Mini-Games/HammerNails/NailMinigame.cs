using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NailMinigame : MonoBehaviour
{
    [SerializeField] GameObject nailGameWidget;
    [SerializeField] TextMeshProUGUI nailGameText;

    // Button variables
    [SerializeField] Button nail1;
    [SerializeField] Button nail2;
    [SerializeField] Button nail3;
    [SerializeField] Button nail4;
    [SerializeField] Button nail5;
    [SerializeField] Button nail6;

    int nailsToLoosen = 0;

    // Timer variables
    float nailTime = 2.0f;
    bool startedNailTimer;
    float lossTime = 3.0f;
    bool startedLossTimer;    

    void Start()
    {
        nailGameText.text = "Hammer the nails!";

        BeginMinigame();
    }

    void Update()
    {
        if (nailGameWidget.activeSelf)
        {
            // Checks if all of the nail buttons are interactable, or loose
            if (nail1.interactable && nail2.interactable && nail3.interactable && nail4.interactable && nail5.interactable && nail6.interactable)
            {
                StopCoroutine(NailTimer());
                startedNailTimer = false;

                if (!startedLossTimer)
                {
                    StartCoroutine(LossTimer());
                }
            }
            // Checks if all of the nail buttons are not interactable, or fixed
            else if (!nail1.interactable && !nail2.interactable && !nail3.interactable && !nail4.interactable && !nail5.interactable && !nail6.interactable)
            {
                StopCoroutine(NailTimer());
                startedNailTimer = false;

                nailGameWidget.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Time.timeScale = 1;
            }
            else
            {
                if (startedLossTimer)
                {
                    StopCoroutine(LossTimer());
                    startedLossTimer = false;
                }

                if (!startedNailTimer)
                {
                    StartCoroutine(NailTimer());
                }
            }
        }
    }

    public void BeginMinigame()
    {
        nailsToLoosen = 0;

        HammerNail(nail1);
        HammerNail(nail2);
        HammerNail(nail3);
        HammerNail(nail4);
        HammerNail(nail5);
        HammerNail(nail6);

        // A random number is chosen to loosen different nails when the minigame starts
        int randNum = Random.Range(0, 4);
        switch (randNum)
        {
            case 0:
                LoosenNail(nail1);
                LoosenNail(nail3);
                LoosenNail(nail5);
                break;

            case 1:
                LoosenNail(nail2);
                LoosenNail(nail4);
                LoosenNail(nail6);
                break;

            case 2:
                LoosenNail(nail1);
                LoosenNail(nail2);
                LoosenNail(nail5);
                break;

            default:
                LoosenNail(nail3);
                LoosenNail(nail4);
                LoosenNail(nail6);
                break;
        }
    }

    void LoosenNail(Button nail)
    {
        TextMeshProUGUI nailText = nail.GetComponentInChildren<TextMeshProUGUI>();
        nailText.text = "Loose";
        nail.GetComponent<Image>().color = Color.white;
        nail.interactable = true;
    }

    public void HammerNail(Button nail)
    {
        TextMeshProUGUI nailText = nail.GetComponentInChildren<TextMeshProUGUI>();
        nailText.text = "Fixed";
        nail.GetComponent<Image>().color = Color.green;
        nail.interactable = false;
    }

    IEnumerator NailTimer()
    {
        startedNailTimer = true;
        yield return new WaitForSecondsRealtime(nailTime);
        if (startedNailTimer)
        {
            switch (nailsToLoosen) 
            {
                case 0:
                    LoosenNail(nail1);
                    LoosenNail(nail3);
                    nailsToLoosen++;
                    break;

                case 1:
                    LoosenNail(nail2);
                    LoosenNail(nail6);
                    nailsToLoosen++;
                    break;

                default:
                    LoosenNail(nail4);
                    LoosenNail(nail5);
                    nailsToLoosen = 0;
                    break;
            }
            startedNailTimer = false;
        }
    }

    IEnumerator LossTimer()
    {
        startedLossTimer = true;
        yield return new WaitForSecondsRealtime(lossTime);
        if (startedLossTimer)
        {
            nailGameText.text = "The monster got inside! Game over!";
        }
    }
}
