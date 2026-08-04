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
    private bool _hasShrugFinished = false;

    private bool _combatTag = false;
    private ProjectileLauncher _launcher;


    // --- PUBLIC PROPERTIES ---
    public bool HasFinishedLookingAround => _hasFinishedLookingAround;
    public bool HasShrugFinished => _hasShrugFinished;
    public float ShootRange => _shootRange;
    public float TooCloseRange => _tooCloseRange;
    public float ShootCooldown => _shootCooldown;
    public bool CombatTag => _combatTag;

    private EnemyFSM _fsm;

    protected override void Awake()
    {
        base.Awake();
        _fsm = new EnemyFSM();
        _fsm.Initialize(this, new Dictionary<EStates, BaseState>
        {
            [EStates.RangedIdle] = new RangedIdleState(),
            [EStates.RangedAlert] = new RangedAlertState(),
            [EStates.RangedPatrol] = new RangedPatrolState(),
            [EStates.RangedChase] = new RangedChaseState(),
            [EStates.RangedShoot] = new RangedShootState(),
            [EStates.RangedWalkAway] = new RangedWalkAwayState()
        },
        EStates.RangedIdle);
        _launcher = GetComponent<ProjectileLauncher>();
    }

    private void Update()
    {
        _fsm.UpdateFSM(); 
    }

    public void ApplyRangedSettings(RangedEnemySettings settings)
    {
        _shootRange = settings.shootRange;
        _shootCooldown = settings.shootCooldown;
        _tooCloseRange = settings.tooCloseRange;
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

    public void OnShrugFinished()
    {
        _hasShrugFinished = true;
        Debug.Log("Shrugging finished");
    }

    public void ResetShrugFinished()
    {
        _hasShrugFinished = false;
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
        _smoke.Play();
        
    }
    // When the player dies, we call the ResetToPatrol method because we subsribed to the PlayerHealth.OnPlayerDeath event in the Enemy base class
    protected override void ResetToPatrol() 
    {
        _fsm.SwitchState(EStates.RangedPatrol);
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
    }
}
