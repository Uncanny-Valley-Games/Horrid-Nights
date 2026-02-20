using System.Collections;
using UnityEngine;

public class PlayerFoodAndWater : MonoBehaviour
{
    PlayerStamina playerStamina;

    [SerializeField] float maxHunger = 50f;
    [SerializeField] float maxThirst = 50f;
    float currentHunger;
    float currentThirst;

    float waitTime = 4f;
    float maxStamina;
    bool canReset;

    void Start()
    {
        playerStamina = GetComponent<PlayerStamina>();

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

    void IncreaseHunger()
    {
        currentHunger += 0.5f;
        if (currentHunger > maxHunger)
        {
            currentHunger = maxHunger;
        }
    }

    void IncreaseThirst()
    {
        currentThirst += 0.5f;
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
