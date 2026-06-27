using UnityEditor;
using UnityEngine;

public enum TestEnum
{
    asdsad,
    sdsd,
    sdssd
}


public class EditorExercise : EditorWindow
{


    private TestEnum _testEnum;

    private string[] _names = { "Andrea", "Phil" };
    private int _nameIndex = 0;


    private PreviewRenderUtility _preview;
    private GameObject _previewPrefab;
    private GameObject _previewModel;



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

        _testEnum = (TestEnum)EditorGUILayout.EnumPopup("Models", _testEnum);
        _nameIndex = EditorGUILayout.Popup("Names:", _nameIndex, _names);




        _previewPrefab = EditorGUILayout.ObjectField("Preview Prefab", _previewPrefab, typeof(GameObject), false) as GameObject;

        if (_previewModel == null && _previewPrefab != null)
        {
            _previewModel = PrefabUtility.InstantiatePrefab(_previewPrefab) as GameObject;
            _previewModel.transform.position = Vector3.zero;
            _preview.AddSingleGO(_previewModel);
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
