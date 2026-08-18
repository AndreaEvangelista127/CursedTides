using UnityEngine;

public class FloatingAnim : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 90f;  // degrees per second
    [SerializeField] private float _floatAmplitude = 0.3f; // how high it floats
    [SerializeField] private float _floatSpeed = 1f;       // how fast it floats

    private Vector3 _startPosition;

    private void Start()
    {
        _startPosition = transform.position;
    }

    private void Update()
    {
        // Rotation
        transform.Rotate(Vector3.up, _rotationSpeed * Time.deltaTime);

        // Floating — sine wave on Y axis
        float newY = _startPosition.y + Mathf.Sin(Time.time * _floatSpeed) * _floatAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
