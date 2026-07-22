using UnityEngine;
using DontLookBehindYou.Managers;

namespace DontLookBehindYou.Player
{
    public class FlashlightSystem : MonoBehaviour
    {
        [Header("References")]
        public Light spotlight;
        
        [Header("Settings")]
        public bool isOn = false;
        public float maxBattery = 100f;
        public float currentBattery;
        public float batteryDrainRate = 1.0f; // battery per second
        
        [Header("Visuals")]
        public float maxIntensity = 2.0f;
        public float minIntensity = 0.5f;
        public AnimationCurve intensityDropCurve;

        [Header("Flickering")]
        public bool isFlickering = false;
        public float flickerMinTime = 0.05f;
        public float flickerMaxTime = 0.2f;

        private float flickerTimer;
        private bool flickerState;

        private void Start()
        {
            currentBattery = maxBattery;
            UpdateLightState();
        }

        private void Update()
        {
            if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing)
                return;

            if (isOn)
            {
                DrainBattery();
                HandleFlickering();
                UpdateLightIntensity();
            }
        }

        public void ToggleFlashlight()
        {
            isOn = !isOn;
            
            // Cannot turn on if battery is dead
            if (isOn && currentBattery <= 0)
            {
                isOn = false;
            }

            UpdateLightState();
            // TODO: Play click sound here
        }

        private void DrainBattery()
        {
            if (currentBattery > 0)
            {
                currentBattery -= batteryDrainRate * Time.deltaTime;
                if (currentBattery <= 0)
                {
                    currentBattery = 0;
                    isOn = false;
                    UpdateLightState();
                }
            }
        }

        private void UpdateLightIntensity()
        {
            if (spotlight == null) return;
            if (isFlickering) return; // Flickering overrides normal intensity calculation

            float batteryPercentage = currentBattery / maxBattery;
            float evaluatedIntensity = minIntensity + (maxIntensity - minIntensity) * intensityDropCurve.Evaluate(batteryPercentage);
            
            // If battery is really low, make it flicker naturally
            if (batteryPercentage < 0.1f)
            {
                if (Random.value > 0.95f)
                {
                    spotlight.intensity = Random.Range(0.1f, evaluatedIntensity);
                    return;
                }
            }
            
            spotlight.intensity = evaluatedIntensity;
        }

        private void HandleFlickering()
        {
            if (!isFlickering || spotlight == null) return;

            flickerTimer -= Time.deltaTime;
            if (flickerTimer <= 0)
            {
                flickerState = !flickerState;
                spotlight.enabled = flickerState;
                flickerTimer = Random.Range(flickerMinTime, flickerMaxTime);
            }
        }

        public void StartFlickerEvent()
        {
            isFlickering = true;
            flickerTimer = Random.Range(flickerMinTime, flickerMaxTime);
        }

        public void StopFlickerEvent()
        {
            isFlickering = false;
            UpdateLightState();
        }

        private void UpdateLightState()
        {
            if (spotlight != null)
            {
                spotlight.enabled = isOn;
                if (isOn)
                {
                    UpdateLightIntensity();
                }
            }
        }

        public void AddBattery(float amount)
        {
            currentBattery = Mathf.Clamp(currentBattery + amount, 0, maxBattery);
            // TODO: Play reload sound here
        }
    }
}
