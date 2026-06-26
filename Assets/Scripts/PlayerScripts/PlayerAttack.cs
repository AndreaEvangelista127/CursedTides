using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    private Animator _playerAnimator;


    private void Start()
    {
        _playerAnimator = GetComponent<Animator>();
    }

    public void OnAttackSlash(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            SlashAttack();
        }
    }

    private void SlashAttack()
    {
        _playerAnimator.SetTrigger("slashAttack");
    }
}
