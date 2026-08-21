using UnityEngine;

[System.Serializable] // This attribute allows the class to be serialized and displayed in the Unity Inspector
public class EnemySettings 
{
    [Header("General")]
    public GameObject Prefab;
    public float MoveSpeed = 3f;
    public float RotationSpeed = 0.8f;
    public float Health = 100f;

    [Header("Idle")]
    public float IdleTime = 3f;

    [Header("Patrol")]
    public float PatrolRadius = 20f;
    public float DistanceBuffer = 1f;

    [Header("Alert")]
    public float AlertTime = 6f;
    public float AlertRotationSpeed = 120f;
    public float AlertRadius = 7f;
    public float MinRotation = 30f;
    public float MaxRotation = 180f;

    [Header("Chase")]
    public float DetectionRange = 10f;
    public float MaxChaseDistance = 20f;
    public float ChaseSpeed = 5f;

    [Header("Sight")]
    public float FieldOfView = 120f;
    public float FovRange = 8f;
}

[System.Serializable]
public class MeleeEnemySettings : EnemySettings
{
    [Header("Melee Attack")]
    public float AttackRange = 2f;
    public float AttackCooldown = 1.5f;
    public float AttackCommitRange = 3f;
}

[System.Serializable]
public class RangedEnemySettings : EnemySettings
{
    [Header("Ranged Attack")]
    public float ShootRange = 10f;
    public float ShootCooldown = 2f;
    public float TooCloseRange = 3f;
}
