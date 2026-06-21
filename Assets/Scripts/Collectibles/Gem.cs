using UnityEngine;
using UnityEngine.InputSystem;

public class Gem : MonoBehaviour, IInteractable
{
    [SerializeField] private GemType _gemType;
    [SerializeField] private Sprite _gemIcon;
    [SerializeField] private GameObject _interactionPrompt;

    private PlayerGemHolder _playerGemHolder;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Get the PlayerGemHolder component from the player and show the interaction prompt
            _playerGemHolder = other.GetComponent<PlayerGemHolder>();
            ShowPrompt();
            // Set this gem as the current interactable for the player
            PlayerInteraction playerInteraction = other.GetComponent<PlayerInteraction>();
            playerInteraction?.SetCurrentInteractable(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerGemHolder = null;
            HidePrompt();
            PlayerInteraction playerInteraction = other.GetComponent<PlayerInteraction>();
            playerInteraction?.SetCurrentInteractable(null);
        }
    }

    public void Interact()
    {
        if(_playerGemHolder != null && !_playerGemHolder.HasGem)
        {
            _playerGemHolder.PickUp(_gemType, _gemIcon);
            HidePrompt();
            Destroy(gameObject); // Remove the gem from the scene
        }
    }

    public void ShowPrompt()
    {
        _interactionPrompt.SetActive(true);
    }

    public void HidePrompt()
    {
        _interactionPrompt.SetActive(false);
    }
}
