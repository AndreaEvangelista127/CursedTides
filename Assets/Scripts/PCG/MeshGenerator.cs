using UnityEngine;
using UnityEngine.UIElements;

public static class MeshGenerator
{
    public static Mesh GenerateMeshFromHeightMap(float[,] heightMap, float amplitudeMultiplier)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        int vertexIndex = 0;
        int triangleIndex = 0;

        Vector3[] vertices = new Vector3[width * height];

        int[] indeces = new int[(width - 1) * (height - 1) * 6]; //tostudy

        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                vertices[vertexIndex] = new Vector3(x, heightMap[x, z] * amplitudeMultiplier, z); // Lenght, Amplitude, depth

                if (x < width - 1 && z < height - 1) // Inside of the bounds for the indeces
                {
                    // Define the triangles
                    indeces[triangleIndex] = vertexIndex;
                    indeces[triangleIndex + 1] = vertexIndex + width;
                    indeces[triangleIndex + 2] = vertexIndex + width + 1;

                    triangleIndex += 3;

                    indeces[triangleIndex] = vertexIndex;
                    indeces[triangleIndex + 1] = vertexIndex + width + 1;
                    indeces[triangleIndex + 2] = vertexIndex + 1;

                    triangleIndex += 3;
                }

                vertexIndex++;
            }
        }

        Mesh mesh = new Mesh();

        mesh.vertices = vertices;
        mesh.triangles = indeces;

        mesh.RecalculateNormals();

        return mesh;

    }

}
