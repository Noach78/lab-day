using System.Collections.Generic;
using UnityEngine;

namespace InventoryFramework
{
    [System.Serializable]
    public class SlotSaveData
    {
        public int itemID;
        public int count;
    }

    [System.Serializable]
    public class InventorySaveData
    {
        public List<SlotSaveData> inventorySlots = new List<SlotSaveData>();
        public List<SlotSaveData> hotbarSlots = new List<SlotSaveData>();
    }

    public class SaveManager : MonoBehaviour
    {
        public Inventory inventory;
        public Hotbar hotbar;
        public ItemDatabase database;

        private void Start()
        {
            LoadGame();
        }

        private void OnApplicationQuit()
        {
            SaveGame();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus) SaveGame();
        }

        public void SaveGame()
        {
            InventorySaveData data = new InventorySaveData();

            foreach (var slot in inventory.slots)
            {
                if (!slot.IsEmpty)
                    data.inventorySlots.Add(new SlotSaveData { itemID = slot.item.id, count = slot.count });
                else
                    data.inventorySlots.Add(new SlotSaveData { itemID = -1, count = 0 });
            }

            foreach (var slot in hotbar.slots)
            {
                if (!slot.IsEmpty)
                    data.hotbarSlots.Add(new SlotSaveData { itemID = slot.item.id, count = slot.count });
                else
                    data.hotbarSlots.Add(new SlotSaveData { itemID = -1, count = 0 });
            }

            string json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString("PlayerInventory", json);
            PlayerPrefs.Save();

            Debug.Log("Sauvegarde automatique effectuée.");
        }

        public void LoadGame()
        {
            if (PlayerPrefs.HasKey("PlayerInventory"))
            {
                string json = PlayerPrefs.GetString("PlayerInventory");
                InventorySaveData data = JsonUtility.FromJson<InventorySaveData>(json);

                for (int i = 0; i < inventory.slots.Count; i++)
                {
                    if (i < data.inventorySlots.Count)
                    {
                        int id = data.inventorySlots[i].itemID;
                        inventory.slots[i].item = (id == -1) ? null : database.GetItemByID(id);
                        inventory.slots[i].count = data.inventorySlots[i].count;
                    }
                }

                for (int i = 0; i < hotbar.slots.Count; i++)
                {
                    if (i < data.hotbarSlots.Count)
                    {
                        int id = data.hotbarSlots[i].itemID;
                        hotbar.slots[i].item = (id == -1) ? null : database.GetItemByID(id);
                        hotbar.slots[i].count = data.hotbarSlots[i].count;
                    }
                }

                FindAnyObjectByType<InventoryUI>()?.RefreshUI();
                FindAnyObjectByType<HotbarUI>()?.RefreshUI();

                Debug.Log("Chargement automatique effectué.");
            }
        }
    }
}