using UnityEngine;

public class RangedAlertState : BaseRangedState
{
    private Vector3 _lastKnownPlayerPosition; // The last known position of the player when the enemy enters the alert state
    private float _totalAlertTimer; // The total time the enemy has been in the alert state
    private bool _hasReachedPosition; // Flag to check if the enemy has reached the last known player position
    private bool _isShrugStarted; 


    public override void OnStateEnter()
    {
        _lastKnownPlayerPosition = _enemy.PlayerTransform.position;
        _totalAlertTimer = 0f;
        _hasReachedPosition = false;
        _isShrugStarted = false;
        _enemyRanged.ResetLookAroundFinished();
        _enemy.Rb.linearVelocity = Vector3.zero; // Stop the enemy's movement when entering the alert state
        _enemy.GetComponent<Animator>().SetTrigger("alert");
        Debug.Log("Entering Alert State, trigger sent");

    }
    public override void OnStateUpdate()
    {
        _totalAlertTimer += Time.deltaTime;

        if (_enemy.CheckIfInFOV())
        {
            Debug.Log("Player spotted in FOV during Alert State, switching to Chase State");
            _fsm.SwitchState(EStates.RangedChase);
            return;
        }

        // Player stayed inside the alert radius for too long, switch to chase state
        if (_totalAlertTimer >= _enemy.AlertTime) 
        {
            Debug.Log("Alert time exceeded, switching to Chase State");
            _fsm.SwitchState(EStates.RangedChase); 
            return;
        }

        // Wait for look around animation to finish
        if (!_enemyRanged.HasFinishedLookingAround) return;

        if (!_hasReachedPosition)
        { 
            _enemy.SetIfIsPatrolling(true); // Activate the walk animation towards the last known player position
            Vector3 direction = _lastKnownPlayerPosition - _enemy.transform.position;
            direction.y = 0; // Ignore vertical difference

            if (direction.sqrMagnitude < _enemy.DistanceBuffer * _enemy.DistanceBuffer)
            {
                _enemy.SetIfIsPatrolling(false);
                _hasReachedPosition = true; // Enemy has reached the last known player position
                _enemy.Rb.linearVelocity = Vector3.zero; // Stop the enemy's movement
                return;
            }
            // If we haven't reached the last known player position, keep moving towards it

            _enemy.RotateToDirection(direction);
            _enemy.Rb.linearVelocity = direction.normalized * _enemy.MoveSpeed;
            return;
        }


        // The enemy has reached the last known player position, decide what to do
        if (_enemy.CheckIfInDetectionRange())
        {
            // Player is still within detection range
            _lastKnownPlayerPosition = _enemy.PlayerTransform.position;
            _hasReachedPosition = false; // Reset the flag to move towards the new last known player position
            _enemyRanged.ResetLookAroundFinished(); // Reset the look around animation to start again
            _enemy.GetComponent<Animator>().SetTrigger("alert"); // Trigger the alert animation again
        }
        else
        {
            // *** SHRUG AND PATROL ***
            if (!_isShrugStarted)
            {
                _isShrugStarted = true; // NEW — trigger only once
                _enemy.Rb.linearVelocity = Vector3.zero;
                _enemy.GetComponent<Animator>().SetTrigger("shrug");
            }

            _fsm.SwitchState(EStates.RangedPatrol);
        }


    }

    public override void OnStateExit()
    {
        _enemy.SetIfIsPatrolling(false);
        _enemy.Rb.linearVelocity = Vector3.zero;
        _enemyRanged.ResetLookAroundFinished();
        _enemy.GetComponent<Animator>().ResetTrigger("alert");
        _enemy.GetComponent<Animator>().ResetTrigger("shrug"); 
    }

    


}
