
using System;
using UnityEngine;


public class TerrainGenerator : MonoBehaviour
{

    [Header("Map Settings")]
    [SerializeField] private float _scale;
    [SerializeField][Range(0,6)] private int _levelOfDetail;
    const int mapChunkSize = 241;

    [Header("Noise Settings")]
    [SerializeField] private int _octaves = 4;
    [SerializeField][Range(0f, 1f)] private float _persistance = 0.5f;
    [SerializeField] private float _lacunarity = 2.0f;
    [SerializeField] private float _amplitudeMultiplier = 1.0f;
    [SerializeField] private AnimationCurve _amplitudeCurveMultiplier;
    public int seed;
    public Vector2 offset;

    [Header("FallOffSettings")]
    [SerializeField] private float _curvePower = 3f;
    [SerializeField] private float _curveScale = 2.2f;

    [Header("References")]
    [SerializeField] private TextureDisplayer _textureDisplayer;
    [SerializeField] private MeshFilter _mf;
    [SerializeField] private ObjectSpawner _objectSpawner;
    

    [SerializeField] public TerrainType[] regions;

    public void Generate()
    {
        float[,] heightMap = HeightMapGenerator.GeneratePerlinNoiseMap(mapChunkSize, mapChunkSize, _scale, seed, _octaves, _persistance, _lacunarity, offset);

        float[,] fallOffMap = HeightMapGenerator.GenerateFallOffMap(mapChunkSize, mapChunkSize, _curvePower, _curveScale);

        float[,] finalMap = new float[mapChunkSize, mapChunkSize];

        for (int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {

                finalMap[x, y] = Mathf.Clamp01(heightMap[x, y] - fallOffMap[x, y]); // substracting from the noise map the fall of map
            }
        }

        Texture2D finalTexture = TextureGenerator.GenerateTextureFromHeightMap(finalMap);
        _textureDisplayer.DisplayNoiseTexture(finalTexture);
        
       
        //Texture2D fallOffTexture = TextureGenerator.GenerateTextureFromHeightMap(fallOffMap);

        // Show noise map on first plane
        //Texture2D noiseTexture = TextureGenerator.GenerateTextureFromHeightMap(heightMap);
        //_textureDisplayer.DisplayNoiseTexture(noiseTexture);

        // Show color map on second plane
        //Texture2D colorTexture = TextureGenerator.GenerateTextureFromColorMap(GenerateColorMap(heightMap), _mapWidth, _mapHeight);
        Texture2D colorTexture = TextureGenerator.GenerateTextureFromColorMap(GenerateColorMap(finalMap), mapChunkSize, mapChunkSize);
        _textureDisplayer.DisplayColorTexture(colorTexture);

        // Generate mesh on third object
        MeshGenerator.MeshData meshData = MeshGenerator.GenerateMeshFromHeightMap(finalMap, _amplitudeMultiplier, _amplitudeCurveMultiplier, _levelOfDetail);
        _mf.sharedMesh = meshData.CreateMesh();
        _textureDisplayer.DisplayTerrainTexture(colorTexture);

        if(_objectSpawner != null)
        {
            _objectSpawner.SpawnObjects(finalMap, seed, _amplitudeMultiplier, _amplitudeCurveMultiplier, _mf.transform, meshData);
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
