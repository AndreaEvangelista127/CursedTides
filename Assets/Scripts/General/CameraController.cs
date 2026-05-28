using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.FilePathAttribute;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float _sensitivity;
    [SerializeField] private float _maxAngleX;
    [SerializeField] private float _mixAngleX;

    private Vector2 _lookInput;
    private float _xRotation; // Pitch (up/down)
    private float _yRotation; // Yaw (left/right)

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        Look();
    }


    public void OnLook(InputAction.CallbackContext context)
    {
        _lookInput = context.ReadValue<Vector2>();
    }

    private void Look()
    {
        // Storing the mouse input in local variables for better readability
        float mouseX = _lookInput.x * _sensitivity * Time.deltaTime; // Multiply by Time.deltaTime to make it frame rate independent
        float mouseY = _lookInput.y * _sensitivity * Time.deltaTime;

        // Accumulate angles
        _xRotation -= mouseY; // Minus: mouse up = camera rotates up
        _yRotation += mouseX;

        _xRotation = Mathf.Clamp(_xRotation, _mixAngleX, _maxAngleX); // Clamp the vertical rotation

        // Rotate this object (CameraTarget) — Main Camera follows automatically
        transform.rotation = Quaternion.Euler(_xRotation, _yRotation, 0f);

    }
}
