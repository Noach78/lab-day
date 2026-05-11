using UnityEngine;

public class SkeletonAI : MonoBehaviour
{
    [Header("Paramètres de Vie")]
    public int maxHealth = 50;         // Vie maximale du squelette
    private int currentHealth;

    [Header("Paramètres d'Attaque")]
    public int damageAmount = 15;      // Dégâts infligés au joueur
    public float moveSpeed = 3f;       // Vitesse de déplacement
    public float attackCooldown = 1.5f;// Temps entre chaque attaque

    private Transform player;
    private PlayerHealth playerHealth;
    private float lastAttackTime;

    private Animator animator;
    private bool isDead = false;       // Empêche le squelette d'agir s'il est mort

    void Start()
    {
        currentHealth = maxHealth; // Initialise la vie au maximum

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerHealth = playerObj.GetComponent<PlayerHealth>();
        }

        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Si le squelette est mort, ou que le joueur est introuvable/mort, on ne fait rien
        if (isDead || player == null || playerHealth == null || playerHealth.currentHealth <= 0)
        {
            if (animator != null && !isDead) animator.SetFloat("Speed", 0f);
            return; 
        }

        // Le squelette regarde le joueur
        Vector3 targetPosition = new Vector3(player.position.x, transform.position.y, player.position.z);
        transform.LookAt(targetPosition);

        // Le squelette avance vers le joueur
        transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);

        // Active l'animation de marche
        if (animator != null) animator.SetFloat("Speed", moveSpeed);
    }

    // --- NOUVELLE FONCTION : Recevoir des dégâts ---
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log("Squelette touché ! Vie restante : " + currentHealth);

        // Si tu as une animation de dégâts, tu peux l'activer ici :
        // if (animator != null) animator.SetTrigger("Hit");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // --- NOUVELLE FONCTION : Gérer la mort ---
    private void Die()
    {
        isDead = true;
        QuestManager.Instance.AddSkeletonKill();
        Debug.Log("Le squelette est mort !");

        moveSpeed = 0f;

    
        Destroy(gameObject, 2f);
    }

    void OnCollisionStay(Collision collision)
    {
        if (isDead) return; // Un squelette mort n'attaque pas

        if (collision.gameObject.CompareTag("Player"))
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                if (playerHealth != null && playerHealth.currentHealth > 0)
                {
                    playerHealth.TakeDamage(damageAmount);
                    lastAttackTime = Time.time;

                    if (animator != null) animator.SetTrigger("Attack");
                }
            }
        }
    }
}