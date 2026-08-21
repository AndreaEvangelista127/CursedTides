using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private GameObject _settingsPanel;

    [SerializeField] private Slider _musicSlider;
    [SerializeField] private TextMeshProUGUI _valueMusicSlider;

    [SerializeField] private Slider _sfxSlider;
    [SerializeField] private TextMeshProUGUI _valueSfxSlider;
    public static SettingsManager Instance { get; private set; }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }


    }

    private void Start()
    {
        _musicSlider.onValueChanged.AddListener((value) =>
        {
            _valueMusicSlider.text = value.ToString("0.00");
            AudioManager.Instance?.SetMusicVolume(value);
        });

        _sfxSlider.onValueChanged.AddListener((value) =>
        {
            _valueSfxSlider.text = value.ToString("0.00");
            AudioManager.Instance?.SetSFXVolume(value);
        });
    }

    public void OnBackToMainMenuButton()
    {
        AudioManager.Instance?.PlayButtonClick();

        if(_settingsPanel != null) _settingsPanel.SetActive(false);

        Time.timeScale = 1f;

        if (GameManager.Instance != null)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }



    public void OpenSettings()
    {
        if(_settingsPanel == null) return;
        _settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (_settingsPanel == null) return;
        _settingsPanel.SetActive(false);
    }

    // This method toggles the settings panel on and off
    public void ToggleSettings()
    {
        if (_settingsPanel == null) return;

        bool isActive = !_settingsPanel.activeSelf;
        _settingsPanel.SetActive(isActive);
        Time.timeScale = isActive ? 0f : 1f;
        Cursor.lockState = isActive ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isActive;
    }

}
