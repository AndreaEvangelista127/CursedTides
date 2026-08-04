using UnityEngine;

public class ProjectileLauncher : MonoBehaviour
{
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private Transform _spawnPoint;

    public void Shoot(Vector3 target)
    {
        Vector3 direction = (target - _spawnPoint.position).normalized;
        GameObject projectile = Instantiate(_projectilePrefab, _spawnPoint.position, Quaternion.identity);

        Projectile projectileComponent = projectile.GetComponent<Projectile>();
        projectile.GetComponent<Rigidbody>().linearVelocity = direction * projectileComponent.Speed;
    }
}
