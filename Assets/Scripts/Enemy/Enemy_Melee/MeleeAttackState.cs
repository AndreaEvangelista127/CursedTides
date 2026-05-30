using UnityEngine;

public class MeleeAttackState : BaseMeleeState
{
    
    private float _attackTimer; // Timer to track the duration of the attack animation


    public override void OnStateEnter()
    {
        _attackTimer = 0f; // Reset timer on enter
        _enemy.SetIfIsChasing(true);
        _enemy.Rb.linearVelocity = Vector3.zero; // Stop the enemy's movement when it enters the attack state
        _enemyMelee.DisableWeaponHitBox();
        _enemy.GetComponent<Animator>().SetTrigger("attack");

    }
    public override void OnStateUpdate()
    {
       if(_enemyMelee.CheckIfPlayerIsInAttackRange())
        {
            _enemyMelee.SetIsInAttackRange(true);
            _enemy.RotateToDirection(_enemyMelee.PlayerTransform.position - _enemy.transform.position); // Rotate towards the player while attacking

            _attackTimer += Time.deltaTime; // Increment the attack timer

            if (_attackTimer >= _enemyMelee.AttackCooldown)
            {
                _attackTimer = 0f;
                _enemy.GetComponent<Animator>().SetTrigger("attack");
            }
        }
        else
        {
            Debug.Log("Player is out of attack range, switching back to chase state.");
            Debug.Log("Enemy dagger is drawn?" + _enemyMelee.IsWeaponDrawn);
            _enemyMelee.SetIsInAttackRange(false);
            _fsm.SwitchState(EStates.MeleeChase); // If the player is out of attack range, switch back to chase state
        }
    }

    public override void OnStateExit()
    {
    }

}
