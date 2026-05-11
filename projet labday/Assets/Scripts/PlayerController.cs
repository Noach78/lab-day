using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Third Person Controller References")]
    [SerializeField]
    private Animator playerAnim;

    [Header("Equip-Unequip parameters")]
    [SerializeField]
    private GameObject sword;
    [SerializeField]
    private GameObject swordOnShoulder;
    public bool isEquipping;
    public bool isEquipped;

    [Header("Attack Parameters")]
    public bool isAttacking;
    private float timeSinceAttack;
    public int currentAttack = 0;

    [Header("Damage Parameters (Nouveau)")]
    public int swordDamage = 25;       // Dégâts infligés par l'épée
    public float attackRange = 1.5f;   // Rayon de la zone d'impact
    public float attackForwardOffset = 1.0f; // Distance de l'attaque devant le joueur

    private void Update()
{
    timeSinceAttack += Time.deltaTime;

    
    if (isAttacking && timeSinceAttack > 1.5f)
    {
        isAttacking = false;
        currentAttack = 0; // On en profite pour réinitialiser le combo
    }

    Attack();
    Equip();
}

    private void Equip()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ActiveWeapon();
            playerAnim.SetBool("isArmed", isEquipped);
        }
    }

    public void ActiveWeapon()
    {
        if (!isEquipped)
        {
            sword.SetActive(true);
            swordOnShoulder.SetActive(false);
            isEquipped = true; 
        }
        else
        {
            sword.SetActive(false);
            swordOnShoulder.SetActive(true);
            isEquipped = false; 
        }
    }

    public void Equipped()
    {
        isEquipping = false;
    }

    private void Attack()
    {
        if (Input.GetMouseButtonDown(0) && isEquipped)
        {
            if (isAttacking) 
                return;

            if (timeSinceAttack > 5.0f)
            {
                currentAttack = 0;
                isAttacking = false;
            }

            currentAttack++;

            if (currentAttack > 3)
            {
                currentAttack = 1;
            }

            isAttacking = true;

            Debug.Log("Lancement de l'attaque : " + currentAttack);
            playerAnim.SetTrigger("Attack" + currentAttack);
            
            timeSinceAttack = 0;
        }
    }

    // --- NOUVELLE FONCTION ---
    // À utiliser comme Animation Event pile quand la lame tranche l'air !
    public void DealDamage()
    {
        // 1. Calcule le centre de l'attaque (devant le joueur, à hauteur de taille)
        Vector3 attackCenter = transform.position + transform.forward * attackForwardOffset + Vector3.up * 1.0f;

        // 2. Crée une sphère de détection
        Collider[] hitColliders = Physics.OverlapSphere(attackCenter, attackRange);

        // 3. Vérifie tout ce que la sphère a touché
        foreach (Collider hit in hitColliders)
        {
            SkeletonAI skeleton = hit.GetComponent<SkeletonAI>();
            
            // Si c'est un squelette, on lui inflige des dégâts !
            if (skeleton != null)
            {
                skeleton.TakeDamage(swordDamage);
            }
        }
    }

    //This will be used at animation event
    public void ResetAttack()
    {
        isAttacking = false;
    } 

    // --- BONUS : Pour t'aider à visualiser la zone d'attaque dans l'éditeur ---
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 attackCenter = transform.position + transform.forward * attackForwardOffset + Vector3.up * 1.0f;
        Gizmos.DrawWireSphere(attackCenter, attackRange);
    }
}