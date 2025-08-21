using UnityEngine;
using Unity.XR.CoreUtils;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;

    [Header("Spawn distance settings")]
    public float minSpawnDistance = 5f;
    public float maxSpawnDistance = 12f;
    [Header("Arc settings")]                     
    [Range(0f, 180f)]
    public float startingarcAngleDegrees = 90f;

    public float arcGrowthRate = 2f;            
    public float maxArcAngleDegrees = 180f;     

    [Header("Spawn Rate")]
    public float initialSpawnInterval = 0.01f;
    public float minSpawnInterval = 0.5f;
    public float spawnAccelerationPerSecond = 0.5f;
    

    [Header("External references")]
    public XROrigin player;
    [SerializeField]
    private ObjectPoolClass enemyPool;

    private int startingEnemyCount;
    private int maxEnemyCount;
    [SerializeField] private string enemyTag = "EnemyCritical";


    private float currentSpawnInterval;
    private float spawnTimer;
    private float elapsed;

    public Vector3 originStartPosition;
    public Vector3 originStartForward;

    private bool canSpawn = false;

    private void OnEnable()
    {
        GameManager.OnGameStart += HandleGameStart;
        EnemyBehaviour.OnEnemyReachedGoal += HandleEnemyReachedGoal;
    }

    private void OnDisable()
    {
        GameManager.OnGameStart -= HandleGameStart;
        EnemyBehaviour.OnEnemyReachedGoal -= HandleEnemyReachedGoal;
    }

    private void Awake()
    {
        if (enemyPool != null)
        {
            startingEnemyCount = enemyPool.initialPoolSize;
            maxEnemyCount = enemyPool.maxPoolSize;
            enemyPool.GameObjectPool(enemyPrefab, startingEnemyCount, maxEnemyCount);
        }
        if (player == null)
        {
            Debug.LogWarning("SpawnManager: Assign XROrigin 'player' in the Inspector.");
        }
        if (minSpawnDistance > maxSpawnDistance)
        {
            Debug.LogWarning("Minimum distance cant be greater than maximum. Swapping.");
            var tmp = minSpawnDistance;
            minSpawnDistance = maxSpawnDistance;
            maxSpawnDistance = tmp;
        }

        CacheOriginStartPose();
        currentSpawnInterval = Mathf.Max(minSpawnInterval, initialSpawnInterval);
        spawnTimer = 0f;
        elapsed = 0f;
    }

    private void Update()
    {
        if (enemyPrefab == null || enemyPool == null) return;
        if (!canSpawn) return;

        float dt = Time.deltaTime;
        elapsed += dt;
        spawnTimer += dt;

        // Expand the arc with time
        startingarcAngleDegrees = Mathf.Min(maxArcAngleDegrees, startingarcAngleDegrees + arcGrowthRate * dt);

        // Accelerate spawn rate with time
        currentSpawnInterval = Mathf.Max(minSpawnInterval, initialSpawnInterval - elapsed * spawnAccelerationPerSecond);

        if (spawnTimer >= currentSpawnInterval && GetActiveEnemyCount() <maxEnemyCount)
        {
            SpawnEnemyOnArc();
            spawnTimer = 0f;
        }
    }

    private void CacheOriginStartPose()
    {
        if (player != null)
        {
            originStartPosition = player.transform.position;
            originStartForward = Vector3.ProjectOnPlane(player.transform.forward, Vector3.up).normalized;
            if (originStartForward.sqrMagnitude < 0.0001f)
            {
                originStartForward = Vector3.forward;
            }
        }
        else
        {
            originStartPosition = Vector3.zero;
            originStartForward = Vector3.forward;
        }
    }

    private void SpawnEnemyOnArc()
    {
        float halfArc = startingarcAngleDegrees * 0.5f;
        float angle = Random.Range(-halfArc, halfArc);
        Quaternion yaw = Quaternion.AngleAxis(angle, Vector3.up);

        float distance = Random.Range(minSpawnDistance, maxSpawnDistance);
        Vector3 dir = yaw * originStartForward;
        Vector3 spawnPos = originStartPosition + dir * distance;

        Vector3 directionToGoal = (originStartPosition - spawnPos).normalized;
        Quaternion spawnRot = Quaternion.FromToRotation(Vector3.right, directionToGoal);

        GameObject enemy = enemyPool.GetObject(spawnPos, spawnRot);
        var behaviour = enemy.GetComponent<EnemyBehaviour>();
        behaviour.directionToGoal = directionToGoal;


        spawnPos.y = originStartPosition.y;
    }

    private void HandleEnemyReachedGoal(GameObject enemy)
    {
        enemyPool.ReleaseObject(enemy);
    }

    private void HandleGameStart()
    {
        canSpawn = true;
    }

    private int GetActiveEnemyCount()
    {
        return GameObject.FindGameObjectsWithTag(enemyTag).Length+1;
    }
}
