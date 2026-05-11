using UnityEngine;

public class TreeInteractable : MonoBehaviour
{
    [Header("Paramètres de l'Arbre")]
    public float searchCooldown = 1.0f; // Temps d'attente entre deux fouilles (en secondes)
    private float nextSearchTime = 0f;  

    private bool isPlayerInRange = false;
    private bool hasGivenSap = false; // Pour que l'arbre ne donne de la sève qu'une seule fois

    private void Update()
    {
        // Si le joueur est proche, que l'arbre n'est pas vide, et qu'il appuie sur E
        if (isPlayerInRange && !hasGivenSap && Input.GetKeyDown(KeyCode.E))
        {
            // Vérifie si le cooldown est passé
            if (Time.time >= nextSearchTime)
            {
                TryHarvestSap();
                nextSearchTime = Time.time + searchCooldown; // Relance le chrono
            }
            else
            {
                Debug.Log("Attends un peu avant de fouiller à nouveau !");
            }
        }
    }

    private void TryHarvestSap()
    {
        // --- LA CHANCE SUR 10 ---
        // Random.Range(1, 11) génère un nombre entier entre 1 et 10.
        int chance = Random.Range(1, 11);

        if (chance == 1) // 10% de chance
        {
            Debug.Log("Succès ! Tu as trouvé de la sève.");
            hasGivenSap = true; // L'arbre est maintenant vide

            // On prévient le QuestManager !
            if (QuestManager.Instance != null)
            {
                QuestManager.Instance.OnSapCollected();
            }
        }
        else
        {
            Debug.Log("Rien... Pas de sève ici pour le moment.");
        }
    }

    // --- DÉTECTION DU JOUEUR ---
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}