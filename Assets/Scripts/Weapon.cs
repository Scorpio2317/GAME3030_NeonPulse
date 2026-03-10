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

    [Header("Ammo")]
    public int magazineSize = 30;
    public int currentAmmo;
    public int reserveAmmo = 90;
    public float reloadTime = 1.5f;
    public bool isReloading { get; private set; }

    [Header("Fire Mode")]
    [SerializeField] private bool automatic = true;

    [Header("Input System")]
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private string attackActionName = "Attack";
    [SerializeField] private string reloadActionName = "Reload";

    [Header("References")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform muzzlePoint;

    [Header("VFX")]
    [SerializeField] private GameObject concreteHitParticle;
    [SerializeField] private GameObject bloodHitParticle;
    [SerializeField] private GameObject muzzleFlashPrefab;

    private InputAction attackAction;
    private InputAction reloadAction;
    private float timeUntilNextShot;

    void Awake()
    {
        attackAction = inputActions.FindAction(attackActionName, true);
        reloadAction = inputActions.FindAction(reloadActionName, true);

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        currentAmmo = magazineSize;
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
    }

    void Update()
    {
        timeUntilNextShot = Mathf.Max(0f, timeUntilNextShot - Time.deltaTime);

        // Manual reload
        if (reloadAction.WasPressedThisFrame() && !isReloading && currentAmmo < magazineSize && reserveAmmo > 0)
            StartCoroutine(Reload());

        if (isReloading) 
            return;

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
                // Auto reload when magazine is empty
                StartCoroutine(Reload());
            }
        }
    }

    void Shoot()
    {
        currentAmmo--;

        SpawnMuzzleFlash();

        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, hitscanDistance))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                SpawnHitParticle(bloodHitParticle, hit);
                hit.collider.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
            }
            else
            {
                SpawnHitParticle(concreteHitParticle, hit);
            }
        }

        Debug.DrawRay(ray.origin, ray.direction * hitscanDistance, Color.red, 0.1f);
    }

    IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");

        yield return new WaitForSeconds(reloadTime);

        int bulletsNeeded = magazineSize - currentAmmo;
        int bulletsToLoad = Mathf.Min(bulletsNeeded, reserveAmmo);
        currentAmmo += bulletsToLoad;
        reserveAmmo -= bulletsToLoad;

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
