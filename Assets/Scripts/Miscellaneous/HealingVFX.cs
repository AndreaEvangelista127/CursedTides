using UnityEngine;

public class HealingVFX : MonoBehaviour
{
    private ParticleSystem _healingParticles;

    private void Awake()
    {
        _healingParticles = GetComponent<ParticleSystem>();
        _healingParticles.Stop(); // Ensure the particle system is stopped at the start
    }
    private void OnEnable()
    {
        // Player enters the collider of the healing station, the event is triggered and the healing particles are played
        PlayerHealth.OnPlayerHealing += StartHealing;
        PlayerHealth.OnPlayerStopHealing += StopHealing;
    }

    private void OnDisable()
    {
        PlayerHealth.OnPlayerHealing -= StartHealing;
        PlayerHealth.OnPlayerStopHealing -= StopHealing;
    }

    private void StartHealing() => _healingParticles.Play();
    private void StopHealing() => _healingParticles.Stop();
}
