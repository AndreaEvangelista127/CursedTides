using UnityEditor;
using UnityEngine;

public class EnemySpawnerWindow : EditorWindow
{
    private MeleeEnemySettings _meleeSettings = new MeleeEnemySettings();
    private RangedEnemySettings _rangedSettings = new RangedEnemySettings();
    private bool _spawnMelee = true; // true = melee selected, false = ranged selected
    private Vector3 _spawnPosition = Vector3.zero;

    [MenuItem("Tools/Enemy Spawner")] // This adds a menu item to the Unity Editor under "Tools" called "Enemy Spawner"
    public static void ShowWindow()
    {
        GetWindow<EnemySpawnerWindow>("Enemy Spawner");
    }

    // Everything that is drawn here will be drawn in the window, we can use GUILayout to draw buttons, labels, etc.
    private void OnGUI()
    {
        GUILayout.Label("Enemy Spawner", EditorStyles.boldLabel);
        GUILayout.Space(10);

        // Tab Selection - Melee or Ranged
        GUILayout.BeginHorizontal();
        if (GUILayout.Toggle(_spawnMelee, "Melee", "Button"))
            _spawnMelee = true;
        if (GUILayout.Toggle(!_spawnMelee, "Ranged", "Button"))
            _spawnMelee = false;
        GUILayout.EndHorizontal();

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

        GUILayout.Space(10);

        _spawnPosition = EditorGUILayout.Vector3Field("Spawn Point", _spawnPosition);

        GUILayout.Space(10) ;

        if (GUILayout.Button("Spawn Enemy"))
        {
            if (_spawnMelee)
                EnemyFactory.CreateMeleeEnemy(_meleeSettings, _spawnPosition);
            else
                EnemyFactory.CreateRangedEnemy(_rangedSettings, _spawnPosition);
        }
    }

    private void DrawCommonSettings(EnemySettings settings)
    {
        settings.prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", settings.prefab, typeof(GameObject), false);

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
