using UnityEngine;

public abstract class BaseMeleeState : BaseState
{
    protected EnemyMelee _enemyMelee;

    public override void Setup(Enemy enemy, EnemyFSM fsm)
    {
        base.Setup(enemy, fsm);
        _enemyMelee = (EnemyMelee)enemy;
    }
}
