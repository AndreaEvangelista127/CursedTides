using UnityEngine;

public class EnemyWeaponSetup : MonoBehaviour
{
    [SerializeField] public GameObject _meleeWeapon; // Hand Dagger reference
    [SerializeField] public GameObject _rangedWeapon; // Web Pistol reference

    public void ActivateMelee()
    {
        if (_meleeWeapon != null) _meleeWeapon.SetActive(true);
        if (_rangedWeapon != null) _rangedWeapon.SetActive(false);
    }

    public void ActivateRanged()
    {
        if (_meleeWeapon != null) _meleeWeapon.SetActive(false);
        if (_rangedWeapon != null) _rangedWeapon.SetActive(true);
    }
}
