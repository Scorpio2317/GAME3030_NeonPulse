using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    [Header("Ammo")]
    public int ammoAmount = 15;

    [Header("Settings")]
    public float lifetime = 20f;
    public float bobSpeed = 2f;
    public float bobHeight = 0.15f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        transform.Rotate(Vector3.up, 90f * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") && !other.transform.root.CompareTag("Player"))
            return;

        WeaponSwitcher switcher = other.GetComponentInParent<WeaponSwitcher>();

        if (switcher == null)
        {
            switcher = other.transform.root.GetComponentInChildren<WeaponSwitcher>();
        }

        if (switcher != null)
        {
            switcher.CurrentWeapon?.AddReserveAmmo(ammoAmount);
        }
        Destroy(gameObject);
    }
}
