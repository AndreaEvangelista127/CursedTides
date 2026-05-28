using UnityEngine;

public class CheckPoint : MonoBehaviour
{

    [SerializeField] private CheckPointManager _checkPointManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _checkPointManager.SetCheckPoint(transform.Find("RespawnPoint"));
            Debug.Log("Checkpoint reached! Respawn position updated.");
        }
    }
}
