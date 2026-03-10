using UnityEngine;
using UnityEngine.UI;

public class PlayerSanity : MonoBehaviour
{
    [SerializeField] Slider sanitySlider;

    [SerializeField] float maxLimit = 100f;
    float currentSanity;

    void Start()
    {
        currentSanity = 0f;
        sanitySlider.value = currentSanity;
    }

    void Update()
    {
        // Will handle any changes when sanity is at certain percentages
    }

    public void LowerSanity(float amount)
    {
        currentSanity += amount;
        if (currentSanity > maxLimit)
        {
            currentSanity = maxLimit;
        }
        sanitySlider.value = currentSanity;
    }

    public void RestoreSanity(float amount)
    {
        currentSanity -= amount;
        if (currentSanity < 0f)
        {
            currentSanity = 0f;
        }
        sanitySlider.value = currentSanity;
    }
}
