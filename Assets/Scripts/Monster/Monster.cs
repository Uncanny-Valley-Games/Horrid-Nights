using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Monster
{
    public class Monster : MonoBehaviour
    {
        private enum MonsterState
        {
            Inactive,
            Lurk,
            Chase,
            Escape
        }

        [Header("References")] 
        [SerializeField] private Transform player;
        [SerializeField] private Transform cabin;

        [Header("Spawn")] 
        [SerializeField] private DayCycleManager.DayState spawnState = DayCycleManager.DayState.Evening;
        [SerializeField] private float spawnDistanceMin = 30f;
        [SerializeField] private float spawnDistanceMax = 60f;

        [Header("Lurk (evening)")] 
        [SerializeField] private float preChaseLurkDuration = 10f; 
        [SerializeField] private float lurkMinDistanceToPlayer = 8f;
        [SerializeField] private float lurkMaxDistanceToPlayer = 20f;
        [SerializeField] private float lurkMoveInterval = 3f;
        [SerializeField] private float lurkSpeed = 2f;

        [Header("Chase (night)")] 
        [SerializeField] private float chaseSpeed = 5f;
        [SerializeField] private float killDistance = 1.5f;

        [Header("Escape")] 
        [SerializeField] private float cabinEnterDistance = 3f;
        [SerializeField] private float escapeDistanceFromCabin = 80f;
        [SerializeField] private float despawnDistance = 120f;
        [SerializeField] private float escapeSpeed = 6f;

        private NavMeshAgent _agent;
        private MonsterState _state = MonsterState.Inactive;
        private float _lurkTimer;
        private bool _spawned;

        void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            if (_agent == null) _agent = gameObject.AddComponent<NavMeshAgent>();
            _agent.updateRotation = true;
            _agent.updateUpAxis = false;

            _agent.enabled = false;
        }

        void OnEnable()
        {
            DayCycleManager.OnDayStateChanged += HandleDayStateChanged;
        }

        void OnDisable()
        {
            DayCycleManager.OnDayStateChanged -= HandleDayStateChanged;
        }

        void Start()
        {
            if (player == null)
            {
                var playerGo = GameObject.FindGameObjectWithTag("Player");
                if (playerGo) player = playerGo.transform;
            }

            if (cabin == null)
            {
                var cabinGo = GameObject.Find("Cabin");
                if (cabinGo) cabin = cabinGo.transform;
            }
        }

        void Update()
        {
            if (!_spawned) return;

            switch (_state)
            {
                case MonsterState.Lurk:
                    break;

                case MonsterState.Chase:
                    if (player)
                    {
                        _agent.destination = player.position;

                        if (Vector3.Distance(transform.position, player.position) <= killDistance)
                        {
                            Debug.Log("OnKilledByMonster");
                            Destroy(gameObject);
                            return;
                        }

                        if (cabin && Vector3.Distance(player.position, cabin.position) <= cabinEnterDistance)
                        {
                            EnterEscape();
                        }
                    }

                    break;

                case MonsterState.Escape:
                    if (cabin && Vector3.Distance(transform.position, cabin.position) >= despawnDistance)
                    {
                        Destroy(gameObject);
                    }

                    break;
            }
        }

        private void HandleDayStateChanged(DayCycleManager.DayState newState)
        {
            if (!_spawned && newState == spawnState)
            {
                SpawnAtRandomAroundCabin();
            }

            if (_spawned && newState == DayCycleManager.DayState.Night && _state == MonsterState.Lurk)
            {
                EnterChase();
            }

            if (_state == MonsterState.Chase && newState == DayCycleManager.DayState.Day)
            {
                EnterEscape();
            }
        }

        private void SpawnAtRandomAroundCabin()
        {
            if (!cabin)
            {
                Debug.LogWarning("Monster: cabin not assigned, cannot spawn properly.");
                return;
            }

            Vector3 spawnPos = GetRandomNavmeshPointAround(cabin.position, spawnDistanceMin, spawnDistanceMax);
            transform.position = spawnPos;
            _agent.enabled = true;
            _spawned = true;
            EnterLurk();
        }

        private void EnterLurk()
        {
            _state = MonsterState.Lurk;
            _agent.speed = lurkSpeed;
            StartCoroutine(LurkCoroutine());
        }

        private IEnumerator LurkCoroutine()
        {
            _lurkTimer = 0f;

            while (_state == MonsterState.Lurk)
            {
                if (player)
                {
                    Vector3 target = ChooseLurkTargetAroundPlayer();
                    _agent.SetDestination(target);
                }

                float elapsed = 0f;
                while (elapsed < lurkMoveInterval)
                {
                    elapsed += Time.deltaTime;
                    _lurkTimer += Time.deltaTime;

                    if (_lurkTimer >= preChaseLurkDuration)
                    {
                        EnterChase();
                        yield break;
                    }

                    yield return null;
                }
            }
        }

        private Vector3 ChooseLurkTargetAroundPlayer()
        {
            if (!player) return transform.position;

            for (int i = 0; i < 12; i++)
            {
                Vector3 dir = Random.insideUnitSphere;
                dir.y = 0f;
                dir.Normalize();
                float dist = Random.Range(lurkMinDistanceToPlayer, lurkMaxDistanceToPlayer);
                Vector3 candidate = player.position + dir * dist;

                if (NavMesh.SamplePosition(candidate, out var hit, 4f, NavMesh.AllAreas))
                {
                    return hit.position;
                }
            }

            if (NavMesh.SamplePosition(transform.position, out var fallback, 5f, NavMesh.AllAreas))
                return fallback.position;

            return transform.position;
        }

        private void EnterChase()
        {
            StopAllCoroutines();
            _state = MonsterState.Chase;
            _agent.speed = chaseSpeed;
            if (player) _agent.SetDestination(player.position);
        }

        private void EnterEscape()
        {
            StopAllCoroutines();
            _state = MonsterState.Escape;
            _agent.speed = escapeSpeed;

            Vector3 runDir;
            if (cabin)
                runDir = (transform.position - cabin.position).normalized;
            else if (player)
                runDir = (transform.position - player.position).normalized;
            else
                runDir = transform.forward;

            Vector3 runTarget = (cabin ? cabin.position : transform.position) + runDir * escapeDistanceFromCabin;

            if (NavMesh.SamplePosition(runTarget, out var hit, 20f, NavMesh.AllAreas))
                _agent.SetDestination(hit.position);
            else
                _agent.SetDestination(transform.position + runDir * escapeDistanceFromCabin);
        }

        private Vector3 GetRandomNavmeshPointAround(Vector3 origin, float minRadius, float maxRadius)
        {
            for (int i = 0; i < 24; i++)
            {
                Vector3 dir = Random.insideUnitSphere;
                dir.y = 0f;
                dir.Normalize();
                float dist = Random.Range(minRadius, maxRadius);
                Vector3 candidate = origin + dir * dist;

                if (NavMesh.SamplePosition(candidate, out var hit, 8f, NavMesh.AllAreas))
                {
                    return hit.position;
                }
            }

            return origin;
        }
    }
}