using UnityEngine;
using UnityEngine.UI;

public class PlayerStamina : MonoBehaviour
{
    [SerializeField] Slider staminaSlider;

    [SerializeField] float maxStamina = 100f;
    float currentStamina;
    bool isSprinting;

    [SerializeField] float lossRate = 25f;
    [SerializeField] float recoveryRate = 15f;

    public float GetCurrentStamina()
    {
        return currentStamina;
    }

    public void SetIsSprinting(bool sprinting)
    {
        isSprinting = sprinting;
    }

    void Start()
    {
        currentStamina = maxStamina;
        staminaSlider.value = currentStamina;
        isSprinting = false;
    }

    void Update()
    {
        if (isSprinting)
        {
            DecreaseStamina();
        }
        else
        {
            IncreaseStamina();
        }
    }

    void IncreaseStamina()
    {
        if (currentStamina < maxStamina)
        {
            currentStamina += recoveryRate * Time.unscaledDeltaTime;
        }
        else
        {
            currentStamina = maxStamina;
        }
        staminaSlider.value = currentStamina;
    }

    void DecreaseStamina()
    {
        if (currentStamina > 0f)
        {
            currentStamina -= lossRate * Time.deltaTime;
        }
        else
        {
            currentStamina = 0f;
        }
        staminaSlider.value = currentStamina;
    }
}
