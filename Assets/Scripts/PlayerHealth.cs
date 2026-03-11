using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth { get; private set; }

    [Header("Regeneration")]
    public float regenDelay = 5f;      
    public float regenRate = 20f;       

    [Header("Damage Flash")]
    public float damageFlashDuration = 0.4f;
    public float damageFlashTimer { get; private set; }

    private float timeSinceLastDamage;
    private bool isDead;
    public bool IsDead => isDead;

    void Start()
    {
        currentHealth = maxHealth;
        timeSinceLastDamage = regenDelay;
    }

    void Update()
    {
        if (isDead) return;

        timeSinceLastDamage += Time.deltaTime;

        if (timeSinceLastDamage >= regenDelay && currentHealth < maxHealth)
        {
            currentHealth = Mathf.Min(maxHealth, currentHealth + regenRate * Time.deltaTime);
        }

        if (damageFlashTimer > 0f)
        {
            damageFlashTimer -= Time.deltaTime;
        }

    }

    void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        timeSinceLastDamage = 0f;
        damageFlashTimer = damageFlashDuration;

        if (currentHealth <= 0f)
            Die();
    }

    void Die()
    {
        isDead = true;
        currentHealth = 0f;

        GetComponent<PlayerMovement>().enabled = false;
        GetComponentInChildren<MouseController>().enabled = false;
        GetComponentInChildren<Weapon>().enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
