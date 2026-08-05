using UnityEngine;

public class BonfireHealing : MonoBehaviour
{
    [SerializeField] private float _healPerSecond = 10f;
    [SerializeField] private float _healLerpSpeed = 2f;

    private PlayerHealth _playerHealth;
    private bool _playerInRange = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _playerHealth = other.GetComponent<PlayerHealth>();
        _playerInRange = true;
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
        _playerHealth.Heal(_healPerSecond * Time.deltaTime);
    }
}
