using UnityEngine;

public class MeleeAttackState : BaseMeleeState
{
    
    private float _attackTimer; // Timer to track the duration of the attack animation
    private float _animationTime = 1.5f;


    public override void OnStateEnter()
    {
        _attackTimer = 0f; // Reset timer on enter
        _enemy.SetIfIsChasing(true);
        _enemy.Rb.linearVelocity = Vector3.zero; // Stop the enemy's movement when it enters the attack state
        _enemyMelee.DisableWeaponHitBox();
        _enemyMelee.GetComponent<Animator>().Play("Attack_Horizontal");

    }
    public override void OnStateUpdate()
    {
        _attackTimer += Time.deltaTime; // Increment the attack timer

        if (_attackTimer >= _animationTime)
        {
            if (_enemyMelee.CheckIfPlayerIsInAttackRange())
            {
                _enemyMelee.SetIsInAttackRange(true);
                _enemy.RotateToDirection(_enemyMelee.PlayerTransform.position - _enemy.transform.position); // Rotate towards the player while attacking
                // ATTACK
                if (_attackTimer >= _enemyMelee.AttackCooldown)
                {
                    _attackTimer = 0f;
                    _enemy.GetComponent<Animator>().Play("Attack_Horizontal");
                }
            }
            else // we are not in the attack range anymore, we should switch back to chase state
            {
                _enemyMelee.SetIsInAttackRange(false);
                _enemyMelee.GetComponent<Animator>().Play("Chase");
                _fsm.SwitchState(EStates.MeleeChase);
            }

        }
    }

    public override void OnStateExit()
    {
    }

}
