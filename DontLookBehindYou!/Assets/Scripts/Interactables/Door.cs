using UnityEngine;

namespace DontLookBehindYou.Interactables
{
    public class Door : MonoBehaviour, IInteractable
    {
        [Header("Settings")]
        public bool isOpen = false;
        public bool isLocked = false;
        public string requiredKeyId = "";
        
        [Header("Animation (Simple Rotation for now)")]
        public float openAngle = 90f;
        public float rotationSpeed = 2f;
        
        private Quaternion closedRotation;
        private Quaternion openRotation;

        private void Start()
        {
            closedRotation = transform.rotation;
            openRotation = Quaternion.Euler(transform.eulerAngles + Vector3.up * openAngle);
            
            if (isOpen)
            {
                transform.rotation = openRotation;
            }
        }

        private void Update()
        {
            Quaternion targetRotation = isOpen ? openRotation : closedRotation;
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

        public string GetInteractText()
        {
            if (isLocked)
            {
                return "Locked";
            }
            return isOpen ? "Close Door" : "Open Door";
        }

        public void Interact()
        {
            if (isLocked)
            {
                // Check if player has the key
                if (Managers.InventoryManager.Instance != null && Managers.InventoryManager.Instance.HasItem(requiredKeyId))
                {
                    isLocked = false;
                    isOpen = true;
                    Debug.Log($"[Door] Unlocked with {requiredKeyId}");
                    // TODO: Play unlock sound
                }
                else
                {
                    Debug.Log("[Door] Door is locked.");
                    // TODO: Play locked rattle sound
                }
            }
            else
            {
                isOpen = !isOpen;
                // TODO: Play open/close sound
            }
        }
    }
}
