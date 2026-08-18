using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

public class WaveManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject _objectPrefab;
    [SerializeField] private int _gridWidth = 50; 
    [SerializeField] private int _gridHeight = 50;
    [SerializeField] private float _spacing = 1f;

    [Header("Wave Settings")]
    [SerializeField] private float _amplitude = 2f;
    [SerializeField] private float _frequency = 0.5f;
    [SerializeField] private float _speed = 1f;

    [Header("General Settings")]
    [SerializeField] private bool _useJobs = true;

    // Used for non-job version
    private List<Transform> _objects = new();

    // Used for job version
    private NativeArray<float3> _basePositions; // Saves the base XZ positions of each object for the job to read, to calculate the Y position each frame
    private TransformAccessArray _transformAccessArray; // Array of transforms for the job to write to, to update the Y position each frame

    private void Start()
    {
        SpawnGrid();
    }

    private void Update()
    {
        if (_useJobs)
        {
            WaveJob job = new WaveJob
            {
                BasePositions = _basePositions,
                Amplitude = _amplitude,
                Frequency = _frequency,
                Speed = _speed,
                Time = Time.time
            };

            JobHandle handle = job.Schedule(_transformAccessArray); // Execute the job on all transforms in the TransformAccessArray in parallel
            handle.Complete();
        }
        else
        {
            foreach (Transform t in _objects)
            {
                // Same math as the job — easy to compare
                float y = math.sin(t.position.x * _frequency + Time.time * _speed) * _amplitude
                        + math.sin(t.position.z * _frequency + Time.time * _speed) * _amplitude;

                t.position = new Vector3(t.position.x, y, t.position.z);
            }
        }
    }

    private void SpawnGrid()
    {
        int total = _gridWidth * _gridHeight;
        Transform[] transforms = new Transform[total];
        _basePositions = new NativeArray<float3>(total, Allocator.Persistent);

        int index = 0;
        for (int z = 0; z < _gridHeight; z++) // Depth
        {
            for (int x = 0; x < _gridWidth; x++) // Width
            {
                // Center the grid on this GameObject
                Vector3 pos = transform.position + new Vector3((x - _gridWidth / 2f) * _spacing, 0 ,(z - _gridHeight / 2f) * _spacing); // Half the width and height to center the grid on this GameObject

                GameObject obj = Instantiate(_objectPrefab, pos, Quaternion.identity, transform);
                _objects.Add(obj.transform);
                transforms[index] = obj.transform;

                // Save the base XZ position — Y will be calculated every frame
                _basePositions[index] = new float3(pos.x, 0, pos.z);

                index++;
            }
        }

        _transformAccessArray = new TransformAccessArray(transforms);
    }

    private void OnDestroy()
    {
        // NativeArray must be disposed to avoid memory leaks
        if (_basePositions.IsCreated) _basePositions.Dispose();
        _transformAccessArray.Dispose();
    }
}

[BurstCompile] // BurstCompile attribute tells Unity to compile this job with the Burst compiler for better performance
public struct WaveJob : IJobParallelForTransform
{
    // Read-only inputs — each thread reads but never writes these (+ performance optimization)
    [ReadOnly] public NativeArray<float3> BasePositions;
    [ReadOnly] public float Amplitude;
    [ReadOnly] public float Frequency;
    [ReadOnly] public float Speed;
    [ReadOnly] public float Time;

    public void Execute(int index, TransformAccess transform)
    {
        float3 base3 = BasePositions[index];

        // Wave formula: two sine waves on X and Z axes combined
        float y = math.sin(base3.x * Frequency + Time * Speed) * Amplitude
                + math.sin(base3.z * Frequency + Time * Speed) * Amplitude;

        transform.position = new float3(base3.x, y, base3.z);
    }
}
