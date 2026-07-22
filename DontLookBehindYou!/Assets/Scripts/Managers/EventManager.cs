using System;
using UnityEngine;

namespace DontLookBehindYou.Managers
{
    public class EventManager : MonoBehaviour
    {
        public static EventManager Instance { get; private set; }

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

        // --- Gameplay Events ---
        
        // Triggered when player stress changes
        public event Action<float> OnStressLevelChanged;
        public void TriggerStressLevelChanged(float currentStress)
        {
            OnStressLevelChanged?.Invoke(currentStress);
        }

        // Triggered when player health changes
        public event Action<float> OnHealthChanged;
        public void TriggerHealthChanged(float currentHealth)
        {
            OnHealthChanged?.Invoke(currentHealth);
        }

        // Triggered when an item is collected
        public event Action<string> OnItemCollected;
        public void TriggerItemCollected(string itemId)
        {
            OnItemCollected?.Invoke(itemId);
        }

        // Triggered when the player enters or exits a safe room
        public event Action<bool> OnSafeRoomStateChanged;
        public void TriggerSafeRoomStateChanged(bool isInside)
        {
            OnSafeRoomStateChanged?.Invoke(isInside);
        }

        // Triggered when a puzzle is solved
        public event Action<string> OnPuzzleSolved;
        public void TriggerPuzzleSolved(string puzzleId)
        {
            OnPuzzleSolved?.Invoke(puzzleId);
        }

        // Triggered when the "Look Behind" mechanic activates the monster
        public event Action OnPlayerLookedBehind;
        public void TriggerPlayerLookedBehind()
        {
            OnPlayerLookedBehind?.Invoke();
        }

        // Triggered on Game Over
        public event Action OnGameOver;
        public void TriggerGameOver()
        {
            OnGameOver?.Invoke();
        }
    }
}
