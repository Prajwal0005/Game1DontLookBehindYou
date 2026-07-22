using UnityEngine;

namespace DontLookBehindYou.Core
{
    public class OptimizationManager : MonoBehaviour
    {
        [Header("Target Settings")]
        public int targetFrameRate = 60;
        
        [Header("Mobile Optimizations")]
        public bool forceDisableShadowsOnLowQuality = true;

        private void Start()
        {
            // Target 60 FPS for Android
            Application.targetFrameRate = targetFrameRate;

            // Optional: Prevent screen from sleeping while playing
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            
            Debug.Log($"[OptimizationManager] Target framerate set to {Application.targetFrameRate}");
        }

        private void Update()
        {
            if (forceDisableShadowsOnLowQuality)
            {
                if (QualitySettings.GetQualityLevel() == 0) // Assuming 0 is lowest
                {
                    // Optionally disable all shadows if needed for extreme performance
                    // This is better done in URP asset settings, but can be forced here if using built-in
                }
            }
        }
    }
}
