using UnityEngine;

public class QuestStation : MonoBehaviour
{
    private bool isPlayerInRange = false;

    private void Update()
    {
        // Si le joueur est proche et appuie sur E
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            // On demande au QuestManager d'ouvrir l'interface
            if (QuestManager.Instance != null)
            {
                QuestManager.Instance.ToggleQuestUI();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            // Optionnel : Afficher un petit texte "Appuyez sur E pour les quêtes"
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            
            // Si le joueur s'en va, on ferme le menu automatiquement
            if (QuestManager.Instance != null && QuestManager.Instance.questUI.activeSelf)
            {
                QuestManager.Instance.ToggleQuestUI();
            }
        }
    }
}