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
        _musicSlider.onValueChanged.AddListener((value) => {
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
        _settingsPanel.SetActive(false);
    }


}
