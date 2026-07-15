using UnityEngine;

public static class MeshGenerator
{
    public static MeshData GenerateMeshFromHeightMap(float[,] heightMap, float heightMultiplier, AnimationCurve heightCurveMultiplier, int levelOfDetail)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        // Centering of the mesh
        /* width=5:
            topLeftX = (5-1) / -2 = -2  ← starts at -2 from left
            topLeftZ = (5-1) / 2  = +2  ← starts at +2 from top
        */
        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (height - 1) / 2f; // z positive when we go up

        int meshSimplificationIncrement = (levelOfDetail == 0) ? 1 : levelOfDetail * 2; // If levelOfDetail is 0, we want to keep all the vertices, otherwise we want to skip some vertices
        int verticesPerLine = (width - 1) / meshSimplificationIncrement + 1; // Number of vertices per line, we add 1 because we want to include the last vertex

        MeshData meshData = new MeshData(verticesPerLine, verticesPerLine);
        int vertexIndex = 0;

        int[] indeces = new int[(width - 1) * (height - 1) * 6]; // Indeces of the mesh -> 3 indeces for triangle so 6 in total for a quad

        for (int y = 0; y < height; y += meshSimplificationIncrement)
        {
            for (int x = 0; x < width; x += meshSimplificationIncrement)
            {
                meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, heightCurveMultiplier.Evaluate(heightMap[x,y]) * heightMultiplier , topLeftZ - y); // Centered

                /* width = 100 pixel
                    x=0   → 0   / 100 = 0.0  → left border of the texture
                    x=50  → 50  / 100 = 0.5  → center of the texture
                    x=100 → 100 / 100 = 1.0  → bright border of the texture

                    Vertex Vertice (x=25, z=75) on mesh 100 x 100
                    UV = (25/100, 75/100) = (0.25, 0.75)
                    HeightMap[25,75] = "p" value of the noise at that point
                    p is <= 0.3? yes -> Region = water applied
                 */
                meshData.uvs[vertexIndex] = new Vector2((float)x / width, (float)y / height); // Coverting vertexPo in a UV coordinate float between 0.0 to 1.0

                if (x < width - 1 && y < height - 1) // Inside of the bounds for the indeces
                {
                    // CLOCKWISE ORDER TO RENDER THE CORRECT FACE OF THE MESH, OTHERWISE WILL BE RENDERED THE BACK  
                    meshData.AddTriangle(vertexIndex, vertexIndex + verticesPerLine + 1, vertexIndex + verticesPerLine); // topleft -> bottom right -> bottom left
                    meshData.AddTriangle(vertexIndex + verticesPerLine + 1, vertexIndex, vertexIndex + 1); // bottom right -> top left -> top right
                }

                vertexIndex++;
            }
        }

        return meshData;
    }

    public class MeshData
    {
        public Vector3[] vertices;
        public int[] triangles;
        public Vector2[] uvs; // creating uv map to add texture to the mesh

        int triangleIndex;

        public MeshData(int meshWidth, int meshHeight)
        {
            vertices = new Vector3[meshWidth * meshHeight];
            uvs = new Vector2[meshWidth * meshHeight];
            triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
        }

        public void AddTriangle(int a, int b, int c)
        {
            triangles[triangleIndex] = a;
            triangles[triangleIndex + 1] = b;
            triangles[triangleIndex + 2] = c;
            triangleIndex += 3;
        }

        public Mesh CreateMesh()
        {
            Mesh mesh = new Mesh();

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;

            mesh.RecalculateNormals();

            return mesh;
        }
    }
}
