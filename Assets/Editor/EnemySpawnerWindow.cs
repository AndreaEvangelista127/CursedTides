using UnityEditor;
using UnityEngine;

public class EnemySpawnerWindow : EditorWindow
{
    private GameObject _sharedPrefab;

    // SETTINGS WINDOW
    private MeleeEnemySettings _meleeSettings = new MeleeEnemySettings();
    private RangedEnemySettings _rangedSettings = new RangedEnemySettings();
    private bool _spawnMelee = true; // true = melee selected, false = ranged selected
    private Vector3 _spawnPosition = Vector3.zero;

    // SKIN PREVIEW WINDOW
    private PreviewRenderUtility _preview; // Unity class to show a preview in the editor tool
    private PreviewModelSelector _previewModelSelector;
    private string[] _skinNames;
    private int _skinIndex = 0;
    private GameObject _previewModel;

    private void OnEnable()
    {
        _preview = new PreviewRenderUtility();
    }

    private void OnDisable()
    {
        _preview.Cleanup();
    }

    [MenuItem("Tools/Enemy Spawner")] // This adds a menu item to the Unity Editor under "Tools" called "Enemy Spawner"
    public static void ShowWindow()
    {
        GetWindow<EnemySpawnerWindow>("Enemy Spawner");
    }

    // Everything that is drawn here will be drawn in the window, we can use GUILayout to draw buttons, labels, etc.
    private void OnGUI()
    {
        GUILayout.Label("ENEMY SPAWNER", EditorStyles.boldLabel);
        GUILayout.Space(10);

        GUILayout.BeginHorizontal(); // BORDER BOX START

        // --- START OF THE LEFT COLUMN ---
        GUILayout.BeginVertical(GUILayout.Width(300)); 

        // Tab Selection - Melee or Ranged
        GUILayout.BeginHorizontal();
        if (GUILayout.Toggle(_spawnMelee, "Melee", "Button"))
            _spawnMelee = true;
        if (GUILayout.Toggle(!_spawnMelee, "Ranged", "Button"))
            _spawnMelee = false;
        GUILayout.EndHorizontal();

        GUILayout.Space(5);
        _sharedPrefab = (GameObject)EditorGUILayout.ObjectField("Enemy Prefab", _sharedPrefab, typeof(GameObject), false);
        GUILayout.Space(10);

        if (_spawnMelee)
        {
            GUILayout.Label("Melee Settings", EditorStyles.boldLabel);
            DrawCommonSettings(_meleeSettings);

            GUILayout.Space(5);
            EditorGUILayout.LabelField("Attack", EditorStyles.boldLabel);
            _meleeSettings.attackRange = EditorGUILayout.FloatField("Attack Range", _meleeSettings.attackRange);
            _meleeSettings.attackCooldown = EditorGUILayout.FloatField("Attack Cooldown", _meleeSettings.attackCooldown);
            
        }
        else
        {
            GUILayout.Label("Ranged Settings", EditorStyles.boldLabel);
            DrawCommonSettings(_rangedSettings);

            GUILayout.Space(5);
            EditorGUILayout.LabelField("Attack", EditorStyles.boldLabel);
            _rangedSettings.shootRange = EditorGUILayout.FloatField("Shoot Range", _rangedSettings.shootRange);
            _rangedSettings.shootCooldown = EditorGUILayout.FloatField("Shoot Cooldown", _rangedSettings.shootCooldown);
            _rangedSettings.tooCloseRange = EditorGUILayout.FloatField("Too Close Range", _rangedSettings.tooCloseRange);
        }

        GUILayout.EndVertical();
        // --- END OF THE LEFT COLUMN ---

        GUILayout.Box("", GUILayout.Width(2), GUILayout.ExpandHeight(true)); // --- MIDDLE LINE ---

        // --- START OF THE RIGHT COLUMN ---
        GUILayout.BeginVertical(GUILayout.Width(300));

        GUILayout.Label("SKIN SELECTOR", EditorStyles.boldLabel);
        GUILayout.Space(10);

        // Instantiate the prefab in the preview scene when assigned
        if (_previewModel == null && _sharedPrefab != null)
        {
            _previewModel = PrefabUtility.InstantiatePrefab(_sharedPrefab) as GameObject;
            _previewModel.transform.position = Vector3.zero;
            _previewModelSelector = _previewModel.GetComponentInChildren<PreviewModelSelector>();
            if (_previewModelSelector != null)
                _skinNames = _previewModelSelector.GetSkinNames();
            _preview.AddSingleGO(_previewModel);
        }

        // Skin dropdown — only shown when skin names are available
        if (_skinNames != null && _skinNames.Length != 0)
        {
            _skinIndex = EditorGUILayout.Popup("Skins:", _skinIndex, _skinNames);
            _previewModelSelector.SelectSkin(_skinIndex);
        }

        // Preview render area
        Rect previewRect = GUILayoutUtility.GetRect(280, 280);
        _preview.BeginPreview(previewRect, GUIStyle.none);
        _preview.camera.transform.position = new Vector3(0, 1, 7);
        _preview.camera.transform.LookAt(new Vector3(0, 1, 0));
        _preview.camera.nearClipPlane = 5f;
        _preview.camera.farClipPlane = 20f;
        _preview.camera.Render();
        Texture frame = _preview.EndPreview();
        GUI.DrawTexture(previewRect, frame, ScaleMode.ScaleToFit);

        GUILayout.EndVertical();
        // --- END OF THE RIGHT COLUMN ---

        GUILayout.EndHorizontal(); // BORDER BOX END

        GUILayout.Space(10);

        _spawnPosition = EditorGUILayout.Vector3Field("Spawn Point", _spawnPosition);

        GUILayout.Space(10) ;

        if (GUILayout.Button("Spawn Enemy"))
        {
            if (_sharedPrefab == null)
            {
                EditorUtility.DisplayDialog("Error", "Please assign a prefab first!", "OK");
                return;
            }

            EnemySettings settings = _spawnMelee ? _meleeSettings : _rangedSettings;

            //EnemyFactory.SpawnEnemy(_sharedPrefab, settings, _spawnPosition, _spawnMelee);
            EnemyFactory.SpawnEnemy(_previewModel, settings, _spawnPosition, _spawnMelee);
        }
    }

    private void DrawCommonSettings(EnemySettings settings)
    {
        GUILayout.Space(5);
        EditorGUILayout.LabelField("General", EditorStyles.boldLabel);
        settings.moveSpeed = EditorGUILayout.FloatField("Move Speed", settings.moveSpeed);
        settings.rotationSpeed = EditorGUILayout.FloatField("Rotation Speed", settings.rotationSpeed);

        GUILayout.Space(5);
        EditorGUILayout.LabelField("Idle", EditorStyles.boldLabel);
        settings.idleTime = EditorGUILayout.FloatField("Idle Time", settings.idleTime);

        GUILayout.Space(5);
        EditorGUILayout.LabelField("Patrol", EditorStyles.boldLabel);
        settings.patrolRadius = EditorGUILayout.FloatField("Patrol Radius", settings.patrolRadius);
        settings.distanceBuffer = EditorGUILayout.FloatField("Distance Buffer", settings.distanceBuffer);

        GUILayout.Space(5);
        EditorGUILayout.LabelField("Alert", EditorStyles.boldLabel);
        settings.alertTime = EditorGUILayout.FloatField("Alert Time", settings.alertTime);
        settings.alertRotationSpeed = EditorGUILayout.FloatField("Alert Rotation Speed", settings.alertRotationSpeed);
        settings.alertRadius = EditorGUILayout.FloatField("Alert Radius", settings.alertRadius);
        settings.minRotation = EditorGUILayout.FloatField("Min Alert Rotation", settings.minRotation);
        settings.maxRotation = EditorGUILayout.FloatField("Max Alert Rotation", settings.maxRotation);

        GUILayout.Space(5);
        EditorGUILayout.LabelField("Chase", EditorStyles.boldLabel);
        settings.detectionRange = EditorGUILayout.FloatField("Detection Range", settings.detectionRange);
        settings.maxChaseDistance = EditorGUILayout.FloatField("Max Chase Distance", settings.maxChaseDistance);
        settings.chaseSpeed = EditorGUILayout.FloatField("Chase Speed", settings.chaseSpeed);

        GUILayout.Space(5);
        EditorGUILayout.LabelField("Sight", EditorStyles.boldLabel);
        settings.fieldOfView = EditorGUILayout.FloatField("Fov Angle", settings.fieldOfView);
        settings.fovRange = EditorGUILayout.FloatField("Fov Range", settings.fovRange);
    }
    
    

}
