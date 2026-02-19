using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStamina : MonoBehaviour
{
    [SerializeField] Slider staminaSlider;

    [SerializeField] float initialMaxStamina = 100f;
    float maxStamina;
    float currentStamina;
    bool isSprinting;
    float recoveryTime = 2.5f;
    bool isTired;

    [SerializeField] float lossRate = 40f;
    [SerializeField] float recoveryRate = 20f;

    public float GetInitialMaxStamina()
    {
        return initialMaxStamina;
    }

    public float GetCurrentStamina()
    {
        return currentStamina;
    }

    public void SetMaxStamina(float newStamina)
    {
        maxStamina = newStamina;
    }

    public void SetIsSprinting(bool sprinting)
    {
        isSprinting = sprinting;
    }

    void Start()
    {
        maxStamina = initialMaxStamina;
        currentStamina = maxStamina;
        staminaSlider.value = currentStamina;
        isSprinting = false;
        isTired = false;
    }

    void Update()
    {
        if (isSprinting && !isTired)
        {
            DecreaseStamina();
        }
        else if (!isSprinting && !isTired)
        {
            IncreaseStamina();
        }
    }

    void IncreaseStamina()
    {
        currentStamina += recoveryRate * Time.unscaledDeltaTime;
        if (currentStamina >= maxStamina)
        {
            currentStamina = maxStamina;
        }
        staminaSlider.value = currentStamina;
    }

    void DecreaseStamina()
    {
        currentStamina -= lossRate * Time.deltaTime;
        if (currentStamina <= 0f)
        {
            currentStamina = 0f;
            StartCoroutine(OutOfStamina());
        }
        staminaSlider.value = currentStamina;
    }

    // Prevents the player from sprinting for a certain time after running out of stamina
    IEnumerator OutOfStamina()
    {
        isTired = true;
        yield return new WaitForSecondsRealtime(recoveryTime);
        isTired = false;
    }
}
