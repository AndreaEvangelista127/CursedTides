using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    // An abstract class with MonoBehaviour can't be used directly on a GameObject, but it can be inherited by other classes that implement specific enemy behaviors.

    [Header("Idle Settings")]
    [SerializeField] private float _idleTime = 2f; // Time the enemy will idle before calculating a new patrol point

    [Header("Patrol Settings")]
    [SerializeField] private float _patrolRadius = 5f; // Radius within which the enemy will patrol
    [SerializeField] private float _stoppingDistance = 1f; // Distance at which the enemy will stop, idle and then calculate a new patrol point

    [Header("Chase Settings")]
    [SerializeField] private float _detectionRange = 10f; // Range within which the enemy can detect the player

    [Header("General Settings")]
    [SerializeField] private float _moveSpeed;

    private Rigidbody _enemyRb; // Reference to the enemy's Rigidbody component
    private Transform _playerTransform; // Reference to the player's Transform component
    private Vector3 _patrolOrigin;

    [Header("Gizmos")]
    [SerializeField] private bool _showMoveRadius;

    // Public properties to access private fields
    public float IdleTime => _idleTime; 
    public float PatrolRadius => _patrolRadius; 
    public float StoppingDistance => _stoppingDistance; 
    public float DetectionRange => _detectionRange; 
    public float MoveSpeed => _moveSpeed; 
    public Transform PlayerTransform => _playerTransform; 
    public Vector3 PatrolOrigin => _patrolOrigin; 
    public Rigidbody EnemyRb => _enemyRb; 

    protected virtual void Awake()
    {
        _enemyRb = GetComponent<Rigidbody>();
        _patrolOrigin = transform.position; // Set the patrol origin to the enemy's initial position
        _playerTransform = GameObject.FindWithTag("Player").transform; // Find the player in the scene by tag and get its Transform
    }

    private void OnDrawGizmos()
    {


        if (_showMoveRadius)
        {
            if(_patrolOrigin == Vector3.zero)
            {
                _patrolOrigin = transform.position;
            }

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(_patrolOrigin, _patrolRadius);

        }

        
    }

}
