using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private Image _fill;
    [SerializeField] private Slider _slider;
    [SerializeField] private float _lerpSpeed = 5f;

    // starting value
    

    private void Update()
    {
        _slider.value = Mathf.Lerp(_slider.value, 1, Time.deltaTime);

    }
}
