using UnityEngine;
using UnityEngine.VFX;

public class Cannon : MonoBehaviour, IInteractable
{
    [SerializeField] private VisualEffect _cannonVFX;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        ShowPrompt();
        PlayerInteraction playerInteraction = other.GetComponent<PlayerInteraction>();
        playerInteraction?.SetCurrentInteractable(this);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        HidePrompt();
        PlayerInteraction playerInteraction = other.GetComponent<PlayerInteraction>();
        playerInteraction?.SetCurrentInteractable(null);
    }

    public void Interact()
    {
        HidePrompt();
        _cannonVFX?.SendEvent("Fire");
    }

    public void ShowPrompt()
    {
        InteractionPrompt.Instance?.Show("Press E to fire");
    }

    public void HidePrompt()
    {
        InteractionPrompt.Instance?.Hide();
    }
}
