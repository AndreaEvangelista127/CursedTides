using UnityEngine;
using UnityEngine.SceneManagement;

public class GameConditions : MonoBehaviour
{
    [SerializeField] private GameObject _victoryPanel;
    [SerializeField] private GameObject _loseScreen;
    private float _timeLimit;

    private bool _gameOver;

    private void Update()
    {
        if (_gameOver) return;

        if (Time.timeSinceLevelLoad >= _timeLimit)
        {
            ShowLoseScreen();
        }
    }

    public void SetTimeLimit(float timeLimit)
    {
        _timeLimit = timeLimit;
    }

    public void ShowLoseScreen()
    {
        _gameOver = true;
        _loseScreen.SetActive(true);
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ShowVictoryScreen()
    {
        _victoryPanel.SetActive(true);
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OnRestartButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }
}
