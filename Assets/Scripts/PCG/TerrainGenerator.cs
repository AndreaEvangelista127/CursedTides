
using System;
using System.Collections;
using UnityEngine;


public class TerrainGenerator : MonoBehaviour
{
    [Header("Map Settings")]
    [SerializeField][Range(0, 6)] private int _levelOfDetail;
    [SerializeField][Range(1, 4)] private int _mapSizeFactor = 2; // The size of the mesh in world units, the higher the value, the bigger the mesh will be
    public const int MAP_CHUNK_SIZE = 241; // 241 is the maximum size for a mesh with 6 levels of detail (LOD)
    private float _minTerrainHeight;
    private float _maxTerrainHeight;

    [Header("Noise Settings")]
    [SerializeField] private float _noiseScale;
    [SerializeField] private int _octaves = 4;
    [SerializeField][Range(0f, 1f)] private float _persistance = 0.5f;
    [SerializeField] private float _lacunarity = 2.0f;
    [SerializeField] private float _amplitudeMultiplier = 1.0f;
    [SerializeField] private AnimationCurve _amplitudeCurveMultiplier;
    public int seed;
    public Vector2 offset;

    [Header("FallOffSettings")]
    [SerializeField] private bool _useFalloff;
    [SerializeField] private float _curvePower = 3f;
    [SerializeField] private float _curveScale = 2.2f;
    [SerializeField][Range(0f, 1f)] private float _minIslandHeight = 0.3f;

    [Header("References")]
    [SerializeField] private TextureDisplayer _textureDisplayer;
    [SerializeField] private MeshFilter _mf;
    [SerializeField] private ObjectSpawner _objectSpawner;

    [Header("Water Settings")]
    [SerializeField] private GameObject _waterPrefab;
    [SerializeField] private float _waterHeight;

    [Header("Firefly Settings")]
    [SerializeField] private GameObject _firefliesPrefab;
    [SerializeField] private float _firefliesHeight = 2f;

    [Header("Player Spawn")]
    [SerializeField] private GameObject _player;
    [SerializeField] private LayerMask _spawnLayer;

    [SerializeField] private Gradient _terrainGradient;
    [SerializeField] public TerrainType[] Regions;

    public void Go()
    {
        StartCoroutine(StartCoroutineGenerate());
    }

    /// <summary>
    /// Coroutine that creates all the required data to be ale to generate the island map
    /// </summary>
    /// <returns></returns>
    public IEnumerator StartCoroutineGenerate()
    {

        seed = PlayerPrefs.GetInt("Seed", 0); // Get the seed from PlayerPrefs, default to 0 if not set

        float[,] heightMap = HeightMapGenerator.GeneratePerlinNoiseMap(MAP_CHUNK_SIZE, MAP_CHUNK_SIZE, _noiseScale, seed, _octaves, _persistance, _lacunarity, offset);

        float[,] baseHeightMap = (float[,])heightMap.Clone(); // Clone the heightMap to keep the original values for the base noise texture

        yield return null;


        if (_useFalloff)
        {
            float[,] fallOffMap = HeightMapGenerator.GenerateFallOffMap(MAP_CHUNK_SIZE, MAP_CHUNK_SIZE, _curvePower, _curveScale);
            for (int y = 0; y < MAP_CHUNK_SIZE; y++)
            {
                for (int x = 0; x < MAP_CHUNK_SIZE; x++)
                {
                    float result = Mathf.Clamp(heightMap[x, y], _minIslandHeight, 1f) - fallOffMap[x, y];
                    result = Mathf.Clamp01(result);
                    heightMap[x, y] = result;
                }
            }
        }

        yield return null;


        // SHOW MESH ON THE THIRD PLANE
        MeshGenerator.MeshData meshData = MeshGenerator.GenerateMeshFromHeightMap(heightMap, _amplitudeMultiplier, _amplitudeCurveMultiplier, _levelOfDetail, _mapSizeFactor);

        yield return null;

        MapGridCellInfo[,] mapInfoGrid = MapInfoGridGenerator.GenerateMapInfoGrid(heightMap, _mapSizeFactor, _amplitudeMultiplier, _amplitudeCurveMultiplier);

        _minTerrainHeight = float.MaxValue;
        _maxTerrainHeight = float.MinValue;

        foreach (Vector3 vertex in meshData.vertices)
        {
            if (vertex.y < _minTerrainHeight) _minTerrainHeight = vertex.y;
            if (vertex.y > _maxTerrainHeight) _maxTerrainHeight = vertex.y;
        }

        yield return null;

        for (int i = 0; i < meshData.vertices.Length; i++)
        {
            float normalizedHeight = Mathf.InverseLerp(_minTerrainHeight, _maxTerrainHeight, meshData.vertices[i].y); //returns a value between 0 and 1 that will represent the height color
            meshData.colors[i] = _terrainGradient.Evaluate(normalizedHeight);
        }

        yield return null;

        Mesh generatedMesh = meshData.CreateMesh();
        _mf.mesh = generatedMesh;

        yield return null;

        // Instantiate water prefab at the center of the mesh with the specified water height
        if (_waterPrefab != null)
        {
            Transform oldWater = _mf.transform.Find("Water");
            if (oldWater != null) DestroyImmediate(oldWater.gameObject);

            GameObject water = Instantiate(_waterPrefab,
                new Vector3(0, _waterHeight, 0),
                Quaternion.identity);
            water.name = "Water";
            water.transform.SetParent(_mf.transform);

            // Scale to cover entire map
            float mapSize = (MAP_CHUNK_SIZE - 1) * _mapSizeFactor;
            water.transform.localScale = new Vector3(mapSize / 10f, 1, mapSize / 10f);
        }

        // Instantiate fireflies prefab at the center of the mesh with the specified fireflies height
        if (_firefliesPrefab != null)
        {
            Transform oldFireflies = _mf.transform.Find("Fireflies");
            if (oldFireflies != null) DestroyImmediate(oldFireflies.gameObject);

            float mapSize = (MAP_CHUNK_SIZE - 1) * _mapSizeFactor;

            GameObject fireflies = Instantiate(_firefliesPrefab,
                new Vector3(0, _firefliesHeight, 0),
                Quaternion.identity);
            fireflies.name = "Fireflies";
            fireflies.transform.SetParent(_mf.transform);
            fireflies.transform.localScale = new Vector3(_mapSizeFactor, _mapSizeFactor, _mapSizeFactor);
        }

        // Add a MeshCollider to the MeshFilter's GameObject if it doesn't already have one
        MeshCollider meshCollider = _mf.GetComponent<MeshCollider>();
        if (meshCollider != null)
            meshCollider.sharedMesh = generatedMesh;


        // OBJECT SPAWNING
        if (_objectSpawner != null)
        {
            yield return StartCoroutine(_objectSpawner.SpawnObjectsCoroutine(mapInfoGrid, seed, _mf.transform));
        }

        // SPAWN PLAYER
        SetPlayerToCorrectHeightOnTheMap();

        yield return null;
    }

    private void OnValidate() // This method is called when the script is loaded or a value is changed in the inspector (Called in the editor only)
    {
        if (_lacunarity < 1) _lacunarity = 1;
        if (_octaves < 0) _octaves = 0;

    }

    /// <summary>
    /// Set the player y position to a value that ensures he wont be stuck in the mesh
    /// </summary>
    private void SetPlayerToCorrectHeightOnTheMap()
    {
        if (_player == null) return;

        Vector3 spawnPosition = new Vector3(0, 5f, 0);

        _player.transform.position = spawnPosition;
    }

    [System.Serializable]
    public struct TerrainType
    {
        public string name;
        public float height;
        public Color color;
    }

}
