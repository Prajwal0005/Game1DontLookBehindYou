using UnityEngine;
using DontLookBehindYou.Managers;

namespace DontLookBehindYou.Interactables
{
    public class FuseBoxPuzzle : MonoBehaviour, IInteractable
    {
        [Header("Settings")]
        public string requiredFuseId = "fuse_01";
        public bool isFixed = false;
        
        [Header("Effects")]
        public Light[] lightsToTurnOn;
        public GameObject sparkEffect;

        public string GetInteractText()
        {
            if (isFixed) return "Power Restored";
            
            if (InventoryManager.Instance != null && InventoryManager.Instance.HasItem(requiredFuseId))
            {
                return "Insert Fuse";
            }
            
            return "Missing Fuse";
        }

        public void Interact()
        {
            if (isFixed) return;

            if (InventoryManager.Instance != null && InventoryManager.Instance.HasItem(requiredFuseId))
            {
                // Fix it
                isFixed = true;
                
                // Turn on lights
                foreach(Light l in lightsToTurnOn)
                {
                    if (l != null) l.enabled = true;
                }

                // Stop sparks
                if (sparkEffect != null) sparkEffect.SetActive(false);

                // Consume the item if needed (we might want to keep it or remove it)
                // Assuming we want to remove the fuse from inventory
                Core.ItemData fuseItem = InventoryManager.Instance.inventoryItems.Find(i => i.id == requiredFuseId);
                if (fuseItem != null)
                {
                    InventoryManager.Instance.RemoveItem(fuseItem);
                }

                if (EventManager.Instance != null)
                {
                    EventManager.Instance.TriggerPuzzleSolved("fuse_box_01");
                }

                Debug.Log("[FuseBox] Power restored!");
                // TODO: Play power on sound
            }
            else
            {
                Debug.Log("[FuseBox] Cannot fix without fuse.");
                // TODO: Play error sound
            }
        }
    }
}
