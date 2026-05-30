using UnityEngine;

public abstract class BaseRangedState : BaseState
{
    protected EnemyRanged _enemyRanged;

    public override void Setup(Enemy enemy, EnemyFSM fsm)
    {
        base.Setup(enemy, fsm);
        _enemyRanged = (EnemyRanged)enemy;
    }
}
