using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    [Header("Weapon Stats")]
    public float fireRate = 10f;
    public float hitscanDistance = 500f;

    [Header("Damage")]
    public float damage = 25f;

    [Header("Pellets")]
    [SerializeField] private int pelletCount = 1;
    [SerializeField] private float pelletSpread = 0f;

    [Header("Ammo")]
    public int magazineSize = 30;
    public int currentAmmo;
    public int reserveAmmo = 90;
    public float reloadTime = 1.07f;          // ReloadOne clip length
    public float priorToReloadTime = 0.67f;   // PriorToReload clip length
    public float reloadLastTime = 0.53f;      // ReloadLastOne clip length
    public bool isReloading { get; private set; }

    public void AddReserveAmmo(int amount)
    {
        reserveAmmo += amount;
    }

    [Header("Fire Mode")]
    [SerializeField] private bool automatic = true;

    [Header("Input System")]
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private string attackActionName = "Attack";
    [SerializeField] private string reloadActionName = "Reload";

    [Header("References")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform muzzlePoint;
    [SerializeField] private Animator weaponAnimator;

    [Header("VFX")]
    [SerializeField] private GameObject concreteHitParticle;
    [SerializeField] private GameObject bloodHitParticle;
    [SerializeField] private GameObject muzzleFlashPrefab;

    [Header("SFX")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip shootSound;
    [SerializeField] private AudioClip reloadSound;

    private InputAction attackAction;
    private InputAction reloadAction;
    private float timeUntilNextShot;
    private GameUI gameUI;
    private PlayerMovement playerMovement;

    void Awake()
    {
        attackAction = inputActions.FindAction(attackActionName, true);
        reloadAction = inputActions.FindAction(reloadActionName, true);

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        currentAmmo = magazineSize;
        gameUI = FindFirstObjectByType<GameUI>();
        playerMovement = GetComponentInParent<PlayerMovement>();
    }

    void OnEnable()
    {
        attackAction.Enable();
        reloadAction.Enable();
    }

    void OnDisable()
    {
        attackAction.Disable();
        reloadAction.Disable();

        StopAllCoroutines();
        isReloading = false;
    }

    void Update()
    {
        timeUntilNextShot = Mathf.Max(0f, timeUntilNextShot - Time.deltaTime);

        // Manual reload
        if (reloadAction.WasPressedThisFrame() && !isReloading && currentAmmo < magazineSize && reserveAmmo > 0)
            StartCoroutine(Reload());

        if (isReloading)
            return;

        bool sprinting = playerMovement != null && playerMovement.isSprinting;
        if (automatic)
            weaponAnimator?.SetBool("isRunning", sprinting);

        if (sprinting)
        {
            weaponAnimator?.SetBool("isShooting", false);
            return;
        }

        bool wantShoot = automatic ? attackAction.IsPressed() : attackAction.WasPressedThisFrame();

        if (wantShoot && timeUntilNextShot <= 0f)
        {
            if (currentAmmo > 0)
            {
                Shoot();
                timeUntilNextShot = 1f / fireRate;
            }
            else if (reserveAmmo > 0)
            {
                StartCoroutine(Reload());
            }
        }

        if (automatic)
            weaponAnimator?.SetBool("isShooting", wantShoot && currentAmmo > 0 && !isReloading);

    }

    void Shoot()
    {
        currentAmmo--;

        SpawnMuzzleFlash();
        if (!automatic)
            weaponAnimator?.SetTrigger("Shoot");
        if (shootSound != null && audioSource != null)
            audioSource.PlayOneShot(shootSound);

        bool hitEnemy = false;

        for (int i = 0; i < pelletCount; i++)
        {
            Vector3 direction = cameraTransform.forward;

            if (pelletSpread > 0f)
            {
                float spreadX = Random.Range(-pelletSpread, pelletSpread);
                float spreadY = Random.Range(-pelletSpread, pelletSpread);
                direction = Quaternion.Euler(spreadY, spreadX, 0f) * direction;
            }

            Ray ray = new Ray(cameraTransform.position, direction);

            if (Physics.Raycast(ray, out RaycastHit hit, hitscanDistance))
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    SpawnHitParticle(bloodHitParticle, hit);
                    hit.collider.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
                    hitEnemy = true;
                }
                else
                {
                    SpawnHitParticle(concreteHitParticle, hit);
                }
            }

            Debug.DrawRay(ray.origin, ray.direction * hitscanDistance, Color.red, 0.1f);
        }

        if (hitEnemy)
            gameUI?.ShowHitmarker();
    }

    IEnumerator Reload()
    {
        isReloading = true;

        if (automatic)
        {
            weaponAnimator?.SetTrigger("Reload");
            if (reloadSound != null && audioSource != null)
                audioSource.PlayOneShot(reloadSound);
            yield return new WaitForSeconds(reloadTime);

            int bulletsNeeded = magazineSize - currentAmmo;
            int bulletsToLoad = Mathf.Min(bulletsNeeded, reserveAmmo);
            currentAmmo += bulletsToLoad;
            reserveAmmo -= bulletsToLoad;
        }
        else
        {
            // PriorToReload — prep animation plays once
            weaponAnimator?.SetTrigger("ReloadStart");
            yield return new WaitForSeconds(priorToReloadTime);

            // Load shells one by one
            while (currentAmmo < magazineSize && reserveAmmo > 0)
            {
                bool isLastShell = (currentAmmo == magazineSize - 1) || (reserveAmmo == 1);

                if (isLastShell)
                {
                    weaponAnimator?.SetTrigger("ReloadLast");
                    if (reloadSound != null && audioSource != null)
                        audioSource.PlayOneShot(reloadSound);
                    yield return new WaitForSeconds(reloadLastTime);
                }
                else
                {
                    weaponAnimator?.SetTrigger("ReloadShell");
                    if (reloadSound != null && audioSource != null)
                        audioSource.PlayOneShot(reloadSound);
                    yield return new WaitForSeconds(reloadTime);
                }

                currentAmmo++;
                reserveAmmo--;
            }
        }

        isReloading = false;
    }

    void SpawnMuzzleFlash()
    {
        if (muzzleFlashPrefab == null || muzzlePoint == null) 
            return;

        GameObject fx = Instantiate(muzzleFlashPrefab, muzzlePoint.position, muzzlePoint.rotation);
        Destroy(fx, 0.1f);
    }

    void SpawnHitParticle(GameObject prefab, RaycastHit hit)
    {
        if (prefab == null) 
            return;

        GameObject fx = Instantiate(prefab, hit.point, Quaternion.LookRotation(hit.normal));
        Destroy(fx, 2f);
    }
}
