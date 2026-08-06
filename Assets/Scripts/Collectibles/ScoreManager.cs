using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    [SerializeField] private TextMeshProUGUI _scoreText;
    private int _currentScore = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        _scoreText.text = "Score: " + _currentScore;
    }

    public void Collect(int scoreValue)
    {
        _currentScore += scoreValue;
    }

}
