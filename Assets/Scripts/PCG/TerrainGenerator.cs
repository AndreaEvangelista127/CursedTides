
using System;
using UnityEngine;


public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] private int _width;
    [SerializeField] private int _height;
    [SerializeField] private float _scale;
    [SerializeField] private float _amplitudeMultiplier;

    [SerializeField] private TextureDisplayer _textureDisplayer;
    [SerializeField] private MeshFilter _mf;

    public void DisplayHeightMap()
    {
        if (_width <= 0) _width = 1;
        if (_height <= 0) _height = 1;

        float[,] heightMap = NoiseGenerator.GenerateNoiseMap(_width, _height, _scale);

        Texture2D texture = TextureGenerator.GenerateTextureFromHeightMap(heightMap);

        if(_textureDisplayer != null)
        {
            _textureDisplayer.DisplayTexture(texture);
        }

    }

    public void GenerateTerrainFromMesh()
    {
        if (_width <= 0) _width = 1;
        if (_height <= 0) _height = 1;

        float[,] heightMap = NoiseGenerator.GenerateNoiseMap(_width, _height, _scale);

        Mesh terrainMesh = MeshGenerator.GenerateMeshFromHeightMap(heightMap, _amplitudeMultiplier);

        _mf.mesh = terrainMesh;
    }



}
