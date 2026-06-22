using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _timerText;

    private void Update()
    {
        float time = Time.timeSinceLevelLoad; // 2 min - 30 sec -> 150.0f
        int minutes = Mathf.FloorToInt(time / 60); // 150 / 60 = 2.5 -> 2
        int seconds = Mathf.FloorToInt(time % 60); // 150 % 60 = 30
        _timerText.text = $"{minutes:00}:{seconds:00}"; // :00 text formatting to always show 2 digits
    }


}
