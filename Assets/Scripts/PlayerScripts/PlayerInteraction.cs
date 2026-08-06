using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    private IInteractable _currentInteractable;

    public void SetCurrentInteractable(IInteractable interactable)
    {
        _currentInteractable = interactable;
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _currentInteractable?.Interact();
        }
    }
}
