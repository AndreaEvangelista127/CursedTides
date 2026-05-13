using System;
using Unity.Mathematics;
using UnityEngine;


public class AlertState : BaseState
{
    private float _alertTimer;
    private float _rotationDirection = 1f; //Initially rotating to the right

    private Quaternion _targetRotation;
    private bool _isRotating = false;
    private float _pauseTime = 0f;
    private float _waitTime = 0.8f;
    

    public override void OnStateEnter()
    {
        _alertTimer = 0f;
        _enemy.EnemyRb.linearVelocity = Vector3.zero;
    }
    public override void OnStateUpdate()
    {
        //Always check if the player is in FOV
        if (_enemy.CheckIfInFOV())
        {
            _fsm.SwitchState(EStates.Chase);
        }

        //The Player is not in FOV go back to Patrol
        _alertTimer += Time.deltaTime;
        if( _alertTimer >= _enemy.AlertTime)
        {
            _fsm.SwitchState(EStates.Patrol);
        }
        
        // ---ALARM STATE---
        if (_isRotating)
        {
            RotateTowardsTarget();
        }
        else
        {
            WaitTimeBeforeRotation(_waitTime);
        }
    }

    public override void OnStateExit()
    {
        _fsm.SwitchState(EStates.Patrol);
    }

    private void RotateTowardsTarget()
    {
        _enemy.transform.rotation = Quaternion.RotateTowards(_enemy.transform.rotation, _targetRotation, 90f * Time.deltaTime);

        if (Quaternion.Angle(_enemy.transform.rotation, _targetRotation) < 1f)
        {
            _pauseTime = _waitTime;
            _isRotating = false;
            _rotationDirection *= -1;
        }
    }

    private void WaitTimeBeforeRotation(float timeToWait)
    {
        _pauseTime -= Time.deltaTime;

        // When enemy waited enough he can generate the new rotation
        if (_pauseTime <= 0)
        {
            _isRotating = true;
            float randomAngle = UnityEngine.Random.Range(_enemy.MinRotation, _enemy.MaxRotation) * _rotationDirection;
            _targetRotation = _enemy.transform.rotation * Quaternion.Euler(0, randomAngle, 0);
        }
    }


}
