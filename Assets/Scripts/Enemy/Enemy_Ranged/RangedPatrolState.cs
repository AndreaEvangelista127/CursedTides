using UnityEngine;

public class RangedPatrolState : BaseRangedState
{
    private Vector3 _destination; // The destination point the enemy will move towards while patrolling

    public override void OnStateEnter()
    {
        _destination = _enemy.GetRandomPatrolPoint();
        _enemyRanged.SetIfIsPatrolling(true); // Set the isPatrolling flag to true in the animator when entering the patrol state
    }
    public override void OnStateUpdate()
    {
        if (_enemy.CheckIfInFOV())
        {
            Debug.Log("Player in FOV");
            _fsm.SwitchState(EStates.RangedChase);
            return;
        }

        if (_enemy.CheckIfInDetectionRange())
        {
            Debug.Log("Player is around me");
            _fsm.SwitchState(EStates.RangedAlert);
            return;
        }

        Vector3 direction = _destination - _enemy.transform.position; // Get the direction from the enemy to the destination

        // Ignore Y difference � if we include Y in the direction, the enemy would fly 
        // towards the player in a straight line instead of following the terrain.
        direction.y = 0;

        Vector3 moveVector = direction.normalized * _enemy.MoveSpeed; // Calculate the movement vector based on the enemy's move speed and the time elapsed since the last frame

        // Preserve the Y velocity calculated by Unity physics (gravity, slope collisions).
        // Without this, we would overwrite the physics Y with 0, causing the enemy to 
        // float on slopes or not fall correctly when walking off edges.
        moveVector.y = _enemy.Rb.linearVelocity.y;

        _enemy.RotateToDirection(direction); //Rotate each frame to face the direction of movement
        _enemy.Rb.linearVelocity = moveVector; // Set the enemy's velocity to move towards the destination

        // Check if the enemy is within the stopping distance of the destination
        float distanceSqr = direction.sqrMagnitude; // Use squared magnitude for performance reasons
        if (distanceSqr < _enemy.DistanceBuffer * _enemy.DistanceBuffer) // Compare with the squared stopping distance
        {
            _enemy.Rb.linearVelocity = Vector3.zero; // Stop the enemy's movement
            _fsm.SwitchState(EStates.RangedIdle); // Switch to the idle state after reaching the destination
        }
    }

    public override void OnStateExit()
    {
        _enemyRanged.SetIfIsPatrolling(false); // Set the isPatrolling flag to false in the animator when exiting the patrol state
    }


}
