using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerFoodAndWater : MonoBehaviour
{
    PlayerStamina playerStamina;
    PlayerSanity playerSanity;
    [SerializeField] Slider hungerThirstSlider;

    [SerializeField] float maxHunger = 50f;
    [SerializeField] float maxThirst = 50f;
    float currentHunger;
    float currentThirst;
    [SerializeField] float hungerRate = 0.5f;
    [SerializeField] float thirstRate = 0.5f;

    float waitTime = 4f;
    float maxStamina;
    bool canReset;

    void Start()
    {
        playerStamina = GetComponent<PlayerStamina>();
        playerSanity = GetComponent<PlayerSanity>();
        hungerThirstSlider.value = 0f;

        currentHunger = 0f;
        currentThirst = 0f;
        canReset = true;
    }

    void Update()
    {
        if (canReset)
        {
            StartCoroutine(IncreaseHungerAndThirst());
        }
    }

    // The player's sanity affects the rate of hunger and thirst
    void IncreaseHunger()
    {
        currentHunger += hungerRate * (playerSanity.GetCurrentSanity() / 50f + 1f);
        if (currentHunger > maxHunger)
        {
            currentHunger = maxHunger;
        }
    }

    void IncreaseThirst()
    {
        currentThirst += thirstRate * (playerSanity.GetCurrentSanity() / 50f + 1f);
        if (currentThirst > maxThirst)
        {
            currentThirst = maxThirst;
        }
    }

    public void RestoreHunger(float amount)
    {
        currentHunger -= amount;
        if (currentHunger < 0)
        {
            currentHunger = 0;
        }

        CalculateMaxStamina();
    }

    public void RestoreThirst(float amount)
    {
        currentThirst -= amount;
        if (currentThirst < 0)
        {
            currentThirst = 0;
        }

        CalculateMaxStamina();
    }

    void CalculateMaxStamina()
    {
        // Lowers the player's max stamina based on their hunger and thirst
        maxStamina = playerStamina.GetInitialMaxStamina() - ((currentHunger + currentThirst) / 2);
        // Updates hunger & thirst slider with current hunger and thirst
        hungerThirstSlider.value = (currentHunger + currentThirst) / 2;
        playerStamina.SetMaxStamina(maxStamina);
    }

    IEnumerator IncreaseHungerAndThirst()
    {
        canReset = false;
        yield return new WaitForSeconds(waitTime);

        IncreaseHunger();
        IncreaseThirst();

        CalculateMaxStamina();

        canReset = true;
    }
}
