using UnityEngine;
using UnityEngine.UI; 
public class PlayerHealth : MonoBehaviour
{
    [Header("Configurations")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Interfaces")]
    public Image healthBarImage;   
    public Image damageOverlay;        
    public Text deathOverlay;    


    [Header("Damage effect")]
    public float fadeSpeed = 5f;   
    public float damageAlpha = 0.5f; 


    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
        
        if (damageOverlay != null)
        {
            Color c = damageOverlay.color;
            c.a = 0f;
            damageOverlay.color = c;
        }
    }

    void Update()
    {
        if (damageOverlay != null)
        {
            if (damageOverlay.color.a > 0)
            {
                Color c = damageOverlay.color;
                c.a -= Time.deltaTime * fadeSpeed; 
                damageOverlay.color = c;
            }
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;

        UpdateHealthBar();

        if (damageOverlay != null)
        {
            Color c = damageOverlay.color;
            c.a = damageAlpha; 
            damageOverlay.color = c;
        }


        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthBar()
    {
        if (healthBarImage != null)
        {
            healthBarImage.fillAmount = (float)currentHealth / maxHealth;
        }
    }

    void Die()
    {
        Debug.Log("Game Over !");
        if (deathOverlay != null)
        {
            Color c = deathOverlay.color;
            c.a = damageAlpha; 
            deathOverlay.color = c;
        }
    }
}