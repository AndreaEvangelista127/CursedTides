using UnityEngine;

public class BonfireHealing : MonoBehaviour
{
    [SerializeField] private float _healPerSecond = 10f;

    private PlayerHealth _playerHealth;
    private bool _playerInRange = false;
    private void OnEnable()
    {
        PlayerHealth.OnPlayerStopBurning += OnBurningEnded;
        PlayerHealth.OnPlayerBurning += OnBurningStarted; 
    }

    private void OnDisable()
    {
        PlayerHealth.OnPlayerStopBurning -= OnBurningEnded;
        PlayerHealth.OnPlayerBurning -= OnBurningStarted; 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _playerHealth = other.GetComponent<PlayerHealth>();
        _playerInRange = true;

        // Only trigger healing VFX if not burning
        if (!_playerHealth.IsBurning)
            PlayerHealth.TriggerHealing();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _playerHealth = null;
        _playerInRange = false;
        PlayerHealth.TriggerStopHealing();
    }

    private void Update()
    {
        if (!_playerInRange || _playerHealth == null) return;
        if(_playerHealth.IsBurning) return;

        _playerHealth.Heal(_healPerSecond * Time.deltaTime);
    }

    // Called when burning starts — stop healing VFX immediately
    private void OnBurningStarted()
    {
        PlayerHealth.TriggerStopHealing(); 
    }

    // Called when the player stops burning, to resume healing if they are in range of the bonfire
    private void OnBurningEnded()
    {
        if (_playerInRange)
            PlayerHealth.TriggerHealing();
    }

}
