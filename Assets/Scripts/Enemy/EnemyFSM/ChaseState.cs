using UnityEngine;

public class ChaseState : BaseState
{

    private Vector3 _lastPosition; 
    private bool _isGoingBackToLastPosition = false; // Flag to indicate whether the enemy is currently going back to the last position
    

    public override void OnStateEnter()
    {
        _enemy.SetIfIsChasing(true);
        Debug.Log("Entering Chase State, last position: " + _lastPosition);
    }

    public override void OnStateUpdate()
    {
        SaveLastPositionInRadius(); // Here we have 

        bool hasReachedTheLimit = Vector3.Distance(_enemy.PlayerTransform.position, _lastPosition) > _enemy.MaxChaseDistance;
        // If it's too far away from the last position where it still was inside the detection range, go back to patrol
        
        //NEED TO ADD EVEN THE 
        if(hasReachedTheLimit)
        {
            Debug.Log("Player is too far away from last position, going back to last position in MoveRadius");
            _isGoingBackToLastPosition = true;
        }

        if (_isGoingBackToLastPosition)
        {
            Vector3 lastPosDir = _lastPosition - _enemy.transform.position;
            Vector3 moveVector = lastPosDir.normalized * (_enemy.MoveSpeed * 2);

            _enemy.RotateToDirection(lastPosDir);
            _enemy.Rb.linearVelocity = moveVector;

            float distanceSqr = lastPosDir.sqrMagnitude; // Use squared magnitude for performance reasons

            if (distanceSqr < _enemy.DistanceBuffer * _enemy.DistanceBuffer) // If the enemy is within stopping distance of the last position, switch back to patrol
            {
                Debug.Log("Reached last position, switching back to patrol");
                _isGoingBackToLastPosition = false; // Reset the flag for the next time the enemy enters the chase state
                _enemy.Rb.linearVelocity = Vector3.zero; // Stop the enemy's movement
                _fsm.SwitchState(EStates.Patrol);
                return;
            }
        }
        else
        {
            // Run towards the player
            MoveTowardsPlayer();
        }

    }

    public override void OnStateExit()
    {
        _enemy.SetIfIsChasing(false);
    }

    private void SaveLastPositionInRadius()
    {
        //Distance between the enemy and the origin of the patrol radius
        float distanceFromOrigin = Vector3.Distance(_enemy.transform.position, _enemy.PatrolOrigin);

        //If it's more than the patrol radius value, means that the enemy stepped outside of the patrol radius, so we save the last position where it was still inside the patrol radius
        if (distanceFromOrigin <= _enemy.PatrolRadius)
        {
            _lastPosition = _enemy.transform.position;
        }
    }

    private void MoveTowardsPlayer()
    {
        Vector3 direction = _enemy.PlayerTransform.position - _enemy.transform.position; // Get the direction from the enemy to the player
        Vector3 moveVector = direction.normalized * _enemy.MoveSpeed; // Calculate the movement vector based on the enemy's move speed and the time elapsed since the last frame
        moveVector.y = 0;
        _enemy.RotateToDirection(direction);

        _enemy.Rb.linearVelocity = moveVector;
    }

}
