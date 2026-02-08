using UnityEngine;
using UnityEngine.SceneManagement;

namespace InventoryFramework
{
    public class PickupItem : MonoBehaviour
    {
        public Item item;
        public int amount;

        private static int treasureMapsCollected = 0;

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                ProcessPickup(collision.gameObject);
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                ProcessPickup(other.gameObject);
            }
        }

        void ProcessPickup(GameObject player)
        {
            
            if ((item != null && item.name == "treasure map") || gameObject.name.Contains("treasure map"))
            {
                treasureMapsCollected++;
                Debug.Log("Cartes au trésor trouvées : " + treasureMapsCollected + "/6");

                if (treasureMapsCollected >= 6)
                {
                    treasureMapsCollected = 0;
                    
                    SceneManager.LoadScene("trésor");
                }
            }

            var handler = player.GetComponent<ItemPickupHandler>();
            if (handler != null)
            {
                handler.PickupItem(item, amount);
            }
            
            Destroy(this.gameObject);
        }
    }
}