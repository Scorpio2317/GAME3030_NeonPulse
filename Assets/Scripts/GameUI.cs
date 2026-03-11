using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Weapon weapon;
    [SerializeField] private PlayerHealth playerHealth;

    [Header("Ammo UI")]
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private TextMeshProUGUI reloadText;

    [Header("Health UI")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private Image damageVignette;

    [Header("Death Screen")]
    [SerializeField] private GameObject deathScreen;

    void Update()
    {
        UpdateAmmoUI();
        UpdateHealthUI();
        UpdateDeathScreen();
    }

    void UpdateAmmoUI()
    {
        if (weapon == null) 
            return;

        ammoText.text = weapon.currentAmmo + " / " + weapon.reserveAmmo;
        reloadText.gameObject.SetActive(weapon.isReloading);
    }

    void UpdateHealthUI()
    {
        if (playerHealth == null) 
            return;

        if (healthBar != null)
        {
            healthBar.value = playerHealth.currentHealth / playerHealth.maxHealth;
        }


        if (damageVignette != null)
        {
            float alpha = Mathf.Clamp01(playerHealth.damageFlashTimer / playerHealth.damageFlashDuration);
            Color c = damageVignette.color;
            c.a = alpha * 0.6f;
            damageVignette.color = c;
        }
    }

    void UpdateDeathScreen()
    {
        if (playerHealth == null || deathScreen == null) 
            return;

        deathScreen.SetActive(playerHealth.IsDead);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
