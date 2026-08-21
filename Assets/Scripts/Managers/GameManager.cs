using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int _totalPedestals;
    [SerializeField] private float _timeLimit;
    [SerializeField] private GameConditions _gameConditions;

    [SerializeField] private LoadingScreen _loadingScreen;
    [SerializeField] private TerrainGenerator _terrainGenerator;

    public bool IsGameStarted { get; private set; }

    private int _completedPedestals;

    public float TimeLimit => _timeLimit;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        _gameConditions.SetTimeLimit(_timeLimit);

        IsGameStarted = false;

        if (_loadingScreen != null && _terrainGenerator != null)
            StartCoroutine(StartGameCoroutine());
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

    public IEnumerator StartGameCoroutine()
    {
        IsGameStarted = false;

        _loadingScreen.gameObject.SetActive(true);
        _loadingScreen.StartAnimation();

        yield return _terrainGenerator.StartCoroutineGenerate();
        Debug.Log("finally really done");

        _loadingScreen.gameObject.SetActive(false);

        IsGameStarted = true;
    }

}
