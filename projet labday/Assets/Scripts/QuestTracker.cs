using UnityEngine;
using UnityEngine.UI; // Pour afficher la progression sur l'UI
using InventoryFramework; 

public class QuestTracker : MonoBehaviour
{
    [Header("Paramètres de l'Interface (UI)")]
    public GameObject questUI; 
    public Text questProgressText; // Un texte pour afficher "Squelettes : 0/5" etc.

    [Header("Objectifs de la Quête")]
    public int skeletonsToKill = 5;
    public int currentSkeletons = 0;
    
    public bool golemKilled = false;
    public Transform golemSpawnPoint; // Coordonnées du Golem
    public GameObject objectiveArrow; // Une flèche UI ou 3D qui pointe vers le Golem

    public int sapToCollect = 3;
    public int currentSap = 0;

    [Header("Récompense")]
    public Item mapFragmentReward;
    public ItemPickupHandler pickupHandler;

    private bool isPlayerInRange = false;
    private bool isQuestCompleted = false;

    private void Update()
    {
        // Ouverture/Fermeture du journal de quête avec "E"
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            ToggleQuestUI();
        }

        // Gestion de la flèche directionnelle vers le Golem
        if (questUI.activeSelf && !golemKilled && golemSpawnPoint != null)
        {
            UpdateObjectiveArrow();
        }
    }

    private void ToggleQuestUI()
    {
        bool isOpening = !questUI.activeSelf;
        questUI.SetActive(isOpening);
        
        // Mise à jour du texte au moment de l'ouverture
        RefreshUI();

        if (isOpening)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // --- MÉTHODES POUR PROGRESSER ---

    public void OnSkeletonKilled()
    {
        if (currentSkeletons < skeletonsToKill)
        {
            currentSkeletons++;
            CheckQuestCompletion();
        }
    }

    public void OnGolemKilled()
    {
        golemKilled = true;
        objectiveArrow.SetActive(false); // On cache la flèche
        CheckQuestCompletion();
    }

    public void OnSapCollected()
    {
        if (currentSap < sapToCollect)
        {
            currentSap++;
            CheckQuestCompletion();
        }
    }

    private void CheckQuestCompletion()
    {
        if (currentSkeletons >= skeletonsToKill && golemKilled && currentSap >= sapToCollect)
        {
            if (!isQuestCompleted)
            {
                CompleteQuest();
            }
        }
    }

    private void CompleteQuest()
    {
        isQuestCompleted = true;
        pickupHandler.PickupItem(mapFragmentReward, 1);
        Debug.Log("Quête terminée ! Fragment de carte obtenu.");
    }

    private void UpdateObjectiveArrow()
    {
        // Logique simple pour que la flèche regarde vers le Golem
        objectiveArrow.SetActive(true);
        Vector3 direction = golemSpawnPoint.position - transform.position;
        objectiveArrow.transform.rotation = Quaternion.LookRotation(direction);
    }

    private void RefreshUI()
    {
        if (questProgressText != null)
        {
            questProgressText.text = $"Objectifs :\n" +
                                     $"- Squelettes : {currentSkeletons}/{skeletonsToKill}\n" +
                                     $"- Golem : {(golemKilled ? "Fait" : "À trouver")}\n" +
                                     $"- Sève d'arbre : {currentSap}/{sapToCollect}";
        }
    }

    // --- DÉTECTION DU JOUEUR (Identique à ton script Shop) ---

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) isPlayerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (questUI.activeSelf) ToggleQuestUI();
        }
    }
}