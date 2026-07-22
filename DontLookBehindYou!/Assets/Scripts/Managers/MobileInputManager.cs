using UnityEngine;
using DontLookBehindYou.Player;
using DontLookBehindYou.UI;

namespace DontLookBehindYou.Managers
{
    public class MobileInputManager : MonoBehaviour
    {
        [Header("References")]
        public PlayerController playerController;
        public VirtualJoystick movementJoystick;
        public SwipeArea cameraSwipeArea;

        [Header("Settings")]
        public float swipeSensitivity = 0.5f;

        private void Update()
        {
            if (playerController == null) return;
            if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing) return;

            // Movement Input
            if (movementJoystick != null)
            {
                playerController.SetMoveInput(movementJoystick.InputValue);
            }

            // Camera Look Input
            Vector2 finalLookInput = Vector2.zero;

            if (cameraSwipeArea != null)
            {
                finalLookInput += cameraSwipeArea.SwipeDelta * swipeSensitivity;
                cameraSwipeArea.GetType().GetProperty("SwipeDelta").SetValue(cameraSwipeArea, Vector2.zero);
            }

            if (GyroManager.Instance != null && GyroManager.Instance.isGyroEnabled)
            {
                finalLookInput += GyroManager.Instance.GyroDelta;
            }

            playerController.SetLookInput(finalLookInput);
        }
        
        // --- Button Callbacks ---
        
        public void OnRunButtonPressed()
        {
            if (playerController != null) playerController.SetRun(true);
        }

        public void OnRunButtonReleased()
        {
            if (playerController != null) playerController.SetRun(false);
        }

        public void OnCrouchButtonToggled(bool isCrouched)
        {
            if (playerController != null) playerController.SetCrouch(isCrouched);
        }

        public void OnInteractButtonPressed()
        {
            PlayerInteractor interactor = playerController?.GetComponent<PlayerInteractor>();
            if (interactor != null)
            {
                interactor.Interact();
            }
        }
    }
}
