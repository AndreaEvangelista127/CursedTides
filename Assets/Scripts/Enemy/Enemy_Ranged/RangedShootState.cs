using UnityEngine;

public class RangedShootState : BaseRangedState
{
    private float _shootTimer = 0f; // Timer to track the duration of the shoot animation
    private float _animationTime = 1.5f;

    public override void OnStateEnter()
    {
        _enemyRanged.Rb.linearVelocity = Vector3.zero; // Stop the enemy's movement when entering the shoot state
        _enemyRanged.SetCombatTag(true); // Set the combat tag to true when entering the shoot state
        _enemyRanged.GetComponent<Animator>().Play("Shoot");
    }
    public override void OnStateUpdate()
    {
        _shootTimer += Time.deltaTime;
        _enemyRanged.RotateToDirection(_enemyRanged.PlayerTransform.position - _enemy.transform.position); // Rotate towards the player while shooting

        if (_shootTimer >= _animationTime) 
        {
            if (_enemyRanged.CheckIfInFOV())
            {
                if (_shootTimer >= _enemyRanged.ShootCooldown)
                {
                    _shootTimer = 0f;
                    _enemyRanged.GetComponent<Animator>().Play("Shoot");
                    //AudioManager.Instance?.PlayGunShot();
                }
            }
            else
            {
                _fsm.SwitchState(EStates.RangedChase);
            }
        }
    }

    public override void OnStateExit()
    {
    }



}
