using System;
using Unity.Mathematics;
using UnityEngine;


public class MeleeAlertState : BaseMeleeState
{
    private float _alertTimer;
    //private float _rotationDirection = 1f; //Initially rotating to the right

    //private Quaternion _targetRotation;
    //private bool _isRotating = false;
    //private float _pauseTime = 0f;
    //private float _waitTime = 0.8f;
    

    public override void OnStateEnter()
    {
        _enemy.SetIfIsInAlert(true);
        _alertTimer = 0f;
        _enemy.Rb.linearVelocity = Vector3.zero; // Stop the enemy's movement when entering the alert state
        Debug.Log("Entering Alert State");
    }
    public override void OnStateUpdate()
    {
        bool playerInRange = _enemy.CheckIfInDetectionRange();
        bool playerInFOV = _enemy.CheckIfInFOV();

        if (playerInRange)
        {
            _alertTimer += Time.deltaTime;
            if (_alertTimer >= _enemy.AlertTime)
            {
                _fsm.SwitchState(EStates.MeleeChase);
                return;
            }
        }
        else
        {
            _fsm.SwitchState(EStates.MeleePatrol);
            return;
        }

        if(playerInFOV)
        {
            _fsm.SwitchState(EStates.MeleeChase);
            return;
        }
    }

    public override void OnStateExit()
    {
        _enemy.SetIfIsInAlert(false);
    }

    // Rotate the enemy to a random angle within the specified rotation range
    //private void RotateTowardsTarget()
    //{
    //    _enemy.transform.rotation = Quaternion.RotateTowards(_enemy.transform.rotation, _targetRotation, 90f * Time.deltaTime);

    //    // Check if the enemy has reached the target rotation
    //    if (Quaternion.Angle(_enemy.transform.rotation, _targetRotation) < 1f)
    //    {
    //        _pauseTime = _waitTime;
    //        _isRotating = false;
    //        _rotationDirection *= -1;
    //    }
    //}

    //private void WaitTimeBeforeRotation(float timeToWait)
    //{
    //    _pauseTime -= Time.deltaTime;

    //    // When enemy waited enough he can generate the new rotation
    //    if (_pauseTime <= 0)
    //    {
    //        _isRotating = true;
    //        float randomAngle = UnityEngine.Random.Range(_enemy.MinRotation, _enemy.MaxRotation) * _rotationDirection;
    //        _targetRotation = _enemy.transform.rotation * Quaternion.Euler(0, randomAngle, 0);
    //    }
    //}


}
