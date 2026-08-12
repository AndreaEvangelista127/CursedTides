using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Singleton because we want to access this class from other classes without having to reference it in the inspector
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _sfxSource;

    [Header("Music Clips")]
    [SerializeField] private AudioClip _menuMusic;
    [SerializeField] private AudioClip _gameMusic;

    [Header("SFX Clips")]
    [SerializeField] private AudioClip _buttonClick;
    [SerializeField] private AudioClip _playerSwordSlash;
    [SerializeField] private AudioClip _enemySwordSlash;
    [SerializeField] private AudioClip _gunShot;
    [SerializeField] private AudioClip _enemyDeath;
    [SerializeField] private AudioClip _playerDeath;
    [SerializeField] private AudioClip _gemCollect;
    [SerializeField] private AudioClip _collectiblePickUp;
    [SerializeField] private AudioClip _pedestalPlace;
    [SerializeField] private AudioClip _chestOpen;

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float MusicVolume = 1f;
    [Range(0f, 1f)] public float SFXVolume = 1f;

    [Header("Fade Settings")]
    [SerializeField] private float _fadeDuration = 1f;

    private Coroutine _fadeCoroutine;

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

    // --- MUSIC ---
    public void PlayMenuMusic() => PlayMusic(_menuMusic); 
    public void PlayGameMusic() => PlayMusic(_gameMusic);

    private void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        if (_musicSource.clip == clip) return;

        // Cancel any existing fade
        if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
        _fadeCoroutine = StartCoroutine(FadeMusic(clip));
    }

    // --- SFX ---
    public void PlayButtonClick() => PlaySFX(_buttonClick);
    public void PlayPlayerSwordSlash() => PlaySFX(_playerSwordSlash);
    public void PlayEnemySwordSlash() => PlaySFX(_enemySwordSlash);
    public void PlayGunShot() => PlaySFX(_gunShot);
    public void PlayEnemyDeath() => PlaySFX(_enemyDeath);
    public void PlayPlayerDeath() => PlaySFX(_playerDeath);
    public void PlayGemCollect() => PlaySFX(_gemCollect);
    public void PlayCollectiblePickup() => PlaySFX(_collectiblePickUp);
    public void PlayPedestalPlace() => PlaySFX(_pedestalPlace);
    public void PlayChestOpen() => PlaySFX(_chestOpen);

    private void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        if (_sfxSource != null)
        {
            _sfxSource.PlayOneShot(clip, SFXVolume);
        }
    }

    // --- VOLUME ---
    public void SetMusicVolume(float volume)
    {
        MusicVolume = volume;
        if(_musicSource != null)
        {
            _musicSource.volume = volume;
        }   
        // Player prefs?

    }

    public void SetSFXVolume(float volume) => SFXVolume = volume;

    // --- FADE MUSIC ---
    private IEnumerator FadeMusic(AudioClip newClip)
    {
        if (_musicSource == null) yield break;

        // Fade out current music
        float startVolume = _musicSource.volume;
        while (_musicSource.volume > 0)
        {
            _musicSource.volume -= startVolume * Time.deltaTime / _fadeDuration; // Decrease volume over time
            yield return null;
        }

        // Previous music is now silent, switch to new music
        _musicSource.Stop();
        _musicSource.clip = newClip;
        _musicSource.loop = true;
        _musicSource.Play();

        // Fade in new music
        while (_musicSource.volume < MusicVolume) // Increase volume over time until it reaches the desired MusicVolume
        {
            _musicSource.volume += MusicVolume * Time.deltaTime / _fadeDuration;
            yield return null;
        }

        _musicSource.volume = MusicVolume;
    }

}
