using UnityEngine;
using UnityEngine.SceneManagement;
using DontLookBehindYou.Core;

namespace DontLookBehindYou.Managers
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance { get; private set; }

        [Header("Objectives Tracker")]
        public int totalPuzzlesInLevel = 3;
        private int puzzlesSolved = 0;

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
            if (EventManager.Instance != null)
            {
                EventManager.Instance.OnPuzzleSolved += HandlePuzzleSolved;
                EventManager.Instance.OnGameOver += HandleGameOver;
            }
        }

        private void OnDestroy()
        {
            if (EventManager.Instance != null)
            {
                EventManager.Instance.OnPuzzleSolved -= HandlePuzzleSolved;
                EventManager.Instance.OnGameOver -= HandleGameOver;
            }
        }

        private void HandlePuzzleSolved(string puzzleId)
        {
            puzzlesSolved++;
            Debug.Log($"[LevelManager] Puzzle Solved: {puzzleId}. Progress: {puzzlesSolved}/{totalPuzzlesInLevel}");

            if (puzzlesSolved >= totalPuzzlesInLevel)
            {
                Debug.Log("[LevelManager] All puzzles solved! Exit is open.");
                // TODO: Open exit door or trigger final sequence
            }
        }

        private void HandleGameOver()
        {
            Debug.Log("[LevelManager] Game Over! Loading Game Over Screen...");
            // Load Game Over Scene or show UI
            // SceneManager.LoadScene("GameOverScene");
            GameManager.Instance.ChangeState(GameState.GameOver);
        }

        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        public void ReloadCurrentScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
