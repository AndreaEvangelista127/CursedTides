using System.Collections.Generic;
using UnityEngine;

public class EnemyRanged : Enemy
{
    [Header("Ranged Settings")]
    [SerializeField] private float _shootRange = 10f;
    [SerializeField] private float _tooCloseRange = 3f;
    [SerializeField] private float _shootCooldown = 2f;
    

    [Header("Alarm State settings")]
    private bool _hasFinishedLookingAround = false;
    private bool _hasShrugFinished = false;

    private bool _combatTag = false; 



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
    }

    private void Update()
    {
        _fsm.UpdateFSM(); 
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
