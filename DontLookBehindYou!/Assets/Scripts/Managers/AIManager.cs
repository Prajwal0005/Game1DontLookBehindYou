using UnityEngine;
using DontLookBehindYou.AI;
using DontLookBehindYou.Player;

namespace DontLookBehindYou.Managers
{
    public class AIManager : MonoBehaviour
    {
        public static AIManager Instance { get; private set; }

        [Header("References")]
        public GameObject monsterPrefab;
        public Transform playerTransform;

        [Header("Spawn Settings")]
        public float spawnDistanceMin = 15f;
        public float spawnDistanceMax = 30f;
        public float baseSpawnInterval = 60f;

        private GameObject currentMonsterInstance;
        private MonsterAI currentMonsterAI;
        private float spawnTimer;

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
            spawnTimer = baseSpawnInterval;
            
            if (EventManager.Instance != null)
            {
                EventManager.Instance.OnSafeRoomStateChanged += HandleSafeRoomState;
            }
        }

        private void OnDestroy()
        {
            if (EventManager.Instance != null)
            {
                EventManager.Instance.OnSafeRoomStateChanged -= HandleSafeRoomState;
            }
        }

        private void Update()
        {
            if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing) return;
            if (playerTransform == null) return;

            PlayerStatsManager playerStats = playerTransform.GetComponent<PlayerStatsManager>();
            if (playerStats != null && playerStats.isInSafeRoom)
            {
                // Never spawn in safe room, despawn if exists
                DespawnMonster();
                return;
            }

            HandleSpawning(playerStats);
        }

        private void HandleSpawning(PlayerStatsManager playerStats)
        {
            if (currentMonsterInstance == null || !currentMonsterInstance.activeInHierarchy)
            {
                // Reduce timer faster if stress is high
                float stressMultiplier = (playerStats != null) ? 1f + (playerStats.currentStress / 100f) : 1f;
                spawnTimer -= Time.deltaTime * stressMultiplier;

                if (spawnTimer <= 0)
                {
                    SpawnMonster();
                }
            }
            else
            {
                // Monster is active. If it's too far away for too long, despawn it
                float distance = Vector3.Distance(playerTransform.position, currentMonsterInstance.transform.position);
                if (distance > spawnDistanceMax * 1.5f)
                {
                    DespawnMonster();
                }
            }
        }

        private void SpawnMonster()
        {
            if (playerTransform == null) return;

            // Pick a random point around the player
            Vector2 randomCircle = Random.insideUnitCircle.normalized * Random.Range(spawnDistanceMin, spawnDistanceMax);
            Vector3 spawnPos = playerTransform.position + new Vector3(randomCircle.x, 0, randomCircle.y);

            // Ensure it spawns out of direct sight (simplistic check for now, can be improved with dot product)
            Vector3 dirToSpawn = (spawnPos - playerTransform.position).normalized;
            if (Vector3.Dot(playerTransform.forward, dirToSpawn) > 0.5f)
            {
                // It's in front of the player, flip it to behind
                spawnPos = playerTransform.position - new Vector3(randomCircle.x, 0, randomCircle.y);
            }

            if (currentMonsterInstance == null)
            {
                currentMonsterInstance = Instantiate(monsterPrefab, spawnPos, Quaternion.identity);
                currentMonsterAI = currentMonsterInstance.GetComponent<MonsterAI>();
                if (currentMonsterAI != null)
                {
                    currentMonsterAI.playerTransform = playerTransform;
                }
            }
            else
            {
                currentMonsterInstance.transform.position = spawnPos;
                currentMonsterInstance.SetActive(true);
            }

            // Sometimes it spawns just to stalk, sometimes it patrols
            if (currentMonsterAI != null)
            {
                MonsterState startState = Random.value > 0.5f ? MonsterState.Patrol : MonsterState.Idle;
                currentMonsterAI.ChangeState(startState);
            }

            Debug.Log("[AIManager] Monster spawned procedurally!");
            
            // Reset timer
            spawnTimer = baseSpawnInterval * Random.Range(0.8f, 1.5f);
        }

        public void DespawnMonster()
        {
            if (currentMonsterInstance != null && currentMonsterInstance.activeInHierarchy)
            {
                if (currentMonsterAI != null)
                {
                    currentMonsterAI.ChangeState(MonsterState.Disappearing);
                }
                else
                {
                    currentMonsterInstance.SetActive(false);
                }
                Debug.Log("[AIManager] Monster despawned.");
            }
        }
    }
}
