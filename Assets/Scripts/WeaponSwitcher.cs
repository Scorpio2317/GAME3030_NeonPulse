using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSwitcher : MonoBehaviour
{
    [SerializeField] private Weapon[] weapons;

    private int currentIndex = 0;

    public Weapon CurrentWeapon => weapons.Length > 0 ? weapons[currentIndex] : null;
    public Weapon[] GetWeapons() => weapons;

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

        // Disable others first, then enable the selected one
        for (int i = 0; i < weapons.Length; i++)
            if (i != index) weapons[i].gameObject.SetActive(false);

        weapons[index].gameObject.SetActive(true);
        currentIndex = index;
    }
}
