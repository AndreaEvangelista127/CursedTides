using System;
using UnityEngine;

public class Weapon_Hitbox : MonoBehaviour
{

    [SerializeField] private float _weaponDmg;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision detected");
        if(TryGetComponent(out IDamageable damageable))
        {
            Debug.Log("Collided with:" + damageable);
            damageable.TakeDamage(_weaponDmg);
        }
    }
}
