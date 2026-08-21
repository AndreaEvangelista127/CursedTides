using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public enum PlayerState
    {
        Idle,
        Walking,
        Sprinting,
        Jumping,
        Dodging,
        Attacking
    }
    private PlayerState _currentState = PlayerState.Idle;
    public PlayerState CurrentState => _currentState; // only get the current state, no set

    [Header("General settings")]
    [SerializeField] private float _speed = 5f;
    [SerializeField] private Transform _model;
    [SerializeField] private float _rotationSpeed = 10f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float _jumpCooldown = 0.4f;
    private float _jumpCooldownTimer = 0f;

    [Header("Ground Check")]
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private Vector3 _groundCheckSize = new Vector3(0.5f, 0.1f, 0.5f);
    [SerializeField] private LayerMask _groundLayer;

    [Header("Sprint")]
    [SerializeField] private float _sprintSpeed = 8f;

    [Header("Dodge Settings")]
    [SerializeField] private float _dodgeCooldown = 1f;
    [SerializeField] private AnimationCurve _dodgeCurve;
    private float _dodgeCooldownTimer = 0f;

    [Header("Gizmos")]
    [SerializeField] private bool _showGroundCheck;

    private bool _isGrounded;

    private Rigidbody _rb;

    private Vector2 _moveInput; // We don't need the y component for movement

    private Transform _cameraTf;

    private Animator _playerAnimator;

    private bool _canMove;
    private bool _isSprintHeld = false;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();

        _playerAnimator = GetComponent<Animator>();

        _cameraTf = Camera.main.transform;

        // Bools
         _canMove = true;
    }

    private void Update()
    {
        // If the player jumped, we start the cooldown timer for the jump
        if (_jumpCooldownTimer > 0)
            _jumpCooldownTimer -= Time.deltaTime;

        // If the player dodged, we start the cooldown timer for the dodge
        if (_dodgeCooldownTimer > 0)
            _dodgeCooldownTimer -= Time.deltaTime;

    }

    private void FixedUpdate()
    {
        // If the game has not started, do not update the player movement
        if (!GameManager.Instance.IsGameStarted) return;

        CheckGround();
        Move();
    }

    // --- INPUT SYSTEMS CALLBACKS ---
    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Jump();
        }
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if(context.performed) _isSprintHeld = true;
        else if(context.canceled) _isSprintHeld = false;
    }

    public void OnDodge(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        if (_currentState == PlayerState.Dodging) return;
        if (_currentState == PlayerState.Jumping) return;
        if(_currentState == PlayerState.Attacking) return;
        if (_dodgeCooldownTimer > 0) return;
        StartCoroutine(Dodge());
    }

    // --- MOVEMENT LOGIC ---
    private void Move()
    {
        if (!_canMove) return;
        if (_currentState == PlayerState.Dodging) return;

        bool isMoving = _moveInput.magnitude > 0.1f;
        bool isSprinting = _isSprintHeld && isMoving;

        if (!isMoving) _currentState = PlayerState.Idle;
        else if(isSprinting) _currentState = PlayerState.Sprinting;
        else _currentState = PlayerState.Walking;

        _playerAnimator.SetBool("isWalking", isMoving);
        _playerAnimator.SetBool("isSprinting", isSprinting);
    
        Vector3 moveVector = new Vector3(_moveInput.x, 0, _moveInput.y);

        moveVector = Quaternion.Euler(0, _cameraTf.eulerAngles.y, 0) * moveVector; // Rotate the movement vector based on the camera's y rotation

        // Move the model of the player to face the direction of movement
        RotateModel(moveVector);

        float currentSpeed = isSprinting ? _sprintSpeed : _speed;
        moveVector *= currentSpeed;
        moveVector.y = _rb.linearVelocity.y;

        _rb.linearVelocity = moveVector; // Apply the movement to the Rigidbody2D
    }

    private void RotateModel(Vector3 moveVector)
    {
        moveVector.y = 0f;

        // If the movement vector is too small, don't rotate the model and remain in the current rotation
        if (moveVector.magnitude < 0.1f) return; //without this line as soon as we stop pressing the movement keys, the model will snap back to the quaternion identity rotation because the moveVector will be (0, 0, 0)

        // Return an angle that rotates the model to face the direction of movement
        Quaternion targetRotation = Quaternion.LookRotation(moveVector);

        // By using Lerp, we can smoothly rotate the model towards the target rotation
        _model.rotation = Quaternion.Lerp(_model.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
    }

    private void Jump()
    {
        if (_currentState == PlayerState.Jumping) return;
        if (_currentState == PlayerState.Dodging) return;
        if (!_isGrounded) return;
        if (_jumpCooldownTimer > 0) return;

        _currentState = PlayerState.Jumping;
        _jumpCooldownTimer = _jumpCooldown;

        _playerAnimator.ResetTrigger("jump");
        _playerAnimator.SetTrigger("jump");
        _playerAnimator.SetBool("isJumping", true);
    }

    public void OnJumpEvent() //Animation event called from the jump animation to apply the jump force at the right time
    {
        _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, jumpForce, _rb.linearVelocity.z);
    }

    private IEnumerator Dodge()
    {
        // Only dodge if moving
        if (_moveInput.magnitude < 0.1f) yield break;

        _currentState = PlayerState.Dodging;
        _dodgeCooldownTimer = _dodgeCooldown;

        // blocca velocity
        _rb.linearVelocity = new Vector3(0, _rb.linearVelocity.y, 0);

        // Get direction
        Vector3 dodgeDir = new Vector3(_moveInput.x, 0, _moveInput.y);
        //Rotate based on the camera, not sure
        dodgeDir = Quaternion.Euler(0, _cameraTf.eulerAngles.y, 0) * dodgeDir;
        if (dodgeDir.magnitude < 0.1f) dodgeDir = _model.forward;
        dodgeDir.Normalize();

        _playerAnimator.SetTrigger("dodge");

        
        float dodgeDuration = _dodgeCurve[_dodgeCurve.length - 1].time;
        float timer = 0f;

        while (timer < dodgeDuration)
        {
            float speed = _dodgeCurve.Evaluate(timer);
            _rb.linearVelocity = dodgeDir * speed + Vector3.up * _rb.linearVelocity.y;
            timer += Time.deltaTime;
            yield return null;
        }

        _rb.linearVelocity = Vector3.zero;
        _currentState = PlayerState.Idle;
    }

    private void CheckGround()
    {
        bool boxCheck = Physics.CheckBox(
            _groundCheckPoint.position,  // center of the box
            new Vector3(_groundCheckSize.x, _groundCheckSize.y, _groundCheckSize.z) / 2f, // half extents
            Quaternion.identity,          // rotation
            _groundLayer                  // layer to check
        );

        _isGrounded = boxCheck;

        if (_isGrounded && _jumpCooldownTimer <= 0)
        {
            if (_currentState == PlayerState.Jumping) // only if i was jumping reset to state to idle
                _currentState = PlayerState.Idle;
            _playerAnimator.ResetTrigger("jump");
            _playerAnimator.SetBool("isJumping", false);
        }
    }

    public void SetIsAttacking(bool isAttacking)
    {
        if (isAttacking) _currentState = PlayerState.Attacking;
        
    }

    private void OnDrawGizmos()
    {

        if (_isGrounded)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
        }
    }

}
