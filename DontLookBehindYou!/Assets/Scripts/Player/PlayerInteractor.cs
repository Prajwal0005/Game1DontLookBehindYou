using UnityEngine;
using DontLookBehindYou.Interactables;

namespace DontLookBehindYou.Player
{
    public class PlayerInteractor : MonoBehaviour
    {
        [Header("Settings")]
        public Transform cameraTransform;
        public float interactionDistance = 3.0f;
        public LayerMask interactableLayerMask;

        private IInteractable currentInteractable;

        private void Update()
        {
            if (cameraTransform == null) return;

            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactionDistance, interactableLayerMask))
            {
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                
                if (interactable != null)
                {
                    if (currentInteractable != interactable)
                    {
                        currentInteractable = interactable;
                        // TODO: Update UI with currentInteractable.GetInteractText()
                    }
                }
                else
                {
                    ClearInteractable();
                }
            }
            else
            {
                ClearInteractable();
            }

            // For testing in editor with E key
            #if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.E))
            {
                Interact();
            }
            #endif
        }

        private void ClearInteractable()
        {
            if (currentInteractable != null)
            {
                currentInteractable = null;
                // TODO: Hide interaction UI
            }
        }

        public void Interact()
        {
            if (currentInteractable != null)
            {
                currentInteractable.Interact();
            }
        }
    }
}
