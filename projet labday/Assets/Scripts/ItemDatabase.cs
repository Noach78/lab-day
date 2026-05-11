using System.Collections.Generic;
using UnityEngine;

namespace InventoryFramework
{
    [CreateAssetMenu(fileName = "New Item Database", menuName = "Inventory/Database")]
    public class ItemDatabase : ScriptableObject
    {
        public List<Item> allItems = new List<Item>();

        public Item GetItemByID(int id)
        {
            foreach (Item item in allItems)
            {
                if (item.id == id)
                {
                    return item;
                }
            }
            return null;
        }
    }
}