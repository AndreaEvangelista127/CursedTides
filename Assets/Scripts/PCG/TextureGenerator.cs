using UnityEngine;
using UnityEngine.UIElements;

public static class TextureGenerator 
{
    public static Texture2D GenerateTextureFromHeightMap(float[,] heightMap)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        Texture2D texture = new Texture2D(width, height);

        Color[] colorsMap = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colorsMap[y * width + x] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
            }
        }

        texture.SetPixels(colorsMap);
        texture.Apply();

        return texture;
    }
}
