using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private float _timeLimit = 300f; // 5 minutes in seconds

    private void Update()
    {
        float time = _timeLimit - Time.timeSinceLevelLoad; // Calculate remaining time
        if (time < 0)
            time = 0;

        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        _timerText.text = $"{minutes:00}:{seconds:00}";
    }


}
