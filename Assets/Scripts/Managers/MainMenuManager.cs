using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField _seedInputField;


    private void Start()
    {
        AudioManager.Instance?.PlayMenuMusic();
    }
    public void OnPlayButton()
    {
        if (string.IsNullOrEmpty(_seedInputField.text))
        {
            // Generate a random seed
            int randomSeed = Random.Range(0, int.MaxValue);
            PlayerPrefs.SetInt("Seed", randomSeed);
        }
        else
        {
            // Use the seed from the input field
            PlayerPrefs.SetInt("Seed", int.Parse(_seedInputField.text)); // Convert the string to an integer and store it in PlayerPrefs
        }

        PlayerPrefs.Save(); // Save the PlayerPrefs to ensure the seed is stored

        AudioManager.Instance?.PlayGameMusic();
        AudioManager.Instance?.PlayButtonClick();
        SceneManager.LoadScene("GameScene"); 
    }

    public void OnSettingsButton()
    {
        AudioManager.Instance?.PlayButtonClick();
        SettingsManager.Instance?.ToggleSettings();
    }

    public void OnQuitButton()
    {
        AudioManager.Instance?.PlayButtonClick();
        Application.Quit();
    }
}
