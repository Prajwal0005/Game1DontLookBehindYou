using UnityEngine;
using DontLookBehindYou.Managers;

namespace DontLookBehindYou.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        public AudioSource musicSource;
        public AudioSource sfxSource;
        public AudioSource ambientSource;
        
        [Header("Dynamic Sources (Tied to Player)")]
        public AudioSource heartbeatSource;
        public AudioSource breathingSource;

        [Header("Clips")]
        public AudioClip safeRoomMusic;
        public AudioClip explorationAmbient;
        public AudioClip chaseMusic;
        public AudioClip heartbeatClip;
        public AudioClip heavyBreathingClip;

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
                EventManager.Instance.OnStressLevelChanged += HandleStressLevelChanged;
                EventManager.Instance.OnSafeRoomStateChanged += HandleSafeRoomStateChanged;
            }

            PlayAmbient(explorationAmbient);
        }

        private void OnDestroy()
        {
            if (EventManager.Instance != null)
            {
                EventManager.Instance.OnStressLevelChanged -= HandleStressLevelChanged;
                EventManager.Instance.OnSafeRoomStateChanged -= HandleSafeRoomStateChanged;
            }
        }

        public void PlaySFX(AudioClip clip, float volume = 1f)
        {
            if (sfxSource != null && clip != null)
            {
                sfxSource.PlayOneShot(clip, volume);
            }
        }

        public void PlayMusic(AudioClip clip)
        {
            if (musicSource != null && musicSource.clip != clip)
            {
                musicSource.clip = clip;
                musicSource.Play();
            }
        }

        public void PlayAmbient(AudioClip clip)
        {
            if (ambientSource != null && ambientSource.clip != clip)
            {
                ambientSource.clip = clip;
                ambientSource.Play();
            }
        }

        private void HandleSafeRoomStateChanged(bool isInside)
        {
            if (isInside)
            {
                PlayMusic(safeRoomMusic);
                if (ambientSource != null) ambientSource.volume = 0.2f; // Lower ambient sounds
            }
            else
            {
                musicSource.Stop(); // Stop safe room music
                if (ambientSource != null) ambientSource.volume = 1.0f; // Restore ambient sounds
            }
        }

        private void HandleStressLevelChanged(float currentStress)
        {
            // Trigger heavy breathing and heartbeat if stress is high
            
            if (heartbeatSource != null)
            {
                if (currentStress > 50f)
                {
                    if (!heartbeatSource.isPlaying)
                    {
                        heartbeatSource.clip = heartbeatClip;
                        heartbeatSource.Play();
                    }
                    // Increase pitch/volume based on stress
                    heartbeatSource.volume = Mathf.Lerp(0f, 1f, (currentStress - 50f) / 50f);
                    heartbeatSource.pitch = Mathf.Lerp(1f, 1.5f, (currentStress - 50f) / 50f);
                }
                else
                {
                    heartbeatSource.Stop();
                }
            }

            if (breathingSource != null)
            {
                if (currentStress > 70f)
                {
                    if (!breathingSource.isPlaying)
                    {
                        breathingSource.clip = heavyBreathingClip;
                        breathingSource.Play();
                    }
                    breathingSource.volume = Mathf.Lerp(0f, 1f, (currentStress - 70f) / 30f);
                }
                else
                {
                    breathingSource.Stop();
                }
            }
        }
    }
}
