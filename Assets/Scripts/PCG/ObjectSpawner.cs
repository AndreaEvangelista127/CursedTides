using UnityEngine;
using System.Collections.Generic;
using UnityEditor.U2D.Sprites;

public class ObjectSpawner : MonoBehaviour
{
    [System.Serializable]
    public struct SpawnableObject
    {
        public string name;
        public GameObject prefab;
        [Range(0f, 1f)] public float minHeight;
        [Range(0f, 1f)] public float maxHeight;
        [Range(0f, 1f)] public float density;
        public float heightOffset;
    }

    [SerializeField] private SpawnableObject[] _spawnableObjects;
    [SerializeField] private LayerMask _mapMeshLayer;

    public void SpawnObjects(float[,] heightMap, int seed, float amplitudeMultiplier, AnimationCurve heightCurve, Transform meshTransform, MeshGenerator.MeshData meshdata)
    {

        List<Transform> meshChildrens = new();

        foreach(Transform t in meshTransform)
        {
            meshChildrens.Add(t);
        }

        foreach (Transform child in meshChildrens)
        {

            if (child == null) continue;
            if (child.name.EndsWith("_Container"))
            {
                Debug.Log($"Destroying container: {child.name}");
                DestroyImmediate(child.gameObject);
            }
        }

        System.Random rand = new System.Random(seed);

        int mapWidth = heightMap.GetLength(0);
        int mapHeight = heightMap.GetLength(1);

        // Create containers for each spawnable object
        Transform[] containers = new Transform[_spawnableObjects.Length];
        for(int i = 0; i < _spawnableObjects.Length; i++)
        {
            if (_spawnableObjects[i].prefab == null) continue;
            
            containers[i] = new GameObject(_spawnableObjects[i].name + "_Container").transform;
            containers[i].SetParent(meshTransform);
        }

        // Iterate through the height map and spawn objects based on their height range and density
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float currentHeight = heightMap[x, y];

                for (int i = 0; i < _spawnableObjects.Length; i++)
                {
                    SpawnableObject obj = _spawnableObjects[i];
                    if (obj.prefab == null) continue;

                    if (currentHeight >= obj.minHeight && currentHeight <= obj.maxHeight) // The Noise value is within the height range for this object?
                    {
                        if (rand.NextDouble() < obj.density) // Randomomly decide to spawn based on density, a value of 0.1 means 10% chance to spawn
                        {
                            // XZ position centered on mesh
                            float worldX = x - mapWidth / 2f;
                            float worldZ = -(y - mapHeight / 2f);

                            // Approximate Y — just above the surface
                            float approxY = heightCurve.Evaluate(currentHeight) * amplitudeMultiplier + 10f; // +10 to ensure it's above the terrain for raycasting

                            // Convert to world space
                            Vector3 localPos = new Vector3(worldX, approxY, worldZ);
                            Vector3 rayOrigin = meshTransform.TransformPoint(localPos); // Tranform a local position to world space to ensure the raycast is done in the correct world space

                            // Shoot ray downward
                            Ray ray = new Ray(rayOrigin, Vector3.down);

                            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _mapMeshLayer))
                            {
                                // Exact position on surface + offset
                                Vector3 spawnPos = hit.point + Vector3.up * obj.heightOffset;

                                // Rotation aligned to terrain normal
                                Quaternion spawnRot = Quaternion.FromToRotation(Vector3.up, hit.normal);

                                GameObject spawned = Instantiate(obj.prefab, spawnPos, spawnRot);
                                spawned.transform.SetParent(containers[i]);
                            }
                        }
                    }
                }
            }
        }
    }
}
