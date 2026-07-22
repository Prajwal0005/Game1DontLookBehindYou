using UnityEngine;
using DontLookBehindYou.Managers;

namespace DontLookBehindYou.Player
{
    public class PlayerStatsManager : MonoBehaviour
    {
        [Header("Health")]
        public float maxHealth = 100f;
        public float currentHealth;

        [Header("Stamina")]
        public float maxStamina = 100f;
        public float currentStamina;
        public float staminaDrainRate = 10f;
        public float staminaRegenRate = 5f;

        [Header("Stress")]
        public float maxStress = 100f;
        public float currentStress;
        public float passiveStressIncreaseRate = 1f; // In darkness or just naturally
        public float safeRoomStressDecreaseRate = 5f;
        
        [Header("State")]
        public bool isInSafeRoom = false;
        public bool isRunning = false;

        private void Start()
        {
            currentHealth = maxHealth;
            currentStamina = maxStamina;
            currentStress = 0f;
            
            if (EventManager.Instance != null)
            {
                EventManager.Instance.OnSafeRoomStateChanged += HandleSafeRoomState;
            }
        }

        private void OnDestroy()
        {
            if (EventManager.Instance != null)
            {
                EventManager.Instance.OnSafeRoomStateChanged -= HandleSafeRoomState;
            }
        }

        private void Update()
        {
            if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing)
                return;

            HandleStamina();
            HandleStress();
        }

        private void HandleStamina()
        {
            if (isRunning)
            {
                currentStamina -= staminaDrainRate * Time.deltaTime;
                if (currentStamina <= 0)
                {
                    currentStamina = 0;
                    isRunning = false; // Force stop running
                    // TODO: Notify PlayerController that we can no longer run
                }
            }
            else
            {
                currentStamina += staminaRegenRate * Time.deltaTime;
                if (currentStamina > maxStamina)
                {
                    currentStamina = maxStamina;
                }
            }
        }

        private void HandleStress()
        {
            if (isInSafeRoom)
            {
                currentStress -= safeRoomStressDecreaseRate * Time.deltaTime;
            }
            else
            {
                currentStress += passiveStressIncreaseRate * Time.deltaTime;
            }

            currentStress = Mathf.Clamp(currentStress, 0, maxStress);
            
            // Notify other systems (like Audio and Camera Effects) that stress changed
            EventManager.Instance.TriggerStressLevelChanged(currentStress);

            // TODO: Apply visual/audio effects based on currentStress (blur, heartbeat, heavy breathing)
        }

        public void TakeDamage(float amount)
        {
            currentHealth -= amount;
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                EventManager.Instance.TriggerGameOver();
            }
            EventManager.Instance.TriggerHealthChanged(currentHealth);
        }

        public void AddStress(float amount)
        {
            if (!isInSafeRoom)
            {
                currentStress += amount;
                currentStress = Mathf.Clamp(currentStress, 0, maxStress);
                EventManager.Instance.TriggerStressLevelChanged(currentStress);
            }
        }

        private void HandleSafeRoomState(bool isInside)
        {
            isInSafeRoom = isInside;
        }
    }
}
