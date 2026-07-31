
using System;
using UnityEngine;


public class TerrainGenerator : MonoBehaviour
{
    [Header("Map Settings")]
    [SerializeField][Range(0,6)] private int _levelOfDetail;
    [SerializeField][Range(1,4)] private int _mapSizeFactor = 2; // The size of the mesh in world units, the higher the value, the bigger the mesh will be
    public const int mapChunkSize = 241; // 241 is the maximum size for a mesh with 6 levels of detail (LOD)
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
    //[SerializeField][Range(0f, 1f)] private float _islandBorderThreshold = 0.2f;

    [Header("References")]
    [SerializeField] private TextureDisplayer _textureDisplayer;
    [SerializeField] private MeshFilter _mf;
    [SerializeField] private ObjectSpawner _objectSpawner;


    [SerializeField] private Gradient _terrainGradient;
    [SerializeField] public TerrainType[] regions;

    public void Generate()
    {

        float[,] heightMap = HeightMapGenerator.GeneratePerlinNoiseMap(mapChunkSize, mapChunkSize, _noiseScale, seed, _octaves, _persistance, _lacunarity, offset);

        float[,] baseHeightMap = (float[,])heightMap.Clone(); // Clone the heightMap to keep the original values for the base noise texture

        Texture2D baseNoiseTexture = TextureGenerator.GenerateTextureFromHeightMap(baseHeightMap);
        _textureDisplayer.DisplayBaseNoiseTexture(baseNoiseTexture);

        if (_useFalloff)
        {
            float[,] fallOffMap = HeightMapGenerator.GenerateFallOffMap(mapChunkSize, mapChunkSize, _curvePower, _curveScale);
            for (int y = 0; y < mapChunkSize; y++)
            {
                for (int x = 0; x < mapChunkSize; x++)
                {
                    //float result = Mathf.Clamp01(heightMap[x, y] - fallOffMap[x, y]);
                    float result = Mathf.Clamp(heightMap[x, y], _minIslandHeight, 1f) - fallOffMap[x, y];
                    result = Mathf.Clamp01(result);
                    // if the fallOffMap value is below the threshold, set the height to at least _minIslandHeight
                    //if (fallOffMap[x, y] < _islandBorderThreshold) result = Mathf.Max(result, _minIslandHeight);
                    heightMap[x, y] = result;
                }
            }
            Texture2D falloffTexture = TextureGenerator.GenerateTextureFromHeightMap(heightMap);
            _textureDisplayer.DisplayFalloffTexture(falloffTexture);
        }  

        // SHOW COLOR MAP ON THE SECOND PLANE
        Texture2D colorTexture = TextureGenerator.GenerateTextureFromColorMap(GenerateColorMap(heightMap), mapChunkSize, mapChunkSize);
        _textureDisplayer.DisplayColorTexture(colorTexture);

        // SHOW MESH ON THE THIRD PLANE
        MeshGenerator.MeshData meshData = MeshGenerator.GenerateMeshFromHeightMap(heightMap, _amplitudeMultiplier, _amplitudeCurveMultiplier, _levelOfDetail,_mapSizeFactor);

        MapGridCellInfo[,] mapInfoGrid = MapInfoGridGenerator.GenerateMapInfoGrid(heightMap, _mapSizeFactor, _amplitudeMultiplier, _amplitudeCurveMultiplier);
        // Give this map to the object spawner, so that we can use the values of the struct to be able to spawn the objects based on the position and if the vertex is occupied

        _minTerrainHeight = float.MaxValue;
        _maxTerrainHeight = float.MinValue;

        foreach (Vector3 vertex in meshData.vertices)
        {
            if (vertex.y < _minTerrainHeight) _minTerrainHeight = vertex.y;
            if (vertex.y > _maxTerrainHeight) _maxTerrainHeight = vertex.y;
        }

        Debug.Log("Min Terrain height:" + _minTerrainHeight + "," + "Max Terrain Height:" + _maxTerrainHeight);

        for (int i = 0; i < meshData.vertices.Length; i++)
        {
            float normalizedHeight = Mathf.InverseLerp(_minTerrainHeight, _maxTerrainHeight, meshData.vertices[i].y); //returns a value between 0 and 1 that will represent the height color
            //Debug.Log(normalizedHeight);
            meshData.colors[i] = _terrainGradient.Evaluate(normalizedHeight);
        }

        Mesh generatedMesh = meshData.CreateMesh();
        _mf.mesh = generatedMesh;

        // Add a MeshCollider to the MeshFilter's GameObject if it doesn't already have one
        MeshCollider meshCollider = _mf.GetComponent<MeshCollider>();
        if (meshCollider != null)
            meshCollider.sharedMesh = generatedMesh;

        _textureDisplayer.DisplayTerrainTexture(colorTexture);

        if(_objectSpawner != null)
        {
            _objectSpawner.SpawnObjects(heightMap, seed, _amplitudeMultiplier, _amplitudeCurveMultiplier, _mf.transform, meshData);
        }
    }

    private Color[] GenerateColorMap(float[,] heightMap)
    {
        Color[] colorMap = new Color[mapChunkSize * mapChunkSize];
        for (int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {
                float currentHeight = heightMap[x, y];
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].height)
                    {

                        colorMap[y * mapChunkSize + x] = regions[i].color;
                        break;
                    }
                }
            }
        }
        return colorMap;
    }

    private void OnValidate() // This method is called when the script is loaded or a value is changed in the inspector (Called in the editor only)
    {
        if(_lacunarity < 1) _lacunarity = 1;
        if(_octaves < 0) _octaves = 0;
        
    }

    [System.Serializable]
    public struct TerrainType
    {
        public string name;
        public float height;
        public Color color;
    }

}
