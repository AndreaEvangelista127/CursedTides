using UnityEngine;

public class EnemyMelee : Enemy
{
    private EnemyFSM _fsm; // Reference to the enemy's finite state machine (FSM)

    private void Awake()
    {
        base.Awake(); // Calling the base for Rigidbody, enemyOrigin and playerOrigin initialization
        _fsm = new EnemyFSM();
        _fsm.Initialize(this); // Initialize the FSM with the current enemy melee instance
    }

    private void Update()
    {
        _fsm.UpdateFSM(); // Update the FSM each frame
    }
}
