using UnityEngine;
using InventoryFramework; 

public class ShopInteractable : MonoBehaviour
{
    [Header("Paramètres de l'Interface (UI)")]
    public GameObject shopUI; 

    [Header("Paramètres de l'Objet")]
    public Item itemToSell;   
    public int itemPrice = 150; 

    [Header("Références")]
    public CoinCollection coinCollection;
    public ItemPickupHandler pickupHandler;

    private bool isPlayerInRange = false;

    private void Update()
    {
        // Si le joueur est dans le cube ET appuie sur "E"
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            ToggleShop();
        }
    }

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
            
            // Si le joueur s'éloigne alors que la boutique est ouverte, on la ferme proprement
            if (shopUI != null && shopUI.activeSelf)
            {
                ToggleShop(); 
            }
        }
    }

    // NOUVELLE MÉTHODE : Gère l'ouverture/fermeture et l'état de la souris
    private void ToggleShop()
    {
        bool isOpening = !shopUI.activeSelf; // Vérifie si on est en train d'ouvrir ou de fermer
        shopUI.SetActive(isOpening);

        if (isOpening)
        {
            // On affiche et déverrouille la souris
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            // Optionnel : Tu peux aussi mettre le jeu en pause ici avec Time.timeScale = 0f;
        }
        else
        {
            // On cache et reverrouille la souris au centre de l'écran
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            // Optionnel : Si tu avais mis en pause, remets Time.timeScale = 1f; ici
        }
    }

    public void BuyItem()
    {
        if (coinCollection.SpendCoins(itemPrice))
        {
            pickupHandler.PickupItem(itemToSell, 1);
            Debug.Log("Achat réussi : " + itemToSell.name);

            if (QuestManager.Instance != null)
            {
                QuestManager.Instance.OnSwordBought();
            }
        }
        else
        {
            Debug.Log("Pas assez de pièces pour acheter " + itemToSell.name + " !");
        }
    }
}