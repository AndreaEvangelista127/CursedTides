using System;
using Unity.Mathematics;
using UnityEngine;


public class MeleeAlertState : BaseMeleeState
{
    private float _alertTimer;
    
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
}
