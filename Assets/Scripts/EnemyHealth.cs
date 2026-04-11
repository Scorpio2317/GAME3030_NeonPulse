using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth { get; private set; }
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
    }

    void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;

        if (currentHealth <= 0f)
            Die();
    }

    void Die()
    {
        isDead = true;
        Animator anim = GetComponentInChildren<Animator>();
        if (anim != null)
            anim.SetTrigger("Die");

        GetComponent<EnemyAI>().enabled = false;
        GetComponent<NavMeshAgent>().enabled = false;

        WaveManager.Instance?.OnEnemyKilled();

        Destroy(gameObject, 3f);
    }
}
