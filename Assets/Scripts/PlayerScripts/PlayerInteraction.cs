using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    private IInteractable _currentInteractable;

    /// <summary>
    /// This method is called by the interactable object when the player enters its trigger zone
    /// Keep track of the current interactable object
    /// </summary>
    /// <param name="interactable"></param>
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
