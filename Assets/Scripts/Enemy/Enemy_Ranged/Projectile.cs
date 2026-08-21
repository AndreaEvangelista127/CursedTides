using UnityEngine;

public class Projectile : MonoBehaviour
{

    [SerializeField] private float _dmg;
    [SerializeField] private float _projectileSpeed;
    [SerializeField] private float _lifetime = 5f;

    // --- PUBLIC PROPERTIES ---
    public float Speed => _projectileSpeed;

    private void Start()
    {        
        Destroy(gameObject, _lifetime); // Destroy the projectile after its lifetime expires to prevent it from existing indefinitely
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<IDamageable>() != null)
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            damageable.TakeDamage(_dmg);
            Debug.Log($"Projectile hit the {other.tag} and dealt {_dmg} damage.");
            Destroy(gameObject); // Destroy the projectile upon hitting the damageable object
        }
        else if (other.CompareTag("Environment"))
        {
            Debug.Log("Projectile hit the environment and is destroyed.");
            Destroy(gameObject); // Destroy the projectile upon hitting the environment
        }
    }

    
}
