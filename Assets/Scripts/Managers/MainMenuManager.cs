using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject _settingsPanel;


    private void Start()
    {
        AudioManager.Instance?.PlayMenuMusic();
    }
    public void OnPlayButton()
    {
        AudioManager.Instance?.PlayGameMusic();
        AudioManager.Instance?.PlayButtonClick();
        SceneManager.LoadScene("GameScene"); 
    }

    public void OnSettingsButton()
    {
        AudioManager.Instance?.PlayButtonClick();
        _settingsPanel.SetActive(true);
    }

    public void OnQuitButton()
    {
        AudioManager.Instance?.PlayButtonClick();
        Application.Quit();
        Debug.Log("Quit Game");
    }
}
