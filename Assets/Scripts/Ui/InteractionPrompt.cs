using TMPro;
using UnityEngine;

public class InteractionPrompt : MonoBehaviour
{
    public static InteractionPrompt Instance { get; private set; }
    private TextMeshProUGUI _text;
    [SerializeField] private GameObject _promptObject; 

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        _text = _promptObject.GetComponent<TextMeshProUGUI>();
    }

    public void Show(string message)
    {
        _text.text = message;
        _promptObject.SetActive(true);
    }

    public void Hide() => _promptObject.SetActive(false);
}
