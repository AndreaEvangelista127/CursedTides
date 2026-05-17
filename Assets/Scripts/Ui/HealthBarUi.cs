using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUi : MonoBehaviour
{
    [SerializeField] private Image _fill;
    [SerializeField] private PlayerHealth _playerHealth;

    private void Start()
    {
        _playerHealth.OnHealthChange += UpdateHealthBar; //Subscribe to the health change event
    }

    private void UpdateHealthBar(float healthPercent)
    {
        Debug.Log("Updating health bar: " + healthPercent);
        _fill.fillAmount = healthPercent;
    }

    private void OnDestroy()
    {
        _playerHealth.OnHealthChange -= UpdateHealthBar;
    }
}
