using UnityEngine;
using UnityEngine.AI;
using DontLookBehindYou.Managers;

namespace DontLookBehindYou.AI
{
    public enum MonsterState
    {
        Idle,
        Patrol,
        Listening,
        Investigating,
        Hunting,
        Chasing,
        Disappearing
    }

    [RequireComponent(typeof(NavMeshAgent))]
    public class MonsterAI : MonoBehaviour
    {
        [Header("References")]
        public Transform playerTransform;
        
        [Header("Settings")]
        public float walkSpeed = 2f;
        public float runSpeed = 5f;
        public float sightDistance = 15f;
        public float attackDistance = 1.5f;
        public LayerMask obstacleMask;

        public MonsterState currentState = MonsterState.Idle;

        private NavMeshAgent agent;
        private Vector3 currentInvestigationTarget;
        private float stateTimer;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
        }

        private void Start()
        {
            ChangeState(MonsterState.Patrol);
            
            if (EventManager.Instance != null)
            {
                EventManager.Instance.OnPlayerLookedBehind += HandleLookBehindEvent;
            }
        }

        private void OnDestroy()
        {
            if (EventManager.Instance != null)
            {
                EventManager.Instance.OnPlayerLookedBehind -= HandleLookBehindEvent;
            }
        }

        private void Update()
        {
            if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing)
            {
                agent.isStopped = true;
                return;
            }
            
            agent.isStopped = false;

            switch (currentState)
            {
                case MonsterState.Idle:
                    HandleIdle();
                    break;
                case MonsterState.Patrol:
                    HandlePatrol();
                    break;
                case MonsterState.Listening:
                    HandleListening();
                    break;
                case MonsterState.Investigating:
                    HandleInvestigating();
                    break;
                case MonsterState.Hunting:
                    HandleHunting();
                    break;
                case MonsterState.Chasing:
                    HandleChasing();
                    break;
                case MonsterState.Disappearing:
                    HandleDisappearing();
                    break;
            }

            CheckPlayerVisibility();
        }

        public void ChangeState(MonsterState newState)
        {
            currentState = newState;
            stateTimer = 0f;
            
            switch (newState)
            {
                case MonsterState.Idle:
                case MonsterState.Listening:
                case MonsterState.Disappearing:
                    agent.isStopped = true;
                    break;
                case MonsterState.Patrol:
                case MonsterState.Investigating:
                case MonsterState.Hunting:
                    agent.isStopped = false;
                    agent.speed = walkSpeed;
                    break;
                case MonsterState.Chasing:
                    agent.isStopped = false;
                    agent.speed = runSpeed;
                    break;
            }
            
            Debug.Log($"[MonsterAI] State changed to {newState}");
        }

        private void HandleIdle()
        {
            stateTimer += Time.deltaTime;
            if (stateTimer > Random.Range(2f, 5f))
            {
                ChangeState(MonsterState.Patrol);
            }
        }

        private void HandlePatrol()
        {
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                Vector3 randomDirection = Random.insideUnitSphere * 10f;
                randomDirection += transform.position;
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomDirection, out hit, 10f, 1))
                {
                    agent.SetDestination(hit.position);
                }
            }
        }

        private void HandleListening()
        {
            stateTimer += Time.deltaTime;
            if (stateTimer > 3f)
            {
                ChangeState(MonsterState.Patrol);
            }
        }

        private void HandleInvestigating()
        {
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                stateTimer += Time.deltaTime;
                if (stateTimer > 3f)
                {
                    ChangeState(MonsterState.Patrol);
                }
            }
        }

        private void HandleHunting()
        {
            // Similar to investigate but stalks the player's general area
            if (playerTransform != null)
            {
                agent.SetDestination(playerTransform.position);
            }
        }

        private void HandleChasing()
        {
            if (playerTransform != null)
            {
                agent.SetDestination(playerTransform.position);
                if (Vector3.Distance(transform.position, playerTransform.position) <= attackDistance)
                {
                    // Attack
                    PlayerStatsManager stats = playerTransform.GetComponent<PlayerStatsManager>();
                    if (stats != null && !stats.isInSafeRoom)
                    {
                        stats.TakeDamage(100f);
                    }
                }
            }
        }

        private void HandleDisappearing()
        {
            // Monster teleports away or fades out
            gameObject.SetActive(false);
        }

        private void CheckPlayerVisibility()
        {
            if (playerTransform == null) return;

            float distance = Vector3.Distance(transform.position, playerTransform.position);
            if (distance < sightDistance)
            {
                Vector3 dirToPlayer = (playerTransform.position - transform.position).normalized;
                
                if (Physics.Raycast(transform.position + Vector3.up, dirToPlayer, distance, obstacleMask))
                {
                    // Obstacle in the way
                    if (currentState == MonsterState.Chasing)
                    {
                        ChangeState(MonsterState.Hunting);
                    }
                }
                else
                {
                    // Can see player
                    if (currentState != MonsterState.Chasing && currentState != MonsterState.Disappearing)
                    {
                        PlayerStatsManager stats = playerTransform.GetComponent<PlayerStatsManager>();
                        if (stats == null || !stats.isInSafeRoom)
                        {
                            ChangeState(MonsterState.Chasing);
                        }
                    }
                }
            }
            else if (currentState == MonsterState.Chasing)
            {
                ChangeState(MonsterState.Hunting);
            }
        }

        public void InvestigateSound(Vector3 position)
        {
            if (currentState != MonsterState.Chasing && currentState != MonsterState.Disappearing)
            {
                currentInvestigationTarget = position;
                agent.SetDestination(position);
                ChangeState(MonsterState.Investigating);
            }
        }

        // Hooked up to EventManager
        private void HandleLookBehindEvent()
        {
            // The signature mechanic logic
            if (currentState == MonsterState.Chasing || currentState == MonsterState.Hunting)
            {
                float randomChance = Random.value;
                if (randomChance < 0.3f)
                {
                    ChangeState(MonsterState.Disappearing);
                }
                else if (randomChance < 0.6f)
                {
                    // Freeze completely
                    ChangeState(MonsterState.Listening);
                }
                // Else continue chasing
            }
        }
    }
}
