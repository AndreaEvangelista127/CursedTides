using UnityEngine;

[System.Serializable] // This attribute allows the class to be serialized and displayed in the Unity Inspector
public class EnemySettings 
{
    [Header("General")]
    public GameObject prefab;
    public float moveSpeed = 3f;
    public float rotationSpeed = 0.8f;
    public float health = 100f;

    [Header("Idle")]
    public float idleTime = 3f;

    [Header("Patrol")]
    public float patrolRadius = 20f;
    public float distanceBuffer = 1f;

    [Header("Alert")]
    public float alertTime = 6f;
    public float alertRotationSpeed = 120f;
    public float alertRadius = 7f;
    public float minRotation = 30f;
    public float maxRotation = 180f;

    [Header("Chase")]
    public float detectionRange = 10f;
    public float maxChaseDistance = 20f;
    public float chaseSpeed = 5f;

    [Header("Sight")]
    public float fieldOfView = 120f;
    public float fovRange = 8f;
}

[System.Serializable]
public class MeleeEnemySettings : EnemySettings
{
    [Header("Melee Attack")]
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    public float attackCommitRange = 3f;
}

[System.Serializable]
public class RangedEnemySettings : EnemySettings
{
    [Header("Ranged Attack")]
    public float shootRange = 10f;
    public float shootCooldown = 2f;
    public float tooCloseRange = 3f;
}
