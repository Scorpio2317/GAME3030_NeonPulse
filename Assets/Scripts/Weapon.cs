using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    [Header("Weapon Stats")]
    public float fireRate = 10f;
    public float hitscanDistance = 500f;

    [Header("Input System")]
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private string attackActionName = "Attack";

    [Header("References")]
    [SerializeField] private Transform cameraTransform;

    [Header("VFX")]
    [SerializeField] private GameObject concreteHitParticle;

    [Header("Fire Mode")]
    [SerializeField] private bool automatic = true;

    private InputAction attackAction;
    private float timeUntilNextShot;

    void Awake()
    {
        attackAction = inputActions.FindAction(attackActionName, true);

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void OnEnable()
    {
        attackAction.Enable();
    }

    void OnDisable()
    {
        attackAction.Disable();
    }

    void Update()
    {
        timeUntilNextShot = Mathf.Max(0f, timeUntilNextShot - Time.deltaTime);

        bool wantShoot = automatic ? attackAction.IsPressed() : attackAction.WasPressedThisFrame();

        if (wantShoot && timeUntilNextShot <= 0f)
        {
            Shoot();
            timeUntilNextShot = 1f / fireRate;
        }
    }

    void Shoot()
    {
        if (cameraTransform == null) return;

        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, hitscanDistance))
        {
            Debug.Log("Hit: " + hit.collider.name);

            SpawnConcreteHit(hit);
        }

        Debug.DrawRay(ray.origin, ray.direction * hitscanDistance, Color.red, 0.1f);
    }

    void SpawnConcreteHit(RaycastHit hit)
    {
        if (concreteHitParticle == null) return;

        GameObject fx = Instantiate(concreteHitParticle, hit.point, Quaternion.LookRotation(hit.normal));

        Destroy(fx, 2f);
    }
}
