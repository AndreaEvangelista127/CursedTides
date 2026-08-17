using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

public class FlockJobManager : MonoBehaviour
{
    [SerializeField] private GameObject _agentPrefab;   
    [SerializeField] private int _agentCount = 500;
    [SerializeField] private float _spawnRadius = 20f;

    // Unity prohibits to use normal arrays in jobs because of race conditions. So we use NativeArrays, which are thread safe and can be used in jobs.
    private NativeArray<float3> _positions; // Where each agent is, so we can calculate the flocking behavior
    private NativeArray<float3> _velocities; // Where each agent is going, so we can calculate the flocking behavior
    private NativeArray<float3> _newVelocities; // Where each agent will go after the flocking behavior is calculated
    private TransformAccessArray _transformAccessArray; // To be able to move the agents in a job, we need to give the job access to their transforms because the transform is a Unity object and cannot be used in jobs directly.

    private void Start()
    {
        // Persistent = they live for the whole run
        _positions = new NativeArray<float3>(_agentCount, Allocator.Persistent);
        _velocities = new NativeArray<float3>(_agentCount, Allocator.Persistent);
        _newVelocities = new NativeArray<float3>(_agentCount, Allocator.Persistent);

        Transform[] transforms = new Transform[_agentCount];

        for (int i = 0; i < _agentCount; i++)
        {
            Vector3 spawnPosition = transform.position + UnityEngine.Random.insideUnitSphere * _spawnRadius;
            GameObject agent = Instantiate(_agentPrefab, spawnPosition, UnityEngine.Random.rotation, transform);

            transforms[i] = agent.transform;
            _positions[i] = spawnPosition;              // starting position 
            _velocities[i] = agent.transform.forward;   // starting direction 
        }

        // Lets the transform job reach all these transforms later
        _transformAccessArray = new TransformAccessArray(transforms);
    }

    private void OnDestroy()
    {
        // Erase the whiteboards, or you leak memory and get errors on exit
        if (_positions.IsCreated) _positions.Dispose();
        if (_velocities.IsCreated) _velocities.Dispose();
        if (_newVelocities.IsCreated) _newVelocities.Dispose();
        if (_transformAccessArray.isCreated) _transformAccessArray.Dispose();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _spawnRadius);
    }
}