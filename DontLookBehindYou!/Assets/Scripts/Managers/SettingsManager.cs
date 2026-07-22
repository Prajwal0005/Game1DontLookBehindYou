using UnityEngine;
using DontLookBehindYou.Managers;

namespace DontLookBehindYou.Core
{
    public class SettingsManager : MonoBehaviour
    {
        public static SettingsManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            LoadSettings();
        }

        public void SetMasterVolume(float volume)
        {
            AudioListener.volume = volume;
            PlayerPrefs.SetFloat("MasterVolume", volume);
        }

        public void SetGyroEnabled(bool isEnabled)
        {
            if (GyroManager.Instance != null)
            {
                GyroManager.Instance.ToggleGyro(isEnabled);
            }
            PlayerPrefs.SetInt("GyroEnabled", isEnabled ? 1 : 0);
        }
        
        public void SetGraphicsQuality(int level)
        {
            QualitySettings.SetQualityLevel(level, true);
            PlayerPrefs.SetInt("GraphicsQuality", level);
        }

        private void LoadSettings()
        {
            float vol = PlayerPrefs.GetFloat("MasterVolume", 1.0f);
            SetMasterVolume(vol);

            bool gyro = PlayerPrefs.GetInt("GyroEnabled", 1) == 1;
            SetGyroEnabled(gyro);
            
            int quality = PlayerPrefs.GetInt("GraphicsQuality", QualitySettings.GetQualityLevel());
            SetGraphicsQuality(quality);
        }
    }
}
