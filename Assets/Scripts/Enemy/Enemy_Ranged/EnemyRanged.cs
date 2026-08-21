using System.Collections.Generic;
using UnityEngine;

public class EnemyRanged : Enemy
{
    [Header("Ranged Settings")]
    [SerializeField] private float _shootRange = 10f;
    [SerializeField] private float _tooCloseRange = 3f;
    [SerializeField] private float _shootCooldown = 2f;
    [SerializeField] private ParticleSystem _smoke;
    
    [Header("Alarm State settings")]
    private bool _hasFinishedLookingAround = false;

    private bool _combatTag = false;
    private ProjectileLauncher _launcher;


    // --- PUBLIC PROPERTIES ---
    public bool HasFinishedLookingAround => _hasFinishedLookingAround;
    public float ShootRange => _shootRange;
    public float TooCloseRange => _tooCloseRange;
    public float ShootCooldown => _shootCooldown;
    public bool CombatTag => _combatTag;

    protected override void Awake()
    {
        base.Awake();
        _enemyFSM = new EnemyFSM();
        _enemyFSM.Initialize(this, new Dictionary<EStates, BaseState>
        {
            [EStates.RangedIdle] = new RangedIdleState(),
            [EStates.RangedAlert] = new RangedAlertState(),
            [EStates.RangedPatrol] = new RangedPatrolState(),
            [EStates.RangedChase] = new RangedChaseState(),
            [EStates.RangedShoot] = new RangedShootState(),
        },
        EStates.RangedIdle);
        _launcher = GetComponent<ProjectileLauncher>();
    }

    private void Update()
    {
        _enemyFSM.UpdateFSM(); 
    }

    public void ApplyRangedSettings(RangedEnemySettings settings)
    {
        _shootRange = settings.ShootRange;
        _shootCooldown = settings.ShootCooldown;
        _tooCloseRange = settings.TooCloseRange;
    }

    public bool CheckIfPlayerIsInShootRange() =>
        Vector3.Distance(transform.position, PlayerTransform.position) <= _shootRange;

    public bool CheckIfPlayerIsTooClose() =>
        Vector3.Distance(transform.position, PlayerTransform.position) <= _tooCloseRange;

    public void OnLookAroundFinished()
    {
        _hasFinishedLookingAround = true;
        Debug.Log("Look around finished");
    }

    public void ResetLookAroundFinished()
    {
        _hasFinishedLookingAround = false;
    }

    public void SetCombatTag(bool value)
    {
        _combatTag = value;
    }

    public void OnShoot() // Method called in the animation event of the shooting animation, which is called at the moment we want the projectile to be launched
    {
        PlayerHealth playerHealth = PlayerTransform.GetComponent<PlayerHealth>();
        Vector3 aimPoint = playerHealth != null ? playerHealth.ChestBone.position : PlayerTransform.position;
        _launcher.Shoot(aimPoint);
        AudioManager.Instance?.PlayGunShot();
        _smoke.Play();
        
    }
    // When the player dies, we call the ResetToPatrol method because we subsribed to the PlayerHealth.OnPlayerDeath event in the Enemy base class
    protected override void ResetToPatrol() 
    {
        _enemyFSM.SwitchState(EStates.RangedPatrol);
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
    }
}
