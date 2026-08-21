using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUi : MonoBehaviour
{
    [SerializeField] private Image _fill;
    [SerializeField] private PlayerHealth _playerHealth;
    [SerializeField] private float _lerpSpeed = 5f;

    private float _targetFill;

    private void Start()
    {
        _targetFill = 1f; // Start with full health
        if(_playerHealth != null)
            _playerHealth.OnHealthChange += UpdateHealthBar; //Subscribe to the health change event
    }

    private void Update()
    {
        if(_fill  != null) 
        _fill.fillAmount = Mathf.Lerp(_fill.fillAmount, _targetFill, Time.deltaTime * _lerpSpeed);
    }

    private void UpdateHealthBar(float healthPercent)
    {
        _targetFill = healthPercent;
    }

    private void OnDestroy()
    {
        if (_playerHealth != null)
            _playerHealth.OnHealthChange -= UpdateHealthBar;
    }
}
