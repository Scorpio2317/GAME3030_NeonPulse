using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum State { Idle, Chase, Attack }
    public State currentState = State.Idle;

    [Header("Detection")]
    public float detectionRange = 30f;
    public float preferredRange = 12f;

    [Header("Attack")]
    public float attackDamage = 10f;
    public float attackCooldown = 1.2f;
    public float accuracySpread = 3f;
    public float shootDistance = 100f;

    private Transform player;
    private NavMeshAgent agent;
    private float attackTimer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // State transitions
        if (distanceToPlayer > detectionRange)
        {
            currentState = State.Idle;
        }
        else if (distanceToPlayer > preferredRange)
        {
            currentState = State.Chase;
        }
        else
        {
            currentState = State.Attack;
        }

        switch (currentState)
        {
            case State.Idle:
                agent.ResetPath();
                break;

            case State.Chase:
                agent.SetDestination(player.position);
                break;

            case State.Attack:
                agent.ResetPath();

                // Face player on Y axis only (don't tilt up/down)
                Vector3 lookDir = player.position - transform.position;
                lookDir.y = 0;
                transform.rotation = Quaternion.LookRotation(lookDir);

                attackTimer -= Time.deltaTime;
                if (attackTimer <= 0f)
                {
                    Shoot();
                    attackTimer = attackCooldown;
                }
                break;
        }
    }

    void Shoot()
    {
        // Shoot from eye level
        Vector3 origin = transform.position + Vector3.up * 1f;
        Vector3 dirToPlayer = (player.position + Vector3.up * 1f - origin).normalized;

        float spreadX = Random.Range(-accuracySpread, accuracySpread);
        float spreadY = Random.Range(-accuracySpread, accuracySpread);
        dirToPlayer = Quaternion.Euler(spreadY, spreadX, 0) * dirToPlayer;

        if (Physics.Raycast(origin, dirToPlayer, out RaycastHit hit, shootDistance))
        {
            if (hit.collider.CompareTag("Player"))
                hit.collider.SendMessage("TakeDamage", attackDamage, SendMessageOptions.DontRequireReceiver);
        }

        Debug.DrawRay(origin, dirToPlayer * shootDistance, Color.yellow, 0.2f);
    }

    void OnDrawGizmosSelected()
    {
        // detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // preferred shooting range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, preferredRange);
    }
}
