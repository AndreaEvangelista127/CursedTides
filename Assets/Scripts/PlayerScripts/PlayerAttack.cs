using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    private Animator _playerAnimator;
    private PlayerMovement _playerMovement;
    private bool _isAttacking = false;

    private void Start()
    {
        _playerAnimator = GetComponent<Animator>();
        _playerMovement = GetComponent<PlayerMovement>();
    }

    public void OnAttackSlash(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        if (_isAttacking) return;
        if (_playerMovement.CurrentState == PlayerMovement.PlayerState.Jumping) return;
        if (_playerMovement.CurrentState == PlayerMovement.PlayerState.Dodging) return;

        _playerMovement.SetCanMove(false);
        _playerAnimator.SetTrigger("slashAttack");
        _isAttacking = true;
    }

    public void OnAttackFinished()
    {
        _isAttacking = false;
        _playerMovement.SetCanMove(true);
    }
}
