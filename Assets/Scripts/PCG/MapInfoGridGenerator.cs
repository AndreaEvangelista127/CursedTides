using UnityEngine;

public static class MapInfoGridGenerator
{
    public static MapGridCellInfo[,] GenerateMapInfoGrid(float[,] heightMap, int mapSizeFactor,float heightMultiplier, AnimationCurve heightCurveMultiplier)
    {

        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        float topLeftX = (width - 1) / -2f * mapSizeFactor;
        float topLeftZ = (height - 1) / 2f * mapSizeFactor;

        MapGridCellInfo[,] infoGrid = new MapGridCellInfo[width, height];


        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                //if error here then you need to create an object of MapGridCellInfo first and put it into the array
                infoGrid[x, y] = new MapGridCellInfo
                {
                    Position = new Vector3(topLeftX + (x * mapSizeFactor), heightCurveMultiplier.Evaluate(heightMap[x, y]) * heightMultiplier, topLeftZ - (y * mapSizeFactor)), // The position of the cell in world space, with the height adjusted by the height curve and multiplier
                    IsOccupied = false,
                    NormalizedHeight = heightMap[x, y] // The real value of the heightmap at this point, which is already normalized between 0 and 1
                };
            }
        }
        return infoGrid;
    }
}

public struct MapGridCellInfo
{
    public Vector3 Position;
    public bool IsOccupied;
    public float NormalizedHeight;
}
