using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSwitcher : MonoBehaviour
{
    [SerializeField] private Weapon[] weapons;

    private int currentIndex = 0;

    public Weapon CurrentWeapon => weapons.Length > 0 ? weapons[currentIndex] : null;

    void Start()
    {
        EquipWeapon(0);
    }

    void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame) EquipWeapon(0);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) EquipWeapon(1);

        float scroll = Mouse.current.scroll.ReadValue().y;
        if (scroll > 0f) EquipWeapon((currentIndex + 1) % weapons.Length);
        if (scroll < 0f) EquipWeapon((currentIndex - 1 + weapons.Length) % weapons.Length);
    }

    void EquipWeapon(int index)
    {
        if (index < 0 || index >= weapons.Length) return;

        for (int i = 0; i < weapons.Length; i++)
            weapons[i].gameObject.SetActive(i == index);

        currentIndex = index;
    }
}
