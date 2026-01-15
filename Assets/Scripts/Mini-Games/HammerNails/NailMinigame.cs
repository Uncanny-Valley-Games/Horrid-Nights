using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NailMinigame : MonoBehaviour
{
    [SerializeField] GameObject nailGameWidget;

    // Button variables
    [SerializeField] Button nail1;
    [SerializeField] Button nail2;
    [SerializeField] Button nail3;
    [SerializeField] Button nail4;
    [SerializeField] Button nail5;
    [SerializeField] Button nail6;

    float lossTimer = 5.0f;

    void Start()
    {
        LoosenNail(nail1);
        LoosenNail(nail2);
        LoosenNail(nail3);
        LoosenNail(nail4);
        LoosenNail(nail5);
        LoosenNail(nail6);
    }

    void Update()
    {
        if (nailGameWidget.activeSelf)
        {
            // Checks if all of the nail buttons are interactable, or loose
            if (nail1.interactable && nail2.interactable && nail3.interactable && nail4.interactable && nail5.interactable && nail6.interactable)
            {
                lossTimer -= Time.deltaTime;
                if (lossTimer <= 0)
                {
                    Debug.Log("The monster got inside! Game over!");
                }
            }
            // Checks if all of the nail buttons are not interactable, or fixed
            else if (!nail1.interactable && !nail2.interactable && !nail3.interactable && !nail4.interactable && !nail5.interactable && !nail6.interactable)
            {
                Debug.Log("You hammered all of the nails!");
                nailGameWidget.SetActive(false);
            }
            else
            {
                lossTimer = 5.0f;
            }
        }
    }

    public void LoosenNail(Button nail)
    {
        TextMeshProUGUI nailText = nail.GetComponentInChildren<TextMeshProUGUI>();
        nailText.text = "Loose";
        nail.interactable = true;
    }

    public void HammerNail(Button nail)
    {
        TextMeshProUGUI nailText = nail.GetComponentInChildren<TextMeshProUGUI>();
        nailText.text = "Fixed";
        nail.interactable = false;
    }
}
