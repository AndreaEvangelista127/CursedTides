using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float jumpForce = 5f;

    private bool _isJumping;
    private float _jumpCooldown = 0.2f;
    private float _jumpCooldownTimer = 0f;

    [SerializeField] private Transform _model;
    [SerializeField] private float _rotationSpeed = 10f;

    [Header("Ground Check")]
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private Vector3 _groundCheckSize = new Vector3(0.5f, 0.1f, 0.5f);
    [SerializeField] private LayerMask _groundLayer;

    [Header("Sprint")]
    [SerializeField] private float _sprintSpeed = 8f;
    private bool _isSprinting = false;

    [Header("Gizmos")]
    [SerializeField] private bool _showGroundCheck;

    private bool _isGrounded;

    private Rigidbody _rb;

    private Vector2 _moveInput; // We don't need the y component for movement

    private Transform _cameraTf;

    private Animator _playerAnimator;

    private bool _canMove;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();

        _playerAnimator = GetComponent<Animator>();

        _cameraTf = Camera.main.transform;

        // Bools
        _canMove = true;
        _isSprinting = false;
    }

    private void Update()
    {
        if (_jumpCooldownTimer > 0)
            _jumpCooldownTimer -= Time.deltaTime;

    }

    private void FixedUpdate()
    {
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
        if (!_isGrounded) return;

        if (context.started)
        {
            _isSprinting = true;
            _playerAnimator.SetBool("isSprinting",true);
        }
        else if(context.canceled) 
        {
            _isSprinting = false;
            _playerAnimator.SetBool("isSprinting", false);
        }

    }

    // --- MOVEMENT LOGIC ---
    private void Move()
    {
        if (!_canMove) return;

        bool isMoving = _moveInput.magnitude > 0.1f;
        _playerAnimator.SetBool("isWalking", isMoving);

        Vector3 moveVector = Vector3.zero;

        // Using the x component for the horizontal movement and z component for the forward movement
        moveVector.x = _moveInput.x;
        moveVector.z = _moveInput.y;

        moveVector = Quaternion.Euler(0, _cameraTf.eulerAngles.y, 0) * moveVector; // Rotate the movement vector based on the camera's y rotation

        // Move the model of the player to face the direction of movement
        RotateModel(moveVector);
        
        float currentSpeed = _isSprinting ? _sprintSpeed : _speed;
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

        //_model.rotation = Quaternion.LookRotation(moveVector); //CLUNKY VERSION FOR ROTATION, NOT SMOOTH 
    }

    private void Jump()
    {
        //Debug.Log($"canMove: {_canMove}, grounded: {_isGrounded}, isJumping: {_isJumping}");
        if ( !_canMove || !_isGrounded || _isJumping) return;
        if (_jumpCooldownTimer > 0) return; // cooldown guard

        _isJumping = true;
        _jumpCooldownTimer = _jumpCooldown; // start cooldown

        _playerAnimator.ResetTrigger("jump");
        _playerAnimator.SetTrigger("jump");

    }

    public void OnJumpEvent() //Animation event called from the jump animation to apply the jump force at the right time
    {
        _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, jumpForce, _rb.linearVelocity.z);
    }

    private void CheckGround()
    {
        bool boxCheck = Physics.CheckBox(
            _groundCheckPoint.position,  // center of the box
            new Vector3(_groundCheckSize.x, _groundCheckSize.y, _groundCheckSize.z) / 2f, // half extents
            Quaternion.identity,          // rotation
            _groundLayer                  // layer to check
        );

        _isGrounded = boxCheck && _rb.linearVelocity.y <= 0.1f; // Check if the player is grounded and not moving upwards

        _playerAnimator.SetBool("isJumping", !_isGrounded); // true if the player is in the air, false if grounded

        if (_isGrounded) _isJumping = false;
    }

    public void SetMovementEnabled(bool enabled)
    {
        _canMove = enabled;
        if (!enabled) _rb.linearVelocity = new Vector3(0, _rb.linearVelocity.y, 0);
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
