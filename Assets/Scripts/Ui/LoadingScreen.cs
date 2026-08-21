using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private RectTransform _sterringWheel;
    [SerializeField] private float _sterringSpeed = 180f;

    private Coroutine _animationCoroutine;

    private void Update()
    {
        if(_sterringWheel != null)
        {
            _sterringWheel.Rotate(0, 0, -_sterringSpeed * Time.unscaledDeltaTime);
        }
    }

    private void OnDisable()
    {
        if (_animationCoroutine != null) 
        {
            StopCoroutine(_animationCoroutine);
            _animationCoroutine = null;
        }
    }



}
