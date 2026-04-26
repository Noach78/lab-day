using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [Header("Configurations")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Interfaces")]
    public Image healthBarImage;   
    public Image damageOverlay;        
    public TextMeshProUGUI deathOverlay;    

    [Header("Damage effect")]
    public float fadeSpeed = 5f;   
    public float damageAlpha = 0.5f; 

    [Header("Animation")]
    public Animator animator;

    [Header("Respawn Settings")]
    public Transform respawnPoint;      
    public GameObject respawnButton;    

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

        if (respawnButton != null)
        {
            respawnButton.SetActive(false);
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
        if (currentHealth <= 0) return;

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
        
        if (animator != null)
        {
            animator.SetTrigger("Die"); 
        }

        if (deathOverlay != null)
        {
            Color c = deathOverlay.color;
            c.a = damageAlpha; 
            deathOverlay.color = c;
        }

        if (respawnButton != null)
        {
            respawnButton.SetActive(true);
        }

        GetComponent<PlayerMovement>().enabled = false;
        GetComponent<Collider>().enabled = false;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Respawn()
    {
        Debug.Log("Respawn au bateau !");

        if (respawnPoint != null)
        {
            transform.position = respawnPoint.position;
            transform.rotation = respawnPoint.rotation;
        }

        currentHealth = maxHealth;
        UpdateHealthBar();

        if (deathOverlay != null)
        {
            Color c = deathOverlay.color;
            c.a = 0f; 
            deathOverlay.color = c;
        }

        if (respawnButton != null)
        {
            respawnButton.SetActive(false);
        }

        if (animator != null)
        {
            animator.Play("Blend Tree"); 
            animator.SetFloat("walk", 0f);
        }

        GetComponent<PlayerMovement>().enabled = true;
        GetComponent<Collider>().enabled = true;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}