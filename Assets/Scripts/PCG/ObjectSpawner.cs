using UnityEngine;
using System.Collections.Generic;
using UnityEditor.U2D.Sprites;

public class ObjectSpawner : MonoBehaviour
{
    public enum HeightZoneType
    {
        Sea,
        Shores,
        Beach,
        Grass,
        Hill
    }

    [System.Serializable]
    public struct HeightZone
    {
        public HeightZoneType zone;
        public float minHeight;
        public float maxHeight;
    }

    [System.Serializable]
    public struct SpawnableObject
    {
        public string name;
        public GameObject prefab;
        public HeightZoneType spawnZone;
        public int maxCount;
        public float minDistance;
        public float heightOffset;
        public bool rotationBasedOnNormal;
    }

    [SerializeField] private HeightZone[] _heightZones;
    [SerializeField] private SpawnableObject[] _spawnableObjects;
    [SerializeField] private LayerMask _mapMeshLayer; // Used for raycasting to find the terrain surface


    public void SpawnObjects(MapGridCellInfo[,] mapInfoGrid, int seed, Transform meshTransform)
    {
        // --- DESTROY OLD CONTAINERS ---
        List<Transform> toDestroy = new List<Transform>();
        foreach (Transform t in meshTransform)
            if (t.name.EndsWith("_Container"))
                toDestroy.Add(t);
        foreach (Transform t in toDestroy)
            DestroyImmediate(t.gameObject);

        System.Random rand = new System.Random(seed);


        // --- CREATE CONTAINERS ---
        Transform[] containers = new Transform[_spawnableObjects.Length];
        for (int i = 0; i < _spawnableObjects.Length; i++)
        {
            if (_spawnableObjects[i].prefab == null) continue;
            containers[i] = new GameObject(_spawnableObjects[i].name + "_Container").transform;
            containers[i].SetParent(meshTransform);
        }

        int mapWidth = mapInfoGrid.GetLength(0);
        int mapHeight = mapInfoGrid.GetLength(1);

        // --- STEP 1: Build height zone lists ---
        List<MapGridCellInfo>[] zoneLists = new List<MapGridCellInfo>[_heightZones.Length]; // Based on the number of height zones, create a list for each zone
        for (int i = 0; i < _heightZones.Length; i++)
        {
            zoneLists[i] = new List<MapGridCellInfo>();
        }

        // Iterate through the map grid and assign each cell to its corresponding height zone list
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float height = mapInfoGrid[x, y].NormalizedHeight;
                for (int z = 0; z < _heightZones.Length; z++) // Loop through height zones to find the right one for this cell, when found, add it to the right list and break the loop
                {
                    if (height >= _heightZones[z].minHeight && height <= _heightZones[z].maxHeight) // if this cell's height is within the zone's range, add it to that zone's list
                    {
                        zoneLists[z].Add(mapInfoGrid[x, y]);
                        break; // Break from the for loop since a cell can only belong to one height zone
                    }
                }
            }
        }

        // --- STEP 2: Shuffle each zone list --- (to ensure random distribution of spawn points, without this, objects would spawn in a predictable pattern)
        // Fisher-Yates shuffle algorithm
        for (int z = 0; z < zoneLists.Length; z++)
        {
            for (int j = zoneLists[z].Count - 1; j > 0; j--)
            {
                int k = rand.Next(0, j + 1);
                MapGridCellInfo temp = zoneLists[z][j];
                zoneLists[z][j] = zoneLists[z][k];
                zoneLists[z][k] = temp;
            }
        }

        // --- STEP 3: Spawn objects ---
        for (int i = 0; i < _spawnableObjects.Length; i++)
        {
            SpawnableObject obj = _spawnableObjects[i];
            if (obj.prefab == null) continue;
            //if (obj.spawnZone == HeightZoneType.Sea) continue; // Skip sea objects for now

            List<MapGridCellInfo> candidates = zoneLists[(int)obj.spawnZone];
            int spawnedCount = 0;

            foreach (MapGridCellInfo cell in candidates)
            {
                if (spawnedCount >= obj.maxCount) break;
                if (cell.IsOccupied) continue;
                if (IsTooClose(cell.Position, obj.minDistance, mapInfoGrid, mapWidth, mapHeight)) continue;

                // Raycast to get exact surface position
                Vector3 rayOrigin = meshTransform.TransformPoint(cell.Position + Vector3.up * 10f);
                Ray ray = new Ray(rayOrigin, Vector3.down);

                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _mapMeshLayer))
                {
                    Vector3 spawnPos = hit.point + Vector3.up * obj.heightOffset;
                    Quaternion spawnRotation = Quaternion.identity;
                    if (obj.rotationBasedOnNormal == true)
                    {
                        spawnRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                    }
                    GameObject spawned = Instantiate(obj.prefab, spawnPos, spawnRotation);
                    spawned.transform.SetParent(containers[i]);

                    MarkOccupied(cell.Position, obj.minDistance, mapInfoGrid, mapWidth, mapHeight);
                    spawnedCount++;
                }
            }
        }
    }

    private bool IsTooClose(Vector3 position, float minDistance, MapGridCellInfo[,] grid, int width, int height)
    {
        for (int y = 0; y < height; y++) 
            for (int x = 0; x < width; x++)
                if (grid[x, y].IsOccupied)
                    if (Vector3.Distance(position, grid[x, y].Position) < minDistance)
                        return true;
        return false;
    }

    private void MarkOccupied(Vector3 position, float minDistance, MapGridCellInfo[,] grid, int width, int height)
    {
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                if (Vector3.Distance(position, grid[x, y].Position) < minDistance) grid[x, y].IsOccupied = true;
    }

    //public void SpawnObjects(MapGridCellInfo[,] mapInfoGrid, int seed, Transform meshTransform)
    //{
    //    List<Transform> meshChildrens = new();

    //    foreach(Transform t in meshTransform)
    //    {
    //        meshChildrens.Add(t);
    //    }

    //    foreach (Transform child in meshChildrens)
    //    {

    //        if (child == null) continue;
    //        if (child.name.EndsWith("_Container"))
    //        {
    //            Debug.Log($"Destroying container: {child.name}");
    //            DestroyImmediate(child.gameObject);
    //        }
    //    }

    //    System.Random rand = new System.Random(seed);

    //    int mapWidth = heightMap.GetLength(0);
    //    int mapHeight = heightMap.GetLength(1);

    //    // Create containers for each spawnable object
    //    Transform[] containers = new Transform[_spawnableObjects.Length];
    //    for(int i = 0; i < _spawnableObjects.Length; i++)
    //    {
    //        if (_spawnableObjects[i].prefab == null) continue;

    //        containers[i] = new GameObject(_spawnableObjects[i].name + "_Container").transform;
    //        containers[i].SetParent(meshTransform);
    //    }
    //    int counter = 0;
    //    // Iterate through the height map and spawn objects based on their height range and density
    //    for (int y = 0; y < mapHeight; y++)
    //    {
    //        for (int x = 0; x < mapWidth; x++)
    //        {
    //            float currentHeight = heightMap[x, y];

    //            for (int i = 0; i < _spawnableObjects.Length; i++)
    //            {
    //                SpawnableObject obj = _spawnableObjects[i];
    //                if (obj.prefab == null) continue;

    //                if (currentHeight >= obj.minHeight && currentHeight <= obj.maxHeight) // The Noise value is within the height range for this object?
    //                {
    //                    if (rand.NextDouble() < obj.density) // Randomomly decide to spawn based on density, a value of 0.1 means 10% chance to spawn
    //                    {
    //                        // XZ position centered on mesh
    //                        float worldX = (x - mapWidth / 2f) * 2f;
    //                        float worldZ = -(y - mapHeight / 2f) * 2f;

    //                        // Approximate Y — just above the surface
    //                        float approxY = heightCurve.Evaluate(currentHeight) * amplitudeMultiplier + 10f; // +10 to ensure it's above the terrain for raycasting

    //                        // Convert to world space
    //                        Vector3 localPos = new Vector3(worldX, approxY, worldZ);
    //                        Vector3 rayOrigin = meshTransform.TransformPoint(localPos); // Tranform a local position to world space to ensure the raycast is done in the correct world space

    //                        // Shoot ray downward
    //                        Ray ray = new Ray(rayOrigin, Vector3.down);

    //                        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _mapMeshLayer))
    //                        {
    //                            // Exact position on surface + offset
    //                            Vector3 spawnPos = hit.point + Vector3.up * obj.heightOffset;
    //                            Quaternion spawnRot = Quaternion.identity;

    //                            if(obj.name != "Palm Tree")
    //                            {
    //                                // Rotation aligned to terrain normal
    //                                spawnRot = Quaternion.FromToRotation(Vector3.up, hit.normal);
    //                            }

    //                            GameObject spawned = Instantiate(obj.prefab, spawnPos, spawnRot);
    //                            spawned.transform.SetParent(containers[i]);
    //                        }
    //                    }
    //                }
    //                counter++;
    //            }
    //        }
    //    }
    //    Debug.Log(counter);
    //}
}
