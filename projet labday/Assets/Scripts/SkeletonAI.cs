using UnityEngine;

public class SkeletonAI : MonoBehaviour
{
    [Header("Paramètres de Vie")]
    public int maxHealth = 50;         
    private int currentHealth;

    [Header("Paramètres d'Attaque")]
    public int damageAmount = 15;      
    public float moveSpeed = 3f;      
    public float attackCooldown = 1.5f;

    private Transform player;
    private PlayerHealth playerHealth;
    private float lastAttackTime;

    private Animator animator;
    private bool isDead = false;       
    void Start()
    {
        currentHealth = maxHealth; 

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
        if (isDead || player == null || playerHealth == null || playerHealth.currentHealth <= 0)
        {
            if (animator != null && !isDead) animator.SetFloat("Speed", 0f);
            return; 
        }

        Vector3 targetPosition = new Vector3(player.position.x, transform.position.y, player.position.z);
        transform.LookAt(targetPosition);

        transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);

        if (animator != null) animator.SetFloat("Speed", moveSpeed);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log("Squelette touché ! Vie restante : " + currentHealth);

       

        if (currentHealth <= 0)
        {
            Die();
        }
    }

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
        if (isDead) return; 

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