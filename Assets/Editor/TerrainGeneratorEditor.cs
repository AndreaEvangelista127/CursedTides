using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TerrainGenerator))]
public class TerrainGeneratorEditor : Editor
{
    private bool _autoGenerate = false;

    public override void OnInspectorGUI()
    {
        TerrainGenerator terrainGenerator = (TerrainGenerator)target;


        if(DrawDefaultInspector() && _autoGenerate)
        {
            terrainGenerator.DisplayHeightMap();
            terrainGenerator.GenerateTerrainFromMesh();
        }

        _autoGenerate = EditorGUILayout.Toggle("Auto Generate",_autoGenerate);

        if (GUILayout.Button("Generate HeighMap"))
        {
            terrainGenerator.DisplayHeightMap();
        }

        if (GUILayout.Button("Generate Mesh"))
        {
            terrainGenerator.GenerateTerrainFromMesh();
        }


    }
}
