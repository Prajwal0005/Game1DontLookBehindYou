using System;
using UnityEngine;

namespace DontLookBehindYou.Managers
{
    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        GameOver
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public GameState CurrentState { get; private set; }

        public event Action<GameState> OnGameStateChanged;

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
            ChangeState(GameState.Playing); // Default to playing for testing scenes
        }

        public void ChangeState(GameState newState)
        {
            if (CurrentState == newState) return;

            CurrentState = newState;
            
            // Handle time scale and cursor state based on game state
            switch (CurrentState)
            {
                case GameState.Playing:
                    Time.timeScale = 1f;
                    // Usually we'd lock the cursor, but this is a mobile game. 
                    // However, we might want to lock it for editor testing.
                    #if UNITY_EDITOR
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    #endif
                    break;
                case GameState.Paused:
                case GameState.MainMenu:
                case GameState.GameOver:
                    Time.timeScale = 0f;
                    #if UNITY_EDITOR
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    #endif
                    break;
            }

            OnGameStateChanged?.Invoke(CurrentState);
            Debug.Log($"[GameManager] State changed to: {CurrentState}");
        }

        public void PauseGame()
        {
            if (CurrentState == GameState.Playing)
            {
                ChangeState(GameState.Paused);
            }
        }

        public void ResumeGame()
        {
            if (CurrentState == GameState.Paused)
            {
                ChangeState(GameState.Playing);
            }
        }
    }
}
