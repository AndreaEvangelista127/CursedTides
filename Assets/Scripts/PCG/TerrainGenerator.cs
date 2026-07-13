
using System;
using UnityEngine;


public class TerrainGenerator : MonoBehaviour
{

    [Header("Map Settings")]
    [SerializeField] private int _mapWidth;
    [SerializeField] private int _mapHeight;
    [SerializeField] private float _scale;

    [Header("Noise Settings")]
    [SerializeField] private int _octaves = 4;
    [SerializeField][Range(0f, 1f)] private float _persistance = 0.5f;
    [SerializeField] private float _lacunarity = 2.0f;
    [SerializeField] private float _amplitudeMultiplier = 1.0f;
    [SerializeField] private AnimationCurve _amplitudeCurveMultiplier;
    public int seed;
    public Vector2 offset;

    [Header("References")]
    [SerializeField] private TextureDisplayer _textureDisplayer;
    [SerializeField] private MeshFilter _mf;
    

    [SerializeField] public TerrainType[] regions;

    public void Generate()
    {
        float[,] heightMap = NoiseGenerator.GenerateNoiseMap(_mapWidth, _mapHeight, _scale, seed, _octaves, _persistance, _lacunarity, offset);

        // Show noise map on first plane
        Texture2D noiseTexture = TextureGenerator.GenerateTextureFromHeightMap(heightMap);
        _textureDisplayer.DisplayNoiseTexture(noiseTexture);

        // Show color map on second plane
        Texture2D colorTexture = TextureGenerator.GenerateTextureFromColorMap(GenerateColorMap(heightMap), _mapWidth, _mapHeight);
        _textureDisplayer.DisplayColorTexture(colorTexture);

        // Generate mesh on third object
        MeshGenerator.MeshData meshData = MeshGenerator.GenerateMeshFromHeightMap(heightMap, _amplitudeMultiplier, _amplitudeCurveMultiplier);
        _mf.mesh = meshData.CreateMesh();
        _textureDisplayer.DisplayTerrainTexture(colorTexture);
    }

    private Color[] GenerateColorMap(float[,] heightMap)
    {
        Color[] colorMap = new Color[_mapWidth * _mapHeight];
        for (int y = 0; y < _mapHeight; y++)
        {
            for (int x = 0; x < _mapWidth; x++)
            {
                float currentHeight = heightMap[x, y];
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].height)
                    {

                        colorMap[y * _mapWidth + x] = regions[i].color;
                        break;
                    }
                }
            }
        }
        return colorMap;
    }

    private void OnValidate() // This method is called when the script is loaded or a value is changed in the inspector (Called in the editor only)
    {
        if(_mapWidth < 1) _mapWidth = 1;
        if (_mapHeight < 1) _mapHeight = 1;
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
