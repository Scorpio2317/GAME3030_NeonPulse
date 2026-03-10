using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Weapon weapon;

    [Header("Ammo UI")]
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private TextMeshProUGUI reloadText;

    void Update()
    {
        if (weapon == null) 
            return;

        ammoText.text = weapon.currentAmmo + " / " + weapon.reserveAmmo;
        reloadText.gameObject.SetActive(weapon.isReloading);
    }
}
