using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _timerText;
    private float _timeLimit;

    private void Start()
    {
        _timeLimit = GameManager.Instance?.TimeLimit ?? 300f;
    }

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
