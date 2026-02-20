using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] Slider healthSlider;

    [SerializeField] float maxHealth = 100f;
    float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
        healthSlider.value = currentHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0f)
        {
            healthSlider.value = 0f;
            Debug.Log("Game over!");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            healthSlider.value = currentHealth;
        }
    }

    public void RestoreHealth(float healAmount)
    {
        currentHealth += healAmount;
        if (currentHealth > maxHealth)
        {
            healthSlider.value = maxHealth;
        }
        else
        {
            healthSlider.value = currentHealth;
        }
    }
}
