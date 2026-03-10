using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum State { Idle, Chase, Attack }
    public State currentState = State.Idle;

    [Header("Detection")]
    public float detectionRange = 15f;
    public float attackRange = 2f;

    [Header("Attack")]
    public float attackDamage = 10f;
    public float attackCooldown = 1.5f;

    private Transform player;
    private NavMeshAgent agent;
    private float attackTimer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // State transitions
        if (distanceToPlayer <= attackRange)
        {
            currentState = State.Attack;
        }
        else if (distanceToPlayer <= detectionRange)
        {
            currentState = State.Chase;
        }
        else
        {
            currentState = State.Idle;
        }

        // State behaviour
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
                transform.LookAt(player);
                attackTimer -= Time.deltaTime;
                if (attackTimer <= 0f)
                {
                    player.SendMessage("TakeDamage", attackDamage, SendMessageOptions.DontRequireReceiver);
                    attackTimer = attackCooldown;
                }
                break;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
