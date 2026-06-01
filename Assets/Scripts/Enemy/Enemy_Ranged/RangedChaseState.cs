using UnityEngine;

public class RangedChaseState : BaseRangedState
{
    private Vector3 _lastPosition;
    private bool _isGoingBackToLastPosition = false;

    public override void OnStateEnter()
    {
        _enemy.SetIfIsChasing(true);
        _lastPosition = _enemy.transform.position;
    }


    public override void OnStateUpdate()
    {
        //if (_enemyRanged.CheckIfInFOV())
        //{
        //    _fsm.SwitchState(EStates.RangedShoot);
        //    return;
        //}

        SaveLastPositionInRadius();

        bool hasReachedTheLimit = Vector3.Distance(_enemyRanged.PlayerTransform.position, _lastPosition) >= _enemyRanged.MaxChaseDistance;

        if (hasReachedTheLimit)
        {
            _isGoingBackToLastPosition = true;
        }

        if (_isGoingBackToLastPosition) //GO BACK TO THE LAST POSITION FOR PATROLLING
        {
            Vector3 lastPosDir = _lastPosition - _enemy.transform.position;
            Vector3 moveVector = lastPosDir.normalized * _enemyRanged.ChaseSpeed;

            _enemy.RotateToDirection(lastPosDir);
            _enemy.Rb.linearVelocity = moveVector;

            float distanceSqr = lastPosDir.sqrMagnitude; // Use squared magnitude for performance reasons

            if (distanceSqr < _enemy.DistanceBuffer * _enemy.DistanceBuffer) // If the enemy is within stopping distance of the last position, switch back to patrol
            {
                Debug.Log("Reached last position, switching back to patrol");
                _isGoingBackToLastPosition = false; // Reset the flag for the next time the enemy enters the chase state
                _enemy.Rb.linearVelocity = Vector3.zero; // Stop the enemy's movement
                _enemyRanged.SetCombatTag(false); // Reset the combat tag
                _fsm.SwitchState(EStates.RangedPatrol);
                return;
            }
        }
        else //CHASE THE PLAYER
        {
            _enemy.MoveTowardsPlayer();
        }
    }
    public override void OnStateExit()
    {
        _enemy.SetIfIsChasing(false);
    }

    private void SaveLastPositionInRadius()
    {
        //Distance between the enemy and the origin of the patrol radius
        float distanceFromOrigin = Vector3.Distance(_enemy.transform.position, _enemyRanged.PatrolOrigin);

        //If it's more than the patrol radius value, means that the enemy stepped outside of the patrol radius, so we save the last position where it was still inside the patrol radius
        if (distanceFromOrigin <= _enemyRanged.PatrolRadius)
        {
            _lastPosition = _enemy.transform.position;
        }
    }
}
