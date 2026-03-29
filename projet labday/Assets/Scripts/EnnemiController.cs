using UnityEngine;
using UnityEngine.AI; 

public class BossController : MonoBehaviour
{
    [Header("Cibles")]
    public Transform player; 

    [Header("Paramètres")]
    public float detectionRange = 15f; 
    public float attackRange = 8f;     
    public float attackCooldown = 2f;  
    public int damageAmount = 20;     

    private NavMeshAgent agent;
    private Animator animator;
    private float lastAttackTime = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectionRange && distance > attackRange)
        {
            ChasePlayer();
        }
        else if (distance <= attackRange)
        {
            AttackPlayer();
        }
        else
        {
            StopChasing();
        }
    }

    void ChasePlayer()
    {
        if (agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
        
        if (animator != null && animator.runtimeAnimatorController != null)
        {
            animator.SetBool("isMoving", true);
        }
    }

    void StopChasing()
    {
        if (agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            agent.isStopped = true;
        }

        if (animator != null && animator.runtimeAnimatorController != null)
        {
            animator.SetBool("isMoving", false);
        }
    }

    void AttackPlayer()
    {
        if (agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            agent.isStopped = true;
        }

        if (animator != null && animator.runtimeAnimatorController != null)
        {
            animator.SetBool("isMoving", false);
        }

        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            if (animator != null && animator.runtimeAnimatorController != null)
            {
                animator.SetTrigger("Attack");
            }
            
            lastAttackTime = Time.time;

            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}