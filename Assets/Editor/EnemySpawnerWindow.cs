using UnityEditor;
using UnityEngine;

public class EnemySpawnerWindow : EditorWindow
{
    // PREVIEW 
    private PreviewRenderUtility _preview; // Unity class to show a preview in the editor tool
    private GameObject _previewModel; // Instance of the prefab that is being previewed
    private PreviewModelSelector _previewModelSelector; // Reference to the PreviewModelSelector component the access the skin selection functionality
    private AttachmentSelector _previewAttachmentSelector; // Reference to the AttachmentSelector component to access the attachment selection functionality

    // PREFABS
    private GameObject _enemyBasePrefab;
    private GameObject _enemyModelPrefab;
    private GameObject _lastModelPrefab;

    // SETTINGS WINDOW
    private MeleeEnemySettings _meleeSettings = new MeleeEnemySettings();
    private RangedEnemySettings _rangedSettings = new RangedEnemySettings();
    private bool _spawnMelee = true; // true = melee selected, false = ranged selected
    private Vector3 _spawnPosition = Vector3.zero;

    // --- SKIN ---
    private string[] _skinNames;
    private int _skinIndex = 0;

    // --- ATTACHMENTS ---
    private string[] _attachmentNames;
    private bool[] _attachmentToggles;

    private Vector2 scrollPosition;

    private GUIStyle _titleLabel;
    private GUIStyle _sectionLabel;
    private GUIStyle _meleeLabel;
    private GUIStyle _rangedLabel;
    private GUIStyle _descriptionLabel;
    

    private void OnEnable()
    {
        _preview = new PreviewRenderUtility();

        // TITLE LABEL SETTINGS
        _titleLabel = new GUIStyle();
        _titleLabel.fontStyle = FontStyle.Bold;
        _titleLabel.alignment = TextAnchor.MiddleCenter;
        _titleLabel.normal.textColor = Color.red;
        _titleLabel.fontSize = 18;

        //SECTION LABEL SETTINGS
        _sectionLabel = new GUIStyle();
        _sectionLabel.fontStyle = FontStyle.Bold;
        _sectionLabel.normal.textColor = Color.crimson;
        _sectionLabel.fontSize = 16;

        // MELEE LABEL SETTINGS
        _meleeLabel = new GUIStyle();
        _meleeLabel.fontStyle = FontStyle.Bold;
        _meleeLabel.normal.textColor = Color.green;
        _meleeLabel.fontSize = 14;

        //RANGED LABEL SETTINGS
        _rangedLabel = new GUIStyle();
        _rangedLabel.fontStyle = FontStyle.Bold;
        _rangedLabel.normal.textColor = Color.yellow;
        _rangedLabel.fontSize = 14;

        //DESCRIPTION LABEL SETTINGS
        _descriptionLabel = new GUIStyle();
        _descriptionLabel.fontStyle = FontStyle.Bold;
        _descriptionLabel.normal.textColor = Color.cyan;
        _descriptionLabel.fontSize = 12;
    }

    private void OnDisable()
    {
        _preview.Cleanup();

        if (_previewModel != null)
            DestroyImmediate(_previewModel); // Destroy the preview model when the window is closed
    }

    [MenuItem("Tools/Enemy Spawner")] // This adds a menu item to the Unity Editor under "Tools" called "Enemy Spawner"
    public static void ShowWindow()
    {
        GetWindow<EnemySpawnerWindow>("Enemy Spawner");
    }

    // Everything that is drawn here will be drawn in the window, we can use GUILayout to draw buttons, labels, etc.
    private void OnGUI()
    {
        GUILayout.Label("ENEMY SPAWNER", _titleLabel);
        GUILayout.Space(10);

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        GUILayout.BeginHorizontal(); // OUTER HORIZONTAL LAYOUT

        // ------------- LEFT SIDE - SETTINGS -------------
        GUILayout.BeginVertical(GUILayout.Width(300)); 

        GUILayout.Label("Enemy Type", _sectionLabel);

        GUILayout.BeginHorizontal();
        if (GUILayout.Toggle(_spawnMelee, "Melee", "Button"))
            _spawnMelee = true;
        if (GUILayout.Toggle(!_spawnMelee, "Ranged", "Button"))
            _spawnMelee = false;
        GUILayout.EndHorizontal();

        if (_previewModel != null)
        {
            EnemyWeaponSetup weaponSetup = _previewModel.GetComponent<EnemyWeaponSetup>();
            if (weaponSetup != null)
            {
                if (_spawnMelee) weaponSetup.ActivateMelee();
                else weaponSetup.ActivateRanged();
            }
        }

        GUILayout.Space(5);
        _enemyBasePrefab = (GameObject)EditorGUILayout.ObjectField("Enemy Base Prefab", _enemyBasePrefab, typeof(GameObject), false);
        GUILayout.Space(10);

        if(_spawnMelee)
        {
            GUILayout.Label("Melee Settings", _meleeLabel);
            DrawCommonSettings(_meleeSettings);
            DrawMeleeSettings(_meleeSettings);
            
        }
        else
        {
            GUILayout.Label("Ranged Settings", _rangedLabel);
            DrawCommonSettings(_rangedSettings);
            DrawRangedSettings(_rangedSettings);
        }

        GUILayout.EndVertical();

        GUILayout.Box("", GUILayout.Width(2), GUILayout.ExpandHeight(true)); // ----- Separator -----

        // ------------- RIGHT SIDE - PREVIEW -------------

        GUILayout.BeginVertical(GUILayout.Width(300));

        GUILayout.Label("Skin Selector", _sectionLabel);
        GUILayout.Space(10);

        _enemyModelPrefab = (GameObject)EditorGUILayout.ObjectField("Model Prefab", _enemyModelPrefab, typeof(GameObject), false);

        // Detection of changes in the model prefab to update the preview model accordingly
        if (_enemyModelPrefab != _lastModelPrefab) 
        {
            if (_previewModel != null) DestroyImmediate(_previewModel);
            _previewModel = null;
            _skinNames = null;
            _attachmentNames = null;
            _attachmentToggles = null;
            _lastModelPrefab = _enemyModelPrefab;
        }

        // Instatiate and show the preview model, the skin and attachments
        if (_previewModel == null && _enemyModelPrefab != null) 
        {
            _previewModel = PrefabUtility.InstantiatePrefab(_enemyModelPrefab) as GameObject;
            _previewModel.transform.position = Vector3.zero;
            _previewModelSelector = _previewModel.GetComponent<PreviewModelSelector>();
            _previewAttachmentSelector = _previewModel.GetComponent<AttachmentSelector>();

            if (_previewModelSelector != null)
                _skinNames = _previewModelSelector.GetSkinNames();

            if (_previewAttachmentSelector != null)
            {
                _attachmentNames = _previewAttachmentSelector.GetAttachmentNames();
                _attachmentToggles = new bool[_attachmentNames.Length];
            }

            _preview.AddSingleGO(_previewModel);
        }

        // Skin dropdown
        if (_skinNames != null && _skinNames.Length > 0)
        {
            _skinIndex = EditorGUILayout.Popup("Skin:", _skinIndex, _skinNames);
            _previewModelSelector.SelectSkin(_skinIndex);
        }

        // Attachment checkboxes
        if (_attachmentNames != null && _attachmentNames.Length > 0)
        {
            GUILayout.Space(5);
            GUILayout.Label("Attachments", EditorStyles.boldLabel);
            for (int i = 0; i < _attachmentNames.Length; i++)
            {
                bool newValue = EditorGUILayout.Toggle(_attachmentNames[i], _attachmentToggles[i]);
                if (newValue != _attachmentToggles[i])
                {
                    _attachmentToggles[i] = newValue;
                    _previewAttachmentSelector.SetAttachmentActive(i, newValue);
                }
            }
        }

        // Preview render
        Rect previewRect = GUILayoutUtility.GetRect(300, 300);
        _preview.BeginPreview(previewRect, GUIStyle.none);
        _preview.camera.transform.position = new Vector3(0, 1, 8);
        _preview.camera.transform.LookAt(new Vector3(0, 1, 0));
        _preview.camera.nearClipPlane = 5f;
        _preview.camera.farClipPlane = 20f;
        _preview.camera.Render();
        Texture frame = _preview.EndPreview();
        GUI.DrawTexture(previewRect, frame, ScaleMode.ScaleToFit);

        GUILayout.EndVertical();

        GUILayout.EndHorizontal(); // END OUTER HORIZONTAL LAYOUT

        GUILayout.Space(10);

        _spawnPosition = EditorGUILayout.Vector3Field("Spawn Point", _spawnPosition);

        GUILayout.Space(10);

        if (GUILayout.Button("Spawn Enemy"))
        {
            // Validate both prefabs are assigned
            if (_enemyBasePrefab == null || _enemyModelPrefab == null)
            {
                EditorUtility.DisplayDialog("Error", "Please assign both Enemy Base and Model prefabs!", "OK");
                return;
            }

            EnemySettings settings = _spawnMelee ? _meleeSettings : _rangedSettings;

            EnemyFactory.GenerateEnemy(
                _enemyBasePrefab,    // the physics base (Rigidbody, Collider)
                _enemyModelPrefab,   // the visual model (Mesh, Bones, Skins)
                settings,            // melee or ranged settings
                _spawnMelee,         // determines which scripts to add
                _spawnPosition,      // where to place in scene
                _skinIndex,          // which skin to activate
                _attachmentToggles   // which attachments to activate
            );

        }
        GUILayout.EndScrollView();

    }

    private void DrawCommonSettings(EnemySettings settings)
    {
        GUILayout.Space(5);
        EditorGUILayout.LabelField("General", _descriptionLabel);
        settings.moveSpeed = EditorGUILayout.FloatField("Move Speed", settings.moveSpeed);
        settings.rotationSpeed = EditorGUILayout.FloatField("Rotation Speed", settings.rotationSpeed);
        settings.health = EditorGUILayout.FloatField("Max Health", settings.health);

        GUILayout.Space(5);
        EditorGUILayout.LabelField("Idle", _descriptionLabel);
        settings.idleTime = EditorGUILayout.FloatField("Idle Time", settings.idleTime);

        GUILayout.Space(5);
        EditorGUILayout.LabelField("Patrol", _descriptionLabel);
        settings.patrolRadius = EditorGUILayout.FloatField("Patrol Radius", settings.patrolRadius);
        settings.distanceBuffer = EditorGUILayout.FloatField("Distance Buffer", settings.distanceBuffer);

        GUILayout.Space(5);
        EditorGUILayout.LabelField("Alert", _descriptionLabel);
        settings.alertTime = EditorGUILayout.FloatField("Alert Time", settings.alertTime);
        settings.alertRotationSpeed = EditorGUILayout.FloatField("Alert Rotation Speed", settings.alertRotationSpeed);
        settings.alertRadius = EditorGUILayout.FloatField("Alert Radius", settings.alertRadius);
        settings.minRotation = EditorGUILayout.FloatField("Min Alert Rotation", settings.minRotation);
        settings.maxRotation = EditorGUILayout.FloatField("Max Alert Rotation", settings.maxRotation);

        GUILayout.Space(5);
        EditorGUILayout.LabelField("Chase", _descriptionLabel);
        settings.detectionRange = EditorGUILayout.FloatField("Detection Range", settings.detectionRange);
        settings.maxChaseDistance = EditorGUILayout.FloatField("Max Chase Distance", settings.maxChaseDistance);
        settings.chaseSpeed = EditorGUILayout.FloatField("Chase Speed", settings.chaseSpeed);

        GUILayout.Space(5);
        EditorGUILayout.LabelField("Sight", _descriptionLabel);
        settings.fieldOfView = EditorGUILayout.FloatField("Fov Angle", settings.fieldOfView);
        settings.fovRange = EditorGUILayout.FloatField("Fov Range", settings.fovRange);
    }

    private void DrawMeleeSettings(MeleeEnemySettings settings)
    {
        GUILayout.Space(5);
        EditorGUILayout.LabelField("Melee Attack Settings", _descriptionLabel);
        settings.attackRange = EditorGUILayout.FloatField("Attack Range", settings.attackRange);
        settings.attackCooldown = EditorGUILayout.FloatField("Attack Cooldown", settings.attackCooldown);
    }

    private void DrawRangedSettings(RangedEnemySettings settings)
    {
        GUILayout.Space(5);
        EditorGUILayout.LabelField("Ranged Attack Settings", _descriptionLabel);
        settings.shootRange = EditorGUILayout.FloatField("Shoot Range", settings.shootRange);
        settings.tooCloseRange = EditorGUILayout.FloatField("Too Close Range", settings.tooCloseRange);
        settings.shootCooldown = EditorGUILayout.FloatField("Shoot Cooldown", settings.shootCooldown);
    }



}
