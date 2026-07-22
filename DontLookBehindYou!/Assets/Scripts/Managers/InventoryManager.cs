using System.Collections.Generic;
using UnityEngine;
using DontLookBehindYou.Core;

namespace DontLookBehindYou.Managers
{
    public class InventoryManager : MonoBehaviour
    {
        public static InventoryManager Instance { get; private set; }

        public List<ItemData> inventoryItems = new List<ItemData>();
        
        // A dictionary to quickly look up all available items in the game by ID
        // In a real project, this might be populated by Resources.LoadAll or an Addressables catalog
        public List<ItemData> allItemDatabase = new List<ItemData>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void AddItem(ItemData item)
        {
            if (item != null)
            {
                inventoryItems.Add(item);
                EventManager.Instance.TriggerItemCollected(item.id);
                Debug.Log($"[Inventory] Added item: {item.itemName}");
            }
        }

        public void RemoveItem(ItemData item)
        {
            if (inventoryItems.Contains(item))
            {
                inventoryItems.Remove(item);
                Debug.Log($"[Inventory] Removed item: {item.itemName}");
            }
        }

        public bool HasItem(string itemId)
        {
            return inventoryItems.Exists(i => i.id == itemId);
        }

        // Used when loading from save file
        public void LoadInventoryFromSave(List<string> savedItemIDs)
        {
            inventoryItems.Clear();
            foreach (string id in savedItemIDs)
            {
                ItemData item = allItemDatabase.Find(i => i.id == id);
                if (item != null)
                {
                    inventoryItems.Add(item);
                }
            }
        }
    }
}
