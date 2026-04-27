using UnityEngine;

public class IdleState : BaseState
{
    
    private float _idleTimer; // Timer to keep track of how long the enemy has been idling

    public override void OnStateEnter()
    {
        _idleTimer = 0f; // Reset the idle timer when entering the idle state
    }
    public override void OnStateUpdate()
    {
        _idleTimer += Time.deltaTime; // Increment the idle timer by the time elapsed since the last frame
        if (_idleTimer >= _enemy.IdleTime) // Check if the idle time has elapsed
        {
            _fsm.SwitchState(EStates.Patrol); // Switch to the patrol state after the idle time has elapsed
        }
    }

    public override void OnStateExit()
    {
    }



}
