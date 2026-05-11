using System.Collections.Generic;
using UnityEngine;

namespace InventoryFramework
{
    // Cette classe représente une case d'inventaire sauvegardable
    [System.Serializable]
    public class ItemSaveData
    {
        public string itemID; // L'ID unique de ton objet (ex: "wood", "sword")
        public int amount;
    }

    // Cette classe englobe toute ta sauvegarde (Inventaire + Hotbar)
    [System.Serializable]
    public class InventorySaveData
    {
        public List<ItemSaveData> inventoryItems = new List<ItemSaveData>();
        public List<ItemSaveData> hotbarItems = new List<ItemSaveData>();
    }
}