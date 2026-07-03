
using UnityEditor;
using UnityEngine;

public class EditorExercise : EditorWindow
{
    private PreviewModelSelector _previewModelSelector;

    private PreviewRenderUtility _preview;
    private GameObject _previewPrefab;
    private GameObject _previewModel;

    private string[] _skinNames;
    private int _skinIndex = 0;

    private void OnEnable()
    {
        _preview = new PreviewRenderUtility();
        
    }

    private void OnDisable()
    {
        _preview.Cleanup();
    }

    [MenuItem("Tools/Exercise")] // This adds a menu item to the Unity Editor under "Tools" called "Enemy Spawner"
    public static void ShowWindow()
    {
        GetWindow<EditorExercise>("Exercise");
    }


    private void OnGUI()
    {
        _previewPrefab = EditorGUILayout.ObjectField("Preview Prefab", _previewPrefab, typeof(GameObject), false) as GameObject;

        if (_previewModel == null && _previewPrefab != null)
        {
            _previewModel = PrefabUtility.InstantiatePrefab(_previewPrefab) as GameObject;
            _previewModel.transform.position = Vector3.zero;

            _previewModelSelector = _previewModel.GetComponentInChildren<PreviewModelSelector>();

            if (_previewModelSelector != null)
            {
                _skinNames = _previewModelSelector.GetSkinNames();
            }


            //CurrentSelected and previuosSelected
            _preview.AddSingleGO(_previewModel);
        }

        if (_skinNames != null && _skinNames.Length != 0)
        {
            _skinIndex = EditorGUILayout.Popup("Skins:", _skinIndex, _skinNames);
            _previewModelSelector.SelectSkin(_skinIndex);

        }


        Rect previewRect = GUILayoutUtility.GetRect(300, 300);

        _preview.BeginPreview(previewRect, GUIStyle.none);

        _preview.camera.transform.position = new Vector3(0, 1, 7);
        _preview.camera.transform.LookAt(new Vector3(0, 1, 0));

        _preview.camera.nearClipPlane = 5f;
        _preview.camera.farClipPlane = 20f;


        _preview.camera.Render();

        Texture frame = _preview.EndPreview();

        GUI.DrawTexture(previewRect, frame, ScaleMode.ScaleToFit);


        GUILayout.Label("end here");

    }


}
