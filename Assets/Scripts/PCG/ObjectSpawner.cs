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
                        if (rand.NextDouble() < obj.density) // Randomly decide to spawn based on density, 0.1 means 10% chance to spawn
                        {
                            // The mesh is centered at the origin, so we offset the x and z coordinates to center the objects on the mesh
                            float worldX = x - mapWidth / 2f;
                            float worldZ = -(y - mapHeight / 2f);
                            float worldY = heightCurve.Evaluate(currentHeight) * amplitudeMultiplier + obj.heightOffset;

                            Vector3 localPos = new Vector3(worldX, worldY, worldZ);
                            Vector3 worldPos = meshTransform.TransformPoint(localPos);

                            GameObject spawned = Instantiate(obj.prefab, worldPos, Quaternion.identity);
                            spawned.transform.SetParent(containers[i]);
                        }
                    }
                }
            }
        }
    }
}
