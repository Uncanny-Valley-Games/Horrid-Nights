using UnityEngine;
using UnityEngine.UI;

public class PlayerSanity : MonoBehaviour
{
    [SerializeField] Slider sanitySlider;
    [SerializeField] Image darkeningImage;

    [SerializeField] float maxLimit = 100f;
    float currentSanity;

    Color newColour;
    float sanityTimer;
    float sanityInterval = 4f;

    public float GetCurrentSanity()
    {
        return currentSanity;
    }

    void Start()
    {
        currentSanity = 0f;
        sanitySlider.value = currentSanity;

        newColour = darkeningImage.color;
    }

    void Update()
    {
        sanityTimer += Time.deltaTime;
        if (sanityTimer >= sanityInterval)
        {
            LowerSanity(1f);
            sanityTimer = 0f;
        }

        // Darkens the screen as the player's sanity increases
        newColour.a = currentSanity / 200f;
        darkeningImage.color = newColour;
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
