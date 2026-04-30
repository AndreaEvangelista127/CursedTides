using UnityEngine;
using UnityEngine.UIElements;

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

    [Header("Sight Settings")]
    [SerializeField] private float _fieldOfView = 120;
    [SerializeField] private float _fovRange = 3.0f;

    [Header("General Settings")]
    [SerializeField] private float _moveSpeed;

    private Rigidbody _enemyRb; // Reference to the enemy's Rigidbody component
    private Transform _playerTransform; // Reference to the player's Transform component
    private Vector3 _patrolOrigin;
    private bool _isInFov = false;

    [Header("Gizmos")]
    [SerializeField] private bool _showMoveRadius;

    // Public properties to access private fields
    public float IdleTime => _idleTime; 
    public float PatrolRadius => _patrolRadius; 
    public float StoppingDistance => _stoppingDistance; 
    public float DetectionRange => _detectionRange; 
    public float MoveSpeed => _moveSpeed;
    public float FieldOfView => _fieldOfView;
    public float FovRange => _fovRange;
    public bool IsInFOv => _isInFov;
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

        Gizmos.color = Color.red;
        float halfFOV = _fieldOfView / 2.0f; // To be able to have 2 different lines that wil shows the right and left end of the FOV
        Quaternion leftRayRotation = Quaternion.AngleAxis(-_fieldOfView, Vector3.up);
        Quaternion rightRayRotation = Quaternion.AngleAxis(_fieldOfView, Vector3.up);
        Vector3 leftRayDirection = leftRayRotation * transform.forward;
        Vector3 rightRayDirection = rightRayRotation * transform.forward;
        Gizmos.DrawRay(transform.position, leftRayDirection * _fovRange);
        Gizmos.DrawRay(transform.position, rightRayDirection * _fovRange);
       


    }

}
