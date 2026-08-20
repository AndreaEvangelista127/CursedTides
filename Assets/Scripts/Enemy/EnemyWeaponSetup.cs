using UnityEngine;

public class EnemyWeaponSetup : MonoBehaviour
{
    [SerializeField] public GameObject MeleeWeapon; // Hand Dagger reference
    [SerializeField] public GameObject RangedWeapon; // Web Pistol reference

    public void ActivateMelee()
    {
        if (MeleeWeapon != null) MeleeWeapon.SetActive(true);
        if (RangedWeapon != null) RangedWeapon.SetActive(false);
    }

    public void ActivateRanged()
    {
        if (MeleeWeapon != null) MeleeWeapon.SetActive(false);
        if (RangedWeapon != null) RangedWeapon.SetActive(true);
    }
}
