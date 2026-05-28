using UnityEngine;

public class EnenmyRanged : Enemy
{
    private EnemyFSM _fsm;

    private void Awake()
    {
        base.Awake(); 
        _fsm = new EnemyFSM();
        _fsm.Initialize(this); 
    }

    private void Update()
    {
        _fsm.UpdateFSM(); 
    }
}
