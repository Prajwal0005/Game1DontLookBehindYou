using UnityEngine;
using UnityEngine.UI;
using DontLookBehindYou.Core;

namespace DontLookBehindYou.Managers
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("UI Panels")]
        public GameObject mainMenuPanel;
        public GameObject gameplayHUD;
        public GameObject pauseMenuPanel;
        public GameObject settingsMenuPanel;
        public GameObject gameOverPanel;

        [Header("UI Elements")]
        public Slider volumeSlider;
        public Toggle gyroToggle;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
                // Initialize UI based on current state
                HandleGameStateChanged(GameManager.Instance.CurrentState);
            }

            // Initialize Settings UI values
            if (SettingsManager.Instance != null)
            {
                if (volumeSlider != null) volumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1.0f);
                if (gyroToggle != null) gyroToggle.isOn = PlayerPrefs.GetInt("GyroEnabled", 1) == 1;
            }
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
            }
        }

        private void HandleGameStateChanged(GameState state)
        {
            HideAllPanels();

            switch (state)
            {
                case GameState.MainMenu:
                    if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
                    break;
                case GameState.Playing:
                    if (gameplayHUD != null) gameplayHUD.SetActive(true);
                    break;
                case GameState.Paused:
                    if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
                    break;
                case GameState.GameOver:
                    if (gameOverPanel != null) gameOverPanel.SetActive(true);
                    break;
            }
        }

        private void HideAllPanels()
        {
            if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
            if (gameplayHUD != null) gameplayHUD.SetActive(false);
            if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
            if (settingsMenuPanel != null) settingsMenuPanel.SetActive(false);
            if (gameOverPanel != null) gameOverPanel.SetActive(false);
        }

        // --- UI Button Callbacks ---

        public void OnPlayButtonClicked()
        {
            // Usually load the game scene here, then change state
            GameManager.Instance.ChangeState(GameState.Playing);
        }

        public void OnPauseButtonClicked()
        {
            GameManager.Instance.PauseGame();
        }

        public void OnResumeButtonClicked()
        {
            GameManager.Instance.ResumeGame();
        }

        public void OnSettingsButtonClicked()
        {
            HideAllPanels();
            if (settingsMenuPanel != null) settingsMenuPanel.SetActive(true);
        }

        public void OnSettingsBackButtonClicked()
        {
            // Return to previous state panel
            HandleGameStateChanged(GameManager.Instance.CurrentState);
        }

        public void OnMainMenuButtonClicked()
        {
            GameManager.Instance.ChangeState(GameState.MainMenu);
            // Typically load main menu scene here
        }

        public void OnQuitButtonClicked()
        {
            Application.Quit();
        }

        // --- Settings Callbacks ---

        public void OnVolumeChanged(float value)
        {
            if (SettingsManager.Instance != null)
            {
                SettingsManager.Instance.SetMasterVolume(value);
            }
        }

        public void OnGyroToggled(bool isOn)
        {
            if (SettingsManager.Instance != null)
            {
                SettingsManager.Instance.SetGyroEnabled(isOn);
            }
        }
    }
}
