using UnityEngine;

public class BonfireBurning : MonoBehaviour
{
    [SerializeField] private float _damagePerSecond = 10f;
    [SerializeField] private float _burnDuration = 3f;

    private PlayerHealth _playerHealth;
    private bool _playerInFire = false;
    private float _burnTimer = 0f;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _playerHealth = other.GetComponent<PlayerHealth>();
        _playerInFire = true;
        PlayerHealth.TriggerBurning();
        _playerHealth.SetBurningState(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _playerInFire = false;
        _burnTimer = _burnDuration;
        //PlayerHealth.TriggerStopBurning();
        //_playerHealth = null;
    }

    private void Update()
    {
        if (_playerHealth == null) return;

        if (_playerInFire)
        {
            // Inside fire — damage continuously, NO timer reset here
            _playerHealth.TakeDamage(_damagePerSecond * Time.deltaTime);
        }
        else if (_burnTimer > 0)
        {
            _burnTimer -= Time.deltaTime;
            _playerHealth.TakeDamage(_damagePerSecond * Time.deltaTime);

            if (_burnTimer <= 0)
            {
                _playerHealth.SetBurningState(false);
                PlayerHealth.TriggerStopBurning();
                _playerHealth = null;
            }
        }
    }
}
