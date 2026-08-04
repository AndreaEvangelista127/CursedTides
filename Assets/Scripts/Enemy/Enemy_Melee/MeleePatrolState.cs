using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class MeleePatrolState : BaseMeleeState
{
    private Vector3 _destination; // The destination point the enemy will move towards while patrolling

    public override void OnStateEnter()
    {
        if (_enemyMelee.IsWeaponDrawn)
        {
            _enemyMelee.ResetSheathingComplete();
            _enemy.GetComponent<Animator>().SetTrigger("sheathing");
            _enemy.Rb.linearVelocity = Vector3.zero; // Stop the enemy's movement while sheathing its weapon
        }
        else
        {
            _enemyMelee.SetSheathingComplete();
        }

        _destination = _enemy.GetRandomPatrolPoint();
        _enemy.SetIfIsPatrolling(true);
        //Debug.Log("Entering Patrol State, new destination: " + _destination);
    }

    public override void OnStateUpdate()
    {
        //if the enemy is still sheathing its weapon, it should not move or check for the player
        if (!_enemyMelee.IsSheathingComplete) return;

        if (_enemy.CheckIfInFOV())
        {
            Debug.Log("Player in FOV");
            _fsm.SwitchState(EStates.MeleeChase);
            return;
        }

        if (_enemy.CheckIfInDetectionRange())
        {
            Debug.Log("Player is around me");
            _fsm.SwitchState(EStates.MeleeAlert);
            return;
        }

        Vector3 direction = _destination - _enemy.transform.position; // Get the direction from the enemy to the destination
        direction.y = 0; // Ignore vertical movement for patrolling
        Vector3 moveVector = direction.normalized * _enemy.MoveSpeed; // Calculate the movement vector based on the enemy's move speed and the time elapsed since the last frame
        moveVector.y = _enemy.Rb.linearVelocity.y; // 

        _enemy.RotateToDirection(direction); //Rotate each frame to face the direction of movement
        _enemy.Rb.linearVelocity = moveVector; // Set the enemy's velocity to move towards the destination

        // Check if the enemy is within the stopping distance of the destination
        float distanceSqr = direction.sqrMagnitude; // Use squared magnitude for performance reasons
        if(distanceSqr < _enemy.DistanceBuffer * _enemy.DistanceBuffer) // Compare with the squared stopping distance
        {
            _enemy.Rb.linearVelocity = Vector3.zero; // Stop the enemy's movement
            _fsm.SwitchState(EStates.MeleeIdle); // Switch to the idle state after reaching the destination
        }
    }

    public override void OnStateExit()
    {
        _enemy.SetIfIsPatrolling(false);

    }
    








}
