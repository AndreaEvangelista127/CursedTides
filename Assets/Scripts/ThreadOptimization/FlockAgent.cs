using System.Net.Sockets;
using UnityEngine;

[RequireComponent (typeof(SphereCollider))]
public class FlockAgent : MonoBehaviour
{
    private SphereCollider _agentCollider;
    public SphereCollider AgentCollider { get { return _agentCollider; } }

    void Start()
    {
        _agentCollider = GetComponent<SphereCollider> ();
    }


    public void Move(Vector3 velocity)
    {
        transform.forward = velocity;
        transform.position += velocity * Time.deltaTime;
    }

}
