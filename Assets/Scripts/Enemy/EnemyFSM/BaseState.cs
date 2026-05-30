using UnityEngine;

public abstract class BaseState
{

    protected Enemy _enemy;
    protected EnemyFSM _fsm;

    // Method used only to setup the dictionary in the FSM for <EState, Enemy>
    public virtual void Setup(Enemy enemy, EnemyFSM fsm)
    {
        _enemy = enemy;
        _fsm = fsm;
    }

    public abstract void OnStateEnter();

    public abstract void OnStateExit();

    public abstract void OnStateUpdate();
}
