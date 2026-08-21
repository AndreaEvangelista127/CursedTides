using System;
using UnityEngine;

public class Weapon_Hitbox : MonoBehaviour
{

    [SerializeField] private float _weaponDmg;

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out Hurtbox hurtbox))
        {
            hurtbox.ReceiveDmg(_weaponDmg);
        }
    }
}
