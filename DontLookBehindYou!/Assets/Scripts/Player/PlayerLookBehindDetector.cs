using UnityEngine;
using DontLookBehindYou.Managers;

namespace DontLookBehindYou.Player
{
    public class PlayerLookBehindDetector : MonoBehaviour
    {
        [Header("Settings")]
        public float turnThresholdDegrees = 140f;
        public float timeWindowSeconds = 1.0f;
        public float cooldownSeconds = 2.0f;

        private float previousYaw;
        private float accumulatedYawDelta;
        private float timeAccumulating;
        private float cooldownTimer;

        private void Start()
        {
            previousYaw = transform.eulerAngles.y;
        }

        private void Update()
        {
            if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing) return;

            if (cooldownTimer > 0)
            {
                cooldownTimer -= Time.deltaTime;
                previousYaw = transform.eulerAngles.y; // Keep track but don't trigger
                return;
            }

            float currentYaw = transform.eulerAngles.y;
            float deltaYaw = Mathf.DeltaAngle(previousYaw, currentYaw);
            previousYaw = currentYaw;

            // Only track fast, continuous movement
            if (Mathf.Abs(deltaYaw) > 1f)
            {
                accumulatedYawDelta += deltaYaw;
                timeAccumulating += Time.deltaTime;

                if (Mathf.Abs(accumulatedYawDelta) >= turnThresholdDegrees)
                {
                    // Trigger Look Behind!
                    TriggerLookBehind();
                }

                if (timeAccumulating > timeWindowSeconds)
                {
                    ResetAccumulator();
                }
            }
            else
            {
                // If they stop turning, reset
                ResetAccumulator();
            }
        }

        private void ResetAccumulator()
        {
            accumulatedYawDelta = 0f;
            timeAccumulating = 0f;
        }

        private void TriggerLookBehind()
        {
            Debug.Log("[PlayerLookBehindDetector] Player looked behind!");
            EventManager.Instance.TriggerPlayerLookedBehind();
            
            // Add a burst of stress
            PlayerStatsManager stats = GetComponent<PlayerStatsManager>();
            if (stats != null)
            {
                stats.AddStress(15f);
            }

            ResetAccumulator();
            cooldownTimer = cooldownSeconds;
        }
    }
}
