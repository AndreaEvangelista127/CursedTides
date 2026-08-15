using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(TerrainGenerator))]
public class TerrainGeneratorEditor : Editor
{
    private bool _autoGenerate = false;

    public override void OnInspectorGUI()
    {
        TerrainGenerator terrainGenerator = (TerrainGenerator)target;

        if (DrawDefaultInspector() && _autoGenerate)
        {
            //terrainGenerator.Generate();
        }

        _autoGenerate = EditorGUILayout.Toggle("Auto Generate", _autoGenerate);

        if (GUILayout.Button("Generate"))
        {
            //terrainGenerator.Generate();
            terrainGenerator.StartCoroutine(terrainGenerator.StartCoroutineGenerate());
            EditorSceneManager.MarkSceneDirty(SceneManager.GetSceneByName("GameScene"));
        }
    }
}
