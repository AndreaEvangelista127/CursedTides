using System.Collections.Generic;
using UnityEngine;

public class EnemyRanged : Enemy
{
    [Header("Ranged Settings")]
    [SerializeField] private float _shootRange = 10f;
    [SerializeField] private float _tooCloseRange = 3f;
    [SerializeField] private float _shootCooldown = 2f;

    public float ShootRange => _shootRange;
    public float TooCloseRange => _tooCloseRange;
    public float ShootCooldown => _shootCooldown;

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
}
