using System;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class Enemy : MonoBehaviour
{
    // An abstract class with MonoBehaviour can't be used directly on a GameObject, but it can be inherited by other classes that implement specific enemy behaviors.

    [Header("Idle Settings")]
    [SerializeField] private float _idleTime = 2f; // Time the enemy will idle before calculating a new patrol point

    [Header("Patrol Settings")]
    [SerializeField] private float _patrolRadius = 5f; // Radius within which the enemy will patrol
    [SerializeField] private float _distanceBuffer = 1f; // Distance at which the enemy will stop, idle and then calculate a new patrol point

    [Header("Alert Settings")]
    [SerializeField] private float _alertTime = 3f; // Time the enemy will stay in the alert state before switching back to patrol if it doesn't see the player again
    [SerializeField] private float _alertRotationSpeed = 120f; // Rotation speed of the enemy when it's in the alert state, it will rotate to look for the player
    [SerializeField] private float _alertRadius = 1f;
    [SerializeField] [Range(30f, 360f)] private float _minRotation, _maxRotation;

    [Header("Chase Settings")]
    [SerializeField] private float _detectionRange = 10f; // Range within which the enemy can detect the player
    [SerializeField] private float _maxChaseDistance = 20f; // Maximum distance the enemy will chase the player before giving up and returning to patrol
    [SerializeField] private float _chaseSpeed = 5f;

    [Header("Sight Settings")]
    [SerializeField] private float _fieldOfView = 120;
    [SerializeField] private float _fovRange = 3.0f; // Range of the FOV detection, max distance at which the enemy sees the player

    [Header("General Settings")]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private Transform _headSkull; // Reference to the enemy's head bone, used for raycasting to check if the player is visible

    [Header("Gizmos")]
    [SerializeField] private bool _showMoveRadius;
    [SerializeField] private bool _showFOVRadius;
    [SerializeField] private bool _showProximityRadius;

    protected EnemyFSM _enemyFSM;
    private Rigidbody _rb; // Reference to the enemy's Rigidbody component
    private Transform _playerTransform; // Reference to the player's Transform component
    private Vector3 _patrolOrigin;
    private float _halfFov; // To be able to have 2 different lines that wil shows the right and left end of the FOV
    private Animator _animator; // Reference to the enemy's Animator component


    // --- PUBLIC PROPERTIES ---
    // IDLE
    public float IdleTime => _idleTime;
    // PATROL
    public Vector3 PatrolOrigin => _patrolOrigin;
    public float PatrolRadius => _patrolRadius;
    public float DistanceBuffer => _distanceBuffer;
    // ALERT
    public float AlertTime => _alertTime;
    public float AlertRotationSpeed => _alertRotationSpeed;
    public float MinRotation => _minRotation;
    public float MaxRotation => _maxRotation;
    // CHASE
    public float DetectionRange => _detectionRange;
    public float MaxChaseDistance => _maxChaseDistance;
    public float ChaseSpeed => _chaseSpeed;
    // SIGHT
    public float FieldOfView => _fieldOfView;
    public float FovRange => _fovRange;
    // GENERAL
    public float MoveSpeed => _moveSpeed;
    public float RotationSpeed => _rotationSpeed;
    public Transform PlayerTransform => _playerTransform;
    public Rigidbody Rb => _rb;

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _patrolOrigin = transform.position; // Set the patrol origin to the enemy's initial position
        _playerTransform = GameObject.FindWithTag("Player").transform; // Find the player in the scene by tag and get its Transform
        _halfFov = _fieldOfView / 2.0f; // Calculate half of the field of view for later use in FOV checks
        _animator = GetComponent<Animator>();
        PlayerHealth.OnPlayerDeath += ResetToPatrol; // Add this method to the list of methods to be called when the player dies, so that the enemy can reset its state to patrol
    }

    /// <summary>
    /// Storing all the required data to pass to the EnemyFactory script
    /// </summary>
    /// <param name="settings"></param>
    public void ApplyEnemySettingForFactory(EnemySettings settings)
    {
        if(settings == null) return;

        // General settings
        _moveSpeed = settings.MoveSpeed;
        _rotationSpeed = settings.RotationSpeed;
        //Idle
        _idleTime = settings.IdleTime;
        //Patrol
        _patrolRadius = settings.PatrolRadius;
        _distanceBuffer = settings.DistanceBuffer;
        //Alert
        _alertTime = settings.AlertTime;
        _alertRotationSpeed = settings.AlertRotationSpeed;
        _alertRadius = settings.AlertRadius;
        _minRotation = settings.MinRotation;
        _maxRotation = settings.MaxRotation;
        //Chase
        _detectionRange = settings.DetectionRange;
        _maxChaseDistance = settings.MaxChaseDistance;
        _chaseSpeed = settings.ChaseSpeed;
        //Sight
        _fieldOfView = settings.FieldOfView;
        _fovRange = settings.FovRange;
    }

    /// <summary>
    /// Check if the player it´s inside of the FOV by using dot product
    /// </summary>
    /// <returns></returns>
    public virtual bool CheckIfInFOV()
    {
        Vector3 dirToPlayer = _playerTransform.position - transform.position; // Get the direction from the enemy to the player

        // -- DISTANCE CHECK --
        float distanceToPlayer = dirToPlayer.magnitude; // Get the distance from the enemy to the player

        if (distanceToPlayer > _fovRange)
        { // If the player is outside the detection range, return false immediately
            return false;
        }

        Vector3 forward;

        // -- FOV CHECK --
        if(_headSkull != null)
        {
            forward = _headSkull.transform.forward;
            forward = new Vector3(forward.x, 0, forward.z).normalized; // Flatten the forward vector to the xz plane and normalize it
        }
        else
        {
            forward = transform.forward;
        }

        float dotProduct = Vector3.Dot(forward, dirToPlayer.normalized); // Calculate the dot product between the enemy's forward direction and the direction to the player

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

    public void RotateToDirection(Vector3 direction)
    {
        direction.y = 0; // Ensure the enemy only rotates on the xz plane, so we set the y component to 0
        Quaternion targetRotation = Quaternion.LookRotation(direction); // Get the target rotation based on the direction vector
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 360f * Time.deltaTime * RotationSpeed); // Smoothly rotate the enemy towards the target rotation
    }

    public Vector3 GetRandomPatrolPoint()
    {
        // Get a random point within a sphere with the radius of the patrol radius
        Vector3 randomDest = UnityEngine.Random.insideUnitSphere * _patrolRadius + _patrolOrigin; //Added patrol origin so that the enemy patrols around its initial position instead of around the world origin

        randomDest.y = 0; // The enemy should only patrol on the xz plane, so we set the y component to 0

        return randomDest;
    }

    public void MoveTowardsPlayer()
    {
        Vector3 direction = _playerTransform.position - transform.position; // Get the direction from the enemy to the player
        direction.y = 0; // Ignore vertical movement for chasing
        Vector3 moveVector = direction.normalized * _chaseSpeed; // Calculate the movement vector based on the enemy's chase speed and the time elapsed since the last frame
        moveVector.y = _rb.linearVelocity.y; // Preserve the current vertical velocity (like gravity)
        RotateToDirection(direction);

        _rb.linearVelocity = moveVector;
    }

    protected abstract void ResetToPatrol(); // Abstract method to be implemented by child classes, called when the player dies to reset the enemy's state to patrol

    private void OnDestroy()
    {
        PlayerHealth.OnPlayerDeath -= ResetToPatrol; // Unsubscribe from the player death event when the enemy is destroyed to prevent memory leaks
    }

    // === ANIMATION METHODS ===

    public void SetIfIsPatrolling(bool isPatrolling)
    {
        _animator.SetBool("isPatrolling", isPatrolling);
    }

    public void SetIfIsInAlert(bool isInAlert)
    {
        _animator.SetBool("isInAlert", isInAlert);
    }

    public void SetIfIsChasing(bool isChasing)
    {
        _animator.SetBool("isChasing", isChasing);
    }

    // === GIZMOS ===
    protected virtual void OnDrawGizmos()
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

        // ---- GIZMOS FOR PROXIMITY CHECK ----
        if (_showProximityRadius)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, _alertRadius);
        }

        // ---- GIZMOS FOR FOV ----
        if (_showFOVRadius)
        {
            float halfFov = _fieldOfView / 2.0f;

            if (_playerTransform != null && CheckIfInFOV())
                Gizmos.color = Color.green;
            else
                Gizmos.color = Color.red;

            // Use head bone forward if available, otherwise fallback to transform.forward
            Vector3 fovForward = _headSkull != null ? _headSkull.forward : transform.forward;
            fovForward = new Vector3(fovForward.x, 0, fovForward.z).normalized; // Flatten the forward vector to the xz plane and normalize it
            Vector3 origin = _headSkull != null ? _headSkull.position : transform.position + Vector3.up;

            Quaternion leftRayRotation = Quaternion.AngleAxis(-halfFov, Vector3.up);
            Quaternion rightRayRotation = Quaternion.AngleAxis(halfFov, Vector3.up);
            Vector3 leftRayDirection = leftRayRotation * fovForward;
            Vector3 rightRayDirection = rightRayRotation * fovForward;
            Gizmos.DrawRay(origin, leftRayDirection * _fovRange);
            Gizmos.DrawRay(origin, rightRayDirection * _fovRange);

            int steps = 20;
            float stepAngle = _fieldOfView / steps;

            Vector3 previousPoint = origin + (Quaternion.AngleAxis(-halfFov, Vector3.up) * fovForward) * _fovRange;

            for (int i = 1; i <= steps; i++)
            {
                float angle = -halfFov + stepAngle * i;
                Vector3 nextPoint = origin + (Quaternion.AngleAxis(angle, Vector3.up) * fovForward) * _fovRange;
                Gizmos.DrawLine(previousPoint, nextPoint);
                previousPoint = nextPoint;
            }
        }

    }


}
