using System;
using UnityEngine;

public class PatrolState : BaseState
{
    private Vector3 _destination; // The destination point the enemy will move towards while patrolling

    public override void OnStateEnter()
    {
        _destination = GetRandomPatrolPoint();
    }

    public override void OnStateUpdate()
    {
        Vector3 direction = _destination - _enemy.transform.position; // Get the direction from the enemy to the destination
        Vector3 moveVector = direction.normalized * _enemy.MoveSpeed; // Calculate the movement vector based on the enemy's move speed and the time elapsed since the last frame

        _enemy.EnemyRb.linearVelocity = moveVector;

        // Check if the enemy is within the stopping distance of the destination
        float distanceSqr = direction.sqrMagnitude; // Use squared magnitude for performance reasons

        if(distanceSqr < _enemy.StoppingDistance * _enemy.StoppingDistance) // Compare with the squared stopping distance
        {
            _enemy.EnemyRb.linearVelocity = Vector3.zero; // Stop the enemy's movement
            _fsm.SwitchState(EStates.Idle); // Switch to the idle state after reaching the destination
        }
    }

    public override void OnStateExit()
    {
       
    }


    private Vector3 GetRandomPatrolPoint()
    {
        // Get a random point within a sphere with the radius of the patrol radius
        Vector3 randomDest = UnityEngine.Random.insideUnitSphere * _enemy.PatrolRadius + _enemy.PatrolOrigin; //Added patrol origin so that the enemy patrols around its initial position instead of around the world origin

        randomDest.y = 0; // The enemy should only patrol on the xz plane, so we set the y component to 0

        return randomDest;
    }


}
