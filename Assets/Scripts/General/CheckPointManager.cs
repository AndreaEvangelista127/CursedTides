using UnityEngine;

public class CheckPointManager : MonoBehaviour
{

    [SerializeField] private GameObject _player;
    private Vector3 _respawnPosition;

    private void Start()
    {
        // Initialize the respawn position to the player's starting position
        _respawnPosition = _player.transform.position;
    }

    public void Respawn()
    {
        _player.transform.position = _respawnPosition;
    }

    public void SetCheckPoint(Transform checkPoint)
    {
        _respawnPosition = checkPoint.position;
    }

    
}
