using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Third Person Controller References
    [SerializeField]
    private Animator playerAnim;

    //Equip-Unequip parameters
    [SerializeField]
    private GameObject sword;
    [SerializeField]
    private GameObject swordOnShoulder;
    public bool isEquipping;
    public bool isEquipped;

    //Attack Parameters
    public bool isAttacking;
    private float timeSinceAttack;
    public int currentAttack = 0;

    private void Update()
    {
        timeSinceAttack += Time.deltaTime;

        Attack();
        Equip();
    }

    private void Equip()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            // On appelle ActiveWeapon pour déplacer l'épée dans la main
            // et inverser la valeur de isEquipped
            ActiveWeapon();
            
            // Maintenant on dit à l'Animator si on est armé ou non
            // (Il manquait le 2ème paramètre ici !)
            playerAnim.SetBool("isArmed", isEquipped);
        }
    }

    public void ActiveWeapon()
    {
        if (!isEquipped)
        {
            sword.SetActive(true);
            swordOnShoulder.SetActive(false);
            isEquipped = true; // On force la valeur à true
        }
        else
        {
            sword.SetActive(false);
            swordOnShoulder.SetActive(true);
            isEquipped = false; // On force la valeur à false
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
            // 1. SÉCURITÉ : Si on est déjà en train de donner un coup, on ignore le clic
            if (isAttacking) 
                return;

            // 2. CHRONO : Si on a attendu trop longtemps, on remet le combo à ZÉRO
            // On le fait avant d'ajouter +1 !
            if (timeSinceAttack > 5.0f)
            {
                currentAttack = 0;
            }

            // 3. COMBO : On passe au coup suivant (0 devient 1, 1 devient 2...)
            currentAttack++;

            if (currentAttack > 3)
            {
                currentAttack = 1;
            }

            // 4. ACTION : On bloque les prochains clics et on lance l'animation
            isAttacking = true;

            Debug.Log("Lancement de l'attaque : " + currentAttack);
            playerAnim.SetTrigger("Attack" + currentAttack);
            
            // 5. On remet le chrono à zéro pour le prochain clic
            timeSinceAttack = 0;
        }
    }

    //This will be used at animation event
    public void ResetAttack()
    {
        isAttacking = false;
    } 
}