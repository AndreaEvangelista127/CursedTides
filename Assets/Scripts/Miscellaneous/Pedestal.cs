using Unity.VisualScripting;
using UnityEngine;

public class Pedestal : MonoBehaviour, IInteractable
{
    [SerializeField] private GemType _requiredGemType; // In the Unity Inspector, set this to the type of gem that this pedestal requires.
    [SerializeField] private GameObject _pedestalGem; // The visual representation of the gem on the pedestal. This will be enabled when the correct gem is placed on it.
    [SerializeField] private GameObject _pedestalPlaceHolder;

    private bool _isOccupied = false; // Indicates whether the pedestal currently has a gem on it.
    private PlayerGemHolder _playerGemHolder;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            _playerGemHolder = other.GetComponent<PlayerGemHolder>();
            if (_playerGemHolder != null && _playerGemHolder.HasGem && _playerGemHolder.HeldGemType == _requiredGemType)
            {
                ShowPrompt();
                PlayerInteraction playerInteraction = other.GetComponent<PlayerInteraction>();
                playerInteraction?.SetCurrentInteractable(this);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            HidePrompt();
            PlayerInteraction playerInteraction = other.GetComponent<PlayerInteraction>();
            playerInteraction?.SetCurrentInteractable(null);
        }
    }

    public void ShowPrompt()
    {
        InteractionPrompt.Instance?.Show("Press E to place gem");
    }

    public void HidePrompt()
    {
        InteractionPrompt.Instance?.Hide();
    }

    public void Interact()
    {
        if (_isOccupied) return;

        _isOccupied = true;
        _playerGemHolder.PlaceGem();
        _pedestalGem.SetActive(true);
        AudioManager.Instance?.PlayPedestalPlace();
        _pedestalPlaceHolder.SetActive(false);
        HidePrompt();

        GameManager.Instance.OnPedestalCompleted();
    }
}
