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

        _destination = GetRandomPatrolPoint();
        _enemyMelee.SetIfIsPatrolling(true);
        Debug.Log("Entering Patrol State, new destination: " + _destination);
    }

    public override void OnStateUpdate()
    {
        //if the enemy is still sheathing its weapon, it should not move or check for the player
        if (!_enemyMelee.IsSheathingComplete) return;

        if (_enemyMelee.CheckIfInFOV())
        {
            Debug.Log("Player in FOV");
            _fsm.SwitchState(EStates.MeleeChase);
            return;
        }

        if (_enemyMelee.CheckIfInDetectionRange())
        {
            Debug.Log("Player is around me");
            _fsm.SwitchState(EStates.MeleeAlert);
            return;
        }

        Vector3 direction = _destination - _enemy.transform.position; // Get the direction from the enemy to the destination
        Vector3 moveVector = direction.normalized * _enemy.MoveSpeed; // Calculate the movement vector based on the enemy's move speed and the time elapsed since the last frame

        _enemy.RotateToDirection(direction);

        _enemy.Rb.linearVelocity = moveVector;

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
       _enemyMelee.SetIfIsPatrolling(false);
    }

    private Vector3 GetRandomPatrolPoint()
    {
        // Get a random point within a sphere with the radius of the patrol radius
        Vector3 randomDest = UnityEngine.Random.insideUnitSphere * _enemyMelee.PatrolRadius + _enemyMelee.PatrolOrigin; //Added patrol origin so that the enemy patrols around its initial position instead of around the world origin

        randomDest.y = 0; // The enemy should only patrol on the xz plane, so we set the y component to 0

        return randomDest;
    }

    








}
