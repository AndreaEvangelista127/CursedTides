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
        if(_loseScreen == null) return;
        _gameOver = true;
        _loseScreen.SetActive(true);
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ShowVictoryScreen()
    {
        if(_victoryPanel == null) return;

        _victoryPanel.SetActive(true);
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OnMainMenuButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }
}
