using System.Collections.Generic;
using UnityEngine;

public class EnemyMelee : Enemy
{
    [Header("Weapon Settings")]
    [SerializeField] private GameObject _daggerHolster;
    [SerializeField] private GameObject _daggerInHand;
    [SerializeField] private Collider _daggerCollider;

    [Header("Attack Settings")]
    [SerializeField] private float _attackRange = 2f;
    [SerializeField] private float _attackCooldown = 1.5f;

    [Header("Gizmos")]
    [SerializeField] private bool _showAttackRange;

    private bool _isWeaponDrawn = false;
    private bool _isSheathingComplete = true;

    public bool IsWeaponDrawn => _isWeaponDrawn;
    public bool IsSheathingComplete => _isSheathingComplete;
    public float AttackCooldown => _attackCooldown;

    protected override void Awake()
    {
        base.Awake();
        _enemyFSM = new EnemyFSM();
        _enemyFSM.Initialize(this, new Dictionary<EStates, BaseState>
        {
            [EStates.MeleeIdle] = new MeleeIdleState(),
            [EStates.MeleeAlert] = new MeleeAlertState(),
            [EStates.MeleePatrol] = new MeleePatrolState(),
            [EStates.MeleeChase] = new MeleeChaseState(),
            [EStates.MeleeAttack] = new MeleeAttackState()
        },
        EStates.MeleeIdle);
    }

    private void Update()
    {
        _enemyFSM.UpdateFSM(); // Update the FSM each frame
    }

    public void ApplyMeleeSettings(MeleeEnemySettings settings)
    {
        _attackRange = settings.attackRange;
        _attackCooldown = settings.attackCooldown;
    }

    public void OnWeaponUnsheathed()
    {
        _daggerHolster.SetActive(false);
        _daggerInHand.SetActive(true);
    }

    public void OnWeaponDrawn()
    {
        _isWeaponDrawn = true;
        DisableWeaponHitBox();
    }

    public void OnWeaponSheathed()
    {
        _daggerHolster.SetActive(true);
        _daggerInHand.SetActive(false);
        _isSheathingComplete = true;
        _isWeaponDrawn = false;
    }

    public void ResetWeaponDrawn() => _isWeaponDrawn = false;
    public void ResetSheathingComplete() => _isSheathingComplete = false;
    public void SetSheathingComplete() => _isSheathingComplete = true;

    public void EnableWeaponHitBox() => _daggerCollider.enabled = true;
    public void DisableWeaponHitBox() => _daggerCollider.enabled = false;

    public bool CheckIfPlayerIsInAttackRange() =>
        Vector3.Distance(transform.position, PlayerTransform.position) <= _attackRange;

    public void SetIsInAttackRange(bool value) =>
        GetComponent<Animator>().SetBool("isInAttackRange", value);

    protected override void ResetToPatrol()
    {
        _enemyFSM.SwitchState(EStates.MeleePatrol);
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        if (_showAttackRange)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, _attackRange);
        }
    }
}
