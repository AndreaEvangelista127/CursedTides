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

    [Header("Alert Settings")]
    [SerializeField] private float _alertTime = 3f; // Time the enemy will stay in the alert state before switching back to patrol if it doesn't see the player again
    [SerializeField] private float _alertRotationSpeed = 120f; // Rotation speed of the enemy when it's in the alert state, it will rotate to look for the player
    [SerializeField] private float _alertRadius = 1f;

    [Header("Chase Settings")]
    [SerializeField] private float _detectionRange = 10f; // Range within which the enemy can detect the player

    [Header("Sight Settings")]
    [SerializeField] private float _fieldOfView = 120;
    [SerializeField] private float _fovRange = 3.0f; // Range of the FOV detection, max distance at which the enemy sees the player

    [Header("General Settings")]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _rotationSpeed;

    private Rigidbody _enemyRb; // Reference to the enemy's Rigidbody component
    private Transform _playerTransform; // Reference to the player's Transform component
    private Vector3 _patrolOrigin;
    private float _halfFov; // To be able to have 2 different lines that wil shows the right and left end of the FOV

    [Header("Gizmos")]
    [SerializeField] private bool _showMoveRadius;
    [SerializeField] private bool _showFOVRadius;

    // --- PUBLIC PROPERTIES ---
    // IDLE
    public float IdleTime => _idleTime;
    // PATROL
    public float PatrolRadius => _patrolRadius;
    public float StoppingDistance => _stoppingDistance;
    // ALERT
    public float AlertTime => _alertTime;
    public float AlertRotationSpeed => _alertRotationSpeed;
    // CHASE
    public float DetectionRange => _detectionRange;
    // SIGHT
    public float FieldOfView => _fieldOfView;
    public float FovRange => _fovRange;
    // GENERAL
    public float MoveSpeed => _moveSpeed;
    public float RotationSpeed => _rotationSpeed;
    public Transform PlayerTransform => _playerTransform;
    public Vector3 PatrolOrigin => _patrolOrigin;
    public Rigidbody EnemyRb => _enemyRb;

    protected virtual void Awake()
    {
        _enemyRb = GetComponent<Rigidbody>();
        _patrolOrigin = transform.position; // Set the patrol origin to the enemy's initial position
        _playerTransform = GameObject.FindWithTag("Player").transform; // Find the player in the scene by tag and get its Transform
        _halfFov = _fieldOfView / 2.0f; // Calculate half of the field of view for later use in FOV checks
    }

    public virtual bool CheckIfInFOV()
    {
        Vector3 dirToPlayer = _playerTransform.position - transform.position; // Get the direction from the enemy to the player

        // -- DISTANCE CHECK --
        float distanceToPlayer = dirToPlayer.magnitude; // Get the distance from the enemy to the player

        if (distanceToPlayer > _fovRange)
        { // If the player is outside the detection range, return false immediately
            return false;
        }

        // -- FOV CHECK --
        float dotProduct = Vector3.Dot(transform.forward, dirToPlayer.normalized); // Calculate the dot product between the enemy's forward direction and the direction to the player

        if (dotProduct >= Mathf.Cos(_halfFov * Mathf.Deg2Rad))
        { // If the dot product is greater than or equal to the cosine of half the field of view, the player is within the FOV angle
            return true;
        }
        else
        {
            return false;
        }
    }

    public virtual bool CheckIfInDetectionRange()
    {
        Vector3 dirToPlayer = _playerTransform.position - transform.position; // Get the direction from the enemy to the player
        float distanceToPlayer = dirToPlayer.magnitude; // Get the distance from the enemy to the player
        if (distanceToPlayer < _alertRadius)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnDrawGizmos()
    {
        // ---- GIZMOS FOR PATROL RADIUS ----
        if (_showMoveRadius)
        {
            if (_patrolOrigin == Vector3.zero)
            {
                _patrolOrigin = transform.position;
            }

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(_patrolOrigin, _patrolRadius);
        }

        // ---- GIZMOS FOR FOV ----
        if (_showFOVRadius)
        {
            float halfFov = _fieldOfView / 2.0f;

            Gizmos.color = Color.red;
            Quaternion leftRayRotation = Quaternion.AngleAxis(-halfFov, Vector3.up);
            Quaternion rightRayRotation = Quaternion.AngleAxis(halfFov, Vector3.up);
            Vector3 leftRayDirection = leftRayRotation * transform.forward;
            Vector3 rightRayDirection = rightRayRotation * transform.forward;
            Gizmos.DrawRay(transform.position + Vector3.up, leftRayDirection * _fovRange); //Vector3.up is added to the position to make the rays start from the enemy's head instead of its feet
            Gizmos.DrawRay(transform.position + Vector3.up, rightRayDirection * _fovRange);

            int steps = 20; // Number of lines to draw the arc, the higher the number, the smoother the arc will look, but it will also be more expensive to draw
            float stepAngle = _fieldOfView / steps; // Angle between each step
            Vector3 origin = transform.position + Vector3.up;

            // Start point of the arc (leftmost point)
            Vector3 previousPoint = origin + (Quaternion.AngleAxis(-halfFov, Vector3.up) * transform.forward) * _fovRange;

            for (int i = 1; i <= steps; i++)
            {
                float angle = -halfFov + stepAngle * i; // First iteration: -100 + 5 = -95, second iteration: -100 + 10 = -90, etc.
                Vector3 nextPoint = origin + (Quaternion.AngleAxis(angle, Vector3.up) * transform.forward) * _fovRange;
                Gizmos.DrawLine(previousPoint, nextPoint);
                previousPoint = nextPoint;
            }
        }

        // ---- GIZMOS FOR PROXIMITY CHECK ----
        if (_showFOVRadius)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, _alertRadius);
        }




    }

}
