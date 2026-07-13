using UnityEngine;
using UnityEngine.UIElements;

public static class TextureGenerator 
{
    // Colored texture based on Regions
    public static Texture2D GenerateTextureFromColorMap(Color[] colorMap, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;
        texture.SetPixels(colorMap); // Draws the colormap from bottom to top, so (0,0) means bottom left
        texture.Apply();
        return texture;
    }

    // Black and White texture base on heightMap(Perlin Noise)
    public static Texture2D GenerateTextureFromHeightMap(float[,] heightMap)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        Color[] colorsMap = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {

                colorsMap[y * width + x] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
            }
        }

        return GenerateTextureFromColorMap(colorsMap, width, height);
    }

}
