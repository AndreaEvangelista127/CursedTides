using System;
using UnityEngine;

public class Weapon_Hitbox : MonoBehaviour
{

    [SerializeField] private float _weaponDmg;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision detected");
        if(other.TryGetComponent(out Hurtbox hurtbox))
        {
            Debug.Log("Collided with:" + hurtbox);
            hurtbox.ReceiveDmg(_weaponDmg);
        }
    }
}
