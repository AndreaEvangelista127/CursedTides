using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private Collider _swordCollider;
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

        _playerAnimator.SetTrigger("slashAttack");
        AudioManager.Instance?.PlayPlayerSwordSlash();
        _isAttacking = true;
        _playerMovement.SetIsAttacking(_isAttacking);
    }

    public void OnAttackFinished()
    {
        _isAttacking = false;
    }

    public void EnableSwordHitbox()
    {
        if(_swordCollider != null)
        _swordCollider.enabled = true;
    }

    public void DisableSwordHitbox()
    {
        if(_swordCollider != null) 
        _swordCollider.enabled = false;
    }
}
