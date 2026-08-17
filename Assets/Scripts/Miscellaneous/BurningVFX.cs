using UnityEngine;

public class BurningVFX : MonoBehaviour
{
    private ParticleSystem _burningParticles;

    private void Awake()
    {
        _burningParticles = GetComponent<ParticleSystem>();
        _burningParticles.Stop();
    }

    private void OnEnable()
    {
        PlayerHealth.OnPlayerBurning += StartBurning;
        PlayerHealth.OnPlayerStopBurning += StopBurning;
    }

    private void OnDisable()
    {
        PlayerHealth.OnPlayerBurning -= StartBurning;
        PlayerHealth.OnPlayerStopBurning -= StopBurning;
    }

    private void StartBurning() => _burningParticles.Play();
    private void StopBurning() => _burningParticles.Stop();
}
