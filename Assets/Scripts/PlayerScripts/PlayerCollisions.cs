using System;
using UnityEngine;

public class PlayerCollisions : MonoBehaviour
{
    [SerializeField] private CheckPointManager _checkPointManager;

    //ELIMINATE THIS SCRIPT ON MOVE THE COLLISION TO THE CHECKPOINT MANAGER

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Player collided with: " + other.name);

        if (other.CompareTag("CheckPoint"))
        {
            Transform respawnPoint = other.transform.Find("RespawnPoint");
            _checkPointManager.SetCheckPoint(respawnPoint);
        }
    }
}
