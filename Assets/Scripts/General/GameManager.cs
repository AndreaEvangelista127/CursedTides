using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int _totalPedestals;
    [SerializeField] private float _timeLimit;
    [SerializeField] private GameConditions _gameConditions;

    private int _completedPedestals;

    public float TimeLimit => _timeLimit;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        _gameConditions.SetTimeLimit(_timeLimit);
    }

    public void OnPedestalCompleted()
    {
        _completedPedestals++;
        if (_completedPedestals >= _totalPedestals)
        {
            Victory();
        }
    }

    private void Victory()
    {
        _gameConditions.ShowVictoryScreen();
    }

    public void OnPlayerDeath()
    {
        _gameConditions.ShowLoseScreen();
    }
}
