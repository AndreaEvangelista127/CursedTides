using UnityEngine;

public class RangedIdleState : BaseRangedState
{
    private float _idleTimer;

    public override void OnStateEnter()
    {
        _idleTimer = 0;
    }
    public override void OnStateUpdate()
    {
        _idleTimer += Time.deltaTime;

        if(_idleTimer >= _enemy.IdleTime)
        {
            _fsm.SwitchState(EStates.RangedPatrol);
        }
    }

    public override void OnStateExit()
    {
    }


    
}
