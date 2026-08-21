using System.Collections.Generic;
using UnityEngine;

public class EnemyFSM
{

    // No MonoBehaviour inheritance here, this class will be used as a component of the Enemy class to manage its state and behavior.

    private Dictionary<EStates, BaseState> _states; // A dictionary to hold the different states of the enemy

    private BaseState _currentState; // The current state of the enemy

    public void Initialize(Enemy enemy, Dictionary<EStates, BaseState> states, EStates initialState)
    {
        _states = states;

        foreach (BaseState state in _states.Values)
        {
            state.Setup(enemy, this);
        }

        _currentState = _states[initialState];
        _currentState.OnStateEnter();
    }

    // Method to change the current state of the enemy
    public void UpdateFSM()
    {
        if (GameManager.Instance != null)
        {
            if(!GameManager.Instance.IsGameStarted)
            {
                return; // If the game is not started, do not update the FSM
            }
            else
            {
                Debug.Log("Game has started, updating FSM");
            }
        }

        _currentState.OnStateUpdate(); // Call the update method of the current state
    }

    public void SwitchState(EStates targetState)
    {
        _currentState.OnStateExit(); // Call the exit method of the current state
        _currentState = _states[targetState]; // Switch to the new state
        _currentState.OnStateEnter(); // Call the enter method of the new state
    }


}
