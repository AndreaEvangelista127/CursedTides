using UnityEngine;

public class MeleeIdleState : BaseMeleeState
{
    
    private float _idleTimer; // Timer to keep track of how long the enemy has been idling

    public override void OnStateEnter()
    {
        _idleTimer = 0f; // Reset the idle timer when entering the idle state
        Debug.Log("Entering Idle State");
    }
    public override void OnStateUpdate()
    {
        if (_enemy.CheckIfInFOV())
        {
            _fsm.SwitchState(EStates.MeleeChase);
            return;
        }

        if (_enemy.CheckIfInDetectionRange())
        {
            _fsm.SwitchState(EStates.MeleeAlert);
            return;
        }


        _idleTimer += Time.deltaTime; // Increment the idle timer by the time elapsed since the last frame
        if (_idleTimer >= _enemy.IdleTime) // Check if the idle time has elapsed
        {
            _fsm.SwitchState(EStates.MeleePatrol); // Switch to the patrol state after the idle time has elapsed
        }
    }

    public override void OnStateExit()
    {
    }



}
