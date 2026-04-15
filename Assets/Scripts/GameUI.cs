using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WeaponSwitcher weaponSwitcher;
    [SerializeField] private PlayerHealth playerHealth;

    [Header("Ammo UI")]
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private TextMeshProUGUI reloadText;

    [Header("Health UI")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private Image damageVignette;

    [Header("Hitmarker")]
    [SerializeField] private Image hitmarkerImage;
    [SerializeField] private float hitmarkerDuration = 0.1f;
    private float hitmarkerTimer;

    [Header("Wave & Score UI")]
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI killCountText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI countdownText;

    [Header("Screens")]
    [SerializeField] private GameObject deathScreen;
    [SerializeField] private TextMeshProUGUI deathScoreText;
    [SerializeField] private GameObject victoryScreen;
    [SerializeField] private TextMeshProUGUI victoryScoreText;

    void Update()
    {
        UpdateAmmoUI();
        UpdateHealthUI();
        UpdateHitmarker();
        UpdateDeathScreen();
        UpdateWaveUI();
    }

    void UpdateAmmoUI()
    {
        Weapon weapon = weaponSwitcher?.CurrentWeapon;
        if (weapon == null) return;

        ammoText.text = weapon.currentAmmo + " / " + weapon.reserveAmmo;
        reloadText.gameObject.SetActive(weapon.isReloading);
    }

    void UpdateHealthUI()
    {
        if (playerHealth == null) return;

        if (healthBar != null)
            healthBar.value = playerHealth.currentHealth / playerHealth.maxHealth;

        if (damageVignette != null)
        {
            float alpha = Mathf.Clamp01(playerHealth.damageFlashTimer / playerHealth.damageFlashDuration);
            Color c = damageVignette.color;
            c.a = alpha * 0.6f;
            damageVignette.color = c;
        }
    }

    public void ShowHitmarker()
    {
        hitmarkerTimer = hitmarkerDuration;
    }

    void UpdateHitmarker()
    {
        if (hitmarkerImage == null) return;

        hitmarkerTimer -= Time.deltaTime;
        float alpha = Mathf.Clamp01(hitmarkerTimer / hitmarkerDuration);
        Color c = hitmarkerImage.color;
        c.a = alpha;
        hitmarkerImage.color = c;
    }

    void UpdateDeathScreen()
    {
        if (playerHealth == null || deathScreen == null) return;

        bool dead = playerHealth.IsDead;
        deathScreen.SetActive(dead);

        if (dead && deathScoreText != null && WaveManager.Instance != null)
            deathScoreText.text = "Score: " + WaveManager.Instance.totalScore + "\nKills: " + WaveManager.Instance.killCount + "\nWave: " + WaveManager.Instance.currentWave;
    }

    void UpdateWaveUI()
    {
        if (WaveManager.Instance == null) return;

        if (waveText != null)
            waveText.text = "WAVE " + WaveManager.Instance.currentWave;

        if (killCountText != null)
            killCountText.text = "Kills: " + WaveManager.Instance.killCount;

        if (scoreText != null)
            scoreText.text = "Score: " + WaveManager.Instance.totalScore;

        if (countdownText != null)
        {
            bool counting = WaveManager.Instance.isCountingDown;
            countdownText.gameObject.SetActive(counting);
            if (counting)
                countdownText.text = "Next wave in " + Mathf.CeilToInt(WaveManager.Instance.countdownTimer) + "...";
        }
    }

    public void ShowVictory()
    {
        if (victoryScreen == null) return;
        victoryScreen.SetActive(true);

        if (victoryScoreText != null && WaveManager.Instance != null)
            victoryScoreText.text = "Score: " + WaveManager.Instance.totalScore + "\nKills: " + WaveManager.Instance.killCount;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
