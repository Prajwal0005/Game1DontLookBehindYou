using UnityEngine;
using DontLookBehindYou.Core;

namespace DontLookBehindYou.Interactables
{
    public class PickupItem : MonoBehaviour, IInteractable
    {
        [Header("Item Settings")]
        public ItemData itemData;
        
        [Header("Specific Settings")]
        public bool isBattery = false;
        public float batteryAmount = 25f;

        public string GetInteractText()
        {
            if (itemData != null)
            {
                return $"Pick up {itemData.itemName}";
            }
            return "Pick up";
        }

        public void Interact()
        {
            if (isBattery)
            {
                // Directly add battery to flashlight
                Player.FlashlightSystem flashlight = FindObjectOfType<Player.FlashlightSystem>();
                if (flashlight != null)
                {
                    flashlight.AddBattery(batteryAmount);
                    Debug.Log($"[Pickup] Picked up battery. Added {batteryAmount}.");
                }
            }
            else if (itemData != null && Managers.InventoryManager.Instance != null)
            {
                Managers.InventoryManager.Instance.AddItem(itemData);
            }

            // Destroy the object in the world after picking it up
            Destroy(gameObject);
            // TODO: Play pickup sound
        }
    }
}
