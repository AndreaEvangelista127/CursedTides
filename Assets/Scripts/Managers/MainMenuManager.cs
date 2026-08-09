using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject _settingsPanel;

    public void OnPlayButton()
    {
        SceneManager.LoadScene("GameScene"); 
    }

    public void OnSettingsButton()
    {
        _settingsPanel.SetActive(true);
    }

    public void OnQuitButton()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }
}
