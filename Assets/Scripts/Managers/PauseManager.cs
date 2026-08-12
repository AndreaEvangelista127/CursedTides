using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    public void OnPause(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        SettingsManager.Instance?.ToggleSettings();
    }
}
