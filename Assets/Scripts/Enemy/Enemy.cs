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

    [Header("Sight Settings")]
    [SerializeField] private float _fieldOfView = 120;
    [SerializeField] private float _fovRange = 3.0f; // Range of the FOV detection, max distance at which the enemy sees the player

    [Header("Attack Settings")]
    [SerializeField] private float _attackRange = 2f; // Range within which the enemy can attack the player
    [SerializeField] private float _attackCooldown = 1.5f; // Time between each attack

    [Header("Weapon Settings")]
    [SerializeField] private GameObject _daggerHolster; // Dagger on the hip
    [SerializeField] private GameObject _daggerInHand; // Dagger in the enemy's hand, active when the enemy is in the chase state

    [Header("General Settings")]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private Collider _daggerCollider;
    [SerializeField] private float _meleeDamage = 10f;

    private Rigidbody _rb; // Reference to the enemy's Rigidbody component
    private Transform _playerTransform; // Reference to the player's Transform component
    private Vector3 _patrolOrigin;
    private float _halfFov; // To be able to have 2 different lines that wil shows the right and left end of the FOV
    private Animator _animator; // Reference to the enemy's Animator component
    private bool _isWeaponDrawn = false;
    private bool _isSheathingComplete = true;

    [Header("Gizmos")]
    [SerializeField] private bool _showMoveRadius;
    [SerializeField] private bool _showFOVRadius;
    [SerializeField] private bool _showProximityRadius;
    [SerializeField] private bool _showAttackRange;

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
    // SIGHT
    public float FieldOfView => _fieldOfView;
    public float FovRange => _fovRange;
    // ATTACK
    public float AttackRange => _attackRange;
    public float AttackCooldown => _attackCooldown;
    // WEAPON
    public bool IsWeaponDrawn => _isWeaponDrawn;
    public bool IsSheathingComplete => _isSheathingComplete;
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

    public void RotateToDirection(Vector3 direction)
    {
        direction.y = 0; // Ensure the enemy only rotates on the xz plane, so we set the y component to 0
        Quaternion targetRotation = Quaternion.LookRotation(direction); // Get the target rotation based on the direction vector
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 360f * Time.deltaTime * RotationSpeed); // Smoothly rotate the enemy towards the target rotation
    }

    public bool CheckIfPlayerIsInAttackRange()
    {
        return Vector3.Distance(transform.position, _playerTransform.position) <= _attackRange;
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

    public void SetIsInAttackRange(bool value)
    {
        _animator.SetBool("isInAttackRange", value);
    }

    // === WEAPON ANIMATION EVENTS ===

    // When the animation reaches the point where the event is called, unity calls this function.
    public void OnWeaponUnsheathed()
    {
        Debug.Log("Weapon unsheathed, switching to chase state");
        _daggerHolster.SetActive(false);
        _daggerInHand.SetActive(true);
    }

    public void OnWeaponDrawn()
    {
        Debug.Log("Weapon drawn, starting to chase the player");
        _isWeaponDrawn = true;
        DisableWeaponHitBox(); 
    }

    public void ResetWeaponDrawn()
    {
        _isWeaponDrawn = false;
    }

    public void OnWeaponSheathed()
    {
        Debug.Log("Weapon sheathed, switching to patrol state");
        _daggerHolster.SetActive(true);
        _daggerInHand.SetActive(false);
        _isSheathingComplete = true;
        _isWeaponDrawn = false;
    }

    public void ResetSheathingComplete()
    {
        _isSheathingComplete = false;
    }

    public void SetSheathingComplete()
    {
        _isSheathingComplete = true;
    }

    // ===COLLISION EVENTS===
    public void EnableWeaponHitBox() => _daggerCollider.enabled = true;
    public void DisableWeaponHitBox() => _daggerCollider.enabled = false;

    // === GIZMOS ===
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

        // ---- GIZMOS FOR PROXIMITY CHECK ----
        if (_showProximityRadius)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, _alertRadius);
        }

        // ---- GIZMOS FOR ATTACK RANGE ----
        if (_showAttackRange)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, _attackRange);
        }

        // ---- GIZMOS FOR FOV ----
        if (_showFOVRadius)
        {
            float halfFov = _fieldOfView / 2.0f;
            if (_playerTransform == null) return;
            if(CheckIfInFOV())
            {
                Gizmos.color = Color.green;
            }
            else
            {
                Gizmos.color = Color.red;
            }
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

    }


}
