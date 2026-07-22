using UnityEngine;
using DontLookBehindYou.Player;

namespace DontLookBehindYou.Managers
{
    public class EndingsManager : MonoBehaviour
    {
        public static EndingsManager Instance { get; private set; }

        public enum EndingType
        {
            None,
            BadEnding,    // Killed by monster
            GoodEnding,   // Escaped successfully
            SecretEnding  // Escaped with all documents/secrets found
        }

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
                EventManager.Instance.OnGameOver += TriggerBadEnding;
            }
        }

        private void OnDestroy()
        {
            if (EventManager.Instance != null)
            {
                EventManager.Instance.OnGameOver -= TriggerBadEnding;
            }
        }

        public void TriggerBadEnding()
        {
            Debug.Log("[EndingsManager] BAD ENDING: You were caught by the creature.");
            GameManager.Instance.ChangeState(GameState.GameOver);
            // TODO: Play bad ending cutscene or UI
        }

        public void TriggerEscapeEnding(int collectedSecrets, int totalSecrets)
        {
            if (collectedSecrets >= totalSecrets && totalSecrets > 0)
            {
                Debug.Log("[EndingsManager] SECRET ENDING: You escaped and uncovered the truth.");
                // TODO: Play secret ending cutscene or UI
            }
            else
            {
                Debug.Log("[EndingsManager] GOOD ENDING: You escaped... but questions remain.");
                // TODO: Play good ending cutscene or UI
            }
            
            GameManager.Instance.ChangeState(GameState.GameOver);
        }
    }
}
