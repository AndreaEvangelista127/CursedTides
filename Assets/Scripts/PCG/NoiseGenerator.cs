using Unity.Mathematics;
using UnityEngine;

public static class NoiseGenerator
{
    public static float[,] GenerateNoiseMap(int width, int height, float scale,int seed, int octaves, float persistance, float lacunarity, Vector2 offset)
    {
        float[,] heightMap = new float[width, height];

        System.Random pseudoRand = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for(int i = 0; i < octaves; i++)
        {
            float offsetX = pseudoRand.Next(-100000, 100000) + offset.x;
            float offsetY = pseudoRand.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }


        if (scale == 0) scale = 0.0001f;

        float maxNoiseHeight = float.MinValue; // Starting with the lowest possible value to find the maximum
        float minNoiseHeight = float.MaxValue; // Starting with the highest possible value to find the minimum

        // Centering the noise in the middle of the map
        float halfWidth = width / 2f; 
        float halfHeight = height / 2f; 

        // Every (x, y) coordinate in the heightMap will be assigned a noise value that will represent the height of the terrain at that point
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float amplitude = 1; // The higher the amplitude, the higher the waves of the noise
                float frequency = 1; // How closer are the waves of the noise
                float noiseHeight = 0; // The final noise value for the current (x, y) coordinate

                
                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x; 
                    float sampleY = (y - halfHeight) / scale * frequency + octaveOffsets[i].y;

                    //heightMap[x, y] = Mathf.PerlinNoise(sampleX, sampleY);
                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1; // Remapping the value from [0, 1] to [-1, 1] to allow for negative values and more variation in the terrain
                    noiseHeight += perlinValue * amplitude; 

                    amplitude *= persistance; // if persistance = 0.5, the amplitude will be halved for each octave, making the waves smaller and smaller
                    frequency *= lacunarity; // if lacunarity = 2, the frequency will be doubled for each octave, making the waves closer and closer

                } // In the end each octave will add more detail to the noise, making it more realistic and less uniform

                // Update the max and min noise heights for normalization later
                if (noiseHeight > maxNoiseHeight)
                    maxNoiseHeight = noiseHeight;

                if (noiseHeight < minNoiseHeight)  
                    minNoiseHeight = noiseHeight;

                heightMap[x, y] = noiseHeight;
            }
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // If value = min => 0, if value = max => 1, if value = in between => normalized value between 0 and 1
                heightMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, heightMap[x, y]); // Normalize the values between 0 and 1]
            }
        }

        return heightMap; // Value 0.0 -> black and lowest point, 0.5 -> grey and middle point, 1.0 -> white and highest point
    }
}
