using UnityEngine;
using DontLookBehindYou.Player;
using DontLookBehindYou.Audio;

namespace DontLookBehindYou.Managers
{
    public class PsychologicalEventsManager : MonoBehaviour
    {
        public static PsychologicalEventsManager Instance { get; private set; }

        [Header("Settings")]
        public float minTimeBetweenEvents = 30f;
        public float maxTimeBetweenEvents = 90f;
        
        [Header("Audio Events")]
        public AudioClip[] creepyWhispers;
        public AudioClip[] distantFootsteps;
        
        private float eventTimer;
        private bool isInSafeRoom;

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
            ResetTimer();
            if (EventManager.Instance != null)
            {
                EventManager.Instance.OnSafeRoomStateChanged += HandleSafeRoomStateChanged;
            }
        }

        private void OnDestroy()
        {
            if (EventManager.Instance != null)
            {
                EventManager.Instance.OnSafeRoomStateChanged -= HandleSafeRoomStateChanged;
            }
        }

        private void Update()
        {
            if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing) return;
            if (isInSafeRoom) return;

            eventTimer -= Time.deltaTime;
            
            if (eventTimer <= 0)
            {
                TriggerRandomEvent();
                ResetTimer();
            }
        }

        private void ResetTimer()
        {
            eventTimer = Random.Range(minTimeBetweenEvents, maxTimeBetweenEvents);
        }

        private void HandleSafeRoomStateChanged(bool isInside)
        {
            isInSafeRoom = isInside;
            if (isInside)
            {
                // Stop ongoing events if needed
            }
            else
            {
                ResetTimer();
            }
        }

        private void TriggerRandomEvent()
        {
            int eventType = Random.Range(0, 3);
            
            switch (eventType)
            {
                case 0:
                    TriggerFlashlightFlicker();
                    break;
                case 1:
                    PlayCreepyAudio(creepyWhispers);
                    break;
                case 2:
                    PlayCreepyAudio(distantFootsteps);
                    break;
            }
        }

        private void TriggerFlashlightFlicker()
        {
            FlashlightSystem flashlight = FindObjectOfType<FlashlightSystem>();
            if (flashlight != null && flashlight.isOn)
            {
                flashlight.StartFlickerEvent();
                Debug.Log("[PsychoEvents] Flashlight is flickering...");
                
                // Stop it after a few seconds
                Invoke(nameof(StopFlashlightFlicker), Random.Range(2f, 5f));
            }
        }

        private void StopFlashlightFlicker()
        {
            FlashlightSystem flashlight = FindObjectOfType<FlashlightSystem>();
            if (flashlight != null)
            {
                flashlight.StopFlickerEvent();
            }
        }

        private void PlayCreepyAudio(AudioClip[] clips)
        {
            if (clips == null || clips.Length == 0) return;
            
            AudioClip clipToPlay = clips[Random.Range(0, clips.Length)];
            AudioManager.Instance.PlaySFX(clipToPlay, 0.5f);
            Debug.Log($"[PsychoEvents] Playing creepy audio: {clipToPlay.name}");
        }
    }
}
