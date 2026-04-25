using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float jumpForce = 5f;

    [SerializeField] private Transform _model;
    [SerializeField] private float _rotationSpeed = 10f;

    private Rigidbody _rb;

    private Vector2 _moveInput; // We don't need the y component for movement

    private Transform _cameraTf;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();

        _cameraTf = Camera.main.transform; //

    }

    private void FixedUpdate()
    {
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


    // --- MOVEMENT LOGIC ---
    private void Move()
    {
        Vector3 moveVector = Vector3.zero;

        // Using the x component for the horizontal movement and z component for the forward movement
        moveVector.x = _moveInput.x;
        moveVector.z = _moveInput.y;

        moveVector = Quaternion.Euler(0, _cameraTf.eulerAngles.y, 0) * moveVector; // Rotate the movement vector based on the camera's y rotation

        // Move the model of the player to face the direction of movement
        RotateModel(moveVector);

        moveVector *= _speed;
        moveVector.y = _rb.linearVelocity.y;

        _rb.linearVelocity = moveVector; // Apply the movement to the Rigidbody2D
    }

    private void RotateModel(Vector3 moveVector)
    {
        moveVector.y = 0f;

        // If the movement vector is too small, don't rotate the model and remain in the current rotation
        if (moveVector.magnitude < 0.1f) return;

        // Return an angle that rotates the model to face the direction of movement
        Quaternion targetRotation = Quaternion.LookRotation(moveVector);

        // By using Lerp, we can smoothly rotate the model towards the target rotation
        _model.rotation = Quaternion.Lerp(_model.rotation, targetRotation, _rotationSpeed * Time.deltaTime);

        //_model.rotation = Quaternion.LookRotation(moveVector); //CLUNKY VERSION FOR ROTATION, NOT SMOOTH 
    }

    private void Jump()
    {
        _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, jumpForce, _rb.linearVelocity.z);
    }

}
