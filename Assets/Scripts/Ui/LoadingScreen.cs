using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private Image _fill;
    [SerializeField] private Slider _slider;

    private Coroutine _animationCoroutine;

    // starting value


    private void OnDisable()
    {
        if (_animationCoroutine != null) 
        {
            StopCoroutine(_animationCoroutine);
            _animationCoroutine = null;
        }
    }

    public IEnumerator LoadingAnimationCoroutine()
    {
        while (true) 
        {
            _slider.value = Mathf.Lerp(_slider.value, 1, Time.deltaTime);
            yield return null;
        }
    }

    public void StartAnimation()
    {
        _animationCoroutine ??= StartCoroutine(LoadingAnimationCoroutine());
    }




    //private void Update()
    //{
    //    _slider.value = Mathf.Lerp(_slider.value, 1, Time.deltaTime);
    //}
}
