using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int _totalPedestals;
    [SerializeField] private GameConditions _gameConditions;

    private int _completedPedestals;

    public static GameManager Istance { get; private set; }

    private void Awake()
    {
        if (Istance == null) Istance = this;
        else Destroy(gameObject);
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
}
