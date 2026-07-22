using UnityEngine;
using DontLookBehindYou.Managers;

namespace DontLookBehindYou.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float walkSpeed = 3.0f;
        public float runSpeed = 6.0f;
        public float crouchSpeed = 1.5f;
        public float gravity = -9.81f;
        
        [Header("Look Settings")]
        public Transform cameraTransform;
        public float lookSensitivity = 1.0f;
        public float upDownRange = 80.0f;
        
        [Header("Crouch Settings")]
        public float standingHeight = 2.0f;
        public float crouchingHeight = 1.0f;
        public float crouchTransitionSpeed = 10f;
        
        [Header("Camera Bob Settings")]
        public float bobSpeed = 10f;
        public float bobAmount = 0.05f;

        private CharacterController characterController;
        private Vector2 currentMoveInput;
        private Vector2 currentLookInput;
        
        private float verticalRotation;
        private Vector3 velocity;
        private bool isGrounded;
        private bool isRunning;
        private bool isCrouching;

        private float defaultCameraY;
        private float bobTimer;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            if (cameraTransform != null)
            {
                defaultCameraY = cameraTransform.localPosition.y;
            }
        }

        private void Update()
        {
            if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing)
                return;

            CheckGrounded();
            HandleMovement();
            HandleLook();
            HandleCrouch();
            HandleCameraBob();
        }

        // --- Input Feeders (Called by Mobile UI / New Input System) ---

        public void SetMoveInput(Vector2 input)
        {
            currentMoveInput = input;
        }

        public void SetLookInput(Vector2 input)
        {
            currentLookInput = input;
        }

        public void SetRun(bool run)
        {
            isRunning = run && !isCrouching;
        }

        public void SetCrouch(bool crouch)
        {
            isCrouching = crouch;
            if (isCrouching) isRunning = false;
        }

        // --- Core Logic ---

        private void CheckGrounded()
        {
            isGrounded = characterController.isGrounded;
            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f; // Small downward force to keep grounded
            }
        }

        private void HandleMovement()
        {
            float targetSpeed = isCrouching ? crouchSpeed : (isRunning ? runSpeed : walkSpeed);
            
            // Allow movement using WASD for editor testing if no mobile input is present
            Vector2 input = currentMoveInput;
            #if UNITY_EDITOR
            if (input == Vector2.zero)
            {
                input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
                targetSpeed = walkSpeed;
                if (Input.GetKey(KeyCode.LeftShift)) targetSpeed = runSpeed;
                if (Input.GetKey(KeyCode.C)) targetSpeed = crouchSpeed;
            }
            #endif

            Vector3 move = transform.right * input.x + transform.forward * input.y;
            characterController.Move(move * targetSpeed * Time.deltaTime);

            // Gravity
            velocity.y += gravity * Time.deltaTime;
            characterController.Move(velocity * Time.deltaTime);
        }

        private void HandleLook()
        {
            if (cameraTransform == null) return;

            Vector2 look = currentLookInput;
            
            #if UNITY_EDITOR
            // Allow mouse look for editor testing
            if (look == Vector2.zero && Input.GetMouseButton(1)) // Right click to look in editor
            {
                look = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            }
            #endif

            float mouseX = look.x * lookSensitivity;
            float mouseY = look.y * lookSensitivity;

            verticalRotation -= mouseY;
            verticalRotation = Mathf.Clamp(verticalRotation, -upDownRange, upDownRange);
            
            cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
            transform.Rotate(Vector3.up * mouseX);
        }

        private void HandleCrouch()
        {
            float targetHeight = isCrouching ? crouchingHeight : standingHeight;
            characterController.height = Mathf.Lerp(characterController.height, targetHeight, Time.deltaTime * crouchTransitionSpeed);
            
            // Adjust center
            Vector3 targetCenter = new Vector3(0, targetHeight / 2, 0);
            characterController.center = Vector3.Lerp(characterController.center, targetCenter, Time.deltaTime * crouchTransitionSpeed);
        }

        private void HandleCameraBob()
        {
            if (cameraTransform == null) return;
            if (!isGrounded) return;

            float speed = new Vector3(characterController.velocity.x, 0, characterController.velocity.z).magnitude;
            
            if (speed > 0.1f)
            {
                bobTimer += Time.deltaTime * (isRunning ? bobSpeed * 1.5f : bobSpeed);
                float newY = defaultCameraY + Mathf.Sin(bobTimer) * bobAmount;
                cameraTransform.localPosition = new Vector3(cameraTransform.localPosition.x, newY, cameraTransform.localPosition.z);
            }
            else
            {
                bobTimer = 0;
                float newY = Mathf.Lerp(cameraTransform.localPosition.y, defaultCameraY, Time.deltaTime * bobSpeed);
                cameraTransform.localPosition = new Vector3(cameraTransform.localPosition.x, newY, cameraTransform.localPosition.z);
            }
        }
    }
}
