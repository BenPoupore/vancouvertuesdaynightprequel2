using System.Collections;
using UnityEngine;
using UnityEngine.AI;

// Attach this to your enemy GameObject.
// Requirements:
//  - A NavMeshAgent component on the same object
//  - A baked NavMesh in the scene (Window > AI > Navigation)
//  - The player GameObject tagged "Player"
//  - Optionally, empty GameObjects placed in the scene as patrol waypoints

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    public enum State { Patrol, Chase, Attack, Search }

    [Header("References")]
    public Transform[] patrolPoints;   // drag empty waypoint objects here
    public Transform firePoint;        // where bullets spawn from (e.g. gun tip)
    public GameObject bulletPrefab;    // optional, for projectile shooting
    public LayerMask obstructionMask;  // walls/geometry that blocks sight (NOT the player)

    [Header("Detection")]
    public float sightRange = 15f;
    public float fieldOfViewAngle = 100f;
    public float attackRange = 10f;
    public Transform eyes;             // usually an empty at head height; falls back to transform

    [Header("Movement")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4.5f;
    public float waitTimeAtPoint = 2f;

    [Header("Combat")]
    public float roundsPerMinute = 60f; // how fast the enemy fires - e.g. 60 = 1 shot/sec, 300 = 5 shots/sec
    public int damage = 10;
    public float bulletSpeed = 30f;
    public bool useHitscan = true;     // true = instant raycast shot, false = spawn projectile

    [Header("Search")]
    public float searchDuration = 4f;  // how long to look around after losing the player

    private NavMeshAgent agent;
    private Transform player;
    private State currentState = State.Patrol;
    private int currentPatrolIndex = 0;
    private float waitTimer = 0f;
    private float fireCooldown = 0f;
    private Vector3 lastKnownPlayerPos;
    private float searchTimer = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
        if (eyes == null) eyes = transform;

        agent.speed = patrolSpeed;
        if (patrolPoints != null && patrolPoints.Length > 0)
            GoToNextPatrolPoint();
    }

    void Update()
    {
        bool canSeePlayer = player != null && CanSeePlayer();

        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                if (canSeePlayer) ChangeState(State.Chase);
                break;

            case State.Chase:
                ChasePlayer();
                if (canSeePlayer) lastKnownPlayerPos = player.position;

                if (canSeePlayer && DistanceToPlayer() <= attackRange)
                    ChangeState(State.Attack);
                else if (!canSeePlayer)
                    ChangeState(State.Search);
                break;

            case State.Attack:
                FacePlayer();
                if (canSeePlayer)
                {
                    lastKnownPlayerPos = player.position;
                    TryShoot();
                    if (DistanceToPlayer() > attackRange)
                        ChangeState(State.Chase);
                }
                else
                {
                    ChangeState(State.Search);
                }
                break;

            case State.Search:
                SearchLastKnownPosition();
                if (canSeePlayer) ChangeState(State.Chase);
                break;
        }
    }

    // ---------------- STATE LOGIC ----------------

    void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTimeAtPoint)
            {
                waitTimer = 0f;
                GoToNextPatrolPoint();
            }
        }
    }

    void GoToNextPatrolPoint()
    {
        agent.speed = patrolSpeed;
        agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }

    void ChasePlayer()
    {
        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);
    }

    void SearchLastKnownPosition()
    {
        agent.speed = chaseSpeed;
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            searchTimer += Time.deltaTime;
            // simple "look around" - rotate slowly while waiting
            transform.Rotate(Vector3.up * 40f * Time.deltaTime);

            if (searchTimer >= searchDuration)
            {
                searchTimer = 0f;
                ChangeState(State.Patrol);
                if (patrolPoints != null && patrolPoints.Length > 0)
                    GoToNextPatrolPoint();
            }
        }
        else
        {
            agent.SetDestination(lastKnownPlayerPos);
        }
    }

    void ChangeState(State newState)
    {
        if (currentState == newState) return;
        currentState = newState;
        searchTimer = 0f;

        if (newState == State.Search)
            lastKnownPlayerPos = player.position;
    }

    // ---------------- DETECTION ----------------

    bool CanSeePlayer()
    {
        Vector3 dirToPlayer = player.position - eyes.position;
        float distance = dirToPlayer.magnitude;

        if (distance > sightRange) return false;

        float angle = Vector3.Angle(transform.forward, dirToPlayer);
        if (angle > fieldOfViewAngle * 0.5f) return false;

        // Line of sight check - make sure nothing solid is in the way
        if (Physics.Raycast(eyes.position, dirToPlayer.normalized, out RaycastHit hit, distance, obstructionMask))
        {
            return false; // something is blocking view
        }

        return true;
    }

    float DistanceToPlayer()
    {
        return Vector3.Distance(transform.position, player.position);
    }

    void FacePlayer()
    {
        Vector3 dir = (player.position - transform.position);
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.001f) return;
        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 8f);
    }

    // ---------------- SHOOTING ----------------

    void TryShoot()
    {
        fireCooldown -= Time.deltaTime;
        if (fireCooldown > 0f) return;

        fireCooldown = 60f / roundsPerMinute;

        if (useHitscan)
        {
            ShootHitscan();
        }
        else
        {
            ShootProjectile();
        }
    }

    void ShootHitscan()
    {
        Vector3 origin = firePoint != null ? firePoint.position : eyes.position;
        Vector3 targetPoint = player.position + Vector3.up * 1f; // aim roughly at chest height
        Vector3 dir = (targetPoint - origin).normalized;

        Debug.DrawRay(origin, dir * attackRange, Color.red, 0.2f);

        if (Physics.Raycast(origin, dir, out RaycastHit hit, sightRange))
        {
            // Try to damage whatever we hit, if it has a health/damage script
            var damageable = hit.collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }
        }
    }

    void ShootProjectile()
    {
        if (bulletPrefab == null || firePoint == null) return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 dir = (player.position + Vector3.up * 1f - firePoint.position).normalized;
            rb.linearVelocity = dir * bulletSpeed; // use rb.velocity if on an older Unity version
        }
    }

    // ---------------- DEBUG GIZMOS ----------------

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Vector3 fovLine1 = Quaternion.AngleAxis(fieldOfViewAngle * 0.5f, transform.up) * transform.forward * sightRange;
        Vector3 fovLine2 = Quaternion.AngleAxis(-fieldOfViewAngle * 0.5f, transform.up) * transform.forward * sightRange;
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, fovLine1);
        Gizmos.DrawRay(transform.position, fovLine2);
    }
}

// Simple interface for anything that can take damage (implement this on your Player health script)
public interface IDamageable
{
    void TakeDamage(int amount);
}
