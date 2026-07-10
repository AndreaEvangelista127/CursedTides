using UnityEngine;

public static class NoiseGenerator 
{
    public static float[,] GenerateNoiseMap(int width, int height, float scale)
    {
        float[,] heightMap = new float[width, height];

        if (scale == 0) scale = 0.0001f;

        for (int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                float sampleX = x / scale;
                float sampleY = y / scale;

                heightMap[x, y] = Mathf.PerlinNoise(sampleX, sampleY);
            }
        }

        return heightMap;
    }
}
